using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Ithoot.Plugin.Widgets.NewArrivalsSlider.Services;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.NewArrivalsSlider.Components
{
    public class WidgetsNewArrivalsSliderViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IProductModelFactory _productModelFactory;
        private readonly INewArrivalsProductService _categoryProductService;


        private readonly ISettingService _settingService;

        public WidgetsNewArrivalsSliderViewComponent(
            IStoreContext storeContext,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IProductModelFactory productModelFactory,
            INewArrivalsProductService categoryProductService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _productModelFactory = productModelFactory;
            _categoryProductService = categoryProductService;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var settings = await _settingService.LoadSettingAsync<NewArrivalsSliderSliderSettings>(store.Id);

            if (settings == null)
            {
                return Content("");
            }

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(new CacheKey(NewArrivalsSliderSliderPlugin.CacheKeyBase, "ProductsOverview"), store);

            var model = await _staticCacheManager.GetAsync(cacheKey,
                 async () =>
                 {
                     var products = await _categoryProductService.GetProductsMarkedAsNewAndHomepageAsync(store.Id);

                     if (!products.Any())
                         return Enumerable.Empty<ProductOverviewModel>();

                     int? productThumbPictureSize = null;
                     return (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
                 }
             );

            if (!model.Any())
                return Content("");

            return View("~/Plugins/Widgets.NewArrivalsSlider/Views/PublicInfo.cshtml", model);
        }
    }
}
