using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TheWorld.Services;
using Microsoft.Extensions.Configuration;
using TheWorld.Models;
using Newtonsoft.Json.Serialization;
using AutoMapper;
using TheWorld.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables(); //Environment Variable on project will now override the config.json

            _config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            if (_env.IsEnvironment("Development")) // From the environment variable in the project settings
            {
                services.AddScoped<IMailService, DebugMailService>(); // AddTransient Created when needed. Either recreated every time or got from cache
                                                                  // AddScoped - creates instance for each request
                                                                  // AddSingleton - create one instance the first time we need it and keep passing it in
            }
            else
            {
                // TODO: Implement Properly
            }

            services.AddIdentity<WorldUser, IdentityRole>(config =>
            {
                config.User.RequireUniqueEmail = true;
                config.Password.RequiredLength = 8;
                config.Cookies.ApplicationCookie.LoginPath = "/Auth/Login";
                config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = async ctx =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") &&
                            ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }
                        else
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                        await Task.Yield();
                    }
                };
            })
            .AddEntityFrameworkStores<WorldContext>();

            services.AddDbContext<WorldContext>();

            services.AddScoped<IWorldRepository, WorldRepository>(); // Test project could have a mocked implementation of the IWorldRepository interface to supply here

            services.AddTransient<GeoCoordsService>();

            services.AddTransient<WorldContextSeedData>(); // DI handles the WorldContext being supplied

            services.AddLogging();

            services.AddMvc // DI is required, MVC requires this first to register the services
                (
                    config =>
                    {
                        if (_env.IsProduction()) // Only want when not in Development. This (ASPNETCORE_ENVIRONMENT=Production) is the default unless otherwise specified
                        {
                            config.Filters.Add(new RequireHttpsAttribute());
                        }
                    }
                )
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Ordering matters in here
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, WorldContextSeedData seeder, ILoggerFactory factory)
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<Trip, TripViewModel>() 
                    .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.DateCreated));

                config.CreateMap<TripViewModel, Trip>() 
                    .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.Created));

                config.CreateMap<Stop, StopViewModel>().ReverseMap(); // ALSO creates map in reverse
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                factory.AddDebug(LogLevel.Information);

            }
            else
            {
                factory.AddDebug(LogLevel.Error);
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "App", action = "Index" } // If either element is not specified, these are the defaults
                    );
            });

            seeder.EnsureSeedData().Wait();
        }
    }
}
