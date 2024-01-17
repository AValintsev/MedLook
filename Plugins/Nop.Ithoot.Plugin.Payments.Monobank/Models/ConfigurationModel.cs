using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Ithoot.Plugin.Payments.Monobank.Models
{
    /// <summary>
    /// Represents configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        public bool IsConfigured { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.Token")]
        [Required]
        public string Token { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.WebhookUrl")]
        public string WebhookUrl { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.RedirectUrl")]
        public string RedirectUrl { get; set; }

        //[NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.Reference")]
        //public string Reference { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.QrId")]
        public string QrId { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.Validity")]
        public int Validity { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.CMS")]
        public string CMS { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.CMSVersion")]
        public string CMSVersion { get; set; }

        [NopResourceDisplayName("Plugins.Ithoot.Payments.Monobank.Fields.TerminalCode")]
        public string TerminalCode { get; set; }
    }
}