using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModelFactories;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private ICart _cart;
        private readonly ICartService _cartService;
        private readonly CartViewModelFactory _cartViewModelFactory;

        public CartViewComponent(ICartService cartService, CartViewModelFactory cartViewModelFactory)
        {
            _cartService = cartService;
            _cartViewModelFactory = cartViewModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var viewModel = _cartViewModelFactory.CreateLargeCartViewModel(Cart);
            return View(viewModel);
        }

        private ICart Cart
        {
            get { return _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName)); }
        }
    }
}
