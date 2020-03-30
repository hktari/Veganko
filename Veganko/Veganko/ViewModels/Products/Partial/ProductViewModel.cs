using System;
using System.Collections.ObjectModel;
using Veganko.Common.Models.Products;
using Veganko.Other;
using Xamarin.Forms;

namespace Veganko.ViewModels.Products.Partial
{
    public enum ProductStateIndicator
    {
        None,
        New,
        Pending,
        Approved,
        Rejected,
        Missing
    }

    public class ProductViewModel : BaseViewModel
    {
        /// <summary>
        /// The duration during which a product is still considered as new after it's been added.
        /// </summary>
        public static readonly TimeSpan IsNewProductTimespan = TimeSpan.FromDays(2);

        private DecimalProductClassifierListConverter classifierConverter = new DecimalProductClassifierListConverter();

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

        public bool HasBeenSeen { get; private set; }

        private bool isNew;
        public bool IsNew
        {
            get => isNew;
            set 
            {
                SetStateIndicatorImage(value ? ProductStateIndicator.New : ProductStateIndicator.None);
                isNew = value;
            }
        }

        private ImageSource stateIndicatorImage;
        public ImageSource StateIndicatorImage
        {
            get => stateIndicatorImage;
            private set => SetProperty(ref stateIndicatorImage, value);
        }

        public ProductStateIndicator CurIndicatorState { get; private set; }

        public DateTime AddedTimestamp { get; set; }

        public DateTime LastUpdateTimestamp { get; set; }

        public void SetStateIndicatorImage(ProductStateIndicator indicatator)
        {
            string indicatorImg = null;
            switch (indicatator)
            {
                case ProductStateIndicator.None:
                    break;
                case ProductStateIndicator.New:
                    indicatorImg = "new_product.png";
                    break;
                case ProductStateIndicator.Pending:
                    indicatorImg = "pending_product.png";
                    break;
                case ProductStateIndicator.Approved:
                    indicatorImg = "approved_product.png";
                    break;
                case ProductStateIndicator.Rejected:
                    indicatorImg = "rejected_product.png";
                    break;
                case ProductStateIndicator.Missing:
                    indicatorImg = "missing_product.png";
                    break;
                default:
                    break;
            }

            CurIndicatorState = indicatator;
            StateIndicatorImage = indicatorImg;
        }

        public void Update(ProductViewModel productViewModel)
        {
            if (productViewModel == this)
            {
                return;
            }

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
            Type = product.ProdType;
            ProductClassifiers = new ObservableCollection<ProductClassifier>(
                classifierConverter.Convert(product.ProductClassifiers));
            Description = product.Description ?? string.Empty;
            Image = product.DetailImage;
            ThumbnailImage = product.ThumbImage;
            Barcode = product.Barcode;
            Brand = product.Brand ?? string.Empty;
            Name = product.Name ?? string.Empty;
            Id = product.Id;
            AddedTimestamp = product.AddedTimestamp;
            LastUpdateTimestamp = product.LastUpdateTimestamp;

            UpdateIsNew();
        }

        /// <summary>
        /// Updates the product with the view model data. Image is not updated,
        /// since there's a seperate api for updating images.
        /// </summary>
        /// <param name="product"></param>
        public void MapToModel(Product product)
        {
            product.ProdType = Type;
            product.ProductClassifiers = ProductClassifiers == null ? default : classifierConverter.ConvertBack(ProductClassifiers);
            product.Description = Description;
            product.Barcode = Barcode;
            product.Brand = Brand;
            product.Name = Name;
            product.Id = Id;
            product.AddedTimestamp = AddedTimestamp;
            product.LastUpdateTimestamp = LastUpdateTimestamp;
        }

        public Product MapToModel()
        {
            Product product = new Product();
            MapToModel(product);
            return product;
        }
        public void SetHasBeenSeen(bool hasBeenSeen)
        {
            HasBeenSeen = hasBeenSeen;
            UpdateIsNew();
        }

        private void UpdateIsNew()
        {
            IsNew = !HasBeenSeen && (DateTime.Now - AddedTimestamp) < IsNewProductTimespan;
        }
    }
}
