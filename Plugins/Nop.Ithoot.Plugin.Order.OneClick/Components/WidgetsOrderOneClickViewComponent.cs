using Microsoft.AspNetCore.Mvc;
using Nop.Ithoot.Plugin.Order.OneClick.Models;
using Nop.Web.Framework.Components;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick.Components
{
    public class WidgetsOrderOneClickViewComponent : NopViewComponent
    {
        private readonly OrderOneClickSettings _settings;

        public WidgetsOrderOneClickViewComponent(OrderOneClickSettings settings)
        {
            _settings = settings;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var model = new OrderOneClickModel
            {
                
            };

            return View("~/Plugins/Order.OneClick/Views/PublicInfo.cshtml", model);
        }
    }
}
