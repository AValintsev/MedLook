using Baroque.NovaPoshta.Client;
using Baroque.NovaPoshta.Client.Domain;
using Baroque.NovaPoshta.Client.Domain.Address;
using Baroque.NovaPoshta.Client.Domain.Countrparty;
using Baroque.NovaPoshta.Client.Domain.Documents;
using Baroque.NovaPoshta.Client.Services.Common;
using Baroque.NovaPoshta.Client.Services.Counterparties;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;
using Nop.Services.Common;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services
{
    public class NovaPoshtaService : INovaPoshtaService
    {
        private readonly Nop.Services.Common.IAddressService _addressService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly NovaPoshtaSettings _novaPoshtaSettings;

        public NovaPoshtaService(
            NovaPoshtaSettings novaPoshtaSettings,
            Nop.Services.Common.IAddressService addressService,
            IAddressAttributeParser addressAttributeParser)
        {
            _novaPoshtaSettings = novaPoshtaSettings;
            _addressService = addressService;
            _addressAttributeParser = addressAttributeParser;
        }

        public IEnumerable<AutocompleteModel> FindCities(string term)
        {
            var gateway = new DefaultNovaPoshtaGateway(_novaPoshtaSettings.ApiKey);

            var commonService = new CommonService(gateway);

            var addressService = new Baroque.NovaPoshta.Client.Services.Address.AddressService(gateway);
            var request = new CitiesGetRequest
            {
                FindByString = term
            };
            var cities = addressService.GetCities(request);
            if (cities.Success)
            {
                return cities.Data
                    .Take(10)
                    .Select(x => new AutocompleteModel
                    {
                        label = x.ToString(),
                        value = x.ToString(),
                        id = x.Reference
                    });
            }

            return Enumerable.Empty<AutocompleteModel>();
        }

        public IEnumerable<AutocompleteModel> FindWirehouses(Guid cityId, string term)
        {
            var gateway = new DefaultNovaPoshtaGateway(_novaPoshtaSettings.ApiKey);
            var addressService = new Baroque.NovaPoshta.Client.Services.Address.AddressService(gateway);

            var request = new WarehousesGetRequestCustom
            {
                Limit = 15,
                CityRef = cityId,
                FindByString = term
            };

            //var wirehouses = addressService.GetWarehouses(request);

            RequestEnvelope<WarehousesGetRequest> request2 = new RequestEnvelope<WarehousesGetRequest>(request)
            {
                ApiKey = _novaPoshtaSettings.ApiKey,
                CalledMethod = "getWarehouses",
                ModelName = "Address"
            };
            var wirehouses = gateway.CreateRequest<WarehousesGetRequest, WarehouseReponse>(request2);

            if (wirehouses.Success)
            {
                return wirehouses.Data
                    .Select(x => new AutocompleteModel
                    {
                        label = x.ToString(),
                        value = x.ToString(),
                        id = x.Reference
                    });
            }

            return Enumerable.Empty<AutocompleteModel>();
        }

        public async Task<string> CreateParcelAsync(Order order, Shipment shipment)
        {
            var gateway = new DefaultNovaPoshtaGateway(_novaPoshtaSettings.ApiKey);
            var service = new NovaPoshtaDocumentService(gateway);

            var address = await _addressService.GetAddressByIdAsync(order.BillingAddressId);

            var recipient = await GetRecipientAsync(order, shipment, address);

            var request = new NovaPoshtaCreateDocumentRequest
            {
                PayerType = "Recipient",
                PaymentMethod = "Cash",
                DateTime = DateTime.Now.ToString("dd.MM.yyyy"),
                CargoType = "Parcel",
                Weight = _novaPoshtaSettings.DefaultWeight,
                //OR calculate weight
                //shipment.TotalWeight.HasValue ? (shipment.TotalWeight.Value == 0 ? 1 : shipment.TotalWeight.Value) : 1,

                VolumeGeneral = _novaPoshtaSettings.DefaultVolumeGeneral,// Or calculate
                ServiceType = "WarehouseWarehouse",
                SeatsAmount = 1,
                Description = $"Order #{order.Id}",
                Cost = (int)order.OrderTotal,

                CitySender = _novaPoshtaSettings.CitySenderId,
                Sender = _novaPoshtaSettings.SenderId,
                SenderAddress = _novaPoshtaSettings.SenderAddressId,
                ContactSender = _novaPoshtaSettings.ContactSenderId,
                SendersPhone = _novaPoshtaSettings.SenderPhoneNumber,

                CityRecipient = recipient.CityRef,
                Recipient = recipient.RecipientRef,
                RecipientAddress = recipient.AddressRef,
                ContactRecipient = recipient.ContactRef,
                RecipientsPhone = recipient.Phone
            };

            if (order.PaymentMethodSystemName.Equals("Payments.CashOnDelivery"))
            {
                request.Cost = (int)order.OrderTotal - _novaPoshtaSettings.PrepaymentValue;
                request.BackwardDeliveryData.Add(new NovaPoshtaCreateDocumentRequest.BackwardDelivery
                {
                    CargoType = "Money",
                    PayerType = "Recipient",
                    RedeliveryString = request.Cost.ToString()
                });
            }

            request.OptionsSeat.Add(new Seat
            {
                VolumetricHeight = 23,
                VolumetricLength = 16,
                VolumetricWidth = 10,
                Weight = _novaPoshtaSettings.DefaultWeight,
                VolumetricVolume = _novaPoshtaSettings.DefaultVolumeGeneral
            });


            var createDocResponse = service.CreateDocument(request);

            if (createDocResponse.Success)
            {
                return createDocResponse.FirstOrDefault.IntDocNumber;
            }

            return null;
        }

        public Task<RecipientModel> GetRecipientAsync(Order order, Shipment shipment, Address address)
        {
            var cityAttr = _addressAttributeParser
                .ParseValues(address.CustomAttributes, _novaPoshtaSettings.CityAttributeId)
                .FirstOrDefault();
            dynamic cityParsedDefaulValue = JsonSerializer.Deserialize<ExpandoObject>(cityAttr);
            Guid recipientCityId = new Guid(cityParsedDefaulValue.id.ToString());

            var warehouseAttr = _addressAttributeParser
                .ParseValues(address.CustomAttributes, _novaPoshtaSettings.WarehouseAttributeId)
                .FirstOrDefault();
            dynamic warehouseParsedDefaulValue = JsonSerializer.Deserialize<ExpandoObject>(warehouseAttr);
            Guid recipientWarehouseId = new Guid(warehouseParsedDefaulValue.id.ToString());

            var recipientCP = CreateCounterparty(recipientCityId, "Recipient", address.FirstName, address.LastName, "", address.PhoneNumber);

            var peronResponse = recipientCP.ContactPerson;
            var contactRef = Guid.Empty;

            if (peronResponse.Success)
            {
                contactRef = peronResponse.FirstOrDefault.Reference;
            }

            return Task.FromResult(new RecipientModel
            {
                RecipientRef = recipientCP.Reference,
                ContactRef = contactRef,
                CityRef = recipientCityId,
                AddressRef = recipientWarehouseId,
                Phone = address.PhoneNumber,
            });
        }

        public Guid GetCounterparty(string apiKey)
        {
            var gateway = new DefaultNovaPoshtaGateway(apiKey);
            var counterpartyService = new CounterpartyService(gateway);

            var counterPartyResponse = counterpartyService.GetCounterparties(new GetCounterpartiesRequest
            {
                CounterpartyProperty = "Sender",
                Page = 1
            });
            var @ref = counterPartyResponse.FirstOrDefault.Reference;

            return @ref;
        }

        public CounterpartyCreateOrUpdate CreateCounterparty(Guid cityId, string type, string firstName, string lastName, string midName, string phone)
        {
            var gateway = new DefaultNovaPoshtaGateway(_novaPoshtaSettings.ApiKey);
            var counterpartyService = new CounterpartyService(gateway);

            var counterPartyCreateResponse = counterpartyService.CreateCounterparty(new CreateCounterpartyRequest
            {
                Email = "",
                CityRef = cityId,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = midName,
                Phone = phone,
                CounterpartyType = "PrivatePerson",
                CounterpartyProperty = type
            });

            return counterPartyCreateResponse.FirstOrDefault;
        }

        public Task<SenderModel> GetSenderAsync(string apiKey)
        {
            var gateway = new DefaultNovaPoshtaGateway(apiKey);
            var counterpartyService = new CounterpartyService(gateway);

            var senderRef = GetCounterparty(apiKey);

            var contactPersonResponse = counterpartyService.GetCounterpartyContactPerson(senderRef);
            var contactPersonRef = contactPersonResponse.FirstOrDefault.Reference;

            //var getAddressRequest = new GetCounterpartyAddressesRequest
            //{
            //    Reference = senderRef,
            //    CounterpartyProperty = "Sender",
            //};
            //RequestEnvelope<GetCounterpartyAddressesRequest> request = new RequestEnvelope<GetCounterpartyAddressesRequest>(getAddressRequest)
            //{
            //    CalledMethod = "getCounterpartyAddresses",
            //    ModelName = "Counterparty"
            //};
            //var response = gateway.CreateRequest<GetCounterpartyAddressesRequest, CounterpartyAddressesResponse>(request);
            //var address = response.FirstOrDefault;

            return Task.FromResult(new SenderModel
            {
                SenderRef = senderRef,
                ContactRef = contactPersonRef,
                SenderFirstName = contactPersonResponse.FirstOrDefault.FirstName,
                SenderLastName = contactPersonResponse.FirstOrDefault.LastName,
                SenderMidName = contactPersonResponse.FirstOrDefault.MiddleName,
                SenderPhoneNumber = contactPersonResponse.FirstOrDefault.Phones
            });
        }
    }
}
