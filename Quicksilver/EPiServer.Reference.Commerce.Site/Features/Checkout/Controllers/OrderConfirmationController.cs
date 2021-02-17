using EPiServer.Commerce.Order;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Reference.Commerce.Site.Features.AddressBook.Services;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Services;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Infrastructure.Facades;
using EPiServer.Web;
using EPiServer.Web.Mvc.Html;
using Mediachase.Commerce.Markets;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Controllers
{
    public class OrderConfirmationController : OrderConfirmationControllerBase<OrderConfirmationPage>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IContextModeResolver _contextModeResolver;

        public OrderConfirmationController(
            ConfirmationService confirmationService,
            IAddressBookService addressBookService,
            CustomerContextFacade customerContextFacade,
            IOrderGroupCalculator orderGroupCalculator,
            IMarketService marketService,
            IRecommendationService recommendationService,
            IContextModeResolver contextModeResolver)
            : base(confirmationService, addressBookService, customerContextFacade, orderGroupCalculator, marketService)
        {
            _recommendationService = recommendationService;
            _contextModeResolver = contextModeResolver;
        }

        [HttpGet]
        public async Task<ActionResult> Index(OrderConfirmationPage currentPage, string notificationMessage, int? orderNumber)
        {
            IPurchaseOrder order = null;
            if(_contextModeResolver.CurrentMode == ContextMode.Edit)
            {
                order = ConfirmationService.CreateFakePurchaseOrder();
            }
            else if (orderNumber.HasValue)
            {
                order = ConfirmationService.GetOrder(orderNumber.Value);

                if (order != null)
                {
                    await _recommendationService.TrackOrderAsync(HttpContext, order);
                }
            }

            if (order != null && order.CustomerId == CustomerContext.CurrentContactId)
            {
                var viewModel = CreateViewModel(currentPage, order);
                viewModel.NotificationMessage = notificationMessage;

                return View(viewModel);
            }

            return Redirect(Url.ContentUrl(ContentReference.StartPage));
        }
    }
}