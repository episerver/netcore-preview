using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.ModelBinders
{
    public class DecimalModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelName = bindingContext.ModelName;
            var attemptedValue = bindingContext.ValueProvider.GetValue(modelName).FirstValue;

            // Depending on CultureInfo, the NumberDecimalSeparator can be "," or "."
            // Both "." and "," should be accepted, but aren't.
            var wantedSeperator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            var alternateSeperator = wantedSeperator == "," ? "." : ",";

            if (attemptedValue.IndexOf(wantedSeperator) == -1
                && attemptedValue.IndexOf(alternateSeperator) != -1)
            {
                attemptedValue = attemptedValue.Replace(alternateSeperator, wantedSeperator);
            }

            if (bindingContext.ModelMetadata.IsNullableValueType && string.IsNullOrWhiteSpace(attemptedValue))
            {
                return null;
            }

            try
            {
                var result = decimal.Parse(attemptedValue, NumberStyles.Any);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (FormatException e)
            {
                bindingContext.ModelState.AddModelError(modelName, e, bindingContext.ModelMetadata);
            }

            return Task.CompletedTask;
        }
    }
}