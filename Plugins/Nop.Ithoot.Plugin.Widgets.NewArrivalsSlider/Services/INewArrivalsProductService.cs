using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Ithoot.Plugin.Widgets.NewArrivalsSlider.Services
{
    public interface INewArrivalsProductService
    {
        Task<IList<Product>> GetProductsMarkedAsNewAndHomepageAsync(int storeId = 0);
    }
}
