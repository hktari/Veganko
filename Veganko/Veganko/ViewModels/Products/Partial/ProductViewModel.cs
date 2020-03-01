using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Veganko.Models;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.Partial
{
    public class ProductViewModel : BaseViewModel
    {
        /// <summary>
        /// The duration during which a product is still considered as new after it's been added.
        /// </summary>
        public static readonly TimeSpan IsNewProductTimespan = TimeSpan.FromDays(2);

        public ProductViewModel()
        {
            Type = ProductType.Ostalo;
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
            get => thumbnailImage;
            set => SetProperty(ref thumbnailImage, value);
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


        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set => SetProperty(ref isNew, value);
        }

        public DateTime AddedTimestamp { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

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
            AddedTimestamp = productViewModel.AddedTimestamp;
            LastUpdateTimestamp = productViewModel.LastUpdateTimestamp;
            IsNew = productViewModel.IsNew;
        }

        public void Update(Product product)
        {
            Type = product.Type;
            ProductClassifiers = new ObservableCollection<ProductClassifier>(
                product.ProductClassifiers ?? new List<ProductClassifier>());
            Description = product.Description ?? string.Empty;
            Image = product.DetailImage;
            ThumbnailImage = product.ThumbImage;
            Barcode = product.Barcode;
            Brand = product.Brand ?? string.Empty;
            Name = product.Name ?? string.Empty;
            Id = product.Id;
            AddedTimestamp = product.AddedTimestamp;
            LastUpdateTimestamp = product.LastUpdateTimestamp;

            IsNew = (DateTime.Now - AddedTimestamp) < IsNewProductTimespan;
        }

        /// <summary>
        /// Updates the product with the view model data. Image is not updated,
        /// since there's a seperate api for updating images.
        /// </summary>
        /// <param name="product"></param>
        public void MapToModel(Product product)
        {
            product.Type = Type;
            product.ProductClassifiers = new List<ProductClassifier>(
                ProductClassifiers ?? new ObservableCollection<ProductClassifier>());
            product.Description = Description;
            product.Barcode = Barcode;
            product.Brand = Brand;
            product.Name = Name;
            product.Id = Id;
            product.AddedTimestamp = AddedTimestamp;
            product.LastUpdateTimestamp = LastUpdateTimestamp;
        }
    }
}
