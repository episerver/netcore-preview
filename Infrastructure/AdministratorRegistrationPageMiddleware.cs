using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Templates.Alloy.Mvc.Extensions;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AlloyMvcTemplates.Infrastructure
{
    public class AdministratorRegistrationPageMiddleware
    {
        private readonly RequestDelegate _next;

        private static bool _isFirstRequest = true;
        private static bool _isLocalRequest = false;
        private static string _registerUrl = VirtualPathResolver.Instance.ToAbsolute("~/Register");
        private static Lazy<bool> _isAnyUserRegistered = new Lazy<bool>(IsAnyUserRegistered);
        private static bool? _isEnabled = null;


        public static bool IsEnabled
        {
            get
            {
                if (_isEnabled.HasValue)
                {
                    return _isEnabled.Value;
                }

                var showUserRegistration = _isLocalRequest && !_isAnyUserRegistered.Value;
                if (!showUserRegistration)
                {
                    _isEnabled = false;
                }

                return showUserRegistration;
            }
            set
            {
                _isEnabled = value;
            }
        }

        public AdministratorRegistrationPageMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            if (_isFirstRequest)
            {
                _isLocalRequest = context.IsLocalRequest();
                _isFirstRequest = false;
            }

            if (context.Request.Path.StartsWithSegments("/css/css.min.css") || context.Request.Path.StartsWithSegments("/js/script.min.js"))
            {
                await _next(context);
            }

            if (IsEnabled && !context.Request.Path.StartsWithSegments(_registerUrl))
            {
                context.Response.Redirect(_registerUrl);
            }

            await _next(context);
        }

        private static bool IsAnyUserRegistered()
        {
            var provider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            var res = provider.GetAllUsersAsync(0, 1).ToListAsync().Result;
            return res?.Count() > 0;
        }
    }
}
