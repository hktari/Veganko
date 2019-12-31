using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.Partial
{
    public class ProductViewModel : BaseViewModel
    {
        public ProductViewModel()
        {

        }

        public ProductViewModel(ProductViewModel productViewModel)
        {
            Update(productViewModel);
        }

        public ProductViewModel(Product product)
        {
            Update(product);
        }

        public string Id { get; set; }

        // ??
        public ProductState State { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string brand = string.Empty;
        public string Brand
        {
            get => brand;
            set => SetProperty(ref brand, value);
        }

        private string barcode;
        public string Barcode
        {
            get => barcode;
            set => SetProperty(ref barcode, value);
        }

        private ImageSource image;
        public ImageSource Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        private ImageSource thumbnailImage;
        public ImageSource ThumbnailImage
        {
            get
            {
                return thumbnailImage;
            }
            set
            {
                SetProperty(ref thumbnailImage, value);
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private ObservableCollection<ProductClassifier> productClassifiers;
        public ObservableCollection<ProductClassifier> ProductClassifiers
        {
            get => productClassifiers;
            set => SetProperty(ref productClassifiers, value);
        }

        private ProductType type;
        public ProductType Type
        {
            get => type;
            set => SetProperty(ref type, value);
        }

        public void Update(ProductViewModel productViewModel)
        {
            Type = productViewModel.Type;
            ProductClassifiers = new ObservableCollection<ProductClassifier>(productViewModel.ProductClassifiers);
            Description = productViewModel.Description;
            Image = productViewModel.Image;
            ThumbnailImage = productViewModel.ThumbnailImage;
            Barcode = productViewModel.Barcode;
            Brand = productViewModel.Brand;
            Name = productViewModel.Name;
            Id = productViewModel.Id;
        }

        public void Update(Product product)
        {
            Type = product.Type;
            ProductClassifiers = new ObservableCollection<ProductClassifier>(product.ProductClassifiers);
            Description = product.Description ?? string.Empty;
            Image = product.DetailImage;
            ThumbnailImage = product.ThumbImage;
            Barcode = product.Barcode;
            Brand = product.Brand ?? string.Empty;
            Name = product.Name ?? string.Empty;
            Id = product.Id;
        }

        /// <summary>
        /// Updates the product with the view model data. Image is not updated,
        /// since there's a seperate api for updating images.
        /// </summary>
        /// <param name="product"></param>
        public void MapToModel(Product product)
        {
            product.Type = Type;
            product.ProductClassifiers = new List<ProductClassifier>(ProductClassifiers);
            product.Description = Description;
            product.Barcode = Barcode;
            product.Brand = Brand;
            product.Name = Name;
            product.Id = Id;
        }
    }
}
