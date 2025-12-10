using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class WishlistViewModel : ViewModelBase
    {
        private readonly IWishlistService _wishlistService;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Product> _wishlistItems = new();

        [ObservableProperty]
        private bool _isEmpty = true;

        [ObservableProperty]
        private int _itemCount;

        [ObservableProperty]
        private int _wishlistItemCount;

        [ObservableProperty]
        private bool _isWishlistEmpty = true;

        [ObservableProperty]
        private ObservableCollection<Product> _recommendedProducts = new();

        // ⭐ PROPIEDAD FALTANTE
        public bool HasItems => !IsWishlistEmpty;

        public WishlistViewModel(
            IWishlistService wishlistService,
            ICartService cartService,
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _wishlistService = wishlistService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Mi Lista de Deseos";

            // ⭐ COMENTADO: IWishlistService no tiene el evento
            // _wishlistService.WishlistUpdated += OnWishlistUpdated;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadWishlistAsync();  // ⭐ Siempre recarga al navegar
            LoadRecommendedProducts();
        }

        // ⭐ COMENTADO: Evento no disponible
        /*
        private async void OnWishlistUpdated(object sender, EventArgs e)
        {
            await LoadWishlistAsync();
        }
        */

        [RelayCommand]
        private async Task LoadWishlistAsync()
        {
            try
            {
                IsBusy = true;

                // ⭐ CAMBIO: No requiere usuario autenticado para testing
                var items = await _wishlistService.GetWishlistAsync(1); // userId = 1 por defecto

                WishlistItems = new ObservableCollection<Product>(items);
                ItemCount = WishlistItems.Count;
                WishlistItemCount = WishlistItems.Count;
                IsEmpty = ItemCount == 0;
                IsWishlistEmpty = ItemCount == 0;
                OnPropertyChanged(nameof(HasItems)); // ⭐ Notificar cambio

                System.Diagnostics.Debug.WriteLine($"✅ Wishlist loaded: {ItemCount} items");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading wishlist: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadRecommendedProducts()
        {
            // Productos recomendados (mock data)
            RecommendedProducts = new ObservableCollection<Product>
            {
                new Product { Id = 1, Name = "Recommended Item 1", Price = 49.99m },
                new Product { Id = 2, Name = "Recommended Item 2", Price = 59.99m },
                new Product { Id = 3, Name = "Recommended Item 3", Price = 39.99m }
            };
        }

        [RelayCommand]
        private async Task AddToCartAsync(Product product)
        {
            if (product == null) return;

            try
            {
                await _cartService.AddToCartAsync(product, 1);
                System.Diagnostics.Debug.WriteLine($"✅ Added to cart: {product.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding to cart: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RemoveFromWishlistAsync(Product product)
        {
            if (product == null) return;

            try
            {
                await _wishlistService.RemoveFromWishlistAsync(product.Id);
                WishlistItems.Remove(product);
                ItemCount = WishlistItems.Count;
                WishlistItemCount = WishlistItems.Count;
                IsEmpty = ItemCount == 0;
                IsWishlistEmpty = ItemCount == 0;
                OnPropertyChanged(nameof(HasItems)); // ⭐ Notificar cambio

                System.Diagnostics.Debug.WriteLine($"✅ Removed from wishlist: {product.Name}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error removing from wishlist: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ViewProductDetail(Product product)
        {
            if (product == null) return;
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
        }

        [RelayCommand]
        private async Task AddAllToCartAsync()
        {
            try
            {
                IsBusy = true;

                foreach (var product in WishlistItems)
                {
                    await _cartService.AddToCartAsync(product, 1);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Added all {WishlistItems.Count} items to cart");
                _navigationService.NavigateTo<CartViewModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error adding all to cart: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ClearWishlistAsync()
        {
            try
            {
                await _wishlistService.ClearWishlistAsync();
                WishlistItems.Clear();
                ItemCount = 0;
                WishlistItemCount = 0;
                IsEmpty = true;
                IsWishlistEmpty = true;
                OnPropertyChanged(nameof(HasItems)); // ⭐ Notificar cambio

                System.Diagnostics.Debug.WriteLine("✅ Wishlist cleared");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error clearing wishlist: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ShareWishlist()
        {
            // Implementar compartir wishlist
            System.Diagnostics.Debug.WriteLine("📤 Share wishlist (not implemented yet)");
        }

        [RelayCommand]
        private void BrowseProducts()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void ContinueShopping()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}