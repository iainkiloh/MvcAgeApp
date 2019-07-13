using AutoMapper;
using Domain;
using Domain.Interfaces;
using Domain.Models;
using Domain.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcAgeApp.Infrastructure.Validators;
using MvcAgeApp.Models;
using System;
using System.Globalization;

namespace MvcAgeApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //uk specific culture settings for this app
            var cultureInfo = new CultureInfo("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //configure EFCore db context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:MvcAgeApp:ConnectionString"]));
          
            //inject (transient => new instance on each injection) repository instances
            services.AddTransient<ILoginAttemptRepository, LoginAttemptRepository>();
            services.AddMvc() //add fluent validators to allow to be included in model state IsValid checks
             .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserLoginViewModelValidator>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //set up AutoMapper to map from Contracts to Entities and Vice Versa
            //todo - switch over to instance based api
            Mapper.Initialize(config =>
            {
                config.CreateMap<LoginAttempt, LoginAttemptViewModel>();
                config.CreateMap<UserLoginViewModel, LoginAttempt>()
                .ForMember(dest => dest.Name, opt => opt.NullSubstitute("Not Supplied"))
                .ForMember(dest => dest.Email, opt => opt.NullSubstitute("Not Supplied"))
                .ForMember(dest => dest.LoginAttemptTime, opt => opt.MapFrom(o => DateTime.Now));
                }
            );

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=UserLogin}/{action=UserLogin}/{id?}");
            });
        }
    }
}
