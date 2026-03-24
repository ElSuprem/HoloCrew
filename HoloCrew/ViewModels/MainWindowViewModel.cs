using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Threading;
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
        private readonly IProductService _productService;

        // Debounce timer for live search
        private CancellationTokenSource _searchCts;

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

        [ObservableProperty]
        private bool _isSearchVisible;

        // Live search results
        [ObservableProperty]
        private ObservableCollection<Product> _searchResults = new();

        [ObservableProperty]
        private bool _hasSearchResults;

        [ObservableProperty]
        private bool _isSearching;

        [ObservableProperty]
        private int _searchResultCount;

        public MainWindowViewModel(
            INavigationService navigationService,
            ICartService cartService,
            IAuthenticationService authenticationService,
            IWishlistService wishlistService,
            INotificationService notificationService,
            IProductService productService)
        {
            _navigationService = navigationService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _wishlistService = wishlistService;
            _notificationService = notificationService;
            _productService = productService;

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
            if (!IsSearchVisible)
            {
                SearchQuery = string.Empty;
                SearchResults.Clear();
                HasSearchResults = false;
                SearchResultCount = 0;
            }
        }

        private void CloseSearch()
        {
            IsSearchVisible = false;
            SearchQuery = string.Empty;
            SearchResults.Clear();
            HasSearchResults = false;
            SearchResultCount = 0;
        }

        // Live search - triggered when SearchQuery changes
        partial void OnSearchQueryChanged(string value)
        {
            _ = PerformLiveSearchAsync(value);
        }

        private async Task PerformLiveSearchAsync(string query)
        {
            // Cancel previous search
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                SearchResults.Clear();
                HasSearchResults = false;
                SearchResultCount = 0;
                IsSearching = false;
                return;
            }

            IsSearching = true;

            try
            {
                // Debounce 300ms
                await Task.Delay(300, token);
                if (token.IsCancellationRequested) return;

                var results = await _productService.SearchProductsAsync(query);
                if (token.IsCancellationRequested) return;

                SearchResults = new ObservableCollection<Product>(results.Take(6));
                SearchResultCount = results.Count;
                HasSearchResults = results.Any();
            }
            catch (TaskCanceledException)
            {
                // Debounce cancelled, ignore
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            }
            finally
            {
                IsSearching = false;
            }
        }

        [RelayCommand]
        private void ViewSearchResult(Product product)
        {
            if (product == null) return;
            CloseSearch();
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
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
                CurrentUserName = user?.FullName ?? "User";
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