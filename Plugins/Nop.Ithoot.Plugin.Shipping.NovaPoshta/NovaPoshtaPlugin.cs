using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Components;
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
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class NovaPoshtaPlugin : BasePlugin, IWidgetPlugin
    {
        #region Fields

        private readonly NovaPoshtaSettings _novaPoshtaSettings;
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

        public NovaPoshtaPlugin(
            ILocalizationService localizationService,
            IShoppingCartService shoppingCartService,
            ISettingService settingService,
            IShippingService shippingService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            NovaPoshtaSettings novaPoshtaSettings,
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
            _novaPoshtaSettings = novaPoshtaSettings;
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
            return $"{_webHelper.GetStoreLocation()}Admin/NovaPoshtaAdmin/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Shipping.NovaPoshta.Fields.ApiKey"] = "Api key",
                ["Plugins.Shipping.NovaPoshta.Fields.City"] = "Місто відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.Warehouse"] = "Відділення відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.FirstName"] = "Ім'я відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.LastName"] = "Фамілія відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.MidName"] = "По батькові відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.Phone"] = "Телефон відправника",
                ["Plugins.Shipping.NovaPoshta.Fields.PrepaymentValue"] = "Сума передплати",


                ["Plugins.Shipping.NovaPoshta.Button.Sync"] = "Синхронізувати",
                ["Plugins.Shipping.NovaPoshta.Button.Add"] = "Зберегти та додати відправлення новою поштою",

            });

            var langs = _languageService.GetAllLanguages();

            var novaPoshtaCityAddressAttribute = new AddressAttribute
            {
                AttributeControlType = Core.Domain.Catalog.AttributeControlType.TextBox,
                DisplayOrder = 10,
                IsRequired = true,
                Name = "NovaPoshtaCity"
            };
            await _addressAttributeService.InsertAddressAttributeAsync(novaPoshtaCityAddressAttribute);

            var novaPoshtaWarehouseAddressAttribute = new AddressAttribute
            {
                AttributeControlType = Core.Domain.Catalog.AttributeControlType.DropdownList,
                DisplayOrder = 11,
                IsRequired = true,
                Name = "NovaPoshtaWarehouse"
            };
            await _addressAttributeService.InsertAddressAttributeAsync(novaPoshtaWarehouseAddressAttribute);
            foreach (var lang in langs)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(novaPoshtaCityAddressAttribute,
                   x => x.Name,
                   "Місто",
                   lang.Id);

                await _localizedEntityService.SaveLocalizedValueAsync(novaPoshtaWarehouseAddressAttribute,
                    x => x.Name,
                    "Відділеня",
                    lang.Id);
            }

            await _settingService.SaveSettingAsync(new NovaPoshtaSettings
            {
                CityAttributeId = novaPoshtaCityAddressAttribute.Id,
                WarehouseAttributeId = novaPoshtaWarehouseAddressAttribute.Id
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            if (_novaPoshtaSettings.CityAttributeId > 0)
            {
                var cityAttr = await _addressAttributeService.GetAddressAttributeByIdAsync(_novaPoshtaSettings.CityAttributeId);
                await _addressAttributeService.DeleteAddressAttributeAsync(cityAttr);
            }

            if (_novaPoshtaSettings.WarehouseAttributeId > 0)
            {
                var warehouseAttr = await _addressAttributeService.GetAddressAttributeByIdAsync(_novaPoshtaSettings.WarehouseAttributeId);
                await _addressAttributeService.DeleteAddressAttributeAsync(warehouseAttr);
            }

            //settings
            await _settingService.DeleteSettingAsync<NovaPoshtaSettings>();

            //fixed rates
            var fixedRates = await (await _shippingService.GetAllShippingMethodsAsync())
                .SelectAwait(async shippingMethod => await _settingService.GetSettingAsync(
                    string.Format(NovaPoshtaDefaults.FixedRateSettingsKey, shippingMethod.Id)))
                .Where(setting => setting != null).ToListAsync();
            await _settingService.DeleteSettingsAsync(fixedRates);

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.NovaPoshta");

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> {
                "NovaPoshtaValues",
                AdminWidgetZones.OrderShipmentAddButtons
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone is null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(AdminWidgetZones.OrderShipmentAddButtons))
            {
                return typeof(WidgetsAdminNovaPoshtaViewComponent);
            }

            return typeof(WidgetsNovaPoshtaViewComponent);
        }

        #endregion
    }
}