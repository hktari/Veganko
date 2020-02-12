using System.Linq;
using Veganko.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Veganko.Effects;

[assembly: ExportEffect(typeof(Veganko.iOS.Effects.TabColorEffect), "TabColorEffect")]

namespace Veganko.iOS.Effects
{
    public class TabColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var tabBar = Container.Subviews.OfType<UITabBar>().FirstOrDefault();
            if (tabBar == null)
            {
                return;
            }

            var effect = Element.Effects.OfType<Veganko.Effects.TabColorEffect>().FirstOrDefault();
            if (effect != null)
            {
                tabBar.SelectedImageTintColor = effect.SelectedColor.ToUIColor();
                tabBar.UnselectedItemTintColor = effect.UnselectedColor.ToUIColor();
            }
        }

        protected override void OnDetached()
        {
        }
    }
}