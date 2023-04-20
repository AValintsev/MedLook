using Nop.Web.Framework.Mvc.ModelBinding;
using System;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models
{
    public record ConfigurationModel
    {
        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.ApiKey")]
        public string ApiKey { get; set; }


        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.City")]
        public string City { get; set; }
        public Guid CityId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.Warehouse")]
        public string Warehouse { get; set; }
        public Guid WarehouseId { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.FirstName")]
        public string SenderFirstName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.LastName")]
        public string SenderLastName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.MidName")]
        public string SenderMidName { get; set; }

        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.Phone")]
        public string SenderPhone { get; set; }
        
        [NopResourceDisplayName("Plugins.Shipping.NovaPoshta.Fields.PrepaymentValue")]
        public int PrepaymentValue { get; set; }

        public ConfigurationModel()
        {
        }

    }
}