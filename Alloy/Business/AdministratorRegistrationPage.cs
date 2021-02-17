using AlloyMvcTemplates.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AlloyTemplates
{
    internal class AdministratorRegistrationStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<AdministratorRegistrationPageMiddleware>();
                next(builder);
            };
        }
    }
}
