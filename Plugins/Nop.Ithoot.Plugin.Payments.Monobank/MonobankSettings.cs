using Nop.Core.Configuration;
using Nop.Ithoot.Plugin.Payments.Monobank.Domain;
using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Payments.Monobank
{
    /// <summary>
    /// Represents plugin settings
    /// </summary>
    public class MonobankSettings : ISettings
    {       
        /// <summary>
        /// Gets or sets a value indicating whether to use sandbox environment
        /// </summary>
        public bool UseSandbox { get; set; }

        /// <summary>
        /// Gets or sets client secret
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// QR cash register ID for setting the payment amount at existing QR cash registers
        /// </summary>
        public string QrId { get; set; }

        /////// <summary>
        /////// Reference for PPRO
        /////// </summary>
        ////public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the payment type
        /// </summary>
        public PaymentType PaymentType { get; set; } = PaymentType.Debit;

        /// <summary>
        /// Gets or sets a period (in seconds) before the request times out
        /// </summary>
        public int? RequestTimeout { get; set; }

        /// <summary>
        /// The validity period in seconds. Default 24 hours
        /// </summary>
        public int Validity { get; set; } = 86400;

        /// <summary>
        /// Gets or sets a webhook URL
        /// </summary>
        public string WebhookUrl { get; set; }
        public string CmsVersion { get; set; } = "4.60";
        public string Cms { get; set; } = "nopCommerce";
        public string TerminalCode { get; set; }
    }
}