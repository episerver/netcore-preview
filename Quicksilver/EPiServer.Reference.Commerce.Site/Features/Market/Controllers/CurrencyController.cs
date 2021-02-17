using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Market.Services;
using Mediachase.Commerce;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Newtonsoft.Json;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICartService _cartService;
        private readonly IOrderRepository _orderRepository;

        public CurrencyController(ICurrencyService currencyService,
            ICartService cartService,
            IOrderRepository orderRepository)
        {
            _currencyService = currencyService;
            _cartService = cartService;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public ActionResult Set(string currencyCode)
        {
            if (!_currencyService.SetCurrentCurrency(currencyCode))
            {
                return new StatusCodeResult(400);
            }

            var cart = _cartService.LoadCart(_cartService.DefaultCartName);
            if (cart != null)
            {
                var currentCurrency = new Currency(currencyCode);
                if (currentCurrency != cart.Currency)
                {
                    _cartService.SetCartCurrency(cart, currentCurrency);
                    _orderRepository.Save(cart);
                }
            }

            return Json(new { returnUrl = Request.Headers["Referer"].ToString() });
        }
    }
}