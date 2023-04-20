using Microsoft.AspNetCore.Http;
using Nop.Core.Domain.Shipping;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Services;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Infrastructure.Events
{
    public class ShipmentCreatedEventConsumer : IConsumer<ShipmentCreatedEvent>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IShipmentService _shipmentService;
        private readonly INovaPoshtaService _poshtaService;
        private readonly IOrderService _orderService;

        public ShipmentCreatedEventConsumer(
            IShipmentService shipmentService,
            INovaPoshtaService poshtaService,
            IOrderService orderService,
            IHttpContextAccessor httpContextAccessor)
        //ILocalizedEntityService localizedEntityService)
        {
            _shipmentService = shipmentService;
            _poshtaService = poshtaService;
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            //_localizedEntityService = localizedEntityService;
        }

        public async Task HandleEventAsync(ShipmentCreatedEvent eventMessage)
        {
            if (string.IsNullOrEmpty(
                _httpContextAccessor.HttpContext.Request.Form[NovaPoshtaSettings.NOVA_POSHTA_FORM_KEY]))
            {
                return;
            }

            var shipment = eventMessage.Shipment;
            var order = await _orderService.GetOrderByIdAsync(shipment.OrderId);

            if (order == null)
            {
                return;
            }

            if (!order.ShippingMethod.Equals(NovaPoshtaSettings.NOVA_POSHTA_SHIPPING_METHOD_NAME, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var trackingNumber = await _poshtaService.CreateParcelAsync(order, shipment);
            if (string.IsNullOrEmpty(trackingNumber))
            {
                return;
            }

            shipment.TrackingNumber = trackingNumber;

            await _shipmentService.UpdateShipmentAsync(shipment);
        }
    }
}
