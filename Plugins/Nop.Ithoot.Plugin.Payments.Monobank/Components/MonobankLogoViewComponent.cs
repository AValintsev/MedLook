using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Nop.Core;
using Nop.Ithoot.Plugin.Payments.Monobank;
using Nop.Services.Payments;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayPalCommerce.Components
{
    /// <summary>
    /// Represents the view component to display logo
    /// </summary>
    public class MonobankLogoViewComponent : NopViewComponent
    {
        #region Fields

        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly MonobankSettings _settings;

        #endregion

        #region Ctor

        public MonobankLogoViewComponent(IPaymentPluginManager paymentPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext,
            MonobankSettings settings)
        {
            _paymentPluginManager = paymentPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
            _settings = settings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the view component result
        /// </returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            if (!await _paymentPluginManager.IsPluginActiveAsync(MonobankDefaults.SystemName, customer, store?.Id ?? 0))
                return Content(string.Empty);

            if (string.IsNullOrEmpty(_settings.Token))
                return Content(string.Empty);

            string script = null;

            return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));
        }

        #endregion
    }
}