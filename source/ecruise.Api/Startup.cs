using System;
using ecruise.Api.Middleware;
using ecruise.Database.Models;
using ecruise.Models;
using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RazorLight.MVC;

// ReSharper disable UnusedMember.Global
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedParameter.Global

namespace ecruise.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            // Schedule background tasks
            JobManager.Initialize(new BackgroundTasker().ScheduleAllTasks());
        }

        private IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddCors();

            // Add DbContext via DI
            services.AddDbContext<EcruiseContext>(options =>
                options.UseMySql(Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                                 Configuration.GetConnectionString("ecruiseMySQL")));

            // Add Razor Light Engine
            services.AddRazorLight("/MailTemplates");

            // Add MVC Service
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            EcruiseContext ecruiseContext)
        {
            // Add logger
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug(LogLevel.Debug);

            // Add forward headers for compatibility with ngnix
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Add CORS to every Request
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // global exception handler

            #region Exception Handler

            app.UseExceptionHandler(builder =>
            {
                builder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    if (error?.Error != null)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new Error(101,
                            "An unexpected exception occured: " + error.Error.Message, error.Error.StackTrace)));
                    }
                    else
                    {
                        await next();
                    }
                });
            });

            #endregion

            // use authentification middleware
            app.UseMiddleware<EcruiseAuthenticationMiddleware>();

            // use MVC
            app.UseMvc();
        }
    }
}
