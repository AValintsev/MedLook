using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nop.Core.Infrastructure;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services;
using Nop.Services.Common;
using Nop.Web.Factories;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Infrastructure
{
	/// <summary>
	/// Represents object for the configuring services on application startup
	/// </summary>
	public class NopStartup : INopStartup
	{
		/// <summary>
		/// Add and configure any of the middleware
		/// </summary>
		/// <param name="services">Collection of service descriptors</param>
		/// <param name="configuration">Configuration of the application</param>
		public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			var npAttrParserDescriptor = new ServiceDescriptor(
								typeof(IAddressAttributeParser),
								typeof(NovaPoshtaAttributeParser),
								ServiceLifetime.Transient);
			services.Replace(npAttrParserDescriptor);

			var npFactoryDescriptor = new ServiceDescriptor(
								typeof(IAddressModelFactory),
								typeof(NovaPoshtaAddressModelFactory),
								ServiceLifetime.Transient);
			services.Replace(npFactoryDescriptor);

			var npAddressFormatterDescriptor = new ServiceDescriptor(
								typeof(IAddressAttributeFormatter),
								typeof(NovaPoshtaAddressAttributeFormatter),
								ServiceLifetime.Transient);
			services.Replace(npAddressFormatterDescriptor);

			services.AddScoped<INovaPoshtaService, NovaPoshtaService>();
		}

		/// <summary>
		/// Configure the using of added middleware
		/// </summary>
		/// <param name="application">Builder for configuring an application's request pipeline</param>
		public void Configure(IApplicationBuilder application)
		{
		}

		/// <summary>
		/// Gets order of this startup configuration implementation
		/// </summary>
		public int Order => 3000;
	}
}