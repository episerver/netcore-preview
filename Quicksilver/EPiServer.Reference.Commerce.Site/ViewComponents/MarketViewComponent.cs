using EPiServer.Core;
using EPiServer.Reference.Commerce.Site.Features.Market.ViewModels;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class MarketViewComponent : ViewComponent
    {
        private readonly IMarketService _marketService;
        private readonly ICurrentMarket _currentMarket;

        public MarketViewComponent(IMarketService marketService, ICurrentMarket currentMarket)
        {
            _marketService = marketService;
            _currentMarket = currentMarket;
        }

        public IViewComponentResult Invoke(ContentReference contentLink)
        {
            var currentMarket = _currentMarket.GetCurrentMarket();
            var model = new MarketViewModel
            {
                Markets = _marketService.GetAllMarkets().Where(x => x.IsEnabled).OrderBy(x => x.MarketName)
                    .Select(x => new SelectListItem
                    {
                        Selected = false,
                        Text = x.MarketName,
                        Value = x.MarketId.Value
                    }),
                MarketId = currentMarket != null ? currentMarket.MarketId.Value : string.Empty,
                ContentLink = contentLink
            };

            return View(model);
        }
    }
}
