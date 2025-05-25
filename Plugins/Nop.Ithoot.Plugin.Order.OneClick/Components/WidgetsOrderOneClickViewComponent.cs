using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Ithoot.Plugin.Order.OneClick.Models;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Order.OneClick.Components
{
    public class WidgetsOrderOneClickViewComponent : NopViewComponent
    {
        private readonly OrderOneClickSettings _settings;
        private readonly IWorkContext _workContext;

        public WidgetsOrderOneClickViewComponent(
            OrderOneClickSettings settings, 
            IWorkContext workContext)
        {
            _settings = settings;
            _workContext = workContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            ProductDetailsModel productModel = additionalData as ProductDetailsModel;

            var model = new OrderOneClickModel
            {
                Phone = customer.Phone,
                ProductId = productModel?.Id,
                ProductSeName = productModel?.SeName,
            };

            return View("~/Plugins/Order.OneClick/Views/PublicInfo.cshtml", model);
        }
    }
}
