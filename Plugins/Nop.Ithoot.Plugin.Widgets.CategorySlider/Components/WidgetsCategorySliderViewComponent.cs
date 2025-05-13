using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Ithoot.Plugin.Widgets.CategorySlider.Services;
using Nop.Services.Configuration;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CategorySlider.Components
{
    public class WidgetsCategorySliderViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryProductService _categoryProductService;


        private readonly ISettingService _settingService;

        public WidgetsCategorySliderViewComponent(
            IStoreContext storeContext,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IProductModelFactory productModelFactory,
            ICategoryProductService categoryProductService)
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
            var settings = await _settingService.LoadSettingAsync<CategorySliderSettings>(store.Id);

            if (settings?.CategoryId == 0)
            {
                return Content("");
            }

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(new CacheKey("ByCategory", "ProductsOverview"), store, settings.CategoryId);

            var model = await _staticCacheManager.GetAsync(cacheKey,
                 async () =>
                 {
                     var products = await _categoryProductService.GetCategoryProductsMarkedAsNewAsync(settings.CategoryId, store.Id);

                     if (!products.Any())
                         return Enumerable.Empty<ProductOverviewModel>();

                     int? productThumbPictureSize = null;
                     return (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize)).ToList();
                 }
             );

            if (!model.Any())
                return Content("");

            return View("~/Plugins/Widgets.CategorySlider/Views/PublicInfo.cshtml", model);
        }
    }
}
