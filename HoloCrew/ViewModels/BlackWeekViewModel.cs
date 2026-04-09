using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

// ViewModel de la página Black Week (ofertas).
// Muestra productos con descuento, cuenta regresiva, filtros por categoría y ordenación.
// Se conecta con ProductService, CartService, WishlistService y NavigationService.

namespace HoloCrew.ViewModels
{
    public partial class BlackWeekViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly INavigationService _navigationService;
        private DispatcherTimer? _countdownTimer;  // temporizador para la cuenta regresiva

        private ObservableCollection<Product> _allBlackWeekProducts = new();  // lista completa sin filtrar

        #region Properties

        [ObservableProperty]
        private ObservableCollection<Product> _blackWeekProducts = new();

        [ObservableProperty]
        private ObservableCollection<Product> _topDeals = new();  // los mejores descuentos

        [ObservableProperty]
        private string _saleTitle = "BLACK WEEK";

        [ObservableProperty]
        private string _saleSubtitle = "The biggest sale of the year is here";

        // tiempo restante
        [ObservableProperty]
        private int _daysRemaining;

        [ObservableProperty]
        private int _hoursRemaining;

        [ObservableProperty]
        private int _minutesRemaining;

        [ObservableProperty]
        private int _secondsRemaining;

        [ObservableProperty]
        private string _countdownText = string.Empty;

        [ObservableProperty]
        private DateTime _saleEndTime;

        [ObservableProperty]
        private bool _isSaleActive = true;

        [ObservableProperty]
        private int _totalProductCount;

        [ObservableProperty]
        private decimal _maxDiscountPercent = 80;

        // filtros
        [ObservableProperty]
        private string _selectedCategory = "All";

        [ObservableProperty]
        private string _selectedDiscount = "All Discounts";

        [ObservableProperty]
        private string _sortBy = "Biggest Discount";

        public ObservableCollection<string> Categories { get; } = new()
        {
            "All",
            "Tops",
            "Bottoms",
            "Footwear",
            "Accessories"
        };

        public ObservableCollection<string> DiscountFilters { get; } = new()
        {
            "All Discounts",
            "50% or more",
            "40% or more",
            "30% or more"
        };

        public ObservableCollection<string> SortOptions { get; } = new()
        {
            "Biggest Discount",
            "Price: Low to High",
            "Price: High to Low",
            "Newest",
            "Best Selling"
        };

        #endregion

        public BlackWeekViewModel(
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService,
            INavigationService navigationService)
        {
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
            _navigationService = navigationService;

            Title = "Black Week";
            EmptyTitle = "Black Week Coming Soon";
            EmptySubtitle = "Stay tuned for our biggest sale of the year!";
            EmptyActionText = "Browse All Products";

            // la oferta dura 7 días
            SaleEndTime = DateTime.Now.Date.AddDays(7).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadBlackWeekProductsAsync();
            StartCountdownTimer();
        }

        public override void OnNavigatedFrom()
        {
            base.OnNavigatedFrom();
            StopCountdownTimer();
        }

        #region Data Loading

        [RelayCommand]
        private async Task LoadBlackWeekProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = "Loading Black Week deals...";

                var blackWeekProducts = await _productService.GetBlackWeekProductsAsync();

                // si no hay, crear datos de ejemplo
                if (blackWeekProducts == null || blackWeekProducts.Count == 0)
                {
                    blackWeekProducts = CreateMockBlackWeekProducts().ToList();
                }

                // asignar descuentos aleatorios si no tienen
                var random = new Random();
                foreach (var product in blackWeekProducts)
                {
                    if (!product.HasDiscount)
                    {
                        var discountPercent = random.Next(30, 81);  // entre 30% y 80%
                        product.OriginalPrice = product.Price;
                        product.Price = Math.Round(product.OriginalPrice.Value * (100 - discountPercent) / 100, 2);
                    }
                    product.IsBlackWeek = true;
                }

                _allBlackWeekProducts = new ObservableCollection<Product>(blackWeekProducts);
                ApplyFiltersAndSort();

                // top deals: los 4 con mayor descuento
                TopDeals = new ObservableCollection<Product>(
                    _allBlackWeekProducts
                        .OrderByDescending(p => p.DiscountPercentage)
                        .Take(4)
                );

                if (TotalProductCount == 0)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        // datos de ejemplo para probar sin base de datos real
        private ObservableCollection<Product> CreateMockBlackWeekProducts()
        {
            return new ObservableCollection<Product>
            {
                new Product { Id = 201, Name = "Premium Leather Jacket", Price = 89.99m, OriginalPrice = 249.99m, Stock = 3, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/jacket_leather.jpg", CategoryId = 1, Category = new Category { Name = "Tops" }, AverageRating = 4.8, ReviewCount = 124 },
                new Product { Id = 202, Name = "Designer Sneakers - Limited", Price = 79.99m, OriginalPrice = 199.99m, Stock = 5, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/sneaker_designer.jpg", CategoryId = 3, Category = new Category { Name = "Footwear" }, AverageRating = 4.9, ReviewCount = 89 },
                new Product { Id = 203, Name = "Cashmere Hoodie", Price = 59.99m, OriginalPrice = 149.99m, Stock = 8, IsNew = true, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/hoodie_cashmere.jpg", CategoryId = 1, Category = new Category { Name = "Tops" }, AverageRating = 4.7, ReviewCount = 56 },
                new Product { Id = 204, Name = "Luxury Watch Cap", Price = 19.99m, OriginalPrice = 59.99m, Stock = 15, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/cap_luxury.jpg", CategoryId = 4, Category = new Category { Name = "Accessories" }, AverageRating = 4.5, ReviewCount = 78 },
                new Product { Id = 205, Name = "Cargo Pants - Military Edition", Price = 34.99m, OriginalPrice = 89.99m, Stock = 12, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/cargo_military.jpg", CategoryId = 2, Category = new Category { Name = "Bottoms" }, AverageRating = 4.6, ReviewCount = 203 },
                new Product { Id = 206, Name = "Oversized Graphic Tee", Price = 19.99m, OriginalPrice = 44.99m, Stock = 25, IsNew = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/tee_graphic.jpg", CategoryId = 1, Category = new Category { Name = "Tops" }, AverageRating = 4.4, ReviewCount = 167 },
                new Product { Id = 207, Name = "Tactical Backpack", Price = 44.99m, OriginalPrice = 99.99m, Stock = 7, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/backpack_tactical.jpg", CategoryId = 4, Category = new Category { Name = "Accessories" }, AverageRating = 4.8, ReviewCount = 92 },
                new Product { Id = 208, Name = "Slim Fit Chinos", Price = 29.99m, OriginalPrice = 69.99m, Stock = 18, IsBlackWeek = true, MainImageUrl = "/Resources/Images/chinos_slim.jpg", CategoryId = 2, Category = new Category { Name = "Bottoms" }, AverageRating = 4.3, ReviewCount = 145 },
                new Product { Id = 209, Name = "Puffer Jacket - Winter Edition", Price = 69.99m, OriginalPrice = 159.99m, Stock = 4, IsNew = true, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/puffer_winter.jpg", CategoryId = 1, Category = new Category { Name = "Tops" }, AverageRating = 4.9, ReviewCount = 34 },
                new Product { Id = 210, Name = "Canvas Belt", Price = 12.99m, OriginalPrice = 29.99m, Stock = 30, IsBlackWeek = true, MainImageUrl = "/Resources/Images/belt_canvas.jpg", CategoryId = 4, Category = new Category { Name = "Accessories" }, AverageRating = 4.2, ReviewCount = 256 },
                new Product { Id = 211, Name = "High-Top Boots", Price = 64.99m, OriginalPrice = 139.99m, Stock = 6, IsFeatured = true, IsBlackWeek = true, MainImageUrl = "/Resources/Images/boots_hightop.jpg", CategoryId = 3, Category = new Category { Name = "Footwear" }, AverageRating = 4.7, ReviewCount = 67 },
                new Product { Id = 212, Name = "Fleece Sweatpants", Price = 24.99m, OriginalPrice = 54.99m, Stock = 20, IsBlackWeek = true, MainImageUrl = "/Resources/Images/sweatpants_fleece.jpg", CategoryId = 2, Category = new Category { Name = "Bottoms" }, AverageRating = 4.5, ReviewCount = 189 }
            };
        }

        #endregion

        #region Filtering and Sorting

        private void ApplyFiltersAndSort()
        {
            var filtered = _allBlackWeekProducts.AsEnumerable();

            // filtrar por categoría
            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            {
                filtered = filtered.Where(p =>
                    p.Category?.Name?.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase) == true ||
                    GetCategoryNameById(p.CategoryId).Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            // ordenar
            filtered = SortBy switch
            {
                "Price: Low to High" => filtered.OrderBy(p => p.Price),
                "Price: High to Low" => filtered.OrderByDescending(p => p.Price),
                "Newest" => filtered.OrderByDescending(p => p.IsNew).ThenByDescending(p => p.Id),
                "Best Selling" => filtered.OrderByDescending(p => p.IsFeatured),
                "Biggest Discount" => filtered.OrderByDescending(p => p.DiscountPercentage),
                _ => filtered.OrderByDescending(p => p.DiscountPercentage)
            };

            BlackWeekProducts = new ObservableCollection<Product>(filtered);
            TotalProductCount = BlackWeekProducts.Count;
        }

        private string GetCategoryNameById(int categoryId)
        {
            return categoryId switch
            {
                1 => "Tops",
                2 => "Bottoms",
                3 => "Footwear",
                4 => "Accessories",
                _ => "Other"
            };
        }

        partial void OnSelectedCategoryChanged(string value) => ApplyFiltersAndSort();
        partial void OnSortByChanged(string value) => ApplyFiltersAndSort();

        #endregion

        #region Countdown Timer

        private void StartCountdownTimer()
        {
            _countdownTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
            UpdateCountdown();
        }

        private void StopCountdownTimer()
        {
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer.Tick -= CountdownTimer_Tick;
                _countdownTimer = null;
            }
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e) => UpdateCountdown();

        private void UpdateCountdown()
        {
            var timeRemaining = SaleEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                IsSaleActive = false;
                DaysRemaining = 0;
                HoursRemaining = 0;
                MinutesRemaining = 0;
                SecondsRemaining = 0;
                CountdownText = "SALE ENDED";
                StopCountdownTimer();
                return;
            }

            DaysRemaining = timeRemaining.Days;
            HoursRemaining = timeRemaining.Hours;
            MinutesRemaining = timeRemaining.Minutes;
            SecondsRemaining = timeRemaining.Seconds;
            CountdownText = $"{DaysRemaining}d {HoursRemaining:D2}:{MinutesRemaining:D2}:{SecondsRemaining:D2}";
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void SelectCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return;
            SelectedCategory = category;
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
                product.IsInWishlist = !product.IsInWishlist;  // revertir si falla
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void BrowseAllProducts()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void EmptyAction()
        {
            BrowseAllProducts();
        }

        #endregion
    }
}