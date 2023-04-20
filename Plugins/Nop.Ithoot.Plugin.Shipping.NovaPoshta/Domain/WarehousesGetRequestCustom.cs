using Baroque.NovaPoshta.Client.Domain.Address;
using System.Runtime.Serialization;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain
{
    [DataContract]
    public class WarehousesGetRequestCustom : WarehousesGetRequest
    {
        [DataMember]
        public string FindByString { get; set; }
    }
}
