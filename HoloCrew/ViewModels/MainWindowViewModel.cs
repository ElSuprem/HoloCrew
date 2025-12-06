using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;

namespace HoloCrew.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authenticationService;

        [ObservableProperty]
        private object _currentView;  // IMPORTANTE: Este binding muestra la vista actual

        [ObservableProperty]
        private int _cartItemCount;

        [ObservableProperty]
        private bool _isUserLoggedIn;

        [ObservableProperty]
        private string _currentUserName;

        [ObservableProperty]
        private string _searchQuery;

        public MainWindowViewModel(
            INavigationService navigationService,
            ICartService cartService,
            IAuthenticationService authenticationService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _authenticationService = authenticationService;

            Title = "HoloCrew";

            // Suscribirse a cambios en el carrito
            _cartService.CartUpdated += OnCartUpdated;

            // Actualizar estado de autenticación
            UpdateAuthenticationState();

            // Actualizar contador del carrito
            CartItemCount = _cartService.GetCartItemCount();
        }

        // ============================
        // COMANDOS DE NAVEGACIÓN
        // ============================

        [RelayCommand]
        private void NavigateToHome()
        {
            _navigationService.NavigateTo<HomeViewModel>();
        }

        [RelayCommand]
        private void NavigateToCatalog()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void NavigateToCart()
        {
            _navigationService.NavigateTo<CartViewModel>();
        }

        [RelayCommand]
        private void NavigateToWishlist()
        {
            _navigationService.NavigateTo<WishlistViewModel>();
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            if (IsUserLoggedIn)
            {
                _navigationService.NavigateTo<ProfileViewModel>();
            }
            else
            {
                _navigationService.NavigateTo<LoginViewModel>();
            }
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            _navigationService.NavigateTo<SettingsViewModel>();
        }

        [RelayCommand]
        private void NavigateToNotifications()
        {
            _navigationService.NavigateTo<NotificationsViewModel>();
        }

        [RelayCommand]
        private void NavigateToOrders()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }

        [RelayCommand]
        private void NavigateToFlashSale()
        {
            _navigationService.NavigateTo<FlashSaleViewModel>();
        }

        [RelayCommand]
        private void NavigateToBlackWeek()
        {
            _navigationService.NavigateTo<BlackWeekViewModel>();
        }

        [RelayCommand]
        private void NavigateToMembersClub()
        {
            _navigationService.NavigateTo<MembersClubViewModel>();
        }

        // ============================
        // BÚSQUEDA
        // ============================

        [RelayCommand]
        private void Search()
        {
            if (!string.IsNullOrWhiteSpace(_searchQuery))
            {
                // Navegar al catálogo con query de búsqueda
                _navigationService.NavigateTo<ProductCatalogViewModel>(_searchQuery);
            }
        }

        // ============================
        // AUTENTICACIÓN
        // ============================

        [RelayCommand]
        private async Task LogoutAsync()
        {
            await _authenticationService.LogoutAsync();
            UpdateAuthenticationState();
            _navigationService.NavigateTo<HomeViewModel>();
        }

        private void OnCartUpdated(object sender, EventArgs e)
        {
            CartItemCount = _cartService.GetCartItemCount();
        }

        private async void UpdateAuthenticationState()
        {
            IsUserLoggedIn = await _authenticationService.IsAuthenticatedAsync();

            if (IsUserLoggedIn)
            {
                var user = _authenticationService.GetCurrentUser();
                CurrentUserName = user?.FullName ?? "Usuario";
            }
            else
            {
                CurrentUserName = null;
            }
        }
    }
}
