using EPiServer.ServiceLocation;
using EPiServer.Tracking.Commerce;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;

namespace EPiServer.Reference.Commerce.Site.Features.Recommendations.Services
{
    /// <summary>
    /// Tracks clicks on the recommended products.
    /// </summary>
    /// <remarks>This class uses cookie to store recommendation id.</remarks>
    public class RecommendationContext : IRecommendationContext
    {
        private static Injected<IHttpContextAccessor> _httpContextAccessor;

        /// <summary>
        /// Gets the id of the recommendation that was clicked to initiate the current request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <returns>The recommendation id, or 0 if the current request was not initiated by clicking on a recommendation.</returns>
        public long GetCurrentRecommendationId(HttpContext context)
        {
            var recommendationIdKey = "RecommendationId";
            if (context.Items.ContainsKey(recommendationIdKey))
            {
                return (long)context.Items[recommendationIdKey];
            }

            long.TryParse(GetKey(recommendationIdKey), out var recommendationId);
            context.Items.Add(recommendationIdKey, recommendationId);

            RemoveKey(recommendationIdKey);

            return recommendationId;
        }

        private static string GetKey(string key)
        {
            if (_httpContextAccessor.Service.HttpContext == null)
            {
                return null;
            }
            var cookie = _httpContextAccessor.Service.HttpContext.Request.Cookies[BuildKey(key)];

            return cookie ?? null;
        }

        private static void RemoveKey(string key)
        {
            if (_httpContextAccessor.Service.HttpContext == null)
            {
                return;
            }

            _httpContextAccessor.Service.HttpContext.Response.Cookies.Append(BuildKey(key), null, new CookieOptions { Expires = DateTime.Now.AddYears(-1) });
        }

        private static string BuildKey(string key)
        {
            return string.Format(CultureInfo.CurrentCulture, "EPiServer_Commerce_{0}", key);
        }
    }
}