using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel del catálogo de productos
    /// </summary>
    public partial class ProductCatalogViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly INavigationService _navigationService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        #region Properties

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
        private string _sortBy = AppConstants.UI.Featured;

        [ObservableProperty]
        private int _totalProductCount;

        [ObservableProperty]
        private decimal _minPrice;

        [ObservableProperty]
        private decimal _maxPrice = 500;

        [ObservableProperty]
        private bool _inStockOnly;

        [ObservableProperty]
        private bool _onSaleOnly;

        public ObservableCollection<string> SortOptions { get; } = new()
        {
            AppConstants.UI.Featured,
            AppConstants.UI.Newest,
            AppConstants.UI.PriceLowToHigh,
            AppConstants.UI.PriceHighToLow,
            AppConstants.UI.BestSelling
        };

        #endregion

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

            Title = AppConstants.UI.Shop;
            EmptyTitle = AppConstants.Empty.ProductsTitle;
            EmptySubtitle = AppConstants.Empty.ProductsSubtitle;
            EmptyActionText = AppConstants.Empty.ProductsAction;
        }

        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is string slug)
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
                await LoadFeaturedProductsAsync();
            }
        }

        #region Data Loading

        private async Task LoadFeaturedProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = AppConstants.UI.Loading;
                var products = await _productService.GetFeaturedProductsAsync();
                SetProducts(products);
            });
        }

        private async Task LoadProductsBySlugAsync(string slug)
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = $"Loading {GetCategoryNameFromSlug(slug)}...";

                // Mapear slug a método apropiado
                var products = slug.ToLower() switch
                {
                    "new" => await _productService.GetNewProductsAsync(),
                    "bestsellers" or "best-sellers" => await _productService.GetBestSellersAsync(),
                    "featured" => await _productService.GetFeaturedProductsAsync(),
                    _ => await _productService.SearchProductsAsync(slug)
                };

                SetProducts(products);
            });
        }

        private async Task LoadProductsByCategoryAsync(int categoryId)
        {
            await ExecuteAsync(async () =>
            {
                var products = await _productService.GetProductsByCategoryAsync(categoryId);
                SetProducts(products);
            });
        }

        private void SetProducts(List<Product> products)
        {
            Products = new ObservableCollection<Product>(products);
            FilteredProducts = new ObservableCollection<Product>(products);
            TotalProductCount = products.Count;

            if (products.Count == 0)
                SetEmpty();
            else
                SetSuccess();
        }

        #endregion

        #region Commands

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
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Search() => ApplyFilters();

        [RelayCommand]
        private void ClearFilters()
        {
            SearchQuery = string.Empty;
            MinPrice = 0;
            MaxPrice = 500;
            InStockOnly = false;
            OnSaleOnly = false;
            SortBy = AppConstants.UI.Featured;
            FilteredProducts = new ObservableCollection<Product>(Products);
            if (FilteredProducts.Count == 0) SetEmpty(); else SetSuccess();
        }

        [RelayCommand]
        private void EmptyAction() => ClearFilters();

        #endregion

        #region Filtering

        private void ApplyFilters()
        {
            var filtered = Products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
                filtered = filtered.Where(p =>
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            filtered = filtered.Where(p => p.Price >= MinPrice && p.Price <= MaxPrice);
            if (InStockOnly) filtered = filtered.Where(p => p.Stock > 0);
            if (OnSaleOnly) filtered = filtered.Where(p => p.HasDiscount);

            filtered = SortBy switch
            {
                var s when s == AppConstants.UI.Newest => filtered.OrderByDescending(p => p.CreatedAt),
                var s when s == AppConstants.UI.PriceLowToHigh => filtered.OrderBy(p => p.Price),
                var s when s == AppConstants.UI.PriceHighToLow => filtered.OrderByDescending(p => p.Price),
                _ => filtered.OrderByDescending(p => p.IsFeatured)
            };

            FilteredProducts = new ObservableCollection<Product>(filtered);
            TotalProductCount = FilteredProducts.Count;
            if (FilteredProducts.Count == 0)
                SetEmpty(AppConstants.Empty.SearchTitle, AppConstants.Empty.SearchSubtitle);
            else
                SetSuccess();
        }

        private string GetCategoryNameFromSlug(string slug) => slug.ToLower() switch
        {
            "new" => "New Arrivals",
            "blackweek" => "Black Week",
            "softs" => "Softs Collection",
            "hoodies" => "Hoodies",
            "tshirts" => "T-Shirts",
            "joggers" => "Joggers",
            "bestsellers" or "best-sellers" => "Best Sellers",
            "featured" => "Featured",
            _ => slug.Replace("-", " ")
        };

        partial void OnSortByChanged(string value) { if (State == LoadingState.Success) ApplyFilters(); }
        partial void OnInStockOnlyChanged(bool value) { if (State == LoadingState.Success) ApplyFilters(); }
        partial void OnOnSaleOnlyChanged(bool value) { if (State == LoadingState.Success) ApplyFilters(); }

        #endregion
    }
}