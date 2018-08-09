using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoteIt.Infrastructure.Identity;
using NoteIt.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using NoteIt.Configuration.Requirements;
using NoteIt.Infrastructure.Services;
using NoteIt.Core.Services;

namespace NoteIt
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });           

            if(Configuration["ASPNETCORE_ENVIRONMENT"].Equals("Development"))
            {
                services.AddDbContext<ApplicationIdentityDbContext>(options =>
                {
                    options.UseInMemoryDatabase("Identity");
                });

                services.AddDbContext<NoteDbContext>(options => 
                { 
                    options.UseInMemoryDatabase("Notes");
                });               
            }
            else
            {
                services.AddDbContext<ApplicationIdentityDbContext>(options => 
                {
                    options.UseSqlServer(Configuration.GetConnectionString("Identity")); 
                });

                services.AddDbContext<NoteDbContext>(options => 
                { 
                    options.UseSqlServer(Configuration.GetConnectionString("Notes")); 
                });
            }
            
            //services.AddDefaultIdentity<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("UserOrAdmin", policy => policy.AddRequirements(new UserOrAdmin()));
            });

            services.AddScoped<IEmailSender, EmailSender>();

            services.AddLogging();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
