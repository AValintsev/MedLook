using Nop.Core.Configuration;
using System;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta
{
    /// <summary>
    /// Represents settings of the "Fixed or by weight" shipping plugin
    /// </summary>
    public class NovaPoshtaSettings : ISettings
    {
        public string ApiKey { get; set; }

        public Guid CitySenderId { get; set; }
        public string CitySender { get; set; }

        public string Sender { get; set; }
        public Guid SenderId { get; set; }

        public string SenderAddress { get; set; }
        public Guid SenderAddressId { get; set; }

        public string ContactSender { get; set; }
        public Guid ContactSenderId { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderMidName { get; set; }
        public string SenderPhoneNumber { get; set; }

        public int CityAttributeId { get; set; }
        public int WarehouseAttributeId { get; set; }
        public string WarehouseName { get; internal set; }
        public string CityName { get; internal set; }
        public int PrepaymentValue { get; set; }

        public decimal DefaultWeight { get; set; } = 1;
        public decimal DefaultVolumeGeneral { get; set; } = 0.0368m;

        public const string NOVA_POSHTA_SHIPPING_METHOD_NAME = "НОВА ПОШТА";
        public const string NOVA_POSHTA_FORM_KEY = "NovaPoshtaAdd";

    }
}