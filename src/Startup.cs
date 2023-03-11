// Copyright (c) Cingulara LLC 2020 and Tutela LLC 2020. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using Prometheus;
using OpenTracing;
using OpenTracing.Util;

namespace openrmf_api_controls
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   
            if (Environment.GetEnvironmentVariable("JAEGER_AGENT_HOST") != null && 
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JAEGER_AGENT_HOST"))) {
                    // Use "OpenTracing.Contrib.NetCore" to automatically generate spans for ASP.NET Core
                    services.AddSingleton<ITracer>(serviceProvider =>  
                    {
                        ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();  
                        // use the environment variables to setup the Jaeger endpoints
                        var config = Jaeger.Configuration.FromEnv(loggerFactory);
                        var tracer = config.GetTracer();
                    
                        GlobalTracer.Register(tracer);  
                    
                        return tracer;  
                    });
                    services.AddOpenTracing();
            }
           
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OpenRMF Controls API", Version = "v1", 
                    Description = "The Controls API that goes with the OpenRMF OSS Application",
                    Contact = new OpenApiContact
                    {
                        Name = "Dale Bingham",
                        Email = "dale.bingham@cingulara.com",
                        Url = new Uri("https://github.com/Cingulara/openrmf-api-controls")
                    } });
            });

            string jwtAuthorityServer = "http://openrmf-keycloak:8080/auth/"; // this is by default the internal keycloak setup
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWTINTERNALAUTHORITY"))) {
                jwtAuthorityServer = Environment.GetEnvironmentVariable("JWTINTERNALAUTHORITY").ToLower();
            }
            // Validate the JWT token sent with the request to make sure it is right
            // use the internal jwtAuthority server so you do not have to go outside the container network
            // allow the JWTINTERNALAUTHORITY if you need it for setup or for K8s specifically
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = jwtAuthorityServer + "realms/openrmf";
                o.Audience = Environment.GetEnvironmentVariable("JWTCLIENT");
                o.IncludeErrorDetails = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("JWTAUTHORITY").ToLower(),
                    ValidateLifetime = true
                };

                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "text/plain";

                        Console.WriteLine("openrmf-api-control JWT Error: " + c.Exception.ToString());

                        return c.Response.WriteAsync("The JWT validation with the server did not return correctly. Please check with your Application Administrator.");
                    }
                };
            });

            // setup the RBAC for this
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrator", policy => policy.RequireRole("roles", "[Administrator]"));
                options.AddPolicy("Download", policy => policy.RequireRole("roles", "[Download]"));
                options.AddPolicy("Editor", policy => policy.RequireRole("roles", "[Editor]"));
                options.AddPolicy("Reader", policy => policy.RequireRole("roles", "[Reader]"));
                options.AddPolicy("Assessor", policy => policy.RequireRole("roles", "[Assessor]"));
            });

            // add the CORS setup
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                    });
            });

            // add service for allowing caching of responses
            services.AddResponseCaching();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("openrmf_controls_api_path_counter", "Counts requests to OpenRMF endpoints", new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });
            // Use the Prometheus middleware
            app.UseMetricServer();
            app.UseHttpMetrics();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "OpenRMF Controls API V1");
            });

            // allow response caching directives in the API Controllers
            app.UseResponseCaching();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors(MyAllowSpecificOrigins);
            // this has to go here
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
