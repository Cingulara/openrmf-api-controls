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
                {context.UseInMemoryDatabase("ControlSet"); });  
            
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
                LoadControlsXML(serviceScope.ServiceProvider.GetService<ControlsDBContext>());
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

        private void LoadControlsXML(ControlsDBContext context) {
            List<Control> controls = Classes.ControlsLoader.LoadControls();
            // for each one, load into the in-memory DB
            ControlSet cs;
            foreach (Control c in controls) {
                cs = new ControlSet(); // the flattened controls table listing for the in memory DB
                cs.family = c.family;
                cs.highimpact = c.highimpact;
                cs.moderateimpact = c.moderateimpact;
                cs.lowimpact = c.lowimpact;
                cs.number = c.number;
                cs.priority = c.priority;
                cs.title = c.title;
                if (!string.IsNullOrEmpty(c.supplementalGuidance))
                    cs.supplementalGuidance = c.supplementalGuidance.Replace("\\r","").Replace("\\n","");
                if (c.childControls.Count > 0)
                {
                    foreach (ChildControl cc in c.childControls) {
                        cs.id = Guid.NewGuid(); // need a new PK ID for each record saved
                        if (!string.IsNullOrEmpty(cc.description))
                            cs.subControlDescription = cc.description.Replace("\\r","").Replace("\\n","");
                        cs.subControlNumber = cc.number.Replace(" ", "").Replace(".",""); // remove periods and empty space for searching later
                        context.ControlSets.Add(cs); // for each sub control, do a save on the whole thing
                        Console.WriteLine("Adding number " + cs.subControlNumber);
                        context.SaveChanges();
                    }
                }
                else {
                    cs.id = Guid.NewGuid();
                    context.ControlSets.Add(cs); // for some reason no sub controls
                    context.SaveChanges();
                }
            }
            context.SaveChanges();
        }
    }
}
