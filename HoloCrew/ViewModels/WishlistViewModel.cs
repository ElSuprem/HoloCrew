using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para la lista de deseos (wishlist)
    /// </summary>
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
                    IsEmpty = ItemCount == 0;
                }
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task AddToCartAsync(Product product)
        {
            if (product == null) return;

            try
            {
                await _cartService.AddToCartAsync(product, 1);
                // TODO: Mostrar notificación de éxito
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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
                IsEmpty = ItemCount == 0;
                // TODO: Mostrar notificación
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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

                // TODO: Mostrar notificación de éxito
                _navigationService.NavigateTo<CartViewModel>();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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
                IsEmpty = true;
                // TODO: Mostrar notificación
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private void ShareWishlist()
        {
            // TODO: Implementar compartir wishlist (generar URL o exportar)
        }

        [RelayCommand]
        private void ContinueShopping()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}