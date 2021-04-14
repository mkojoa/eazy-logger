using eazy.logger.Builders;
using eazy.logger.ui.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace eazy.logger.ui
{
    public static class UIExtention
    {
        public static IApplicationBuilder UseEazyUi(this IApplicationBuilder applicationBuilder, Action<UiOptions> options = null)
        { 
            if (applicationBuilder == null)
                throw new ArgumentNullException(nameof(applicationBuilder));

            var uiOptions = new UiOptions();
            options?.Invoke(uiOptions);

            //var scope = applicationBuilder.ApplicationServices.CreateScope();
            //var authOptions = scope.ServiceProvider.GetService<AuthorizationOptions>();
            //uiOptions.AuthType = authOptions.AuthenticationType.ToString();

            //scope.Dispose();

            return applicationBuilder.UseMiddleware<MiddlewareExtention>(uiOptions);
        }
    }
}
