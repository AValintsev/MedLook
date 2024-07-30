using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Misc.PDF
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class PDFPlugin : BasePlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public bool HideInWidgetList { get; }

        #endregion

        #region Ctor

        public PDFPlugin(
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
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
            return $"{_webHelper.GetStoreLocation()}Admin/PDFAdmin/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            await _settingService.SaveSettingAsync(new PDFSettings());

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Misc.PDF.Fields.Title"] = "Заголовок",
                ["Plugins.Misc.PDF.Fields.FopName"] = "Назва",
                ["Plugins.Misc.PDF.Fields.FopPhone"] = "Телефон",

                ["Plugins.Misc.PDF.Fields.FopBankName"] = "Банк",
                ["Plugins.Misc.PDF.Fields.FopIBAN"] = "IBAN",
                ["Plugins.Misc.PDF.Fields.FopEDRPOU"] = "ЄДРПОУ",
                ["Plugins.Misc.PDF.Fields.FopCardNumber"] = "Карта"
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
            await _settingService.DeleteSettingAsync<PDFSettings>();
            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.PDF");

            await base.UninstallAsync();
        }

        #endregion
    }
}