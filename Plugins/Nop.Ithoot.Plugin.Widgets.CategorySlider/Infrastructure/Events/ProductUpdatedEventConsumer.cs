﻿using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Plugin.Widgets.CategorySlider;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Infrastructure.Events
{
    public class ProductUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Product>>
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public ProductUpdatedEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(new CacheKey(CategorySliderPlugin.CacheKeyBase, "ProductsOverview"));
            await _staticCacheManager.RemoveAsync(new CacheKey(CategorySliderPlugin.CacheKeyBase, "Products"));
        }
    }
}
