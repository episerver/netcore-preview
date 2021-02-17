using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPiServer.Reference.Commerce.Site.Features.ErrorHandling.Controllers
{
    public class ErrorHandlingController : Controller
    {
        [HttpGet]
        public ActionResult PageNotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [HttpGet]
        public ActionResult InternalError(string message = null)
        {
            Response.StatusCode = 500;
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        [PreventDirectAccess]
        public ActionResult Forbidden(string message = null)
        {
            Response.StatusCode = 403;
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        [PreventDirectAccess]
        public ActionResult OtherHttpStatusCode(int httpStatusCode)
        {
            Response.StatusCode = httpStatusCode;
            ViewBag.StatusCode = httpStatusCode;
            return View(httpStatusCode);
        }

        private class PreventDirectAccessAttribute : ActionFilterAttribute, IAuthorizationFilter
        {
            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                var value = filterContext.RouteData.Values["fromAppErrorEvent"];
                if (!(value is bool && (bool)value))
                {
                    filterContext.Result = new ViewResult { ViewName = "PageNotFound" };
                }
            }
        }
    }
}