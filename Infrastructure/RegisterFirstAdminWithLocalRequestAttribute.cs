using System;
using AlloyTemplates;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AlloyMvcTemplates.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class RegisterFirstAdminWithLocalRequestAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AdministratorRegistrationPageMiddleware.IsEnabled == false)
            {
                context.Result = new NotFoundResult();
                return;
            }
        }
    }
}
