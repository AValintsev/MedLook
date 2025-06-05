using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Ithoot.Plugin.Order.OneClick.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework.Mvc.Routing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick.Controllers
{
    public class OrderOneClickController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly OrderOneClickSettings _settings;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IQueuedEmailService _queuedEmailService;


        public OrderOneClickController(
            IWorkContext workContext,
            ICustomerService customerService,
            ILocalizationService localizationService,
            OrderOneClickSettings settings,
            IWorkflowMessageService workflowMessageService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            IMessageTemplateService messageTemplateService,
            IStoreContext storeContext,
            IProductService productService,
            IQueuedEmailService queuedEmailService)
        {
            _workContext = workContext;
            _customerService = customerService;
            _localizationService = localizationService;
            _settings = settings;
            _workflowMessageService = workflowMessageService;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _messageTemplateService = messageTemplateService;
            _storeContext = storeContext;
            _productService = productService;
            _queuedEmailService = queuedEmailService;
        }

        [HttpPost]
        public async Task<IActionResult> Order(OrderOneClickModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            if (await _customerService.IsGuestAsync(customer))
            {
                customer.Phone = model.Phone;
                await _customerService.UpdateCustomerAsync(customer);
            }
            else
            {
                customer.Phone ??= model.Phone;
                await _customerService.UpdateCustomerAsync(customer);
            }

            await SendOneClickOrderPlacedNotificationToSalesAsync(store, model, customer,  language);

            return Ok(await _localizationService.GetResourceAsync("Plugins.Order.OneClick.SuccessNotifyMessage"));
        }

        private async Task SendOneClickOrderPlacedNotificationToSalesAsync(Store store, OrderOneClickModel model, Customer customer, Core.Domain.Localization.Language language)
        {
            var phone = model.Phone;
            var customerEditUrl = Url.Action("Edit", "Customer", new { area = "Admin", id = customer.Id }, Request.Scheme);

            string productUrl = Url.RouteUrl<Product>(new { SeName = model.ProductSeName }, Request.Scheme);
            string productName = model.ProductSeName;

            var customerName = (await _customerService.IsGuestAsync(customer)) ?
                await _localizationService.GetResourceAsync("customer.guest", language.Id) :
                customer.Username ?? customer.FirstName ?? customer.LastName;

            var commonTokens = new List<Token>() {
                new Token("Customer.Phone", phone),
                new Token("Store.Name", await _localizationService.GetLocalizedAsync(store, x => x.Name)),
                new Token("Store.URL", store.Url, true),
                new Token("Customer.URL", customerEditUrl),
                new Token("Customer.Name", customerName),
                new Token("Product.Url", productUrl),
                new Token("Product.Name", productName),
                new Token("Product.Size", model.ProductSize)
            };

            var messageTemplate = await _messageTemplateService.GetMessageTemplatesByNameAsync(MessageTemplateSystemNames.OneClickOrderPlaced);

            var salesPersonsEmails = _settings.NotificationEmails.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in salesPersonsEmails)
            {
                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ??
                                  (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

                var orderPlacedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendNotificationAsync(
                    messageTemplate.First(),
                    emailAccount,
                    language.Id,
                    commonTokens,
                    email.Trim(),
                    null);
            }
        }
    }
}
