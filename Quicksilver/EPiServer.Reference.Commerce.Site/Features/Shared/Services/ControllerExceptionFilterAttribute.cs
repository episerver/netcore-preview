using EPiServer.Data;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace EPiServer.Reference.Commerce.Site.Features.Shared.Services
{
    public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private Injected<IDatabaseMode> _databaseMode = default;
        private readonly string _actionName;

        public ControllerExceptionFilterAttribute()
        {
        }

        public ControllerExceptionFilterAttribute(string actionName) => _actionName = actionName;

        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }

            var routedAction = context.RouteData.Values["action"].ToString();
            if (!routedAction.Equals(_actionName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var model = new EmptyModelMetadataProvider();
            var result = new ViewResult
            {
                ViewName = "ControllerException",
                ViewData = new ViewDataDictionary(model, context.ModelState)
                {
                    { "Exception: ", context.Exception.GetType().Name },
                    { "MessageDetail: ", context.Exception.Message },
                    { "StatusCode: ", StatusCodes.Status500InternalServerError }
                }
            };

            context.Result = result;

            // Handle the read-only flag so that it works even in the exception-handling scenario
            if (context.Result is ViewResult viewResult)
            {
                viewResult.ViewData["IsReadOnly"] = false;
                if (_databaseMode.Service != null)
                {
                    viewResult.ViewData["IsReadOnly"] = _databaseMode.Service.DatabaseMode == DatabaseMode.ReadOnly;
                }
            }

            if (context.Result != null && !(context.Result is EmptyResult))
            {
                context.ExceptionHandled = true;
            }
        }
    }
}