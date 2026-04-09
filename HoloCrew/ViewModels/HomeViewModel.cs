using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// ViewModel de la página principal (Home).
// Muestra productos destacados, categorías, y tiene navegación a otras secciones.
// Se conecta con ProductService, NavigationService, CartService y WishlistService.

namespace HoloCrew.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        [ObservableProperty]
        private ObservableCollection<Product> _featuredProducts = new();

        [ObservableProperty]
        private ObservableCollection<Product> _bestSellers = new();

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private string _bannerImageUrl = string.Empty;

        [ObservableProperty]
        private string _newsletterEmail = string.Empty;

        [ObservableProperty]
        private bool _isSubscribing;

        [ObservableProperty]
        private string _subscriptionMessage = string.Empty;

        public HomeViewModel(
            IProductService productService,
            INavigationService navigationService,
            ICartService cartService,
            IWishlistService wishlistService)
        {
            _productService = productService;
            _navigationService = navigationService;
            _cartService = cartService;
            _wishlistService = wishlistService;

            Title = AppConstants.UI.Home;
        }

        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var featuredTask = _productService.GetFeaturedProductsAsync();
                var categoriesTask = _productService.GetCategoriesAsync();

                await Task.WhenAll(featuredTask, categoriesTask);

                FeaturedProducts = new ObservableCollection<Product>(await featuredTask);
                Categories = new ObservableCollection<Category>(await categoriesTask);

                BannerImageUrl = "/Resources/Images/banner.jpg";

                if (FeaturedProducts.Count == 0)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        [RelayCommand]
        private void ViewProductDetail(Product product)
        {
            if (product == null) return;
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
        }

        [RelayCommand]
        private void NavigateToCatalog()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void NavigateToCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return;
            _navigationService.NavigateTo<ProductCatalogViewModel>(category);
        }

        [RelayCommand]
        private void NavigateToBlackWeek()
        {
            _navigationService.NavigateTo<BlackWeekViewModel>();
        }

        [RelayCommand]
        private void NavigateToFlashSale()
        {
            _navigationService.NavigateTo<FlashSaleViewModel>();
        }

        [RelayCommand]
        private void NavigateToMembers()
        {
            _navigationService.NavigateTo<MembersClubViewModel>();
        }

        [RelayCommand]
        private void ViewLookbook()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>("softs");
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
                System.Diagnostics.Debug.WriteLine($"Error adding to cart: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ToggleWishlistAsync(Product product)
        {
            if (product == null) return;

            product.IsInWishlist = !product.IsInWishlist;

            try
            {
                if (product.IsInWishlist)
                    await _wishlistService.AddToWishlistAsync(product);
                else
                    await _wishlistService.RemoveFromWishlistAsync(product.Id);
            }
            catch (Exception ex)
            {
                product.IsInWishlist = !product.IsInWishlist;
                System.Diagnostics.Debug.WriteLine($"Error toggling wishlist: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SubscribeNewsletterAsync()
        {
            if (string.IsNullOrWhiteSpace(NewsletterEmail))
            {
                SubscriptionMessage = "Please enter a valid email";
                return;
            }

            if (!NewsletterEmail.Contains("@"))
            {
                SubscriptionMessage = "Please enter a valid email";
                return;
            }

            IsSubscribing = true;
            SubscriptionMessage = string.Empty;

            try
            {
                await Task.Delay(1000);  // simula el envío del email
                SubscriptionMessage = "Thanks for subscribing!";
                NewsletterEmail = string.Empty;
            }
            catch (Exception ex)
            {
                SubscriptionMessage = "Error subscribing. Please try again.";
                System.Diagnostics.Debug.WriteLine($"Error subscribing: {ex.Message}");
            }
            finally
            {
                IsSubscribing = false;
            }
        }
    }
}