using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.NewArrivalsSlider
{
    public class NewArrivalsSliderSliderSettings : ISettings
    {
        public int CategoryId { get; set; }

        public int Count { get; set; }
    }
}