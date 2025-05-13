using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Widgets.CategorySlider;
using Nop.Services.Security;
using Nop.Services.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Services
{
    public class CategoryProductService : ICategoryProductService
    {
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IAclService _aclService;

        public CategoryProductService(
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Product> productRepository,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IWorkContext workContext,
            IAclService aclService,
            IStaticCacheManager staticCacheManager)
        {
            _productCategoryRepository = productCategoryRepository;
            _productRepository = productRepository;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _aclService = aclService;
            _staticCacheManager = staticCacheManager;
        }

        public async Task<IList<Product>> GetCategoryProductsMarkedAsNewAsync(int categoryId, int storeId = 0)
        {
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(new CacheKey(CategorySliderPlugin.CacheKeyBase, "Products"), storeId, categoryId);

            var productsTask = _staticCacheManager.GetAsync(
               cacheKey,
               async () =>
               {
                   var query = from p in _productRepository.Table
                               join pc in _productCategoryRepository.Table on p.Id equals pc.ProductId
                               where p.Published && p.VisibleIndividually && p.MarkAsNew && !p.Deleted &&
                                       DateTime.UtcNow >= (p.MarkAsNewStartDateTimeUtc ?? DateTime.MinValue) &&
                                       DateTime.UtcNow <= (p.MarkAsNewEndDateTimeUtc ?? DateTime.MaxValue)
                                       && categoryId == pc.CategoryId
                               select p;

                   //apply store mapping constraints
                   query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                   //apply ACL constraints
                   var customer = await _workContext.GetCurrentCustomerAsync();
                   query = await _aclService.ApplyAcl(query, customer);
                   query = query.OrderByDescending(p => p.CreatedOnUtc);

                   return await query.ToPagedListAsync(0, 20);
               });
            var products = await productsTask;

            return products;
        }
    }
}
