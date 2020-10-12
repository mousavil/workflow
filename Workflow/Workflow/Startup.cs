using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Workflow.Context;
using Workflow.Interfaces;
using Workflow.Repositories;

namespace Workflow
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<WorkflowDbContext>(options =>
            {
                var postgresSqlConnectionString = Configuration["ConnectionStrings:PostgresSqlConnection"];
                options.UseNpgsql(postgresSqlConnectionString);
                //options.EnableSensitiveDataLogging();
            });
            services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("EnabledCores", builder =>
            //    {
            //        builder.AllowAnyOrigin()
            //            .AllowAnyHeader()
            //            .AllowAnyMethod()
            //            .AllowCredentials()
            //            .Build();
            //    });
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workflow"});
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Locations");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseCors("EnabledCores");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}