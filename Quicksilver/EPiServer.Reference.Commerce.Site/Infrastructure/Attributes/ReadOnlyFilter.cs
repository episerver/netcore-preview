using EPiServer.Data;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    public class ReadOnlyFilter : ActionFilterAttribute
    {
        private Injected<IDatabaseMode> _databaseMode;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            switch(filterContext.Result)
            {
                case ViewResult view:
                    view.ViewData["IsReadOnly"] = _databaseMode.Service.DatabaseMode == DatabaseMode.ReadOnly;
                    break;
                case PartialViewResult partialView:
                    partialView.ViewData["IsReadOnly"] = _databaseMode.Service.DatabaseMode == DatabaseMode.ReadOnly;
                    break;
            }
          
            base.OnActionExecuted(filterContext);
        }
    }
}