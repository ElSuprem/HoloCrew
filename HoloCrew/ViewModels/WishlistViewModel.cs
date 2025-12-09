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

        // ⭐ PROPIEDADES AGREGADAS
        [ObservableProperty]
        private int _wishlistItemCount;

        [ObservableProperty]
        private bool _isWishlistEmpty = true;

        [ObservableProperty]
        private ObservableCollection<Product> _recommendedProducts = new();

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
            try
            {
                IsBusy = true;

                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    var items = await _wishlistService.GetWishlistAsync(currentUser.Id);
                    WishlistItems = new ObservableCollection<Product>(items);
                    ItemCount = WishlistItems.Count;
                    WishlistItemCount = WishlistItems.Count;
                    IsEmpty = ItemCount == 0;
                    IsWishlistEmpty = ItemCount == 0;
                }
            }
            catch (Exception ex)
            {
                // Manejar error
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
            }
            catch (Exception ex)
            {
                // Manejar error
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
            }
            catch (Exception ex)
            {
                // Manejar error
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

                _navigationService.NavigateTo<CartViewModel>();
            }
            catch (Exception ex)
            {
                // Manejar error
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
            }
            catch (Exception ex)
            {
                // Manejar error
            }
        }

        [RelayCommand]
        private void ShareWishlist()
        {
            // Implementar compartir wishlist
        }

        [RelayCommand]
        private void ContinueShopping()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}
