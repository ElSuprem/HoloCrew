using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel de la página de inicio
    /// Muestra productos destacados y categorías principales
    /// </summary>
    public partial class HomeViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;

        [ObservableProperty]
        private ObservableCollection<Product> _featuredProducts = new();

        [ObservableProperty]
        private ObservableCollection<Product> _bestSellers = new();

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private string _bannerImageUrl;

        public HomeViewModel(
            IProductService productService,
            INavigationService navigationService,
            ICartService cartService)
        {
            _productService = productService;
            _navigationService = navigationService;
            _cartService = cartService;

            Title = "Inicio";
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
                var categoriesTask = _productService.GetCategoriesAsync();

                await Task.WhenAll(featuredTask, categoriesTask);

                FeaturedProducts = new ObservableCollection<Product>(await featuredTask);
                Categories = new ObservableCollection<Category>(await categoriesTask);

                // Banner por defecto
                BannerImageUrl = "/Resources/Images/banner.jpg";
            }
            catch (Exception ex)
            {
                // TODO: Manejar error (mostrar notificación)
            }
            finally
            {
                IsBusy = false;
            }
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
                // TODO: Mostrar notificación de éxito
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private void NavigateToCategory(Category category)
        {
            if (category == null) return;
            _navigationService.NavigateTo<ProductCatalogViewModel>(category.Id);
        }
    }
}