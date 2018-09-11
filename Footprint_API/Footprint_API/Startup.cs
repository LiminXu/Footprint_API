using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Footprint_API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                                           JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                                           JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = "https://localhost:5002";
                //o.Audience = "scope.readaccess";
                o.SaveToken = true;
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    // Scopes supported by API as defined in Config.cs - new ApiResource... or in DB ApiScopes table
                    ValidAudiences = new List<string>() {
                        "scope.readaccess",
                        "scope.fullaccess"
                    },
                    ValidateIssuer = true
                };
            });

            services.AddAuthorization(options =>
            {
                // some examples
                options.AddPolicy("Fullaccess", policy =>
                {
                    policy.RequireClaim("scope", "scope.fullaccess");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //add this configuration for the middleware needed to validate the tokens
            app.UseAuthentication();

            //add this line to show the ERROR STATUS CODES
            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
