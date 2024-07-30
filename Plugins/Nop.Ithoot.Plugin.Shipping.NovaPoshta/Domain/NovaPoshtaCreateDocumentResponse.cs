using Baroque.NovaPoshta.Client.Domain;
using System.Runtime.Serialization;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain
{
    [DataContract]
    public class NovaPoshtaCreateDocumentResponse : BaseResponseEnvelope<NovaPoshtaCreateDocumentResponse.CreationResult>
    {
        [DataContract]
        public class CreationResult : BaseRefItem
        {
            [DataMember]
            public double CostOnSite { get; set; }

            [DataMember]
            public string EstimatedDeliveryDate { get; set; }

            [DataMember]
            public string IntDocNumber { get; set; }

            [DataMember]
            public string TypeDocument { get; set; }
        }

        [DataMember(Name = "errors")]
        public string[] Errors { get; set; } = new string[0];


        [DataMember(Name = "info")]
        public string[] Info { get; set; } = new string[0];


        [DataMember(Name = "warnings")]
        public string[] Warnings { get; set; } = new string[0];

    }

}