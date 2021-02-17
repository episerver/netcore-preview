using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class LanguageViewComponent : ViewComponent
    {
        private readonly LanguageService _languageService;

        public LanguageViewComponent(LanguageService languageService)
        {
            _languageService = languageService;
        }

        public IViewComponentResult Invoke(ContentReference contentLink, string language)
        {
            var model = new LanguageViewModel
            {
                Languages = _languageService.GetAvailableLanguages()
                    .Select(x => new SelectListItem
                    {
                        Selected = false,
                        Text = x.DisplayName,
                        Value = x.Name
                    }),
                Language = string.IsNullOrEmpty(language) ? _languageService.GetCurrentLanguage().Name : CultureInfo.GetCultureInfo(language).Name,
                ContentLink = contentLink
            };

            return View(model);
        }
    }
}
