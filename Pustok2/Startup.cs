using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pustok2.DAL;
using Pustok2.Models;
using Pustok2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok2
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config=config;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddControllersWithViews().AddNewtonsoftJson(x =>
            //x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddDbContext<PustokDbContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("Default"));
            });
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;//reqem olmasi mecburi olmasin
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;//default 6-di
                options.Password.RequireNonAlphanumeric = false;//simvol olsun
                //options.Lockout.DefaultLockoutTimeSpan = new TimeSpan(Int32.MaxValue, 0, 0, 0);
                //reqem mecburo
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<PustokDbContext>();
            //Bu identity-i PustokDbContext-de yaradir
            //Identity bizim user mentiqimizi olusduracaq sistemdir
            services.AddScoped<LayoutService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSession(opt =>
            {
                opt.IdleTimeout = TimeSpan.FromSeconds(59);
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LogoutPath = "/account/login";
                options.AccessDeniedPath = "/account/login";
                
                //access denied path sen sisteme login olmus birisen,senin rolun bura girmeye icaze vermir
            });
            services.AddHttpContextAccessor();
            services.AddSignalR();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();//
            app.UseRouting();
            app.UseAuthentication();//
            app.UseAuthorization();//
            app.UseSession();//
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute(
                //    name: "areas",
                //    pattern: "{area:exists}/{controller}/{action}/{id?}"
                //);
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=publisher}/{action=Index}/{id?}"
                );
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action}/{id?}"
                    );
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=account}/{action=register}/{id?}"
                    );
                endpoints.MapHub<PustokHub>("/pustokhub");
            });
        }
    }
}
