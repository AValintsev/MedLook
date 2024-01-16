using System.Collections.Generic;
using Nop.Core;

namespace Nop.Ithoot.Plugin.Payments.Monobank
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public class MonobankDefaults
    {
        /// <summary>
        /// Gets the plugin system name
        /// </summary>
        public static string SystemName => "Payments.Monobank";

        /// <summary>
        /// Gets the user agent used to request third-party services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";

        /// <summary>
        /// Gets the nopCommerce partner code
        /// </summary>
        public static string PartnerCode => "NopCommerce_Mono";

        /// <summary>
        /// Gets the configuration route name
        /// </summary>
        public static string ConfigurationRouteName => "Plugin.Ithoot.Payments.Monobank.Configure";

        /// <summary>
        /// Gets the webhook route name
        /// </summary>
        public static string WebhookRouteName => "Plugin.Ithoot.Payments.Monobank.MonobankWebhook";

        /// <summary>
        /// Gets the one page checkout route name
        /// </summary>
        public static string OnePageCheckoutRouteName => "CheckoutOnePage";

        /// <summary>
        /// Gets the shopping cart route name
        /// </summary>
        public static string ShoppingCartRouteName => "ShoppingCart";

        /// <summary>
        /// Gets the session key to get process payment request
        /// </summary>
        public static string PaymentRequestSessionKey => "OrderPaymentInfo";

        /// <summary>
        /// Gets the name of a generic attribute to store the refund identifier
        /// </summary>
        public static string RefundIdAttributeName => "MonobankRefundId";

        /// <summary>
        /// Gets the service js script URL
        /// </summary>

        /// <summary>
        /// Gets a default period (in seconds) before the request times out
        /// </summary>
        public static int RequestTimeout => 10;

        /// <summary>
        /// Gets webhook event names to subscribe
        /// </summary>
        public static List<string> WebhookEventNames => new()
        {
            "CHECKOUT.ORDER.APPROVED",
            "CHECKOUT.ORDER.COMPLETED",
            "PAYMENT.AUTHORIZATION.CREATED",
            "PAYMENT.AUTHORIZATION.VOIDED",
            "PAYMENT.CAPTURE.COMPLETED",
            "PAYMENT.CAPTURE.DENIED",
            "PAYMENT.CAPTURE.PENDING",
            "PAYMENT.CAPTURE.REFUNDED"
        };


        #region Monobank api endpoints

        public static class ApiEndpoints
        {
            public static string Base => "https://api.monobank.ua";

            public static string InvoiceCreate => "api/merchant/invoice/create";
            public static string Refund => "api/merchant/invoice/cancel";
        }
    }
    #endregion
}