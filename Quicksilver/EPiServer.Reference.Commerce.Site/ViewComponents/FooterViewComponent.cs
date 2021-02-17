using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Navigation.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using EPiServer.SpecializedProperties;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IContentLoader _contentLoader;

        public FooterViewComponent(IContentLoader contentLoader)
        {
            _contentLoader = contentLoader;
        }

        public IViewComponentResult Invoke()
        {
            var viewModel = new FooterViewModel
            {
                FooterLinks = _contentLoader.Get<StartPage>(ContentReference.StartPage).FooterLinks ?? new LinkItemCollection()
            };

            return View(viewModel);
        }
    }
}
