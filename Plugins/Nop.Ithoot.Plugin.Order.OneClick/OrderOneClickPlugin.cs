using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Ithoot.Plugin.Order.OneClick.Components;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IMessageTemplateService _messageTemplateService;


        public bool HideInWidgetList { get; }

        #endregion

        #region Ctor

        public OrderOneClickPlugin(
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            OrderOneClickSettings settings,
            IAddressAttributeService addressAttributeService,
            ICustomerActivityService customerActivityService,
            ILocalizedEntityService localizedEntityService,
            ILanguageService languageService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IMessageTemplateService messageTemplateService)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _oneClickSettings = settings;
            _addressAttributeService = addressAttributeService;
            _customerActivityService = customerActivityService;
            _localizedEntityService = localizedEntityService;
            _languageService = languageService;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _messageTemplateService = messageTemplateService;
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

            var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ??
                              (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

            var emailTemplate = new MessageTemplate
            {
                Name = MessageTemplateSystemNames.OneClickOrderPlaced,
                Subject = "%Store.Name%. New one click order placed form %Customer.Name% phone %Customer.Phone%",
                Body = $"<p>{Environment.NewLine}<br />{Environment.NewLine}A new one click order has been placed by client <a href=\"%Customer.URL%\">%Customer.Name%</a>{Environment.NewLine}<br /> phone: <a href=\"tel:%Customer.Phone%\">%Customer.Phone%</a>{Environment.NewLine}<br />{Environment.NewLine}<br />{Environment.NewLine}Requested product: <br />{Environment.NewLine}size: %Product.Size% <a href=\"%Product.Url%\">%Product.Name%</a><br />{Environment.NewLine}</p>",
                IsActive = true,
                EmailAccountId = emailAccount.Id
            };
            await _messageTemplateService.InsertMessageTemplateAsync(emailTemplate);

            var languages = await _languageService.GetAllLanguagesAsync(true);
            var ukrainianLanguage = languages.Where(l => l.UniqueSeoCode == "ua").FirstOrDefault();

            if (ukrainianLanguage != null)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(
                    emailTemplate,
                    x => x.Subject,
                    "%Store.Name%. Надійшло нове швидке замовлення від %Customer.Name% тел. %Customer.Phone%",
                    ukrainianLanguage.Id);

                await _localizedEntityService.SaveLocalizedValueAsync(
                    emailTemplate,
                    x => x.Body,
                    $"<p>{Environment.NewLine}<br />{Environment.NewLine}Нове швидке замовлення надійшло від клієнта <a href=\"%Customer.URL%\">%Customer.Name%</a> <br />{Environment.NewLine}телефон: <a href=\"tel:%Customer.Phone%\">%Customer.Phone%</a><br />{Environment.NewLine}<br />{Environment.NewLine}Товар: <br />{Environment.NewLine}розмір: %Product.Size% <a href=\"%Product.Url%\">%Product.Name%</a><br />{Environment.NewLine}</p>",
                    ukrainianLanguage.Id);
            }

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

            // email templates
            var emailTemplates = await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.OneClickOrderPlaced);
            foreach (var emailTemplate in emailTemplates)
            {
                await _messageTemplateService.DeleteMessageTemplateAsync(emailTemplate);
            }

            await base.UninstallAsync();
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.ProductDetailsInsideOverviewButtonsBefore,
                PublicWidgetZones.OrderSummaryTotals
            });
        }

        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone is null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(PublicWidgetZones.OrderSummaryTotals))
            {
                return typeof(WidgetsOrderOneClickViewComponent);
            }

            return typeof(WidgetsOrderOneClickViewComponent);
        }

        #endregion
    }
}