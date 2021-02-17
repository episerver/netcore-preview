using Microsoft.AspNetCore.Http;
using System;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool IsLocalUrl(this string url, HttpRequest request)
        {
            Uri absoluteUri;
            return Uri.TryCreate(url, UriKind.Absolute, out absoluteUri) && String.Equals(request.Host.Value, absoluteUri.Host, StringComparison.OrdinalIgnoreCase);
        }
    }
}