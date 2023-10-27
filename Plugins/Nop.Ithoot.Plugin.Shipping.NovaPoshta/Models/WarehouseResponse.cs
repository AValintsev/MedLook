using Baroque.NovaPoshta.Client.Domain;
using Baroque.NovaPoshta.Client.Domain.Address;
using System;
using System.Runtime.Serialization;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models
{
    [DataContract]
    public class WarehouseReponse : BaseResponseEnvelope<WarehouseReponse.Warehouse>
    {
        [DataContract]
        public class Warehouse : BaseRefItem
        {
            [DataMember]
            public decimal SiteKey { get; set; }

            [DataMember]
            public string Description { get; set; }

            [DataMember]
            public string DescriptionRu { get; set; }

            [DataMember]
            public string ShortAddress { get; set; }

            [DataMember]
            public string ShortAddressRu { get; set; }

            [DataMember]
            public string Phone { get; set; }

            [DataMember]
            public Guid TypeOfWarehouse { get; set; }

            [DataMember]
            public int Number { get; set; }

            [DataMember]
            public Guid CityRef { get; set; }

            [DataMember]
            public string CityDescription { get; set; }

            [DataMember]
            public string CityDescriptionRu { get; set; }

            [DataMember]
            public Guid SettlementRef { get; set; }

            [DataMember]
            public string SettlementDescription { get; set; }

            [DataMember]
            public string SettlementAreaDescription { get; set; }

            [DataMember]
            public string SettlementRegionsDescription { get; set; }

            [DataMember]
            public string SettlementTypeDescription { get; set; }

            [DataMember]
            public string SettlementTypeDescriptionRu { get; set; }

            [DataMember]
            public string Longitude { get; set; }

            [DataMember]
            public string Latitude { get; set; }

            [DataMember]
            public int PostFinance { get; set; }

            [DataMember]
            public int BicycleParking { get; set; }

            [DataMember]
            public int PaymentAccess { get; set; }

            [DataMember]
            public int POSTerminal { get; set; }

            [DataMember]
            public int InternationalShipping { get; set; }

            [DataMember]
            public int SelfServiceWorkplacesCount { get; set; }

            [DataMember]
            public int TotalMaxWeightAllowed { get; set; }

            [DataMember]
            public int PlaceMaxWeightAllowed { get; set; }

            [DataMember]
            public WarehouseLimitations SendingLimitationsOnDimensions { get; set; }

            [DataMember]
            public WarehouseLimitations ReceivingLimitationsOnDimensions { get; set; }

            [DataMember]
            public Week Reception { get; set; } = new Week();


            [DataMember]
            public Week Delivery { get; set; } = new Week();


            [DataMember]
            public Week Schedule { get; set; } = new Week();


            [DataMember]
            public string DistrictCode { get; set; }

            [DataMember]
            public string WarehouseStatus { get; set; }

            [DataMember]
            public DateTime WarehouseStatusDate { get; set; }

            [DataMember]
            public string CategoryOfWarehouse { get; set; }

            [DataMember]
            public string RegionCity { get; set; }

            [DataMember]
            public int WarehouseForAgent { get; set; }

            [DataMember]
            public int MaxDeclaredCost { get; set; }

            [DataMember]
            public int DenyToSelect { get; set; }

            [DataMember]
            public string PostMachineType { get; set; }

            [DataMember]
            public string PostalCodeUA { get; set; }

            [DataMember]
            public int? OnlyReceivingParcel { get; set; }

            [DataMember]
            public string WarehouseIndex { get; set; }

            public override string ToString()
            {
                return Description;
            }
        }

        [DataContract]
        public class WarehouseLimitations : IDimensions
        {
            [DataMember]
            public int Width { get; set; }

            [DataMember]
            public int Height { get; set; }

            [DataMember]
            public int Length { get; set; }
        }
    }
}
