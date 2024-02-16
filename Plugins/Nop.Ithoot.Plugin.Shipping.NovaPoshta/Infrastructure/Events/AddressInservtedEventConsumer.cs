using Nop.Core.Domain.Common;
using Nop.Core.Events;
using Nop.Data;
using Nop.Services.Common;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Infrastructure.Events
{
    public class AddressInservtedEventConsumer : IConsumer<EntityInsertedEvent<Address>>
    {
        private readonly INopDataProvider _dataProvider;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;

        public AddressInservtedEventConsumer(
            INopDataProvider dataProvider,
            IAddressAttributeFormatter addressAttributeFormatter)
        {
            _dataProvider = dataProvider;
            _addressAttributeFormatter = addressAttributeFormatter;
        }

        public async Task HandleEventAsync(EntityInsertedEvent<Address> eventMessage)
        {
            var address = eventMessage.Entity;

            if (!string.IsNullOrEmpty(address.CustomAttributes))
            {
                var attrs = await _addressAttributeFormatter.FormatAttributesAsync(address.CustomAttributes, " ");
                address.Address1 = attrs;
                
                await _dataProvider.UpdateEntityAsync(address);
            }
        }
    }
}
