using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Veganko.Models;

namespace Veganko.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public static ObservableCollection<Product> Products 
            => new ObservableCollection<Product>()
            {
                new Product
                {
                    Name = "Vegan Cheese", Description = "100% VEGAN", Image = "raspeberry_meringue.jpg",
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.VEGAN,
                        ProductClassifier.GLUTENFREE
                    }
                },
                new Product
                {
                    Name = "Lepotna krema", Description = "Za fajn namazane roke", Image = "raspeberry_meringue.jpg",
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.VEGAN,
                        ProductClassifier.GLUTENFREE,
                        ProductClassifier.CRUELTYFREE
                    }
                },
                new Product
                {
                    Name = "Čokoladni namaz", Description = "Kdo pa nima rad nutelle... Še posebej, če je vegan.", Image = "raspeberry_meringue.jpg",
                    ProductClassifiers = new ObservableCollection<ProductClassifier>
                    {
                        ProductClassifier.VEGAN,
                        ProductClassifier.GLUTENFREE
                    }
                }
            }; 
    }
}
