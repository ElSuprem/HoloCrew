using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// ViewModel de la página de lista de deseos (Wishlist).
// Muestra los productos que el usuario ha marcado como favoritos.
// Permite añadir al carrito, eliminar de la wishlist, añadir todo al carrito, etc.
// Se conecta con WishlistService, CartService, AuthenticationService y NavigationService.

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
        private int _itemCount;

        [ObservableProperty]
        private int _wishlistItemCount;

        [ObservableProperty]
        private bool _isWishlistEmpty = true;

        [ObservableProperty]
        private ObservableCollection<Product> _recommendedProducts = new();

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

            Title = AppConstants.UI.Wishlist;
            EmptyTitle = AppConstants.Empty.WishlistTitle;
            EmptySubtitle = AppConstants.Empty.WishlistSubtitle;
            EmptyActionText = AppConstants.Empty.WishlistAction;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadWishlistAsync();
            LoadRecommendedProducts();
        }

        [RelayCommand]
        private async Task LoadWishlistAsync()
        {
            await ExecuteAsync(async () =>
            {
                var items = await _wishlistService.GetWishlistAsync(1);

                WishlistItems = new ObservableCollection<Product>(items);
                ItemCount = WishlistItems.Count;
                WishlistItemCount = WishlistItems.Count;
                IsWishlistEmpty = ItemCount == 0;
                OnPropertyChanged(nameof(HasItems));

                if (IsWishlistEmpty)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        // productos recomendados para mostrar cuando la wishlist está vacía
        private void LoadRecommendedProducts()
        {
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
                IsWishlistEmpty = ItemCount == 0;
                OnPropertyChanged(nameof(HasItems));

                if (IsWishlistEmpty) SetEmpty();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
            await ExecuteAsync(async () =>
            {
                foreach (var product in WishlistItems)
                {
                    await _cartService.AddToCartAsync(product, 1);
                }
                _navigationService.NavigateTo<CartViewModel>();
            }, isRefresh: true);
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
                IsWishlistEmpty = true;
                OnPropertyChanged(nameof(HasItems));
                SetEmpty();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ShareWishlist()
        {
            // pendiente: compartir la wishlist
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

        [RelayCommand]
        private void EmptyAction()
        {
            BrowseProducts();
        }
    }
}