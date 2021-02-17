using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Checkout.Models;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Reference.Commerce.Site.Features.Checkout.Services
{
    [ServiceConfiguration(Lifecycle = ServiceInstanceScope.Singleton)]
    public class ShippingManagerFacade
    {
        private readonly IEnumerable<IShippingPlugin> _shippingPlugins;
        private readonly IEnumerable<IShippingGateway> _shippingGateways;

        public ShippingManagerFacade(IEnumerable<IShippingPlugin> shippingPlugins, IEnumerable<IShippingGateway> shippingGateways)
        {
            _shippingPlugins = shippingPlugins;
            _shippingGateways = shippingGateways;
        }

        public virtual IList<ShippingMethodInfoModel> GetShippingMethodsByMarket(string marketid, bool returnInactive)
        {
            var methods = ShippingManager.GetShippingMethodsByMarket(marketid, returnInactive);
            return methods.ShippingMethod.Select(method => new ShippingMethodInfoModel
            {
                MethodId = method.ShippingMethodId,
                Currency = method.Currency,
                LanguageId = method.LanguageId,
                Ordering = method.Ordering,
                ClassName = methods.ShippingOption.FindByShippingOptionId(method.ShippingOptionId).ClassName
            }).ToList();
        }

        public virtual ShippingRate GetRate(IShipment shipment, ShippingMethodInfoModel shippingMethodInfoModel, IMarket currentMarket)
        {
            var type = Type.GetType(shippingMethodInfoModel.ClassName);
            if (type == null)
            {
                throw new TypeInitializationException(shippingMethodInfoModel.ClassName, null);
            }
            string message = string.Empty;
            var shippingPlugin = _shippingPlugins.FirstOrDefault(s => s.GetType() == type);
            if (shippingPlugin != null)
            {
                return shippingPlugin.GetRate(currentMarket, shippingMethodInfoModel.MethodId, shipment, ref message);
            }

            var shippingGateway = _shippingGateways.FirstOrDefault(s => s.GetType() == type);
            if (shippingGateway != null)
            {
                return shippingGateway.GetRate(currentMarket, shippingMethodInfoModel.MethodId, (Shipment)shipment, ref message);
            }
            throw new InvalidOperationException($"There is no registered {nameof(IShippingPlugin)} or {nameof(IShippingGateway)} instance.");
        }
    }
}