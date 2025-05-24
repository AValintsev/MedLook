using Nop.Core;
using Nop.Ithoot.Plugin.Order.OneClick.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class OrderOneClickPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly OrderOneClickSettings _oneClickSettings;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILanguageService _languageService;

        private readonly IAddressAttributeService _addressAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;

        public bool HideInWidgetList { get; }

        #endregion

        #region Ctor

        public OrderOneClickPlugin(
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            ISettingService settingService,
            IShippingService shippingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            OrderOneClickSettings novaPoshtaSettings,
            IAddressAttributeService addressAttributeService,
            ICustomerActivityService customerActivityService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService)
        {
            _localizationService = localizationService;
            _shoppingCartService = shoppingCartService;
            _settingService = settingService;
            _shippingService = shippingService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _oneClickSettings = novaPoshtaSettings;
            _addressAttributeService = addressAttributeService;
            _customerActivityService = customerActivityService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods


        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/OrderOneClickAdmin/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new OrderOneClickSettings
            {

            };
            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Order.OneClick.Fields.ButtonName"] = "Назва кнопки",
                ["Plugins.Order.OneClick.ButtonName"] = "Замовити швидко",
                ["Plugins.Order.OneClick.OrderButtonName"] = "Замовити",
                ["Plugins.Order.OneClick.CancelButtonName"] = "Скасувати",
                ["Plugins.Order.OneClick.Fields.SuccessNotifyMessage"] = "Повідомленя",
                ["Plugins.Order.OneClick.SuccessNotifyMessage"] = "Дякуємо! Найближчим часом, наш консультант зв'яжеться із Вами.",
                ["Plugins.Order.OneClick.Fields.NotificationEmails"] = "E-mail для оповіщення",
                ["Plugins.Order.OneClick.Fields.Phone"] = "Телефон",
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<OrderOneClickSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Order.OneClick");

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.ProductDetailsInsideOverviewButtonsBefore
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone is null)
                throw new ArgumentNullException(nameof(widgetZone));

            //if (widgetZone.Equals(AdminWidgetZones.OrderShipmentAddButtons))
            //{
            //    return typeof(WidgetsAdminNovaPoshtaViewComponent);
            //}

            return typeof(WidgetsOrderOneClickViewComponent);
        }

        #endregion
    }
}