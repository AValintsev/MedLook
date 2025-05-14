using Nop.Core;
using Nop.Plugin.Widgets.CategorySlider.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CategorySlider
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class CategorySliderPlugin : BasePlugin, IWidgetPlugin
    {
        public static string CacheKeyBase = "CategorySlider.";
        public static string BlockNameKey = "Plugins.Widgets.CategorySlider.Name.Locale";

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        string widgetZoneName = "ProductsByCategory";

        public CategorySliderPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the widget zones
        /// </returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { widgetZoneName });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsCategorySlider/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(WidgetsCategorySliderViewComponent);
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new CategorySliderSettings
            {

            };
            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.CategorySlider.Count"] = "Кількість продуктів у слайдері",
                ["Plugins.Widgets.CategorySlider.CategoryId"] = "Категорія",
                ["Plugins.Widgets.CategorySlider.CategoryId.Hint"] = "Продукти із цієї категорії будуть відображатись в слайдері. ",
                ["Plugins.Widgets.CategorySlider.Homepage.CategorySliderProducts"] = "Продукти з категорії",
                ["Plugins.Widgets.CategorySlider.Name"] = "Назва слайдеру (назва блоку)",
                [BlockNameKey] = $"Змініть назву категорії. Ключ {BlockNameKey}",
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
            await _settingService.DeleteSettingAsync<CategorySliderSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Widgets.CategorySlider");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}