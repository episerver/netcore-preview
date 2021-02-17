using Adyen;
using Adyen.Model.Checkout;
using EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOptions<AdyenPaymentOptions> _adyenPaymentOptions;

        public PaymentController(IOptions<AdyenPaymentOptions> adyenPaymentOptions)
        {
            _adyenPaymentOptions = adyenPaymentOptions;
        }

        [HttpPost]
        public IActionResult SetPaymentMethod(Guid paymentMethodId)
        {
            return ViewComponent("Payment", new { paymentMethodId = paymentMethodId });
        }

        [HttpPost]
        public ActionResult<string> GetAdyenPaymentMethods([FromBody] PaymentMethodsRequest req)
        {
            if (string.IsNullOrWhiteSpace(_adyenPaymentOptions.Value.ApiKey))
            {
                throw new ArgumentNullException($"Invalid payment configuration. {nameof(_adyenPaymentOptions.Value.ApiKey)}");
            }

            if (string.IsNullOrWhiteSpace(_adyenPaymentOptions.Value.MerchantAccount))
            {
                throw new ArgumentNullException($"Invalid payment configuration. {nameof(_adyenPaymentOptions.Value.MerchantAccount)}");
            }

            var client = new Client(_adyenPaymentOptions.Value.ApiKey, Adyen.Model.Enum.Environment.Test); // Test Environment;
            var checkout = new Adyen.Service.Checkout(client);
            req.MerchantAccount = _adyenPaymentOptions.Value.MerchantAccount;
            req.Channel = PaymentMethodsRequest.ChannelEnum.Web;

            try
            {
                return checkout.PaymentMethods(req).ToJson();
            }
            catch (Adyen.HttpClient.HttpClientException e)
            {
                throw e;
            }
        }
    }
}