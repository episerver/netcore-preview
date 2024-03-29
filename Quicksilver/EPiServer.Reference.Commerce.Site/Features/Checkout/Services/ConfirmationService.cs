﻿using System;
using System.Linq;
using EPiServer.Commerce.Order;
using EPiServer.Commerce.Order.Internal;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Services
{
    [ServiceConfiguration]
    public class ConfirmationService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentMarket _currentMarket;

        public ConfirmationService(
            IOrderRepository orderRepository,
            ICurrentMarket currentMarket)
        {
            _orderRepository = orderRepository;
            _currentMarket = currentMarket;
        }

        public IPurchaseOrder GetOrder(int orderNumber)
        {
            return _orderRepository.Load<IPurchaseOrder>(orderNumber);
        }

        public IPurchaseOrder CreateFakePurchaseOrder()
        {
            var form = new InMemoryOrderForm
            {
                Payments =
                {
                    new InMemoryPayment
                    {
                        BillingAddress = new InMemoryOrderAddress(),
                        PaymentMethodName = "CashOnDelivery"
                    }
                }
            };

            form.Shipments.First().ShippingAddress = new InMemoryOrderAddress();
            var market = _currentMarket.GetCurrentMarket();
            var purchaseOrder = new InMemoryPurchaseOrder
            {
                Currency = _currentMarket.GetCurrentMarket().DefaultCurrency,
                MarketId = market.MarketId,
                MarketName = market.MarketName,
                PricesIncludeTax = market.PricesIncludeTax,
                OrderLink = new OrderReference(0, string.Empty, Guid.Empty, typeof(IPurchaseOrder))
            };
            purchaseOrder.Forms.Add(form);

            return purchaseOrder;
        }
    }
}