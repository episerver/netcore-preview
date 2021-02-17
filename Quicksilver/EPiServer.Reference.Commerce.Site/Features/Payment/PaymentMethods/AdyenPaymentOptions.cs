using EPiServer.ServiceLocation;

namespace EPiServer.Reference.Commerce.Site.Features.Payment.PaymentMethods
{
    [Options(ConfigurationSection = ConfigurationSectionConstants.Commerce)]
    public class AdyenPaymentOptions
    {
        public string ApiKey { get; set; }
        public string MerchantAccount { get; set; }
        public string ClientKey { get; set; }
    }
}
