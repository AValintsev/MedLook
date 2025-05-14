using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.CategorySlider.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CategorySlider.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WidgetsCategorySliderController : BasePluginController
    {
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedModelFactory _localizedModelFactory;
        private readonly IStaticCacheManager _staticCacheManager;

        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;


        public WidgetsCategorySliderController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext,
            IBaseAdminModelFactory baseAdminModelFactory,
            ILocalizedModelFactory localizedModelFactory,
            IStaticCacheManager staticCacheManager)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _localizedModelFactory = localizedModelFactory;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CategorySliderSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Count = settings.Count,
                CategoryId = settings.CategoryId,
                AvailableCategories = new List<SelectListItem>(),
                Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync<ConfigurationModel.CategorySliderLocalizedModel>(async (locale, languageId) =>
                {
                    locale.LanguageId = languageId;
                    var translation = await _localizationService.GetLocaleStringResourceByNameAsync(CategorySliderPlugin.BlockNameKey, languageId, true);
                    locale.Name = translation != null ? translation.ResourceValue : "";
                })
            };

            await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories);

            return View("~/Plugins/Widgets.CategorySlider/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<CategorySliderSettings>(storeScope);

            settings.CategoryId = model.CategoryId;
            settings.Count = model.Count;

            foreach (var localized in model.Locales)
            {
                await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
                {
                    [CategorySliderPlugin.BlockNameKey] = string.IsNullOrEmpty(localized.Name) ? model.Name : localized.Name,
                }, localized.LanguageId);
            }
            await _settingService.SaveSettingAsync(settings);

            await _staticCacheManager.RemoveAsync(new CacheKey(CategorySliderPlugin.CacheKeyBase, "ProductsOverview"));
            await _staticCacheManager.RemoveAsync(new CacheKey(CategorySliderPlugin.CacheKeyBase, "Products"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }
    }
}