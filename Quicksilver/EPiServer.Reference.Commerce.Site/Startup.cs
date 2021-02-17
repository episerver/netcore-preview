using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Commerce.Security;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using EPiServer.Framework.Web.Resources;
using EPiServer.Personalization.Commerce.Tracking;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure;
using EPiServer.Reference.Commerce.Site.Infrastructure.Attributes;
using EPiServer.Reference.Commerce.Site.Infrastructure.Indexing;
using EPiServer.ServiceLocation;
using EPiServer.Tracking.Commerce;
using EPiServer.Web;
using EPiServer.Web.Internal;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Anonymous;
using Mediachase.Commerce.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace EPiServer.Reference.Commerce.Site
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
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));
            var cmsConnectionstring = _configuration.GetConnectionString("EPiServerDB");
            var commerceConnectionstring = _configuration.GetConnectionString("EcfSqlConnection");
            services.Configure<DataAccessOptions>(o =>
            {
                o.UpdateDatabaseSchema = true;

                o.SetConnectionString(cmsConnectionstring);
                o.ConnectionStrings.Add(new ConnectionStringOptions
                {
                    ConnectionString = commerceConnectionstring,
                    Name = "EcfSqlConnection"
                });
            });

            services.AddCmsAspNetIdentity<ApplicationUser>(o =>
            {
                if (string.IsNullOrEmpty(o.ConnectionStringOptions?.ConnectionString))
                {
                    o.ConnectionStringOptions = new ConnectionStringOptions
                    {
                        Name = "EcfSqlConnection",
                        ConnectionString = commerceConnectionstring
                    };
                }
            });
            
            if (_webHostingEnvironment.IsDevelopment())
            {
                services.Configure<ClientResourceOptions>(uiOptions =>
                {
                    uiOptions.Debug = true;
                });
            }
            services.Configure<UIOptions>(uiOptions =>
            {
                uiOptions.UIShowGlobalizationUserInterface = true;
            });
            services.Configure<JsonOptions>(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            //Commerce
            services.AddCommerce();
            
            //site specific
            services.Configure<IISServerOptions>(options => options.AllowSynchronousIO = true);
            services.TryAddEnumerable(Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Singleton(typeof(IFirstRequestInitializer), typeof(UsersInstaller)));
            services.AddDisplayResolutions();
            services.TryAddSingleton<IRecommendationContext, RecommendationContext>();
            services.AddSingleton<ICurrentMarket, CurrentMarket>();
            services.TryAddSingleton<ITrackingResponseDataInterceptor, TrackingResponseDataInterceptor>();
            services.AddHttpContextOrThreadScoped<SiteContext, CustomCurrencySiteContext>();
            services.AddTransient<CatalogIndexer>();
            services.TryAddSingleton<ServiceAccessor<IContentRouteHelper>>(locator => locator.GetInstance<IContentRouteHelper>);
            services.AddEmbeddedLocalization<Startup>();
            services.Configure<MvcOptions>(o =>
            {
                o.Filters.Add(new ControllerExceptionFilterAttribute());
                o.Filters.Add(new ReadOnlyFilter());
                o.Filters.Add(new AJAXLocalizationFilterAttribute());
                o.ModelBinderProviders.Insert(0, new ModelBinderProvider());
            });
            services.AddMvc().AddRazorRuntimeCompilation();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/util/Login";
            });
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAnonymousId();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "Default", pattern: "{controller}/{action}/{id?}");
                endpoints.MapContent();
            });
        }
    }

    public static class Extensions
    {
        public static void AddDisplayResolutions(this IServiceCollection services)
        {
            services.AddSingleton<StandardResolution>();
            services.AddSingleton<IpadHorizontalResolution>();
            services.AddSingleton<IphoneVerticalResolution>();
            services.AddSingleton<AndroidVerticalResolution>();
        }

    }
}
