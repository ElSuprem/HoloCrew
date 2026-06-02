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

// ViewModel de la página de venta flash (ofertas rápidas con cuenta regresiva).
// Mientras no llega la fecha de inicio (OpenDate) se muestra el panel "Próximamente"
// con una cuenta atrás; al llegar la fecha, IsAvailable pasa a true y se ve el contenido.
// Se conecta con ProductService, CartService, WishlistService y NavigationService.

namespace HoloCrew.ViewModels
{
    public partial class FlashSaleViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly INavigationService _navigationService;
        private DispatcherTimer? _countdownTimer;

        // lista completa sin filtrar
        private ObservableCollection<Product> _allFlashSaleProducts = new();

        #region Properties

        [ObservableProperty]
        private ObservableCollection<Product> _flashSaleProducts = new();

        [ObservableProperty]
        private string _saleTitle = "FLASH SALE";

        [ObservableProperty]
        private string _saleSubtitle = "Limited time offers - Don't miss out!";

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

        // Fecha en la que EMPIEZA la Flash Sale (la cuenta atrás va hasta aquí).
        [ObservableProperty]
        private DateTime _openDate;

        // True cuando ya ha llegado la fecha de inicio. Si es false, se muestra
        // el panel "Próximamente"; si es true, el contenido normal.
        [ObservableProperty]
        private bool _isAvailable;

        [ObservableProperty]
        private int _totalProductCount;

        [ObservableProperty]
        private decimal _maxDiscountPercent = 70;

        // filtros
        [ObservableProperty]
        private string _selectedCategory = "All";

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

        public ObservableCollection<string> SortOptions { get; } = new()
        {
            "Biggest Discount",
            "Price: Low to High",
            "Price: High to Low",
            "Newest",
            "Best Selling"
        };

        #endregion

        public FlashSaleViewModel(
            IProductService productService,
            ICartService cartService,
            IWishlistService wishlistService,
            INavigationService navigationService)
        {
            _productService = productService;
            _cartService = cartService;
            _wishlistService = wishlistService;
            _navigationService = navigationService;

            Title = "Flash Sale";
            EmptyTitle = "No Flash Sales Active";
            EmptySubtitle = "Check back soon for amazing deals!";
            EmptyActionText = "Browse All Products";

            // Fecha de inicio de la Flash Sale (oferta relámpago de verano).
            OpenDate = new DateTime(2026, 7, 11, 10, 0, 0);
            // Dura 24 horas desde que abre.
            SaleEndTime = OpenDate.AddHours(24);
            // Estado inicial síncrono para que no parpadee al entrar.
            IsAvailable = DateTime.Now >= OpenDate;
        }

        public override async void OnNavigatedTo(object? parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadFlashSaleProductsAsync();
            StartCountdownTimer();
        }

        public override void OnNavigatedFrom()
        {
            base.OnNavigatedFrom();
            StopCountdownTimer();
        }

        #region Data Loading

        [RelayCommand]
        private async Task LoadFlashSaleProductsAsync()
        {
            await ExecuteAsync(async () =>
            {
                LoadingMessage = "Loading flash deals...";

                var allProducts = await _productService.GetFeaturedProductsAsync();

                var flashProducts = new ObservableCollection<Product>();
                var random = new Random();

                foreach (var product in allProducts)
                {
                    var discountPercent = random.Next(30, 71);  // descuento entre 30% y 70%
                    product.OriginalPrice = product.Price;
                    product.Price = Math.Round(product.OriginalPrice.Value * (100 - discountPercent) / 100, 2);
                    product.IsBlackWeek = true;
                    flashProducts.Add(product);
                }

                if (flashProducts.Count == 0)
                {
                    flashProducts = CreateMockFlashSaleProducts();
                }

                _allFlashSaleProducts = flashProducts;
                ApplyFiltersAndSort();

                if (TotalProductCount == 0)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        // datos de ejemplo para probar sin base de datos real
        private ObservableCollection<Product> CreateMockFlashSaleProducts()
        {
            return new ObservableCollection<Product>
            {
                new Product { Id = 101, Name = "Premium Hoodie - Limited Edition", Price = 39.99m, OriginalPrice = 89.99m, Stock = 5, IsFeatured = true, MainImageUrl = "/Resources/Images/hoodie1.jpg", CategoryId = 1, Category = new Category { Name = "Tops" } },
                new Product { Id = 102, Name = "Cargo Pants - Street Style", Price = 29.99m, OriginalPrice = 79.99m, Stock = 8, IsFeatured = true, MainImageUrl = "/Resources/Images/cargo1.jpg", CategoryId = 2, Category = new Category { Name = "Bottoms" } },
                new Product { Id = 103, Name = "Classic Logo Tee", Price = 14.99m, OriginalPrice = 34.99m, Stock = 15, IsFeatured = true, MainImageUrl = "/Resources/Images/tee1.jpg", CategoryId = 1, Category = new Category { Name = "Tops" } },
                new Product { Id = 104, Name = "Armbo Low Sneakers", Price = 59.99m, OriginalPrice = 129.99m, Stock = 3, IsFeatured = true, MainImageUrl = "/Resources/Images/sneaker1.jpg", CategoryId = 3, Category = new Category { Name = "Footwear" } },
                new Product { Id = 105, Name = "Track Jacket - Retro", Price = 44.99m, OriginalPrice = 99.99m, Stock = 7, IsNew = true, IsFeatured = true, MainImageUrl = "/Resources/Images/jacket1.jpg", CategoryId = 1, Category = new Category { Name = "Tops" } },
                new Product { Id = 106, Name = "Beanie - Winter Collection", Price = 9.99m, OriginalPrice = 24.99m, Stock = 20, IsFeatured = true, MainImageUrl = "/Resources/Images/beanie1.jpg", CategoryId = 4, Category = new Category { Name = "Accessories" } },
                new Product { Id = 107, Name = "Joggers - Comfort Fit", Price = 24.99m, OriginalPrice = 59.99m, Stock = 12, IsFeatured = true, MainImageUrl = "/Resources/Images/joggers1.jpg", CategoryId = 2, Category = new Category { Name = "Bottoms" } },
                new Product { Id = 108, Name = "Crossbody Bag", Price = 19.99m, OriginalPrice = 49.99m, Stock = 6, IsNew = true, IsFeatured = true, MainImageUrl = "/Resources/Images/bag1.jpg", CategoryId = 4, Category = new Category { Name = "Accessories" } }
            };
        }

        #endregion

        #region Filtering and Sorting

        private void ApplyFiltersAndSort()
        {
            var filtered = _allFlashSaleProducts.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
            {
                filtered = filtered.Where(p =>
                    p.Category?.Name?.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase) == true ||
                    GetCategoryNameById(p.CategoryId).Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            filtered = SortBy switch
            {
                "Price: Low to High" => filtered.OrderBy(p => p.Price),
                "Price: High to Low" => filtered.OrderByDescending(p => p.Price),
                "Newest" => filtered.OrderByDescending(p => p.IsNew).ThenByDescending(p => p.Id),
                "Best Selling" => filtered.OrderByDescending(p => p.IsFeatured),
                "Biggest Discount" => filtered.OrderByDescending(p => p.DiscountPercentage),
                _ => filtered.OrderByDescending(p => p.DiscountPercentage)
            };

            FlashSaleProducts = new ObservableCollection<Product>(filtered);
            TotalProductCount = FlashSaleProducts.Count;
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
            var now = DateTime.Now;
            IsAvailable = now >= OpenDate;

            // Si aún no ha empezado, contamos hasta el INICIO.
            // Si ya está activa, contamos hasta que TERMINA.
            var target = IsAvailable ? SaleEndTime : OpenDate;
            var timeRemaining = target - now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                // Solo llega aquí si la oferta ya estaba activa y se acabó el tiempo.
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
                product.IsInWishlist = !product.IsInWishlist;
                System.Diagnostics.Debug.WriteLine($"Error toggling wishlist: {ex.Message}");
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