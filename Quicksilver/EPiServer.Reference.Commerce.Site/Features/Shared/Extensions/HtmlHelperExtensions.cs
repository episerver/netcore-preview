using EPiServer.Data;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Web;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Extensions
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent RenderReadonlyMessage(this IHtmlHelper htmlHelper)
        {
            if (ServiceLocator.Current.GetInstance<IDatabaseMode>().DatabaseMode == DatabaseMode.ReadWrite)
            {
                return htmlHelper.Raw(string.Empty);
            }
            return htmlHelper.Raw(
                $"<div class=\"container-fluid\"><div class=\"alert alert-info\" role=\"alert\"><p class=\"text-center\">{LocalizationService.Current.GetString("/Readonly/Message")}</p></div></div>");
        }
    }
}