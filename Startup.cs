using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Site.Data;
using Site.Models;
using Site.Models.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using ExtCore.WebApplication;
using Microsoft.AspNetCore.Identity;
using Site.Services;
using Microsoft.Extensions.FileProviders;

namespace Site
{
    public class Startup
    {
        public IConfiguration _configuration { get; set; }
        public static string ConnectionString { get; private set; }
        public static IConfiguration AppSettings { get; private set; }

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration["ConnectionStrings:DatabaseCon"];
            AppSettings = _configuration.GetSection("AppSettings");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            // Add framework services.
            services.AddApplicationInsightsTelemetry(_configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            //services.AddDbContextPool<SiteContext>(options => options.UseSqlServer(ConnectionString));
            //services.AddDbContext<SiteContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));


            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<SiteContext>(options =>
                    {
                        options.UseSqlServer(ConnectionString);
                    });

            //services.AddAuthentication(options => options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddMvc();

            services.AddSingleton<IConfiguration>(_configuration);

            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));


            services.Configure<AuthMessageSenderOptions>(options => _configuration.GetSection("SendGridSettings").Bind(options));

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions(){
                //    HotModuleReplacement = true
                //});
            }
            else
            {
                app.UseExceptionHandler("/404");
            }

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @".well-known")),
                RequestPath = new PathString("/.well-known"),
                ServeUnknownFileTypes = true // serve extensionless file
            });

            app.UseRequestLocalization();
            app.UseMvc(Routing.RegisterRoutes);
        }
    }
}
