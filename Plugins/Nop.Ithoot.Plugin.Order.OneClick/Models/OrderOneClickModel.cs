using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Ithoot.Plugin.Order.OneClick.Models
{
    public class OrderOneClickModel
    {
        [NopResourceDisplayName("Plugins.Order.OneClick.Fields.Phone")]
        public string Phone { get; set; }

        public int? ProductId { get; set; }

        public string ProductSeName { get; set; }
    }
}
