using System;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class HttpRequestExtensions
    {
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Headers != null ? request.Headers["X-Requested-With"] == "XMLHttpRequest" : false;
        }
    }
}