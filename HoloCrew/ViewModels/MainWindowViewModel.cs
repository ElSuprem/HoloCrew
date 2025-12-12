using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IWishlistService _wishlistService;
        private readonly INotificationService _notificationService;

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

        [ObservableProperty]
        private int _unreadNotificationCount;

        // ⭐ NUEVO: Control de visibilidad de la barra de búsqueda
        [ObservableProperty]
        private bool _isSearchVisible;

        public MainWindowViewModel(
            INavigationService navigationService,
            ICartService cartService,
            IAuthenticationService authenticationService,
            IWishlistService wishlistService,
            INotificationService notificationService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _wishlistService = wishlistService;
            _notificationService = notificationService;

            Title = "HoloCrew";

            // Suscribirse a eventos
            _cartService.CartUpdated += OnCartUpdated;
            _wishlistService.WishlistUpdated += OnWishlistUpdated;
            _notificationService.NotificationReceived += OnNotificationReceived;

            // Inicializar estados
            UpdateAuthenticationState();
            CartItemCount = _cartService.GetCartItemCount();
            UpdateWishlistCount();
            UpdateNotificationCount();
        }

        // ============================
        // COMANDOS DE NAVEGACIÓN
        // ============================

        [RelayCommand]
        private void NavigateToHome()
        {
            _navigationService.NavigateTo<HomeViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToCatalog(object parameter = null)
        {
            // Si viene un parámetro de categoría, pasarlo al catálogo
            if (parameter != null && parameter is string category)
            {
                _navigationService.NavigateTo<ProductCatalogViewModel>(category);
            }
            else
            {
                _navigationService.NavigateTo<ProductCatalogViewModel>();
            }
            CloseSearch();
        }

        [RelayCommand]
        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                return;

            _navigationService.NavigateTo<ProductCatalogViewModel>(SearchQuery);
            SearchQuery = string.Empty;
            IsSearchVisible = false;
        }

        // ⭐ NUEVO: Toggle de la barra de búsqueda
        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchVisible = !IsSearchVisible;
        }

        private void CloseSearch()
        {
            IsSearchVisible = false;
            SearchQuery = string.Empty;
        }

        [RelayCommand]
        private void NavigateToCart()
        {
            _navigationService.NavigateTo<CartViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToWishlist()
        {
            _navigationService.NavigateTo<WishlistViewModel>();
            UpdateWishlistCount();
            CloseSearch();
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
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToSettings()
        {
            _navigationService.NavigateTo<SettingsViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToNotifications()
        {
            _navigationService.NavigateTo<NotificationsViewModel>();
            UpdateNotificationCount();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToOrders()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToFlashSale()
        {
            _navigationService.NavigateTo<FlashSaleViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToBlackWeek()
        {
            _navigationService.NavigateTo<BlackWeekViewModel>();
            CloseSearch();
        }

        [RelayCommand]
        private void NavigateToMembersClub()
        {
            _navigationService.NavigateTo<MembersClubViewModel>();
            CloseSearch();
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

        // ============================
        // EVENT HANDLERS
        // ============================

        private void OnCartUpdated(object sender, EventArgs e)
        {
            CartItemCount = _cartService.GetCartItemCount();
        }

        private void OnWishlistUpdated(object sender, EventArgs e)
        {
            UpdateWishlistCount();
        }

        private void OnNotificationReceived(object sender, Models.Notification notification)
        {
            UpdateNotificationCount();
        }

        // ============================
        // MÉTODOS AUXILIARES
        // ============================

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

        private void UpdateNotificationCount()
        {
            UnreadNotificationCount = _notificationService.GetUnreadCount();
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            UpdateWishlistCount();
            CartItemCount = _cartService.GetCartItemCount();
            UpdateNotificationCount();
        }
    }
}