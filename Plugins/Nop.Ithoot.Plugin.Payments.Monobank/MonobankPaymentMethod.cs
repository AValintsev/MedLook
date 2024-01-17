using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Http.Extensions;
using Nop.Ithoot.Plugin.Payments.Monobank.Components;
using Nop.Ithoot.Plugin.Payments.Monobank.Data;
using Nop.Ithoot.Plugin.Payments.Monobank.Domain;
using Nop.Ithoot.Plugin.Payments.Monobank.Models;
using Nop.Plugin.Payments.PayPalCommerce.Components;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Stores;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Payments.Monobank
{
    /// <summary>
    /// Represents a payment method implementation
    /// </summary>
    public class MonobankPaymentMethod : BasePlugin, IPaymentMethod, IWidgetPlugin
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICurrencyService _currencyService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly PaymentSettings _paymentSettings;
        private readonly MonobankSettings _settings;
        private readonly WidgetSettings _widgetSettings;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly IWebHelper _webHelper;

        private readonly MediaSettings _mediaSettings;
        private readonly HttpClient _httpClient;

        #endregion

        #region Ctor

        public MonobankPaymentMethod(CurrencySettings currencySettings,
            IActionContextAccessor actionContextAccessor,
            ICurrencyService currencyService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IStoreService storeService,
            IUrlHelperFactory urlHelperFactory,
            PaymentSettings paymentSettings,
            MonobankSettings settings,
            WidgetSettings widgetSettings,
            HttpClient httpClient,
            IOrderService orderService,
            IProductService productService,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            IHttpContextAccessor httpContextAccessor,
            IWebHelper webHelper)
        {
            _currencySettings = currencySettings;
            _actionContextAccessor = actionContextAccessor;
            _currencyService = currencyService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _storeService = storeService;
            _urlHelperFactory = urlHelperFactory;
            _paymentSettings = paymentSettings;
            _settings = settings;
            _widgetSettings = widgetSettings;
            _httpClient = httpClient;
            _orderService = orderService;
            _productService = productService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult
            {
                NewPaymentStatus = PaymentStatus.Pending
            };

            return Task.FromResult(result);
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var orderItems = await _orderService.GetOrderItemsAsync(postProcessPaymentRequest.Order.Id);
            var products = await _productService.GetProductsByIdsAsync(orderItems.Select(x => x.ProductId).ToArray());

            var basketOrders = new List<BasketOrder>(orderItems.Count);
            foreach (var oi in orderItems)
            {
                var product = products.FirstOrDefault(p => p.Id == oi.ProductId);
                var orderItemPicture = await _pictureService.GetProductPictureAsync(product, oi.AttributesXml);
                var imageUrl = (await _pictureService.GetPictureUrlAsync(orderItemPicture, _mediaSettings.OrderThumbPictureSize, true)).Url;

                basketOrders.Add(new BasketOrder
                {
                    Code = product.Sku,
                    Name = product.Name,
                    Qty = oi.Quantity,
                    Sum = ((int)oi.PriceInclTax) * 100,
                    Icon = imageUrl
                });
            }

            var redirectUrl = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext)
                .RouteUrl("OrderDetails", new { orderId = postProcessPaymentRequest.Order.Id }, _webHelper.GetCurrentRequestProtocol());

            var monoRequest = new MonoInvoiceCreateRequest
            {
                Token = _settings.Token,
                Data = new PostData
                {
                    Amount = ((int)postProcessPaymentRequest.Order.OrderTotal) * 100,
                    Ccy = 980,
                    Code = _settings.TerminalCode,
                    PaymentType = _settings.PaymentType == PaymentType.Debit ? "debit" : "hold",
                    QrId = _settings.QrId,
                    Validity = _settings.Validity,
                    RedirectUrl = redirectUrl,
                    WebHookUrl = _settings.WebhookUrl,
                    SaveCardData = new SaveCardData { SaveCard = false },
                    MerchantPaymInfo = new MerchantPaymInfo
                    {
                        Destination = "Order #" + postProcessPaymentRequest.Order.Id.ToString(),
                        Comment = "Order #" + postProcessPaymentRequest.Order.Id.ToString(),
                        Reference = postProcessPaymentRequest.Order.Id.ToString(),
                        BasketOrder = basketOrders
                    }
                }
            };

            var response = await PostInvoiceCreateAsync(monoRequest);

            if (!string.IsNullOrEmpty(response.ErrCode))
            {
                throw new NopException($"${response.ErrCode} {response.ErrText}");
            }

            postProcessPaymentRequest.Order.CaptureTransactionId = response.InvoiceId;
            await _orderService.UpdateOrderAsync(postProcessPaymentRequest.Order);

            _httpContextAccessor.HttpContext.Response.Redirect(response.PageUrl);
        }


        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public async Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            try
            {
                var orderItems = await _orderService.GetOrderItemsAsync(refundPaymentRequest.Order.Id);
                var products = await _productService.GetProductsByIdsAsync(orderItems.Select(x => x.ProductId).ToArray());

                var orderRefundItems = new List<MonoRefundRequestItem>(orderItems.Count);
                foreach (var oi in orderItems)
                {
                    var product = products.FirstOrDefault(p => p.Id == oi.ProductId);
                    var orderItemPicture = await _pictureService.GetProductPictureAsync(product, oi.AttributesXml);
                    var imageUrl = (await _pictureService.GetPictureUrlAsync(orderItemPicture, _mediaSettings.OrderThumbPictureSize, true)).Url;

                    orderRefundItems.Add(new MonoRefundRequestItem
                    {
                        Code = product.Sku,
                        Name = product.Name,
                        Qty = oi.Quantity,
                        Sum = ((int)oi.PriceInclTax) * 100
                    });
                }

                var request = new MonoRefundRequest
                {
                    Amount = refundPaymentRequest.IsPartialRefund ? (int)refundPaymentRequest.AmountToRefund : null,
                    InvoiceId = refundPaymentRequest.Order.CaptureTransactionId,
                    ExtRef = refundPaymentRequest.Order.Id.ToString(),
                    Items = orderRefundItems
                };

                MonoRefundResonse response = await PostRefundAsync(request);

                if (response.Status == "failure")
                {
                    throw new Exception($"{response.Status} {response.CreatedDate} {response.ModifiedDate}");
                }

                if (response.Status == "processing")
                {
                    throw new Exception($"{response.Status} {response.CreatedDate} {response.ModifiedDate}");
                }
            }
            catch (Exception ex)
            {
                return new RefundPaymentResult { Errors = new[] { "Refund has been failde", ex.Message } };
            }

            return new RefundPaymentResult
            {
                NewPaymentStatus = refundPaymentRequest.IsPartialRefund ? PaymentStatus.PartiallyRefunded : PaymentStatus.Refunded
            };

        }

        #region Post helpers

        private async Task<MonoRefundResonse> PostRefundAsync(MonoRefundRequest request)
        {
            _httpClient.BaseAddress = new Uri(MonobankDefaults.ApiEndpoints.Base);
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.RequestTimeout ?? 30);
            _httpClient.DefaultRequestHeaders.Add("X-Token", _settings.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonConvert.SerializeObject(request);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(MonobankDefaults.ApiEndpoints.Refund, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MonoRefundResonse>(responseData);
            }
            else
            {
                throw new Exception($"Somthing went wrong tring refund {response.StatusCode}");
            }
        }

        private async Task<MonoInvoiceCreateResponse> PostInvoiceCreateAsync(MonoInvoiceCreateRequest monoRequest)
        {
            _httpClient.BaseAddress = new Uri(MonobankDefaults.ApiEndpoints.Base);
            _httpClient.Timeout = TimeSpan.FromSeconds(_settings.RequestTimeout ?? 30);
            _httpClient.DefaultRequestHeaders.Add("X-Token", _settings.Token);
            _httpClient.DefaultRequestHeaders.Add("X-Cms", _settings.Cms);
            _httpClient.DefaultRequestHeaders.Add("X-Cms-Version", _settings.CmsVersion);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonConvert.SerializeObject(monoRequest.Data);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(MonobankDefaults.ApiEndpoints.InvoiceCreate, content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<MonoInvoiceCreateResponse>(responseData);
            }
            else
            {
                return new MonoInvoiceCreateResponse
                {
                    ErrCode = response.StatusCode.ToString()
                };
            }
        }

        #endregion

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the process payment result
        /// </returns>
        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rue - hide; false - display.
        /// </returns>
        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the additional handling fee
        /// </returns>
        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the result
        /// </returns>
        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //Monobank is the redirection payment method
            //It also validates whether order is also paid (after redirection) so customers will not be able to pay twice

            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return Task.FromResult(false);

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of validating errors
        /// </returns>
        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var errors = new List<string>();

            //try to get errors from the form parameters
            if (form.TryGetValue(nameof(PaymentInfoModel.Errors), out var errorValue) && !StringValues.IsNullOrEmpty(errorValue))
                errors.Add(errorValue.ToString());

            return Task.FromResult<IList<string>>(errors);
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the payment info holder
        /// </returns>
        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            //already set
            return Task.FromResult(_actionContextAccessor.ActionContext.HttpContext.Session
                .Get<ProcessPaymentRequest>(MonobankDefaults.PaymentRequestSessionKey));
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(MonobankDefaults.ConfigurationRouteName);
        }

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        public Type GetPublicViewComponent()
        {
            return typeof(PaymentInfoViewComponent);
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
            return Task.FromResult<IList<string>>(new List<string>
            {
                PublicWidgetZones.CheckoutPaymentInfoTop,
                PublicWidgetZones.HeaderLinksBefore,
                PublicWidgetZones.Footer
            });
        }

        /// <summary>
        /// Gets a type of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component type</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            if (widgetZone.Equals(PublicWidgetZones.CheckoutPaymentInfoTop))
            {
                return typeof(PaymentInfoViewComponent);
            }

            if (widgetZone.Equals(PublicWidgetZones.HeaderLinksBefore) || widgetZone.Equals(PublicWidgetZones.Footer))
                return typeof(MonobankLogoViewComponent);

            return null;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new MonobankSettings
            {
                PaymentType = PaymentType.Debit,
                RequestTimeout = MonobankDefaults.RequestTimeout
            });

            if (!_paymentSettings.ActivePaymentMethodSystemNames.Contains(MonobankDefaults.SystemName))
            {
                _paymentSettings.ActivePaymentMethodSystemNames.Add(MonobankDefaults.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(MonobankDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(MonobankDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Payments.Monobank.Domain.PaymentType.Hold"] = "Hold",
                ["Enums.Nop.Plugin.Payments.Monobank.Domain.PaymentType.Debit"] = "Debit",

                ["Plugins.Ithoot.Payments.Monobank.Fields.Token"] = "Token",
                ["Plugins.Ithoot.Payments.Monobank.Fields.Token.Hint"] = "Токен з особистого кабінету https://web.monobank.ua/ або тестовий токен з https://api.monobank.ua/.",

                ["Plugins.Ithoot.Payments.Monobank.Fields.WebhookUrl"] = "Webhook url",
                ["Plugins.Ithoot.Payments.Monobank.Fields.WebhookUrl.Hint"] = "Адреса для CallBack (POST) – на цю адресу буде надіслано дані про стан платежу при кожній зміні статусу.",

                ["Plugins.Ithoot.Payments.Monobank.Fields.RedirectUrl"] = "Redirect url",
                ["Plugins.Ithoot.Payments.Monobank.Fields.RedirectUrl.Hint"] = "Адреса для повернення (GET) - на цю адресу буде переадресовано користувача після завершення оплати (у разі успіху або помилки).",

                ["Plugins.Ithoot.Payments.Monobank.Fields.Reference"] = "Reference",
                ["Plugins.Ithoot.Payments.Monobank.Fields.Reference.Hint"] = "Інформаційні дані замовлення, яке буде оплачуватсь. Обовʼязково вказувати при активній звʼязці з ПРРО (звʼязка створюється у веб-кабінеті https://web.monobank.ua).",

                ["Plugins.Ithoot.Payments.Monobank.Fields.Validity"] = "Validity",
                ["Plugins.Ithoot.Payments.Monobank.Fields.Validity.Hint"] = "Строк дії в секундах, за замовчуванням рахунок перестає бути дійсним через 24 години.",

                ["Plugins.Ithoot.Payments.Monobank.Fields.QrId"] = "QrId",
                ["Plugins.Ithoot.Payments.Monobank.Fields.QrId.Hint"] = "Ідентифікатор QR - каси для встановлення суми оплати на існуючих QR - кас.",

                ["Plugins.Ithoot.Payments.Monobank.Fields.TerminalCode"] = "TerminalCode",
                ["Plugins.Ithoot.Payments.Monobank.Fields.TerminalCode.Hint"] = "Код терміналу субмерчанта, з апі 'Список субмерчантів'. Доступний обмеженому колу мерчантів, які точно знають, що їм це потрібно.",

                ["Plugins.Ithoot.Payments.Monobank.Fields.CMS"] = "CMS",
                ["Plugins.Ithoot.Payments.Monobank.Fields.CMS.Hint"] = "Назва CMS, якщо ви розробляєте платіжний модуль для CMS",

                ["Plugins.Ithoot.Payments.Monobank.Fields.CMSVersion"] = "CMSVersion",
                ["Plugins.Ithoot.Payments.Monobank.Fields.CMSVersion.Hint"] = "Версія CMS, якщо ви розробляєте платіжний модуль для CMS",

                ["Plugins.Ithoot.Payments.Monobank.PaymentMethodDescription"] = "Оплата через Monobank",

                ["Plugins.Ithoot.Payments.Monobank.RedirectionTip"] = "Після підтвердження замовлення, Вас перенаправить на платіжну систему Монобанку для здійснення оплати",

            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {
            //webhooks
            var stores = await _storeService.GetAllStoresAsync();
            var storeIds = new List<int> { 0 }.Union(stores.Select(store => store.Id));
            foreach (var storeId in storeIds)
            {
                var settings = await _settingService.LoadSettingAsync<MonobankSettings>(storeId);
                //if (!string.IsNullOrEmpty(settings.WebhookUrl))
                //    await _serviceManager.DeleteWebhookAsync(settings);
            }

            //settings
            if (_paymentSettings.ActivePaymentMethodSystemNames.Contains(MonobankDefaults.SystemName))
            {
                _paymentSettings.ActivePaymentMethodSystemNames.Remove(MonobankDefaults.SystemName);
                await _settingService.SaveSettingAsync(_paymentSettings);
            }

            if (_widgetSettings.ActiveWidgetSystemNames.Contains(MonobankDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(MonobankDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            await _settingService.DeleteSettingAsync<MonobankSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Ithoot.Payments.Monobank");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Ithoot.Payments.Monobank");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Ithoot.Payments.Monobank.PaymentMethodDescription");
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => true;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => true;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => true;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => true;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;

        #endregion
    }
}