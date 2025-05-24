using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Ithoot.Plugin.Order.OneClick.Models;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick.Controllers
{
    public class OrderOneClickController : Controller
    {
        private readonly IWorkContext _workContext;
        private readonly IEmailSender _emailSender;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly OrderOneClickSettings _settings;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;


        public OrderOneClickController(
            IWorkContext workContext,
            IEmailSender emailSender,
            ICustomerService customerService,
            ILocalizationService localizationService,
            OrderOneClickSettings settings,
            IWorkflowMessageService workflowMessageService,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings)
        {
            _workContext = workContext;
            _emailSender = emailSender;
            _customerService = customerService;
            _localizationService = localizationService;
            _settings = settings;
            _workflowMessageService = workflowMessageService;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
        }

        [HttpPost]
        public async Task<IActionResult> Order(OrderOneClickModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            if (await _customerService.IsGuestAsync(customer))
            {
                customer.Phone = model.Phone;
                await _customerService.UpdateCustomerAsync(customer);
            }

            var phone = customer.Phone;

            // send email for sales persons to notify about new quick order
            // phone must be in email
            // good to have direct link to customer, to allow sale person to impersonate as the current customer

            var salesPersonsEmails = _settings.NotificationEmails.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var email in salesPersonsEmails)
            {
                var template = new Core.Domain.Messages.MessageTemplate
                {
                    Body = "Прийшов запит на швидке замовлення %Customer.Phone%",
                    Subject = "Швидке замовлення %Customer.Phone%"
                };

                var token = new Token("Customer.Phone", phone);

                var emailAccount = await _emailAccountService.GetEmailAccountByIdAsync(_emailAccountSettings.DefaultEmailAccountId) ??
                                  (await _emailAccountService.GetAllEmailAccountsAsync()).FirstOrDefault();

                var orderPlacedCustomerNotificationQueuedEmailIds = await _workflowMessageService.SendNotificationAsync(
                    template,
                    emailAccount,
                    language.Id,
                    new List<Token>() { token },
                    email,
                    null);
            }

            return Ok(await _localizationService.GetResourceAsync("Plugins.Order.OneClick.SuccessNotifyMessage"));
        }

    }
}
