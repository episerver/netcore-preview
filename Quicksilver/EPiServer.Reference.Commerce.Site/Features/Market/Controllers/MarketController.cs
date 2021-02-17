using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.Web.Routing;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Controllers
{
   
    public class MarketController : Controller
    {
        private readonly IMarketService _marketService;
        private readonly ICurrentMarket _currentMarket;
        private readonly UrlResolver _urlResolver;
        private readonly LanguageService _languageService;
        private readonly ICartService _cartService;
        private readonly ICurrencyService _currencyService;
        private readonly IContentRouteHelper _contentRouteHelper;

        public MarketController(
            IMarketService marketService,
            ICurrentMarket currentMarket,
            UrlResolver urlResolver,
            LanguageService languageService,
            ICartService cartService,
            ICurrencyService currencyService,
            IContentRouteHelper contentRouteHelper)
        {
            _marketService = marketService;
            _currentMarket = currentMarket;
            _urlResolver = urlResolver;
            _languageService = languageService;
            _cartService = cartService;
            _currencyService = currencyService;
            _contentRouteHelper = contentRouteHelper;
        }

     
        [HttpPost]
        public ActionResult Set(string marketId, ContentReference contentLink)
        {
            var newMarketId = new MarketId(marketId);
            _currentMarket.SetCurrentMarket(newMarketId);
            var currentMarket = _marketService.GetMarket(newMarketId);
            var cart = _cartService.LoadCart(_cartService.DefaultCartName);

            if (cart != null && cart.Currency != null)
            {
                _currencyService.SetCurrentCurrency(cart.Currency);
            }
            else
            {
                _currencyService.SetCurrentCurrency(currentMarket.DefaultCurrency);
            }

            _languageService.SetRoutedContent(_contentRouteHelper.Content, currentMarket.DefaultLanguage.Name);

            var returnUrl = _urlResolver.GetUrl(Request, contentLink, currentMarket.DefaultLanguage.Name);
            return Json(new { returnUrl });
        }
    }
}