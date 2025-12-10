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
        private readonly IWishlistService _wishlistService;

        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private int _cartItemCount;

        [ObservableProperty]
        private int _wishlistItemCount;

        [ObservableProperty]
        private bool _isUserLoggedIn;

        [ObservableProperty]
        private string _currentUserName;

        public MainWindowViewModel(
            INavigationService navigationService,
            ICartService cartService,
            IAuthenticationService authenticationService,
            IWishlistService wishlistService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _wishlistService = wishlistService;

            Title = "HoloCrew";

            // Suscribirse a cambios en el carrito
            _cartService.CartUpdated += OnCartUpdated;

            // ⭐ Suscribirse a cambios en wishlist
            _wishlistService.WishlistUpdated += OnWishlistUpdated;

            // Actualizar estado de autenticación
            UpdateAuthenticationState();

            // Actualizar contador del carrito
            CartItemCount = _cartService.GetCartItemCount();

            // Actualizar contador de wishlist
            UpdateWishlistCount();
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
        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            _navigationService.NavigateTo<ProductCatalogViewModel>(SearchQuery);
            SearchQuery = string.Empty;
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
            UpdateWishlistCount(); // Actualizar al navegar
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

        // ⭐ NUEVO: Evento para actualizar contador de wishlist automáticamente
        private void OnWishlistUpdated(object sender, EventArgs e)
        {
            UpdateWishlistCount();
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

        private async void UpdateWishlistCount()
        {
            WishlistItemCount = await _wishlistService.GetWishlistCountAsync(1);
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            UpdateWishlistCount();
            CartItemCount = _cartService.GetCartItemCount();
        }
    }
}