using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using static Nop.Plugin.Widgets.CategorySlider.Models.ConfigurationModel;

namespace Nop.Plugin.Widgets.CategorySlider.Models
{
    public record ConfigurationModel : BaseNopModel, ILocalizedModel<CategorySliderLocalizedModel>
    {
        [NopResourceDisplayName("Plugins.Widgets.CategorySlider.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Plugins.Widgets.CategorySlider.CategoryId")]
        public int CategoryId { get; set; }

        public IList<SelectListItem> AvailableCategories { get; set; }

        public IList<CategorySliderLocalizedModel> Locales { get; set; }

        public class CategorySliderLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }

            [NopResourceDisplayName("Plugins.Widgets.CategorySlider.Name")]
            public string Name { get; set; }
        }
    }
}