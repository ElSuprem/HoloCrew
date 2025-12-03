using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    public partial class ProductCatalogViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        [ObservableProperty]
        private string _searchQuery;

        [ObservableProperty]
        private Category _selectedCategory;

        [ObservableProperty]
        private decimal _minPrice;

        [ObservableProperty]
        private decimal _maxPrice = 1000;

        [ObservableProperty]
        private bool _isFiltering;

        public ProductCatalogViewModel(
            IProductService productService,
            INavigationService navigationService,
            ICartService cartService)
        {
            _productService = productService;
            _navigationService = navigationService;
            _cartService = cartService;

            Title = "Catálogo de Productos";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int categoryId)
            {
                // Navegar con filtro de categoría
                await LoadProductsByCategoryAsync(categoryId);
            }
            else
            {
                await LoadAllProductsAsync();
            }
        }

        [RelayCommand]
        private async Task LoadAllProductsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var products = await _productService.GetProductsByCategoryAsync(0); // 0 = todas
                Products = new ObservableCollection<Product>(products);
                FilteredProducts = new ObservableCollection<Product>(products);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadProductsByCategoryAsync(int categoryId)
        {
            try
            {
                IsBusy = true;
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                Products = new ObservableCollection<Product>(products);
                FilteredProducts = new ObservableCollection<Product>(products);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredProducts = new ObservableCollection<Product>(Products);
                return;
            }

            try
            {
                IsFiltering = true;
                var results = await _productService.SearchProductsAsync(SearchQuery);
                FilteredProducts = new ObservableCollection<Product>(results);
            }
            finally
            {
                IsFiltering = false;
            }
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            var filtered = Products.Where(p =>
                p.Price >= MinPrice &&
                p.Price <= MaxPrice
            ).ToList();

            FilteredProducts = new ObservableCollection<Product>(filtered);
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            MinPrice = 0;
            MaxPrice = 1000;
            FilteredProducts = new ObservableCollection<Product>(Products);
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
            await _cartService.AddToCartAsync(product, 1);
        }
    }
}