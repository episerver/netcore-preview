using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Cart.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    [AllowAnonymous]
    public class MultiShipmentController : PageController<MultiShipmentPage>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IAddressBookService _addressBookService;
        private readonly LocalizationService _localizationService;
        private readonly MultiShipmentViewModelFactory _checkoutViewModelFactory;
        private readonly IContentRepository _contentRepository;

        private ICart _cart;

        public MultiShipmentController(
            IOrderRepository orderRepository,
            MultiShipmentViewModelFactory checkoutViewModelFactory,
            ICartService cartService,
            IAddressBookService addressBookService,
            LocalizationService localizationService, 
            IContentRepository contentRepository)
        {
            _orderRepository = orderRepository;
            _checkoutViewModelFactory = checkoutViewModelFactory;
            _cartService = cartService;
            _addressBookService = addressBookService;
            _localizationService = localizationService;
            _contentRepository = contentRepository;
        }

        [HttpGet]
        public IActionResult Index(MultiShipmentPage currentPage)
        {
            return View(_checkoutViewModelFactory.CreateMultiShipmentViewModel(Cart, User.Identity.IsAuthenticated));
        }

        [HttpPost]
        public IActionResult Index(MultiShipmentPage currentPage, MultiShipmentViewModel viewModel)
        {
            for (var i = 0; i < viewModel.CartItems.Length; i++)
            {
                if (string.IsNullOrEmpty(viewModel.CartItems[i].AddressId))
                {
                    ModelState.AddModelError($"CartItems[{i}].AddressId", _localizationService.GetString("/Checkout/MultiShipment/Empty/AddressId"));
                }
            }

            if (!ModelState.IsValid)
            {
                return View(_checkoutViewModelFactory.CreateMultiShipmentViewModel(Cart, User.Identity.IsAuthenticated));
            }
            
            _cartService.RecreateLineItemsBasedOnShipments(Cart, viewModel.CartItems, GetAddresses(viewModel));

            _orderRepository.Save(Cart);

            var parentPage = _contentRepository.Get<PageData>(currentPage.ParentLink);
            return Redirect(new UrlBuilder(parentPage.LinkURL).ToString());
        }

        private ICart Cart => _cart ?? (_cart = _cartService.LoadCart(_cartService.DefaultCartName));

        private IList<AddressModel> GetAddresses(MultiShipmentViewModel viewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                var addresses = new List<AddressModel>();

                foreach (var addressId in viewModel.CartItems.Select(x => x.AddressId).Distinct())
                {
                    var address = new AddressModel { AddressId = addressId };
                    _addressBookService.LoadAddress(address);
                    addresses.Add(address);
                }

                return addresses;
            }

            return _addressBookService.MergeAnonymousShippingAddresses(viewModel.AvailableAddresses, viewModel.CartItems);
        }
    }
}