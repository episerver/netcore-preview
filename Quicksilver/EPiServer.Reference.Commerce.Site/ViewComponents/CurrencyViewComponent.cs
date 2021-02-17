using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class CurrencyViewComponent : ViewComponent
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyViewComponent(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public IViewComponentResult Invoke()
        {
            var model = new CurrencyViewModel
            {
                Currencies = _currencyService.GetAvailableCurrencies()
                    .Select(x => new SelectListItem
                    {
                        Selected = false,
                        Text = x.CurrencyCode,
                        Value = x.CurrencyCode
                    }),
                CurrencyCode = _currencyService.GetCurrentCurrency().CurrencyCode,
            };

            return View(model);
        }
    }
}
