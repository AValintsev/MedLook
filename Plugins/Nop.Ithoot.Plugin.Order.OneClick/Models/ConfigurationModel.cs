using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using static Nop.Ithoot.Plugin.Order.OneClick.Models.ConfigurationModel;

namespace Nop.Ithoot.Plugin.Order.OneClick.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<OrderOneClickLocalizedModel>
    {
        [NopResourceDisplayName("Plugins.Order.OneClick.Fields.ButtonName")]
        public string ButtonName { get; set; }

        [NopResourceDisplayName("Plugins.Order.OneClick.Fields.SuccessNotifyMessage")]
        public string SuccessNotifyMessage { get; set; }
        
        [NopResourceDisplayName("Plugins.Order.OneClick.Fields.NotificationEmails")]
        public string NotificationEmails { get; set; }

        public IList<OrderOneClickLocalizedModel> Locales { get; set; }

        public class OrderOneClickLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Order.OneClick.Fields.ButtonName")]
            public string ButtonName { get; set; }

            [NopResourceDisplayName("Plugins.Order.OneClick.Fields.SuccessNotifyMessage")]
            public string SuccessNotifyMessage { get; set; }
        }
    }
}