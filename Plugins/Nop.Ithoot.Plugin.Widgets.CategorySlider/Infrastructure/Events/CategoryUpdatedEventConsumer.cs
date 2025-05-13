using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Events;
using Nop.Services.Events;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Infrastructure.Events
{
    public class CategoryUpdatedEventConsumer : IConsumer<EntityUpdatedEvent<Category>>
    {
        private readonly IStaticCacheManager _staticCacheManager;

        public CategoryUpdatedEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync("Products");
            await _staticCacheManager.RemoveByPrefixAsync("ProductsOverview");
        }
    }
}
