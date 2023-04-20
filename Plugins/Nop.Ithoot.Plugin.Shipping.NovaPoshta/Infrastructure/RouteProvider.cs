using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Infrastructure
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
            endpointRouteBuilder.MapControllerRoute("NovaPoshtaWirehouses", 
                "/NovaPoshta/GetWirehouses/",
                new { controller = "NovaPoshta", action = "GetWirehouses" });

            endpointRouteBuilder.MapControllerRoute("NovaPoshtaCities",
				"NovaPoshta/GetCities/",
                new { controller = "NovaPoshta", action = "GetCities" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
