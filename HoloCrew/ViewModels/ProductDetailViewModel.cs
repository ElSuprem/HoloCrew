using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

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

            Title = "Detalle del Producto";
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
            try
            {
                IsBusy = true;

                Product = await _productService.GetProductByIdAsync(productId);

                if (Product != null)
                {
                    Images = new ObservableCollection<string>(Product.ImageUrls ?? new List<string>());
                    SelectedImage = Product.MainImageUrl;

                    // Cargar productos relacionados
                    var related = await _productService.GetRelatedProductsAsync(productId);
                    RelatedProducts = new ObservableCollection<Product>(related);

                    // Verificar si está en wishlist
                    IsInWishlist = await _wishlistService.IsInWishlistAsync(productId);

                    System.Diagnostics.Debug.WriteLine($"🔍 Product {productId} IsInWishlist: {IsInWishlist}");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddToCartAsync()
        {
            if (Product == null || Quantity < 1) return;

            try
            {
                await _cartService.AddToCartAsync(Product, Quantity);
                // TODO: Mostrar notificación
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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
                System.Diagnostics.Debug.WriteLine($"🔍 ToggleWishlist called. Current state: {IsInWishlist}");

                if (IsInWishlist)
                {
                    // Quitar de wishlist
                    System.Diagnostics.Debug.WriteLine($"❌ Removing product {Product.Id} from wishlist");
                    await _wishlistService.RemoveFromWishlistAsync(Product.Id);
                    IsInWishlist = false;
                }
                else
                {
                    // Agregar a wishlist
                    System.Diagnostics.Debug.WriteLine($"✅ Adding product {Product.Id} to wishlist");
                    await _wishlistService.AddToWishlistAsync(Product.Id);
                    IsInWishlist = true;
                }

                System.Diagnostics.Debug.WriteLine($"🔍 New state: {IsInWishlist}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error toggling wishlist: {ex.Message}");
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