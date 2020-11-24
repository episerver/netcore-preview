using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EPiServer.ServiceLocation;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using System.IO;
using EPiServer.Cms.UI.AspNetIdentity;
using AlloyMvcTemplates.Extensions;
using EPiServer.Web.Routing;
using EPiServer.Framework.Web.Resources;
using EPiServer.Web.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace EPiServer.Templates.Alloy.Mvc
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data\\Alloy.mdf");
            var connectionstring = _configuration.GetConnectionString("EPiServerDB") ?? $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={dbPath};Initial Catalog=alloy_netcore_preview;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";

            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionstring);
            });

            services.Configure<ClientResourceOptions>(o => o.Debug = true);
            services.AddCmsAspNetIdentity<ApplicationUser>(o =>
            {
                if (string.IsNullOrEmpty(o.ConnectionStringOptions?.ConnectionString))
                {
                    o.ConnectionStringOptions = new ConnectionStringOptions()
                    {
                        ConnectionString = connectionstring
                    };
                }
            });

            services.AddMvc();
            services.AddAlloy();
            services.AddCms();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
            });

            services.Configure<UIOptions>(uiOptions =>
            {
                uiOptions.UIShowGlobalizationUserInterface = true;
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapControllerRoute("Register", "/Register", new { controller = "Register", action = "Index" });
                endpoints.MapRazorPages();
            });
        }
    }
}
