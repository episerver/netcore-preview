using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Cart.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Pages;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.Models;
using EPiServer.Reference.Commerce.Site.Features.Start.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.ViewModels
{
    [Bind] 
    public class CheckoutViewModel
    {
        public const string MultiShipmentCheckoutViewName = "MultiShipmentCheckout";

        public const string SingleShipmentCheckoutViewName = "SingleShipmentCheckout";

        public StartPage StartPage { get; set; }

        public CheckoutPage CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets a collection of all coupon codes that have been applied.
        /// </summary>
        public IEnumerable<string> AppliedCouponCodes { get; set; }

        public string ReferrerUrl { get; set; }

        /// <summary>
        /// Gets or sets all existing shipments related to the current order.
        /// </summary>
        public IList<ShipmentViewModel> Shipments { get; set; } = new List<ShipmentViewModel>();

        /// <summary>
        /// Gets or sets a list of all existing addresses for the current customer and that can be used for billing and shipment.
        /// </summary>
        public IList<AddressModel> AvailableAddresses { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public AddressModel BillingAddress { get; set; }

        /// <summary>
        /// Gets or sets the payment method associated to the current purchase.
        /// </summary>
        public IPaymentMethod Payment { get; set; }

        /// <summary>
        /// Gets or sets whether the shipping address should be the same as the billing address.
        /// </summary>
        public bool UseBillingAddressForShipment { get; set; }

        /// <summary>
        /// Gets or sets the view message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets whether the user is anthenticated or anonymous.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets the name of the checkout view required depending on the number of distinct shipping addresses.
        /// </summary>
        public string ViewName
        {
            get
            {
                return Shipments.Count > 1 ? MultiShipmentCheckoutViewName : SingleShipmentCheckoutViewName;
            }
        }
        /// <summary>
        /// Gets or sets the payment redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}