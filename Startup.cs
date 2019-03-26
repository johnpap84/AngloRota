using AngloRota.Data;
using AngloRota.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace AngloRota
{
    public class Startup
    {

        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _config = builder.Build();
            _env = env;
        }


        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<AngloRotaContext>(cfg =>
            {
                cfg.UseSqlServer(_config.GetConnectionString("AngloRota"));
            });
            services.AddScoped<IRepository, Repository>();

            services.AddAutoMapper();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AngloRotaContext>();

            services.AddAuthentication().AddJwtBearer(cfg =>
                 {
                     cfg.RequireHttpsMetadata = true;
                     cfg.SaveToken = true;

                     cfg.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])),

                         ValidateIssuer = true,
                         ValidIssuer = _config["Tokens:Issuer"],

                         ValidateAudience = true,
                         ValidAudience = _config["Tokens:Issuer"],

                         ValidateLifetime = true,
                         ClockSkew = TimeSpan.Zero
                     };

                 });

            services.AddCors(config =>
            {
                config.AddPolicy("AngloRota", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .WithOrigins("http://localhost:4200");
                });
            });

            services.AddMvc(opt =>
            {
                if (!_env.IsProduction())
                {
                    opt.SslPort = 44344;
                }
                opt.Filters.Add(new RequireHttpsAttribute());
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(ConfigureRoutes);
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("default", "/{controller}/{action}/{id?}", new { controller = "App", Action = "Index" });
        }
    }
}