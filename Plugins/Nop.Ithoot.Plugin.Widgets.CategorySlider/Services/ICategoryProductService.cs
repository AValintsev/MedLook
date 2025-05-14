using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Services
{
    public interface ICategoryProductService
    {
        Task<IList<Product>> GetCategoryProductsMarkedAsNewAsync(int categoryId, int storeId = 0, int count = 20);
    }
}
