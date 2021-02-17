using Adyen;
using Adyen.Model.Checkout;
using EPiServer.Commerce.Order;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods
{
    public class AdyenPaymentGateway : IPaymentPlugin
    {
        private readonly ILogger<AdyenPaymentGateway> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderNumberGenerator _orderNumberGenerator;
        private readonly Adyen.Service.Checkout _checkout;
        private readonly string _merchant_account;
        public IDictionary<string, string> Settings { get; set; }

        public AdyenPaymentGateway() : this(
            ServiceLocator.Current.GetInstance<ILogger<AdyenPaymentGateway>>(),
            ServiceLocator.Current.GetInstance<IHttpContextAccessor>(),
            ServiceLocator.Current.GetInstance<IOrderRepository>(),
            ServiceLocator.Current.GetInstance<IOrderNumberGenerator>(),
            ServiceLocator.Current.GetInstance<IOptions<AdyenPaymentOptions>>()
            )
        {
        }

        public AdyenPaymentGateway(ILogger<AdyenPaymentGateway> logger, IHttpContextAccessor httpContextAccessor, IOrderRepository orderRepository, IOrderNumberGenerator orderNumberGenerator, IOptions<AdyenPaymentOptions> paymentOptions)
        {
            if (string.IsNullOrWhiteSpace(paymentOptions.Value.ApiKey))
            {
                throw new ArgumentNullException($"Invalid payment configuration. {nameof(paymentOptions.Value.ApiKey)}");
            }

            if (string.IsNullOrWhiteSpace(paymentOptions.Value.MerchantAccount))
            {
                throw new ArgumentNullException($"Invalid payment configuration. {nameof(paymentOptions.Value.MerchantAccount)}");
            }

            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _orderNumberGenerator = orderNumberGenerator;
            var client = new Client(paymentOptions.Value.ApiKey, Adyen.Model.Enum.Environment.Test);
            _checkout = new Adyen.Service.Checkout(client);
            _merchant_account = paymentOptions.Value.MerchantAccount;
        }


        public PaymentProcessingResult ProcessPayment(IOrderGroup orderGroup, IPayment payment)
        {
            _logger.LogInformation($"Request for AdyenPayments API");
            var additionalPaymentData = payment.Properties["additionalPaymentData"] as Dictionary<string, string>;

            var raw = JsonConvert.DeserializeObject<Dictionary<string, object>>(additionalPaymentData["paymentData"]);

            var paymentMethodToken = Newtonsoft.Json.Linq.JObject.Parse(raw["paymentMethod"].ToString());

            var pmType = paymentMethodToken.GetValue("type").ToString();
            var pm = ParsePaymentMethodDetails(JsonConvert.SerializeObject(paymentMethodToken), pmType);

            var pmreq = new PaymentRequest
            {
                PaymentMethod = pm,
                MerchantAccount = _merchant_account,
                Channel = PaymentRequest.ChannelEnum.Web
            };

            var amount = new Amount(orderGroup.Currency.CurrencyCode, (long)(payment.Amount * 100));
            pmreq.Amount = amount;

            var merchRef = DateTime.Now.Ticks.ToString();
            payment.Properties["Reference"] = merchRef; // A unique reference number for each transaction
            pmreq.Reference = merchRef;

            if (raw.ContainsKey("browserInfo"))
            {
                pmreq.BrowserInfo = JsonConvert.DeserializeObject<BrowserInfo>(JsonConvert.SerializeObject(raw["browserInfo"]));
            }

            pmreq.ShopperIP = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(); // required by some issuers for 3ds2

            try
            {
                var res = _checkout.Payments(pmreq);
                _logger.LogInformation($"Response for Payment API::\n{res.ResultCode}\n");
                if (res.Action != null && res.Action.PaymentData != "")
                {
                    _logger.LogInformation($"Setting payment data cache for {merchRef}\n");
                    return PaymentProcessingResult.CreateSuccessfulResult("Done");
                }
                else
                {
                    var dict = new Dictionary<string, string>()
                    {
                        { "pspReference", res.PspReference },
                        { "resultCode", res.ResultCode.ToString() },
                        { "refusalReason", res.RefusalReason }
                    };

                    if (res.ResultCode == PaymentsResponse.ResultCodeEnum.Authorised)
                    {
                        return PaymentProcessingResult.CreateSuccessfulResult(string.Empty);
                    }
                    else
                    {
                        return PaymentProcessingResult.CreateUnsuccessfulResult(res.RefusalReason);
                    }
                }
            }
            catch (Adyen.HttpClient.HttpClientException e)
            {
                _logger.LogError($"Request for Payments failed::\n{e.ResponseBody}\n");
                return PaymentProcessingResult.CreateUnsuccessfulResult(e.Message);
            }
        }

        private IPaymentMethodDetails ParsePaymentMethodDetails(string pm, string type)
        {
            switch (type)
            {
                case "ideal":
                    return JsonConvert.DeserializeObject<IdealDetails>(pm);
                case "dotpay":
                    return JsonConvert.DeserializeObject<DotpayDetails>(pm);
                case "giropay":
                    return JsonConvert.DeserializeObject<GiropayDetails>(pm);
                case "ach":
                    return JsonConvert.DeserializeObject<AchDetails>(pm);
                default:
                    return JsonConvert.DeserializeObject<DefaultPaymentMethodDetails>(pm);
            }
        }
    }
}
