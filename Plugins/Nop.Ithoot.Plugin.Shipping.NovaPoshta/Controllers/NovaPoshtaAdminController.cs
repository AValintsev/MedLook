using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class NovaPoshtaAdminController : BasePluginController
    {
        #region Fields

        private readonly NovaPoshtaSettings _novaPoshtaSettings;
        private readonly INovaPoshtaService _novaPoshtaService;
        private readonly CurrencySettings _currencySettings;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IShippingService _shippingService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreService _storeService;
        private readonly INotificationService _notificationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;

        #endregion

        #region Ctor

        public NovaPoshtaAdminController(CurrencySettings currencySettings,
            ICountryService countryService,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPermissionService permissionService,
            ISettingService settingService,
            IShippingService shippingService,
            IStateProvinceService stateProvinceService,
            IStoreService storeService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            NovaPoshtaSettings novaPoshtaSettings,
            INotificationService notificationService,
            INovaPoshtaService novaPoshtaService)
        {
            _currencySettings = currencySettings;
            _countryService = countryService;
            _currencyService = currencyService;
            _localizationService = localizationService;
            _measureService = measureService;
            _permissionService = permissionService;
            _settingService = settingService;
            _stateProvinceService = stateProvinceService;
            _shippingService = shippingService;
            _storeService = storeService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _novaPoshtaSettings = novaPoshtaSettings;
            _notificationService = notificationService;
            _novaPoshtaService = novaPoshtaService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ApiKey = _novaPoshtaSettings.ApiKey,

                CityId = _novaPoshtaSettings.CitySenderId,
                City = _novaPoshtaSettings.CityName,
                WarehouseId = _novaPoshtaSettings.SenderAddressId,
                Warehouse = _novaPoshtaSettings.WarehouseName,

                SenderFirstName = _novaPoshtaSettings.SenderFirstName,
                SenderLastName = _novaPoshtaSettings.SenderLastName,
                SenderMidName = _novaPoshtaSettings.SenderMidName,
                SenderPhone = _novaPoshtaSettings.SenderPhoneNumber,

                PrepaymentValue = _novaPoshtaSettings.PrepaymentValue
            };

            //show configuration tour
            if (showtour)
            {
                var customer = await _workContext.GetCurrentCustomerAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View("~/Plugins/Shipping.NovaPoshta/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            _novaPoshtaSettings.ApiKey = model.ApiKey;

            try
            {
                _novaPoshtaSettings.CitySenderId = model.CityId;
                _novaPoshtaSettings.CityName = model.City;
                _novaPoshtaSettings.SenderAddressId = model.WarehouseId;
                _novaPoshtaSettings.WarehouseName = model.Warehouse;
                _novaPoshtaSettings.PrepaymentValue = model.PrepaymentValue;

                var senderInfo = await _novaPoshtaService.GetSenderAsync();

                _novaPoshtaSettings.SenderId = senderInfo.SenderRef;
                _novaPoshtaSettings.ContactSenderId = senderInfo.ContactRef;
                
                _novaPoshtaSettings.SenderFirstName = senderInfo.SenderFirstName;
                _novaPoshtaSettings.SenderLastName = senderInfo.SenderLastName;
                _novaPoshtaSettings.SenderMidName = senderInfo.SenderMidName;
                _novaPoshtaSettings.SenderPhoneNumber = senderInfo.SenderPhoneNumber;

                await _settingService.SaveSettingAsync(_novaPoshtaSettings);
            }
            catch (Exception e)
            {
                _notificationService.ErrorNotification(e.Message);
                return await Configure();
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}