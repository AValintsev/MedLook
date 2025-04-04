using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain
{
    [DataContract]
    public class NovaPoshtaCreateDocumentRequest : Baroque.NovaPoshta.Client.Domain.Documents.CreateDocumentRequest
    {
        [DataMember]
        public int AfterpaymentOnGoodsCost { get; set; }

        [DataMember]
        public IList<BackwardDelivery> BackwardDeliveryData { get; set; } = new List<BackwardDelivery>();

        [DataContract]
        public class BackwardDelivery
        {
            [DataMember]
            public string PayerType { get; set; }

            [DataMember]
            public string CargoType { get; set; }

            [DataMember]
            public string RedeliveryString { get; set; }
        }
    }
}
