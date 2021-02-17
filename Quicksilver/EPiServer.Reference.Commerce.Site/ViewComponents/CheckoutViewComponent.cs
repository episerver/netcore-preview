using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModelFactories;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class CheckoutViewComponent : ViewComponent
    {
        private ICart _cart;
        private readonly ICartService _cartService;
        private readonly OrderSummaryViewModelFactory _orderSummaryViewModelFactory;

        public CheckoutViewComponent(ICartService cartService, OrderSummaryViewModelFactory orderSummaryViewModelFactory)
        {
            _cartService = cartService;
            _orderSummaryViewModelFactory = orderSummaryViewModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var viewModel = _orderSummaryViewModelFactory.CreateOrderSummaryViewModel(Cart);
            return View(viewModel);
        }

        private ICart Cart => _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName));
    }
}
