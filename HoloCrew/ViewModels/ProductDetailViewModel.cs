using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class ProductDetailViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private Product _product;

        [ObservableProperty]
        private string _selectedImage;

        [ObservableProperty]
        private ObservableCollection<string> _images = new();

        [ObservableProperty]
        private int _quantity = 1;

        [ObservableProperty]
        private ObservableCollection<Review> _reviews = new();

        [ObservableProperty]
        private ObservableCollection<Product> _relatedProducts = new();

        [ObservableProperty]
        private bool _isInWishlist;

        public ProductDetailViewModel(
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService,
            INavigationService navigationService)
        {
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
            _navigationService = navigationService;

            Title = "Product Detail";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int productId)
            {
                await LoadProductAsync(productId);
            }
        }

        private async Task LoadProductAsync(int productId)
        {
            await ExecuteAsync(async () =>
            {
                Product = await _productService.GetProductByIdAsync(productId);

                if (Product != null)
                {
                    Images = new ObservableCollection<string>(Product.ImageUrls ?? new List<string>());
                    SelectedImage = Product.MainImageUrl;

                    var related = await _productService.GetRelatedProductsAsync(productId);
                    RelatedProducts = new ObservableCollection<Product>(related);

                    IsInWishlist = await _wishlistService.IsInWishlistAsync(productId);

                    SetSuccess();
                }
                else
                {
                    SetError(AppConstants.Errors.ProductNotAvailable);
                }
            });
        }

        [RelayCommand]
        private async Task AddToCartAsync()
        {
            if (Product == null || Quantity < 1) return;

            try
            {
                await _cartService.AddToCartAsync(Product, Quantity);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task BuyNowAsync()
        {
            await AddToCartAsync();
            _navigationService.NavigateTo<CheckoutViewModel>();
        }

        [RelayCommand]
        private async Task ToggleWishlistAsync()
        {
            if (Product == null) return;

            try
            {
                if (IsInWishlist)
                {
                    await _wishlistService.RemoveFromWishlistAsync(Product.Id);
                    IsInWishlist = false;
                }
                else
                {
                    await _wishlistService.AddToWishlistAsync(Product.Id);
                    IsInWishlist = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void IncreaseQuantity()
        {
            if (Product != null && Quantity < Product.Stock)
            {
                Quantity++;
            }
        }

        [RelayCommand]
        private void DecreaseQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
            }
        }
    }
}