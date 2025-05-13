using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Services
{
    public interface ICategoryProductService
    {
        Task<IList<Product>> GetCategoryProductsMarkedAsNewAsync(int categoryId, int storeId = 0);
    }
}
