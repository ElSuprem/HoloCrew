using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
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

            Title = AppConstants.UI.Home;
        }

        public override async void OnNavigatedTo(object parameter)
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

                if (FeaturedProducts.Count == 0) SetEmpty(); else SetSuccess();
            });
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
            try { await _cartService.AddToCartAsync(product, 1); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}"); }
        }

        [RelayCommand]
        private void NavigateToCategory(Category category)
        {
            if (category == null) return;
            _navigationService.NavigateTo<ProductCatalogViewModel>(category.Id);
        }

        [RelayCommand]
        private void NavigateToCatalog() => _navigationService.NavigateTo<ProductCatalogViewModel>();

        [RelayCommand]
        private void NavigateToMembers() => _navigationService.NavigateTo<MembersClubViewModel>();
    }
}