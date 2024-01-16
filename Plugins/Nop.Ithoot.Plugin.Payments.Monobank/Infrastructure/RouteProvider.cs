using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(MonobankDefaults.ConfigurationRouteName,
                "Admin/Monobank/Configure",
                new { controller = "Monobank", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(MonobankDefaults.WebhookRouteName,
                "Plugins/MonobankWebhook/Callback",
                new { controller = "MonobankWebhook", action = "Callback" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}