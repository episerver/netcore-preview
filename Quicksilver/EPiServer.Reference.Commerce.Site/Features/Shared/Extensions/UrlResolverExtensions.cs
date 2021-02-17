using System.Web;
using EPiServer.Core;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class UrlResolverExtensions
    {
        public static string GetUrl(this UrlResolver urlResolver, HttpRequest request, ContentReference contentLink, string language)
        {
            if (!ContentReference.IsNullOrEmpty(contentLink))
            {
                return urlResolver.GetUrl(contentLink, language);
            }

            return request.GetTypedHeaders().Referer == null ? "/" : request.GetTypedHeaders().Referer.PathAndQuery;
        }
    }
}