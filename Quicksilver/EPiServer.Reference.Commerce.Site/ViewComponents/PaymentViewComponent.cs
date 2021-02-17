using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModelFactories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EPiServer.Reference.Commerce.Site.Views.ViewComponents
{
    public class PaymentViewComponent : ViewComponent
    {
        private readonly PaymentMethodViewModelFactory _paymentMethodViewModelFactory;

        public PaymentViewComponent(PaymentMethodViewModelFactory paymentMethodViewModelFactory)
        {
            _paymentMethodViewModelFactory = paymentMethodViewModelFactory;
        }

        public IViewComponentResult Invoke(Guid paymentMethodId)
        {
            var viewModel = _paymentMethodViewModelFactory.CreatePaymentMethodSelectionViewModel(paymentMethodId);
            return View(viewModel);
        }
    }
}
