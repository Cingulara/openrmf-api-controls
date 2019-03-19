using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

using openstig_api_controls.Models;
using openstig_api_controls.Database;

namespace openstig_api_controls
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the database components
            services.AddDbContext<ControlsDBContext>(context => 
                {context.UseInMemoryDatabase("Controls"); });  
            
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "openSTIG Controls API", Version = "v1", 
                    Description = "The Controls API that goes with the openSTIG tool",
                    Contact = new Contact
                    {
                        Name = "Dale Bingham",
                        Email = "dale.bingham@cingulara.com",
                        Url = "https://github.com/Cingulara/openstig-api-controls"
                    } });
            });

            // ********************
            // USE CORS
            // ********************
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin() 
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ControlsDBContext>();
                // Seed the database.
                Control c = new Control();
                c.family = "ACCESS CONTROL";
                c.number = "AC-1";
                c.title = "ACCESS CONTROL POLICY AND PROCEDURES";
                c.priority = "P1";
                c.lowimpact = true;
                c.moderateimpact = true;
                c.highimpact = true;

                ChildControl cc = new ChildControl();
                cc.number = "AC-1a";
                cc.description = "Develops, documents, and disseminates to [Assignment: organization-defined personnel or roles]";
                c.childControls.Add(cc);
                cc.number = "AC-1a1";
                cc.description = "An access control policy that addresses purpose, scope, roles, responsibilities, management commitment, coordination among organizational entities, and compliance";
                c.childControls.Add(cc);
                // save it
                context.Controls.Add(c);
                context.SaveChanges();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "openSTIG Controls API V1");
            });

            // ********************
            // USE CORS
            // ********************
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();
            app.UseMvc();
        }

    }
}
