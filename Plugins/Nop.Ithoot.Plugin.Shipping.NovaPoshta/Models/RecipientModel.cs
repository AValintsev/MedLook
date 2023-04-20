using System;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models
{
    public class RecipientModel
    {
        public string Description { get; set; }
        public Guid ContactRef { get; set; }
        public Guid RecipientRef { get; set; }
        public Guid AddressRef { get;set; }
        public Guid CityRef { get; set; }
        public string Phone { get; internal set; }
    }
}
