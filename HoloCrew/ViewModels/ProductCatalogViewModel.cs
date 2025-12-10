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
        private readonly IWishlistService _wishlistService;  // ⭐ AGREGADO

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        // BÚSQUEDA
        [ObservableProperty]
        private string _searchQuery;

        // PRECIO
        [ObservableProperty]
        private decimal _minPrice = 0;

        [ObservableProperty]
        private decimal _maxPrice = 1000;

        // CATEGORÍA
        [ObservableProperty]
        private string _selectedCategory = "All";

        // TALLAS (múltiple selección)
        [ObservableProperty]
        private ObservableCollection<string> _selectedSizes = new();

        public ObservableCollection<string> AvailableSizes { get; } = new()
        {
            "XS", "S", "M", "L", "XL", "XXL"
        };

        // COLORES (múltiple selección)
        [ObservableProperty]
        private ObservableCollection<string> _selectedColors = new();

        public ObservableCollection<string> AvailableColors { get; } = new()
        {
            "Black", "White", "Gray", "Blue", "Red", "Green"
        };

        // GÉNERO
        [ObservableProperty]
        private bool _genderMen = false;

        [ObservableProperty]
        private bool _genderWomen = false;

        [ObservableProperty]
        private bool _genderUnisex = false;

        // DISPONIBILIDAD
        [ObservableProperty]
        private bool _inStockOnly = false;

        [ObservableProperty]
        private bool _onSaleOnly = false;

        [ObservableProperty]
        private bool _isFiltering;

        public ProductCatalogViewModel(
            IProductService productService,
            INavigationService navigationService,
            ICartService cartService,
            IWishlistService wishlistService)  // ⭐ AGREGADO
        {
            _productService = productService;
            _navigationService = navigationService;
            _cartService = cartService;
            _wishlistService = wishlistService;  // ⭐ AGREGADO

            Title = "Catálogo de Productos";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int categoryId)
            {
                await LoadProductsByCategoryAsync(categoryId);
            }
            else if (parameter is string searchQuery)
            {
                SearchQuery = searchQuery;
                await SearchAsync();
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
                var products = await _productService.GetProductsByCategoryAsync(0);
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
                Products = new ObservableCollection<Product>(results);
                ApplyAllFilters();
            }
            finally
            {
                IsFiltering = false;
            }
        }

        [RelayCommand]
        private void SelectCategory(string category)
        {
            SelectedCategory = category;
            ApplyFilters();
        }

        [RelayCommand]
        private void ToggleSize(string size)
        {
            if (SelectedSizes.Contains(size))
            {
                SelectedSizes.Remove(size);
            }
            else
            {
                SelectedSizes.Add(size);
            }
        }

        [RelayCommand]
        private void ToggleColor(string color)
        {
            if (SelectedColors.Contains(color))
            {
                SelectedColors.Remove(color);
            }
            else
            {
                SelectedColors.Add(color);
            }
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            ApplyAllFilters();
        }

        private void ApplyAllFilters()
        {
            var filtered = Products.AsEnumerable();

            // Filtro de PRECIO
            filtered = filtered.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice);

            // Filtro de CATEGORÍA
            if (SelectedCategory != "All" && !string.IsNullOrEmpty(SelectedCategory))
            {
                filtered = filtered.Where(p => p.Category?.Name == SelectedCategory);
            }

            // Filtro de TALLAS (si hay alguna seleccionada)
            if (SelectedSizes.Any())
            {
                // Asumiendo que Product tiene una propiedad AvailableSizes
                // filtered = filtered.Where(p => p.AvailableSizes?.Any(s => SelectedSizes.Contains(s)) == true);

                // Si no tienes esa propiedad aún, se omite este filtro por ahora
            }

            // Filtro de COLORES (si hay alguno seleccionado)
            if (SelectedColors.Any())
            {
                // Asumiendo que Product tiene una propiedad AvailableColors
                // filtered = filtered.Where(p => p.AvailableColors?.Any(c => SelectedColors.Contains(c)) == true);

                // Si no tienes esa propiedad aún, se omite este filtro por ahora
            }

            // Filtro de GÉNERO
            if (GenderMen || GenderWomen || GenderUnisex)
            {
                filtered = filtered.Where(p =>
                {
                    if (GenderMen && p.Gender == "Men") return true;
                    if (GenderWomen && p.Gender == "Women") return true;
                    if (GenderUnisex && p.Gender == "Unisex") return true;
                    return false;
                });
            }

            // Filtro de DISPONIBILIDAD
            if (InStockOnly)
            {
                filtered = filtered.Where(p => p.Stock > 0);
            }

            if (OnSaleOnly)
            {
                filtered = filtered.Where(p => p.HasDiscount);
            }

            FilteredProducts = new ObservableCollection<Product>(filtered.ToList());
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            MinPrice = 0;
            MaxPrice = 1000;
            SelectedCategory = "All";
            SelectedSizes.Clear();
            SelectedColors.Clear();
            GenderMen = false;
            GenderWomen = false;
            GenderUnisex = false;
            InStockOnly = false;
            OnSaleOnly = false;

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
            // TODO: Mostrar mensaje "Added to cart"
        }

        // ⭐ NUEVO: Agregar a wishlist
        [RelayCommand]
        private async Task AddToWishlistAsync(int productId)
        {
            try
            {
                await _wishlistService.AddToWishlistAsync(productId);
                // TODO: Mostrar mensaje "Added to wishlist ❤️"
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
                System.Diagnostics.Debug.WriteLine($"Error adding to wishlist: {ex.Message}");
            }
        }

        // Auto-filtrar cuando cambian los valores
        partial void OnMinPriceChanged(decimal value) => ApplyFilters();
        partial void OnMaxPriceChanged(decimal value) => ApplyFilters();
        partial void OnGenderMenChanged(bool value) => ApplyFilters();
        partial void OnGenderWomenChanged(bool value) => ApplyFilters();
        partial void OnGenderUnisexChanged(bool value) => ApplyFilters();
        partial void OnInStockOnlyChanged(bool value) => ApplyFilters();
        partial void OnOnSaleOnlyChanged(bool value) => ApplyFilters();
    }
}