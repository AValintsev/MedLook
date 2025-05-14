using Nop.Web.Models.Catalog;
using System.Collections.Generic;

namespace Nop.Ithoot.Plugin.Widgets.CategorySlider.Models
{
    public class PublicInfoModel
    {
        public IEnumerable<ProductOverviewModel> ProductsList { get; set; }

        public string CategorySeName { get; set; }
    }
}
