using EPiServer.Globalization;
using EPiServer.Reference.Commerce.Site.Features.Shared.Extensions;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    public class AJAXLocalizationFilterAttribute : ActionFilterAttribute
    {
        private Injected<IUpdateCurrentLanguage> _currentLanguageUpdater = default(Injected<IUpdateCurrentLanguage>);

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                _currentLanguageUpdater.Service.SetRoutedContent(null, null);
            }
        }
    }
}