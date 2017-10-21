using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace WebApp
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
            services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
            
            services.AddIdentity<ApplicationUser, ApplicationRole>()                
                .AddDefaultTokenProviders();
                
            services.Configure<CookieAuthenticationOptions>(options =>
            {
                options.Events= new CookieAuthenticationEvents
                {
                     
                OnRedirectToLogin = ctx =>
                {
                    ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    if ( ctx.Response.StatusCode == (int) HttpStatusCode.OK)
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    }
                    return Task.FromResult(0);
                    /*if (ctx.Request.Path.StartsWithSegments("/api") &&
                        ctx.Response.StatusCode == (int) HttpStatusCode.OK)
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    }
                    else
                    {
                        ctx.Response.Redirect(ctx.RedirectUri);
                    }
                    return Task.FromResult(0);
                    */
                }
                };
                options.LoginPath = PathString.FromUriComponent("/Auth/Login");
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
               // options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
               
                //options.ReturnUrlParameter = CookieAuthenticationDefaults..ReturnUrlParameter;
            });

/**/
            
            

           /* services.ConfigureApplicationCookie(options =>
            {
                
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;                
               // options.
                // Requires `using Microsoft.AspNetCore.Authentication.Cookies;`
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            });*/
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

           
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
