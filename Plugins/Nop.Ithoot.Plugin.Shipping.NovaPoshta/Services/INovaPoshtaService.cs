using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services
{
    public interface INovaPoshtaService
    {
        IEnumerable<AutocompleteModel> FindCities(string term);
        IEnumerable<AutocompleteModel> FindWirehouses(Guid cityId, string term);

        Task<string> CreateParcelAsync(Order order, Shipment shipment);

        Task<SenderModel> GetSenderAsync();

        Task<RecipientModel> GetRecipientAsync(Order order, Shipment shipment, Address address);

        Guid CreateCounterparty(Guid cityId, string type, string firstName, string lastName, string midName, string phone);
    }
}
