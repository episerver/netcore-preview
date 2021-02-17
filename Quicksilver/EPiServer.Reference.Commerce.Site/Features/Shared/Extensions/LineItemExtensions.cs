using EPiServer.Commerce.Catalog;
using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;
using Microsoft.AspNetCore.Http;
using System;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class LineItemExtensions
    {
        private static Injected<ReferenceConverter> _referenceConverter;
        private static Injected<ThumbnailUrlResolver> _thumbnailUrlResolver;
        private static Injected<IHttpContextAccessor> _httpContextAccessor;

        public static string GetUrl(this ILineItem lineItem)
        {
            return lineItem.GetEntryContent()?.GetUrl();
        }

        public static string GetFullUrl(this ILineItem lineItem)
        {
            var rightUrl = lineItem.GetUrl();
            var baseUrl = _httpContextAccessor.Service.HttpContext.Request.Host.Value;
            return new Uri(new Uri(baseUrl), rightUrl).ToString();
        }

        public static string GetThumbnailUrl(this ILineItem lineItem)
        {
            return GetThumbnailUrl(lineItem.Code);
        }

        public static string GetThumbnailUrl(this CartItemViewModel cartItem)
        {
            return GetThumbnailUrl(cartItem.Code);
        }

        private static string GetThumbnailUrl(string code)
        {
            var entryContentLink = _referenceConverter.Service.GetContentLink(code);
            return ContentReference.IsNullOrEmpty(entryContentLink) ?
                string.Empty : 
                _thumbnailUrlResolver.Service.GetThumbnailUrl(entryContentLink, "thumbnail");
        }
    }
}