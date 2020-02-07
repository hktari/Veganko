using Xamarin.Forms;

namespace Veganko.Effects
{
    public class TabColorEffect : RoutingEffect
    {
        public Color SelectedColor { get; set; }
        
        public Color UnselectedColor { get; set; }

        public TabColorEffect(Color selectedColor, Color unselectedColor)
            : base($"Veganko.{nameof(TabColorEffect)}")
        {
            SelectedColor = selectedColor;
            UnselectedColor = unselectedColor;
        }
    }
}
