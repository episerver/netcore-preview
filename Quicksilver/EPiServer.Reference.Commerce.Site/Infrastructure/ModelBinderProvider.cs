using EPiServer.Commerce.Order;
using EPiServer.Reference.Commerce.Site.Features.Payment.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Shared.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;

namespace EPiServer.Reference.Commerce.Site.Infrastructure
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        private static readonly IDictionary<Type, Type> _modelBinderTypeMappings = new Dictionary<Type, Type>
        {
            {typeof(FilterOptionViewModel), typeof(FilterOptionViewModelBinder)},
            {typeof(IPaymentMethod), typeof(PaymentMethodViewModelBinder)},
            {typeof(decimal), typeof(Features.Shared.ModelBinders.DecimalModelBinder)},
            {typeof(decimal?), typeof(Features.Shared.ModelBinders.DecimalModelBinder)}
        };

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (_modelBinderTypeMappings.ContainsKey(context.Metadata.ModelType))
            {
                return new BinderTypeModelBinder(_modelBinderTypeMappings[context.Metadata.ModelType]);
            }
            return null;
        }
    }
}