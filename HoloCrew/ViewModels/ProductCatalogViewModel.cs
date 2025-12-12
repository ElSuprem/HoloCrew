using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class ProductCatalogViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        // Categoría/Subcategoría actual
        [ObservableProperty]
        private string _currentCategoryName = "All Products";

        [ObservableProperty]
        private string _currentSlug;

        [ObservableProperty]
        private int _totalProductCount;

        // BÚSQUEDA
        [ObservableProperty]
        private string _searchQuery;

        // PRECIO
        [ObservableProperty]
        private decimal _minPrice = 0;

        [ObservableProperty]
        private decimal _maxPrice = 500;

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
            "Black", "White", "Gray", "Navy", "Red", "Green"
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

        // ORDENACIÓN
        [ObservableProperty]
        private string _sortBy = "Featured";

        public ObservableCollection<string> SortOptions { get; } = new()
        {
            "Featured", "Newest", "Price: Low to High", "Price: High to Low", "Best Selling"
        };

        public ProductCatalogViewModel(
            IProductService productService,
            INavigationService navigationService,
            ICartService cartService,
            IWishlistService wishlistService)
        {
            _productService = productService;
            _navigationService = navigationService;
            _cartService = cartService;
            _wishlistService = wishlistService;

            Title = "Catálogo";

            // Suscribirse a cambios en wishlist
            _wishlistService.WishlistUpdated += OnWishlistUpdated;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int categoryId)
            {
                // Navegación por ID de categoría
                await LoadProductsByCategoryAsync(categoryId);
            }
            else if (parameter is string param)
            {
                // Puede ser un slug o una búsqueda
                if (IsSearchQuery(param))
                {
                    SearchQuery = param;
                    await SearchAsync();
                }
                else
                {
                    // Es un slug de subcategoría
                    await LoadProductsBySlugAsync(param);
                }
            }
            else
            {
                await LoadAllProductsAsync();
            }
        }

        /// <summary>
        /// Determina si el parámetro es una búsqueda o un slug
        /// </summary>
        private bool IsSearchQuery(string param)
        {
            // Los slugs conocidos
            var knownSlugs = new[]
            {
                "all", "new", "blackweek", "softs", "classic", "activewear", "tracksuits",
                "tshirts", "hoodies", "trackjackets", "jerseys", "knitwear", "jackets",
                "denim", "cargo", "joggers", "trackpants", "jorts", "shorts", "swimshorts", "underwear",
                "armbo", "vortex", "venture", "vitoria", "vslides",
                "caps", "bags", "beanies", "cardholder", "belts", "rings", "rugs"
            };

            return !knownSlugs.Contains(param.ToLower());
        }

        [RelayCommand]
        private async Task LoadAllProductsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                CurrentCategoryName = "All Products";
                CurrentSlug = "all";

                var products = await _productService.GetProductsByCategoryAsync(0);
                Products = new ObservableCollection<Product>(products);
                TotalProductCount = products.Count;
                ApplyAllFilters();
                await LoadWishlistStates();
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
                TotalProductCount = products.Count;

                // Actualizar nombre de categoría
                var categories = await _productService.GetCategoriesAsync();
                var category = FindCategoryById(categories, categoryId);
                CurrentCategoryName = category?.Name ?? "Products";

                ApplyAllFilters();
                await LoadWishlistStates();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Carga productos por slug de subcategoría (desde el mega menú)
        /// </summary>
        private async Task LoadProductsBySlugAsync(string slug)
        {
            try
            {
                IsBusy = true;
                CurrentSlug = slug.ToLower();

                var products = await _productService.GetProductsBySlugAsync(slug);
                Products = new ObservableCollection<Product>(products);
                TotalProductCount = products.Count;

                // Actualizar nombre basado en slug
                CurrentCategoryName = GetCategoryNameFromSlug(slug);
                Title = CurrentCategoryName;

                ApplyAllFilters();
                await LoadWishlistStates();
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Obtiene el nombre legible de una categoría por su slug
        /// </summary>
        private string GetCategoryNameFromSlug(string slug)
        {
            return slug.ToLower() switch
            {
                "all" => "All Products",
                "new" => "New Arrivals",
                "blackweek" => "Black Week",
                "softs" => "Softs Collection",
                "classic" => "Classic Collection",
                "activewear" => "Activewear",
                "tracksuits" => "Tracksuits",
                "tshirts" => "T-Shirts",
                "hoodies" => "Hoodies",
                "trackjackets" => "Track Jackets",
                "jerseys" => "Jerseys",
                "knitwear" => "Knitwear",
                "jackets" => "Jackets",
                "denim" => "Denim Pants",
                "cargo" => "Cargo Pants",
                "joggers" => "Joggers",
                "trackpants" => "Track Pants",
                "jorts" => "Jorts",
                "shorts" => "Shorts",
                "swimshorts" => "Swimshorts",
                "underwear" => "Underwear",
                "armbo" => "Armbo Lows",
                "vortex" => "Vortex",
                "venture" => "Venture",
                "vitoria" => "Vitoria",
                "vslides" => "V-Slides",
                "caps" => "Caps",
                "bags" => "Bags",
                "beanies" => "Beanies",
                "cardholder" => "Cardholder",
                "belts" => "Belts",
                "rings" => "Rings",
                "rugs" => "Rugs",
                _ => slug.ToUpper()
            };
        }

        private Category FindCategoryById(System.Collections.Generic.List<Category> categories, int id)
        {
            foreach (var cat in categories)
            {
                if (cat.Id == id) return cat;
                if (cat.SubCategories != null)
                {
                    var sub = cat.SubCategories.FirstOrDefault(s => s.Id == id);
                    if (sub != null) return sub;
                }
            }
            return null;
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
                CurrentCategoryName = $"Results for \"{SearchQuery}\"";

                var results = await _productService.SearchProductsAsync(SearchQuery);
                Products = new ObservableCollection<Product>(results);
                TotalProductCount = results.Count;
                ApplyAllFilters();
                await LoadWishlistStates();
            }
            finally
            {
                IsFiltering = false;
            }
        }

        private async Task LoadWishlistStates()
        {
            foreach (var product in FilteredProducts)
            {
                product.IsInWishlist = await _wishlistService.IsInWishlistAsync(product.Id);
            }
        }

        private async void OnWishlistUpdated(object sender, EventArgs e)
        {
            await LoadWishlistStates();
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
                SelectedSizes.Remove(size);
            else
                SelectedSizes.Add(size);
            ApplyFilters();
        }

        [RelayCommand]
        private void ToggleColor(string color)
        {
            if (SelectedColors.Contains(color))
                SelectedColors.Remove(color);
            else
                SelectedColors.Add(color);
            ApplyFilters();
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

            // Filtro de TALLAS
            if (SelectedSizes.Any())
            {
                filtered = filtered.Where(p =>
                    p.AvailableSizes != null &&
                    p.AvailableSizes.Any(s => SelectedSizes.Contains(s)));
            }

            // Filtro de COLORES
            if (SelectedColors.Any())
            {
                filtered = filtered.Where(p =>
                    p.AvailableColors != null &&
                    p.AvailableColors.Any(c => SelectedColors.Contains(c)));
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

            // ORDENACIÓN
            filtered = SortBy switch
            {
                "Newest" => filtered.OrderByDescending(p => p.CreatedAt),
                "Price: Low to High" => filtered.OrderBy(p => p.Price),
                "Price: High to Low" => filtered.OrderByDescending(p => p.Price),
                "Best Selling" => filtered.OrderByDescending(p => p.ReviewCount),
                _ => filtered.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
            };

            FilteredProducts = new ObservableCollection<Product>(filtered.ToList());
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            MinPrice = 0;
            MaxPrice = 500;
            SelectedCategory = "All";
            SelectedSizes.Clear();
            SelectedColors.Clear();
            GenderMen = false;
            GenderWomen = false;
            GenderUnisex = false;
            InStockOnly = false;
            OnSaleOnly = false;
            SortBy = "Featured";

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

        // Auto-filtrar cuando cambian los valores
        partial void OnMinPriceChanged(decimal value) => ApplyFilters();
        partial void OnMaxPriceChanged(decimal value) => ApplyFilters();
        partial void OnGenderMenChanged(bool value) => ApplyFilters();
        partial void OnGenderWomenChanged(bool value) => ApplyFilters();
        partial void OnGenderUnisexChanged(bool value) => ApplyFilters();
        partial void OnInStockOnlyChanged(bool value) => ApplyFilters();
        partial void OnOnSaleOnlyChanged(bool value) => ApplyFilters();
        partial void OnSortByChanged(string value) => ApplyFilters();
    }
}