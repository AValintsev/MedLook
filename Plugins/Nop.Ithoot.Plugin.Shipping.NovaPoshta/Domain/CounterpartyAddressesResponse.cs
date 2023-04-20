using Baroque.NovaPoshta.Client.Domain;
using System;
using System.Runtime.Serialization;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain
{
    [DataContract]
    public class CounterpartyAddressesResponse : BaseResponseEnvelope<CounterpartyAddressesResponse.CounterPartyAddress>
    {
        [DataContract]
        public class CounterPartyAddress : BaseRefItem
        {
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public Guid CityRef { get; set; }
            [DataMember]
            public string CityDescription { get; set; }
            [DataMember]
            public Guid StreetRef { get; set; }
            [DataMember]
            public string StreetDescription { get; set; }
            [DataMember]
            public Guid BuildingRef { get; set; }
            [DataMember]
            public string BuildingDescription { get; set; }
            [DataMember]
            public string Note { get; set; }
            [DataMember]
            public string AddressName { get; set; }
            [DataMember]
            public string MarketplacePartnerDescription { get; set; }

        }
    }
}
