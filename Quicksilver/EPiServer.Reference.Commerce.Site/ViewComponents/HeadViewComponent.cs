using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Start.Extensions;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class HeadViewComponent : ViewComponent
    {
        private readonly IContentLoader _contentLoader;
        private readonly IContentRouteHelper _contentRouteHelper;
        private const string FormatPlaceholder = "{title}";

        public HeadViewComponent(IContentLoader contentLoader, IContentRouteHelper contentRouteHelper)
        {
            _contentLoader = contentLoader;
            _contentRouteHelper = contentRouteHelper;
        }

        public IViewComponentResult Invoke()
        {
            var content = _contentRouteHelper.Content;
            if (content == null)
            {
                return Content("");
            }

            if (content is EntryContentBase product)
            {
                // Note: If this product is placed in more than one category, we might pick the wrong category here
                var parentContent = _contentLoader.Get<CatalogContentBase>(content.ParentLink);

                var title = parentContent is NodeContent node ? node.SeoInformation.Title.NullIfEmpty() ?? node.DisplayName : parentContent.Name;
                return Content(FormatTitle($"{product.SeoInformation.Title.NullIfEmpty() ?? product.DisplayName} - {title}"));
            }

            if (content is NodeContent category)
            {
                return Content(FormatTitle(category.SeoInformation.Title.NullIfEmpty() ?? category.DisplayName) ?? "");
            }

            return content is StartPage startPage ?
                Content(startPage.Title.NullIfEmpty() ?? startPage.Name) :
                Content(content.Name);
        }

        private string FormatTitle(string title)
        {
            var format = _contentLoader.Get<StartPage>(ContentReference.StartPage).TitleFormat;
            return string.IsNullOrWhiteSpace(format) || !format.Contains(FormatPlaceholder) ? title : format.Replace(FormatPlaceholder, title);
        }
    }
}
