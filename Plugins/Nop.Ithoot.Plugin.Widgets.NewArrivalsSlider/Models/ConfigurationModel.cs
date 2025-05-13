using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using static Nop.Plugin.Widgets.NewArrivalsSlider.Models.ConfigurationModel;

namespace Nop.Plugin.Widgets.NewArrivalsSlider.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<CategorySliderLocalizedModel>
    {
        [NopResourceDisplayName("Plugins.Widgets.NewArrivalsSlider.Name")]
        public string Name { get; set; }

        public IList<CategorySliderLocalizedModel> Locales { get; set; }

        public class CategorySliderLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.CategorySlider.Name")]
            public string Name { get; set; }
        }
    }
}