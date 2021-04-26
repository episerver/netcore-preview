using EPiServer.Commerce.Order;
using Mediachase.Commerce.Customers;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Shared.Identity
{
    public class SiteUser
    {
        public SiteUser()
        {

        }
        /// <summary>
        /// Returns a new instance of an ApplicationUser based on a previously made purchase order.
        /// </summary>
        /// <param name="purchaseOrder"></param>
        public SiteUser(IPurchaseOrder purchaseOrder)
        {
            Addresses = new List<CustomerAddress>();

            var billingAddress = purchaseOrder.GetFirstForm().Payments.First().BillingAddress;

            if (billingAddress != null)
            {
                Email = billingAddress.Email;
                UserName = billingAddress.Email;
                FirstName = billingAddress.FirstName;
                LastName = billingAddress.LastName;

                var addressesToAdd = new HashSet<IOrderAddress>(purchaseOrder.GetFirstForm().Shipments.Select(x => x.ShippingAddress));

                foreach (var shippingAddress in addressesToAdd)
                {
                    if (shippingAddress.Id != billingAddress.Id)
                    {
                        Addresses.Add(CreateCustomerAddress(shippingAddress, CustomerAddressTypeEnum.Shipping));
                    }
                }

                Addresses.Add(CreateCustomerAddress(billingAddress, CustomerAddressTypeEnum.Billing));
            }
        }

        public List<CustomerAddress> Addresses { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string RegistrationSource { get; set; }
        public string Password { get; set; }
        public bool AcceptMarketingEmail { get; set; }

        private CustomerAddress CreateCustomerAddress(IOrderAddress orderAddress, CustomerAddressTypeEnum addressType)
        {
            var address = CustomerAddress.CreateInstance();
            address.Name = orderAddress.Id;
            address.AddressType = addressType;
            address.PostalCode = orderAddress.PostalCode;
            address.City = orderAddress.City;
            address.CountryCode = orderAddress.CountryCode;
            address.CountryName = orderAddress.CountryName;
            address.State = orderAddress.RegionName;
            address.Email = orderAddress.Email;
            address.FirstName = orderAddress.FirstName;
            address.LastName = orderAddress.LastName;
            address.Line1 = orderAddress.Line1;
            address.Line2 = orderAddress.Line2;
            address.DaytimePhoneNumber = orderAddress.DaytimePhoneNumber;
            address.EveningPhoneNumber = orderAddress.EveningPhoneNumber;
            address.RegionCode = orderAddress.RegionCode;
            address.RegionName = orderAddress.RegionName;
            return address;
        }
    }
}