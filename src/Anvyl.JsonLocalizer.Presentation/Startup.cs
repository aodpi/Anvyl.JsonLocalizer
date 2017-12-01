using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Anvyl.JsonLocalizer.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (Environment.IsProduction())
            {
                services.AddDistributedRedisCache(opts =>
                {
                    opts.Configuration = "localhost";
                    opts.InstanceName = "Anvyl.JsonLocalizer";
                });
            }

            if (Environment.IsDevelopment())
                services.AddDistributedMemoryCache();

            services.Configure<JsonLocalizerOptions>(Configuration.GetSection(nameof(JsonLocalizerOptions)));
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IStringLocalizerFactory stringLocalizerFactory)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US")),
                SupportedCultures = new CultureInfo[]{
                    new CultureInfo("en-US"),
                    new CultureInfo("ro-RO")
                },
                SupportedUICultures = new CultureInfo[]{
                    new CultureInfo("en-US"),
                    new CultureInfo("ro-RO")
                }
            };

            options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());

            app.UseRequestLocalization(options);
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
