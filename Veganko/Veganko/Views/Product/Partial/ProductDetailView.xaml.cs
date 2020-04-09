
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Veganko.Views.Product.Partial
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductDetailView : ContentView
    {
        public static readonly BindableProperty CanEditProductProperty =
            BindableProperty.Create(nameof(CanEditProduct), typeof(bool), typeof(ProductDetailView), true);

        public bool CanEditProduct
        {
            get => (bool)GetValue(CanEditProductProperty);
            set => SetValue(CanEditProductProperty, value);
        }

        public ProductDetailView()
        {
            InitializeComponent();
        }
    }
}