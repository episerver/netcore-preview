using EPiServer.Data;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPiServer.Reference.Commerce.Site.Infrastructure.Attributes
{
    public class AllowDBWriteAttribute : ActionFilterAttribute
    {
        protected Injected<IDatabaseMode> DBMode;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!CanWriteToDB())
            {
                filterContext.Result = new NotFoundResult();
            }
        }

        public bool CanWriteToDB() => DBMode.Service?.DatabaseMode != DatabaseMode.ReadOnly;
    }

}