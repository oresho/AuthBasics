using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                //Check cookie to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";
                //when we sign in we will deal out a cookie
                config.DefaultSignInScheme = "ClientCookie";
                //Use this to check if user is allowed to do something
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer", config =>
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    //url the user is redirected to when auth is complete
                    config.CallbackPath = "/oauth/callback";
                    // Auth endpoint redirects us to our auth server where we auth the user
                    config.AuthorizationEndpoint = "https://localhost:62041/oauth/authorize";
                    config.TokenEndpoint = "https://localhost:62041/oauth/token";

                });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
