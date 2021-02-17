using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using Mediachase.Commerce.Markets;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    public class OrderConfirmationMailPageController : OrderConfirmationControllerBase<OrderConfirmationMailPage>
    {
        private readonly IContextModeResolver _contextModeResolver;

        public OrderConfirmationMailPageController(
            ConfirmationService confirmationService, 
            IAddressBookService addressBookService, 
            CustomerContextFacade customerContextFacade, 
            IOrderGroupCalculator orderGroupCalculator,
            IMarketService marketService,
            IContextModeResolver contextModeResolver)
            : base(confirmationService, addressBookService, customerContextFacade, orderGroupCalculator, marketService)
        {
            _contextModeResolver = contextModeResolver;
        }

        [HttpGet]
        public IActionResult Index(OrderConfirmationMailPage currentPage, int? orderNumber)
        {
            IPurchaseOrder order;
            if (_contextModeResolver.CurrentMode == ContextMode.Edit) 
            {
                order = ConfirmationService.CreateFakePurchaseOrder();
            }
            else
            {
                order = ConfirmationService.GetOrder(orderNumber.Value);
                if (order == null)
                {
                    return Redirect(Url.ContentUrl(ContentReference.StartPage));
                }
            }
            
            var viewModel = CreateViewModel(currentPage, order);

            return View(viewModel);
        }
    }
}