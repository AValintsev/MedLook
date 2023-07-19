using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nop.Core.Infrastructure;
using Nop.Ithoot.Plugin.Misc.PDF.Services;
using Nop.Services.Common;

namespace Nop.Ithoot.Plugin.Misc.PDF.Infrastructure
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
								typeof(IPdfService),
								typeof(FopPdfService),
								ServiceLifetime.Transient);
			services.Replace(npAttrParserDescriptor);		
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