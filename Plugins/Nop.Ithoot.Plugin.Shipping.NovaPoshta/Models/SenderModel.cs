using System;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models
{
    public class SenderModel
    {
        public Guid SenderRef { get; set; }
        public Guid ContactRef { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderMidName { get; set; }
        public string SenderPhoneNumber { get; set; }
    }
}
