using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Services
{
    [ServiceConfiguration]
    public class CookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public virtual string Get(string cookie)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return null;
            }

            return _httpContextAccessor.HttpContext.Request.Cookies[cookie] == null ? null : _httpContextAccessor.HttpContext.Request.Cookies[cookie];
        }

        public virtual void Set(string cookie, string value)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var cookieBuilder = new CookieBuilder();
                var cookieOptions = cookieBuilder.Build(_httpContextAccessor.HttpContext);
                cookieOptions.Expires = DateTime.UtcNow.AddYears(1);

                _httpContextAccessor.HttpContext.Response.Cookies.Append(cookie, value, cookieOptions);
            }
        }

        public virtual void Remove(string cookie)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
            }
        }
    }
}