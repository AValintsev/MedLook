using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Ithoot.Plugin.Widgets.CategorySlider.Models;
using Nop.Ithoot.Plugin.Widgets.CategorySlider.Services;
using Nop.Services.Configuration;
using Nop.Services.Seo;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.CategorySlider.Components
{
    public class WidgetsCategorySliderViewComponent : NopViewComponent
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IProductModelFactory _productModelFactory;

        private readonly ICategoryProductService _categoryProductService;


        private readonly ISettingService _settingService;

        public WidgetsCategorySliderViewComponent(
            IStoreContext storeContext,
            ISettingService settingService,
            IStaticCacheManager staticCacheManager,
            IProductModelFactory productModelFactory,
            ICategoryProductService categoryProductService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext)
        {
            _storeContext = storeContext;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _productModelFactory = productModelFactory;
            _categoryProductService = categoryProductService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var language = await _workContext.GetWorkingLanguageAsync();

            var settings = await _settingService.LoadSettingAsync<CategorySliderSettings>(store.Id);

            if (settings?.CategoryId == 0)
            {
                return Content("");
            }

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(new CacheKey(CategorySliderPlugin.CacheKeyBase, "ProductsOverview"), store, settings.CategoryId);

            var model = await _staticCacheManager.GetAsync(cacheKey,
                 async () =>
                 {
                     var products = await _categoryProductService.GetCategoryProductsMarkedAsNewAsync(settings.CategoryId, store.Id, settings.Count);

                     if (!products.Any())
                         return new PublicInfoModel { ProductsList = Enumerable.Empty<ProductOverviewModel>(), CategorySeName = null };

                     int? productThumbPictureSize = null;
                     var productsOverviewModel = await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, productThumbPictureSize);
                     return new PublicInfoModel
                     {
                         ProductsList = productsOverviewModel.ToList(),
                         CategorySeName = await _urlRecordService.GetSeNameAsync(settings.CategoryId, nameof(Category), language.Id)
                     };
                 }
             );

            if (!model.ProductsList.Any())
                return Content("");

            return View("~/Plugins/Widgets.CategorySlider/Views/PublicInfo.cshtml", model);
        }
    }
}
