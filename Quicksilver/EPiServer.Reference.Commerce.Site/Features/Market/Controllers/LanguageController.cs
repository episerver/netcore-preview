using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Controllers
{
    public class LanguageController : Controller
    {
        private readonly LanguageService _languageService;
        private readonly UrlResolver _urlResolver;
        private readonly IContentRouteHelper _contentRouteHelper;

        public LanguageController(LanguageService languageService, UrlResolver urlResolver, IContentRouteHelper contentRouteHelper)
            
        {
            _languageService = languageService;
            _urlResolver = urlResolver;
            _contentRouteHelper = contentRouteHelper;
        }

        [HttpPost]
        public ActionResult Set(string language, ContentReference contentLink)
        {
            _languageService.SetRoutedContent(_contentRouteHelper.Content, language);

            var returnUrl = _urlResolver.GetUrl(Request, contentLink, language);
            return Json(new { returnUrl });
        }
    }
}