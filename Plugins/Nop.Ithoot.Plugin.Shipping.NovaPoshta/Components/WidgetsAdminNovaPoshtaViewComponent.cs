using Microsoft.AspNetCore.Mvc;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Models;
using Nop.Services.Common;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Components
{
    public class WidgetsAdminNovaPoshtaViewComponent : NopViewComponent
    {
        private readonly NovaPoshtaSettings _novaPoshtaSettings;

        public WidgetsAdminNovaPoshtaViewComponent(NovaPoshtaSettings novaPoshtaSettings)
        {
            _novaPoshtaSettings = novaPoshtaSettings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new NovaPoshtaModel
            {
                CityControlName = string.Format(NopCommonDefaults.AddressAttributeControlName, _novaPoshtaSettings.CityAttributeId),
                WarehouseControlName = string.Format(NopCommonDefaults.AddressAttributeControlName, _novaPoshtaSettings.WarehouseAttributeId),
            };

            return View("~/Plugins/Shipping.NovaPoshta/Views/PublicInfoAdmin.cshtml", model);
        }
    }
}
