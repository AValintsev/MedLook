using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Ithoot.Plugin.Misc.PDF.Models;
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

namespace Nop.Ithoot.Plugin.Misc.PDF.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PDFAdminController : BasePluginController
    {
        #region Fields

        private readonly PDFSettings _pdfSettings;
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

        public PDFAdminController(CurrencySettings currencySettings,
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
            PDFSettings pdfSettings,
            INotificationService notificationService)
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
            _pdfSettings = pdfSettings;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                Title = _pdfSettings.Title,
                FopBankName = _pdfSettings.FopBankName,
                FopCardNumber = _pdfSettings.FopCardNumber,
                FopEDRPOU = _pdfSettings.FopEDRPOU,
                FopIBAN = _pdfSettings.FopIBAN,
                FopName = _pdfSettings.FopName,
                FopPhone = _pdfSettings.FopPhone
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

            return View("~/Plugins/Misc.FopPdf/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            try
            {
                _pdfSettings.Title = model.Title;
                _pdfSettings.FopName = model.FopName;
                _pdfSettings.FopPhone = model.FopPhone;
                _pdfSettings.FopBankName = model.FopBankName;
                _pdfSettings.FopIBAN = model.FopIBAN;
                _pdfSettings.FopEDRPOU = model.FopEDRPOU;
                _pdfSettings.FopCardNumber = model.FopCardNumber;
            
                await _settingService.SaveSettingAsync(_pdfSettings);
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