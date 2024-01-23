using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Ithoot.Plugin.Payments.Monobank.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    public class MonobankController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public MonobankController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion        

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<MonobankSettings>(storeId);

            var model = new ConfigurationModel
            {
                IsConfigured = !string.IsNullOrEmpty(settings.Token),
                Token = settings.Token,
                WebhookUrl = settings.WebhookUrl,
                Validity = settings.Validity,
                TerminalCode = settings.TerminalCode,
                QrId = settings.QrId,
                CMS = settings.Cms,
                CMSVersion = settings.CmsVersion,
                RequestTimeout = settings.RequestTimeout
            };

            return View("~/Plugins/Payments.Monobank/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<MonobankSettings>(storeId);

            settings.Token = model.Token;
            settings.WebhookUrl = model.WebhookUrl;
            settings.QrId = model.QrId;
            settings.Validity = model.Validity;
            settings.Cms = model.CMS;
            settings.CmsVersion = model.CMSVersion;
            settings.TerminalCode = model.TerminalCode;
            settings.RequestTimeout = model.RequestTimeout;

            await _settingService.SaveSettingAsync(settings);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}