using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Ithoot.Plugin.Order.OneClick.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class OrderOneClickAdminController : BasePluginController
    {
        #region Fields

        private readonly OrderOneClickSettings _oneClickSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;

        public OrderOneClickAdminController(
            OrderOneClickSettings oneClickSettings,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreService storeService,
            INotificationService notificationService,
            IWorkContext workContext,
            ILocalizedModelFactory localizedModelFactory)
        {
            _oneClickSettings = oneClickSettings;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _notificationService = notificationService;
            _localizedModelFactory = localizedModelFactory;
        }


        #endregion

        #region Ctor



        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                NotificationEmails = _oneClickSettings.NotificationEmails,
                Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<ConfigurationModel.OrderOneClickLocalizedModel>(async (locale, languageId) =>
                {
                    locale.LanguageId = languageId;
                    var translation = await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Order.OneClick.ButtonName.Locale", languageId, true);
                    locale.ButtonName = translation != null ? translation.ResourceValue : "";

                    var translation1 = await _localizationService.GetLocaleStringResourceByNameAsync("Plugins.Order.OneClick.SuccessNotifyMessage.Locale", languageId, true);
                    locale.SuccessNotifyMessage = translation != null ? translation.ResourceValue : "";
                })
            };

            return View("~/Plugins/Order.OneClick/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            try
            {
                _oneClickSettings.NotificationEmails = model.NotificationEmails;

                foreach (var localized in model.Locales)
                {
                    await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                    {
                        ["Plugins.Order.OneClick.ButtonName.Locale"] = string.IsNullOrEmpty(localized.ButtonName) ? model.ButtonName : localized.ButtonName,
                        ["Plugins.Order.OneClick.SuccessNotifyMessage.Locale"] = string.IsNullOrEmpty(localized.SuccessNotifyMessage) ? model.SuccessNotifyMessage : localized.SuccessNotifyMessage,
                    }, localized.LanguageId);
                }

                await _settingService.SaveSettingAsync(_oneClickSettings);
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