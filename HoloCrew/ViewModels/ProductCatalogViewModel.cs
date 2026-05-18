using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// ViewModel del catálogo de productos.
// Muestra productos con filtros por categoría, precio, talla, color, género, etc.
// Se conecta con ProductService, NavigationService, CartService y WishlistService.

namespace HoloCrew.ViewModels
{
    public partial class ProductCatalogViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        private List<Product> _allProducts = new();
        private bool _isResetting = false;
        private bool _isInitialized = false;

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<Product> _products = new();

        [ObservableProperty]
        private ObservableCollection<Product> _filteredProducts = new();

        [ObservableProperty]
        private string _currentCategoryName = "All Products";

        [ObservableProperty]
        private string _currentSlug = string.Empty;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private string _sortBy = "Featured";

        [ObservableProperty]
        private int _totalProductCount;

        // precio
        [ObservableProperty]
        private decimal _minPrice = 0;

        [ObservableProperty]
        private decimal _maxPrice = 500;

        // categorías
        [ObservableProperty]
        private string _selectedCategory = "All";

        [ObservableProperty]
        private bool _categoryAll = true;
        [ObservableProperty]
        private bool _categoryTops = false;
        [ObservableProperty]
        private bool _categoryBottoms = false;
        [ObservableProperty]
        private bool _categoryFootwear = false;
        [ObservableProperty]
        private bool _categoryAccessories = false;

        // disponibilidad
        [ObservableProperty]
        private bool _inStockOnly;

        [ObservableProperty]
        private bool _onSaleOnly;

        // tallas
        [ObservableProperty]
        private bool _sizeXS;
        [ObservableProperty]
        private bool _sizeS;
        [ObservableProperty]
        private bool _sizeM;
        [ObservableProperty]
        private bool _sizeL;
        [ObservableProperty]
        private bool _sizeXL;
        [ObservableProperty]
        private bool _sizeXXL;

        // colores
        [ObservableProperty]
        private bool _colorBlack;
        [ObservableProperty]
        private bool _colorWhite;
        [ObservableProperty]
        private bool _colorGray;
        [ObservableProperty]
        private bool _colorNavy;
        [ObservableProperty]
        private bool _colorRed;
        [ObservableProperty]
        private bool _colorGreen;

        // género
        [ObservableProperty]
        private bool _genderMen;
        [ObservableProperty]
        private bool _genderWomen;
        [ObservableProperty]
        private bool _genderUnisex;

        #endregion

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

            Title = "Shop";
        }

        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is string slug && !string.IsNullOrEmpty(slug))
            {
                CurrentSlug = slug;
                CurrentCategoryName = GetCategoryNameFromSlug(slug);
                await LoadProductsBySlugAsync(slug);
            }
            else if (parameter is int categoryId)
            {
                await LoadProductsByCategoryAsync(categoryId);
            }
            else
            {
                CurrentCategoryName = "All Products";
                await LoadAllProductsAsync();
            }
        }

        #region Data Loading

        private async Task LoadAllProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = "Loading products...";
                var products = await _productService.GetAllProductsAsync();
                SetProducts(products);
            });
        }

        private async Task LoadProductsBySlugAsync(string slug)
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = $"Loading {GetCategoryNameFromSlug(slug)}...";

                var products = slug.ToLower() switch
                {
                    "new" => await _productService.GetNewProductsAsync(),
                    "bestsellers" or "best-sellers" => await _productService.GetBestSellersAsync(),
                    "featured" => await _productService.GetFeaturedProductsAsync(),
                    "blackweek" => await _productService.GetBlackWeekProductsAsync(),
                    _ => await _productService.GetProductsBySlugAsync(slug)
                };

                SetProducts(products);
            });
        }

        private async Task LoadProductsByCategoryAsync(int categoryId)
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = "Loading category...";
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                SetProducts(products);
            });
        }

        private void SetProducts(List<Product> products)
        {
            _allProducts = products;
            Products = new ObservableCollection<Product>(products);
            _isInitialized = true;
            ApplyFiltersInternal();
            SetSuccess();
        }

        #endregion

        #region Commands - Navigation

        [RelayCommand]
        private void ViewProductDetail(Product product)
        {
            if (product == null) return;
            _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
        }

        #endregion

        #region Commands - Cart & Wishlist

        [RelayCommand]
        private async Task AddToCartAsync(Product product)
        {
            if (product == null) return;

            // Si el producto tiene varias tallas, llevamos al detalle para que el usuario
            // elija una antes de añadir al carrito (UX estándar de e-commerce).
            if (product.AvailableSizes != null && product.AvailableSizes.Count > 0)
            {
                _navigationService.NavigateTo<ProductDetailViewModel>(product.Id);
                return;
            }

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

        #endregion

        #region Commands - Filtering

        [RelayCommand]
        private void Search()
        {
            ApplyFiltersInternal();
        }

        [RelayCommand]
        private void SelectCategory(string category)
        {
            if (_isResetting) return;

            SelectedCategory = category ?? "All";
            UpdateCategoryFlags();
            ApplyFiltersInternal();
        }

        [RelayCommand]
        private void ToggleSize(string size)
        {
            if (string.IsNullOrEmpty(size) || _isResetting) return;

            switch (size)
            {
                case "XS": _sizeXS = !_sizeXS; OnPropertyChanged(nameof(SizeXS)); break;
                case "S": _sizeS = !_sizeS; OnPropertyChanged(nameof(SizeS)); break;
                case "M": _sizeM = !_sizeM; OnPropertyChanged(nameof(SizeM)); break;
                case "L": _sizeL = !_sizeL; OnPropertyChanged(nameof(SizeL)); break;
                case "XL": _sizeXL = !_sizeXL; OnPropertyChanged(nameof(SizeXL)); break;
                case "XXL": _sizeXXL = !_sizeXXL; OnPropertyChanged(nameof(SizeXXL)); break;
            }

            ApplyFiltersInternal();
        }

        [RelayCommand]
        private void ToggleColor(string color)
        {
            if (string.IsNullOrEmpty(color) || _isResetting) return;

            switch (color)
            {
                case "Black": _colorBlack = !_colorBlack; OnPropertyChanged(nameof(ColorBlack)); break;
                case "White": _colorWhite = !_colorWhite; OnPropertyChanged(nameof(ColorWhite)); break;
                case "Gray": _colorGray = !_colorGray; OnPropertyChanged(nameof(ColorGray)); break;
                case "Navy": _colorNavy = !_colorNavy; OnPropertyChanged(nameof(ColorNavy)); break;
                case "Red": _colorRed = !_colorRed; OnPropertyChanged(nameof(ColorRed)); break;
                case "Green": _colorGreen = !_colorGreen; OnPropertyChanged(nameof(ColorGreen)); break;
            }

            ApplyFiltersInternal();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            _isResetting = true;

            SearchQuery = string.Empty;
            SelectedCategory = "All";

            _categoryAll = true;
            _categoryTops = false;
            _categoryBottoms = false;
            _categoryFootwear = false;
            _categoryAccessories = false;

            MinPrice = 0;
            MaxPrice = 500;
            InStockOnly = false;
            OnSaleOnly = false;

            _sizeXS = _sizeS = _sizeM = _sizeL = _sizeXL = _sizeXXL = false;
            _colorBlack = _colorWhite = _colorGray = _colorNavy = _colorRed = _colorGreen = false;
            _genderMen = _genderWomen = _genderUnisex = false;

            SortBy = "Featured";

            OnPropertyChanged(nameof(CategoryAll));
            OnPropertyChanged(nameof(CategoryTops));
            OnPropertyChanged(nameof(CategoryBottoms));
            OnPropertyChanged(nameof(CategoryFootwear));
            OnPropertyChanged(nameof(CategoryAccessories));
            OnPropertyChanged(nameof(SizeXS));
            OnPropertyChanged(nameof(SizeS));
            OnPropertyChanged(nameof(SizeM));
            OnPropertyChanged(nameof(SizeL));
            OnPropertyChanged(nameof(SizeXL));
            OnPropertyChanged(nameof(SizeXXL));
            OnPropertyChanged(nameof(ColorBlack));
            OnPropertyChanged(nameof(ColorWhite));
            OnPropertyChanged(nameof(ColorGray));
            OnPropertyChanged(nameof(ColorNavy));
            OnPropertyChanged(nameof(ColorRed));
            OnPropertyChanged(nameof(ColorGreen));
            OnPropertyChanged(nameof(GenderMen));
            OnPropertyChanged(nameof(GenderWomen));
            OnPropertyChanged(nameof(GenderUnisex));

            _isResetting = false;
            ApplyFiltersInternal();
        }

        #endregion

        #region Filter Logic

        private void UpdateCategoryFlags()
        {
            _categoryAll = SelectedCategory == "All";
            _categoryTops = SelectedCategory == "Tops";
            _categoryBottoms = SelectedCategory == "Bottoms";
            _categoryFootwear = SelectedCategory == "Footwear";
            _categoryAccessories = SelectedCategory == "Accessories";

            OnPropertyChanged(nameof(CategoryAll));
            OnPropertyChanged(nameof(CategoryTops));
            OnPropertyChanged(nameof(CategoryBottoms));
            OnPropertyChanged(nameof(CategoryFootwear));
            OnPropertyChanged(nameof(CategoryAccessories));
        }

        private void ApplyFiltersInternal()
        {
            if (!_isInitialized || _allProducts == null) return;

            var filtered = _allProducts.AsEnumerable();

            // 1. búsqueda por texto
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                filtered = filtered.Where(p =>
                    p.Name.ToLower().Contains(query) ||
                    p.Description.ToLower().Contains(query) ||
                    (p.Category?.Name?.ToLower().Contains(query) ?? false));
            }

            // 2. categoría
            filtered = SelectedCategory switch
            {
                // Categorías principales de Supabase: 1=Tops, 2=Bottoms, 3=Footwear, 4=Accessories, 5=Outerwear
                // Los productos pueden tener category_id directo (raíz) o de subcategoría (10, 12, 20...).
                // Por convención de la BD: subcategorías de Tops empiezan con 1 (10, 11, 12, 13, 14, 15, 16, 17),
                // Bottoms con 2 (20-29), Footwear con 3 (30-37), Accessories con 4 (40-49), Outerwear con 5 (50-52).
                "Tops" => filtered.Where(p => p.CategoryId == 1 || (p.CategoryId >= 10 && p.CategoryId < 20)),
                "Bottoms" => filtered.Where(p => p.CategoryId == 2 || (p.CategoryId >= 20 && p.CategoryId < 30)),
                "Footwear" => filtered.Where(p => p.CategoryId == 3 || (p.CategoryId >= 30 && p.CategoryId < 40)),
                "Accessories" => filtered.Where(p => p.CategoryId == 4 || (p.CategoryId >= 40 && p.CategoryId < 50)),
                _ => filtered
            };

            // 3. precio
            filtered = filtered.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice);

            // 4. disponibilidad
            if (InStockOnly) filtered = filtered.Where(p => p.Stock > 0);
            if (OnSaleOnly) filtered = filtered.Where(p => p.HasDiscount);

            // 5. tallas seleccionadas
            var selectedSizes = new List<string>();
            if (_sizeXS) selectedSizes.Add("XS");
            if (_sizeS) selectedSizes.Add("S");
            if (_sizeM) selectedSizes.Add("M");
            if (_sizeL) selectedSizes.Add("L");
            if (_sizeXL) selectedSizes.Add("XL");
            if (_sizeXXL) selectedSizes.Add("XXL");

            if (selectedSizes.Any())
            {
                filtered = filtered.Where(p =>
                    p.AvailableSizes != null &&
                    p.AvailableSizes.Any(s => selectedSizes.Contains(s)));
            }

            // 6. colores seleccionados
            var selectedColors = new List<string>();
            if (_colorBlack) selectedColors.Add("Black");
            if (_colorWhite) selectedColors.Add("White");
            if (_colorGray) selectedColors.Add("Gray");
            if (_colorNavy) selectedColors.Add("Navy");
            if (_colorRed) selectedColors.Add("Red");
            if (_colorGreen) selectedColors.Add("Green");

            if (selectedColors.Any())
            {
                filtered = filtered.Where(p =>
                    p.AvailableColors != null &&
                    p.AvailableColors.Any(c => selectedColors.Contains(c)));
            }

            // 7. género
            var selectedGenders = new List<string>();
            if (_genderMen) selectedGenders.Add("Men");
            if (_genderWomen) selectedGenders.Add("Women");
            if (_genderUnisex) selectedGenders.Add("Unisex");

            if (selectedGenders.Any())
            {
                filtered = filtered.Where(p => selectedGenders.Contains(p.Gender));
            }

            // 8. ordenación
            filtered = SortBy switch
            {
                "Newest" => filtered.OrderByDescending(p => p.CreatedAt),
                "Price: Low to High" => filtered.OrderBy(p => p.Price),
                "Price: High to Low" => filtered.OrderByDescending(p => p.Price),
                "Best Selling" => filtered.OrderByDescending(p => p.ReviewCount),
                _ => filtered.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt)
            };

            var resultList = filtered.ToList();
            FilteredProducts = new ObservableCollection<Product>(resultList);
            TotalProductCount = resultList.Count;
        }

        #endregion

        #region Helper Methods

        private string GetCategoryNameFromSlug(string slug) => slug.ToLower() switch
        {
            "new" => "New Arrivals",
            "blackweek" => "Black Week",
            "softs" => "Softs Collection",
            "classic" => "Classic Collection",
            "hoodies" => "Hoodies",
            "tshirts" => "T-Shirts",
            "joggers" => "Joggers",
            "shorts" => "Shorts",
            "trackjackets" => "Track Jackets",
            "trackpants" => "Track Pants",
            "polos" => "Polos",
            "tanks" => "Tank Tops",
            "venture" => "Venture",
            "vitoria" => "Vitoria",
            "vslides" => "V-Slides",
            "caps" => "Caps",
            "bags" => "Bags",
            "beanies" => "Beanies",
            "cardholder" => "Cardholders",
            "belts" => "Belts",
            "rings" => "Rings",
            "rugs" => "Rugs",
            "bestsellers" or "best-sellers" => "Best Sellers",
            "featured" => "Featured",
            "all" => "All Products",
            _ => slug.Replace("-", " ").ToUpper()
        };

        partial void OnSortByChanged(string value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnInStockOnlyChanged(bool value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnOnSaleOnlyChanged(bool value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnMinPriceChanged(decimal value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnMaxPriceChanged(decimal value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnGenderMenChanged(bool value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnGenderWomenChanged(bool value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        partial void OnGenderUnisexChanged(bool value)
        {
            if (!_isResetting && _isInitialized) ApplyFiltersInternal();
        }

        #endregion
    }
}