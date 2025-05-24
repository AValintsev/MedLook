using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Ithoot.Plugin.Order.OneClick.Infrastructure
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
            endpointRouteBuilder.MapControllerRoute("OrderOneClickOrder",
                "/OrderOneClick/Order/",
                new { controller = "OrderOneClick", action = "Order" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
