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
    /// <summary>
    /// ViewModel de la página de inicio
    /// Muestra productos destacados, categorías y colecciones
    /// </summary>
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        [ObservableProperty]
        private ObservableCollection<Product> _featuredProducts = new();

        [ObservableProperty]
        private ObservableCollection<Product> _newArrivals = new();

        [ObservableProperty]
        private ObservableCollection<Product> _bestSellers = new();

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private string _bannerImageUrl;

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

            Title = "Inicio";

            // Suscribirse a cambios en wishlist
            _wishlistService.WishlistUpdated += OnWishlistUpdated;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Cargar datos en paralelo
                var featuredTask = _productService.GetFeaturedProductsAsync();
                var newArrivalsTask = _productService.GetNewProductsAsync();
                var categoriesTask = _productService.GetCategoriesAsync();

                await Task.WhenAll(featuredTask, newArrivalsTask, categoriesTask);

                FeaturedProducts = new ObservableCollection<Product>(await featuredTask);
                NewArrivals = new ObservableCollection<Product>(await newArrivalsTask);
                Categories = new ObservableCollection<Category>(await categoriesTask);

                // Cargar estado de wishlist
                await LoadWishlistStatesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading home data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadWishlistStatesAsync()
        {
            foreach (var product in FeaturedProducts)
            {
                product.IsInWishlist = await _wishlistService.IsInWishlistAsync(product.Id);
            }
            foreach (var product in NewArrivals)
            {
                product.IsInWishlist = await _wishlistService.IsInWishlistAsync(product.Id);
            }
        }

        private async void OnWishlistUpdated(object sender, EventArgs e)
        {
            await LoadWishlistStatesAsync();
        }

        [RelayCommand]
        private void ViewProductDetail(Product product)
        {
            if (product == null) return;
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
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

            try
            {
                if (product.IsInWishlist)
                {
                    await _wishlistService.RemoveFromWishlistAsync(product.Id);
                    product.IsInWishlist = false;
                }
                else
                {
                    await _wishlistService.AddToWishlistAsync(product.Id);
                    product.IsInWishlist = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error toggling wishlist: {ex.Message}");
            }
        }

        /// <summary>
        /// Navega a una categoría por su slug (desde el mega menú o las cards)
        /// </summary>
        [RelayCommand]
        private void NavigateToCategory(object parameter)
        {
            if (parameter == null) return;

            if (parameter is string slug)
            {
                _navigationService.NavigateTo<ProductCatalogViewModel>(slug);
            }
            else if (parameter is Category category)
            {
                _navigationService.NavigateTo<ProductCatalogViewModel>(category.Slug ?? category.Id.ToString());
            }
        }

        [RelayCommand]
        private void NavigateToCatalog()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void NavigateToMembers()
        {
            _navigationService.NavigateTo<MembersClubViewModel>();
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
    }
}