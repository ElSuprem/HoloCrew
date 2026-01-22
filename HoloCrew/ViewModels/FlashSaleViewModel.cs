using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HoloCrew.ViewModels
{
    public partial class FlashSaleViewModel : ViewModelBase
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly INavigationService _navigationService;
        private DispatcherTimer? _countdownTimer;

        #region Properties

        [ObservableProperty]
        private ObservableCollection<Product> _flashSaleProducts = new();

        [ObservableProperty]
        private string _saleTitle = "FLASH SALE";

        [ObservableProperty]
        private string _saleSubtitle = "Limited time offers - Don't miss out!";

        // Countdown properties
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
        private decimal _maxDiscountPercent = 70;

        // Filtros
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

            // Establecer fin de la venta (ejemplo: 24 horas desde ahora)
            SaleEndTime = DateTime.Now.Date.AddDays(1).AddHours(23).AddMinutes(59).AddSeconds(59);
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

                // Cargar productos con descuento
                var allProducts = await _productService.GetFeaturedProductsAsync();

                // Filtrar solo productos con descuento y crear datos mock de flash sale
                var flashProducts = new ObservableCollection<Product>();
                var random = new Random();

                foreach (var product in allProducts)
                {
                    // Simular descuentos de flash sale (30-70%)
                    var discountPercent = random.Next(30, 71);

                    product.OriginalPrice = product.Price;
                    product.Price = Math.Round(product.OriginalPrice.Value * (100 - discountPercent) / 100, 2);
                    product.IsBlackWeek = true; // Reusar para indicar flash sale

                    flashProducts.Add(product);
                }

                // Si no hay productos, crear algunos mock
                if (flashProducts.Count == 0)
                {
                    flashProducts = CreateMockFlashSaleProducts();
                }

                FlashSaleProducts = flashProducts;
                TotalProductCount = FlashSaleProducts.Count;

                if (TotalProductCount == 0)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        private ObservableCollection<Product> CreateMockFlashSaleProducts()
        {
            return new ObservableCollection<Product>
            {
                new Product
                {
                    Id = 101,
                    Name = "Premium Hoodie - Limited Edition",
                    Price = 39.99m,
                    OriginalPrice = 89.99m,
                    Stock = 5,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/hoodie1.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 102,
                    Name = "Cargo Pants - Street Style",
                    Price = 29.99m,
                    OriginalPrice = 79.99m,
                    Stock = 8,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/cargo1.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 103,
                    Name = "Classic Logo Tee",
                    Price = 14.99m,
                    OriginalPrice = 34.99m,
                    Stock = 15,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/tee1.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 104,
                    Name = "Armbo Low Sneakers",
                    Price = 59.99m,
                    OriginalPrice = 129.99m,
                    Stock = 3,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/sneaker1.jpg",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 105,
                    Name = "Track Jacket - Retro",
                    Price = 44.99m,
                    OriginalPrice = 99.99m,
                    Stock = 7,
                    IsNew = true,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/jacket1.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 106,
                    Name = "Beanie - Winter Collection",
                    Price = 9.99m,
                    OriginalPrice = 24.99m,
                    Stock = 20,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/beanie1.jpg",
                    CategoryId = 4
                },
                new Product
                {
                    Id = 107,
                    Name = "Joggers - Comfort Fit",
                    Price = 24.99m,
                    OriginalPrice = 59.99m,
                    Stock = 12,
                    IsNew = false,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/joggers1.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 108,
                    Name = "Crossbody Bag",
                    Price = 19.99m,
                    OriginalPrice = 49.99m,
                    Stock = 6,
                    IsNew = true,
                    IsFeatured = true,
                    MainImageUrl = "/Resources/Images/bag1.jpg",
                    CategoryId = 4
                }
            };
        }

        #endregion

        #region Countdown Timer

        private void StartCountdownTimer()
        {
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();

            // Actualizar inmediatamente
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

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            var timeRemaining = SaleEndTime - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
            {
                IsSaleActive = false;
                HoursRemaining = 0;
                MinutesRemaining = 0;
                SecondsRemaining = 0;
                CountdownText = "SALE ENDED";
                StopCountdownTimer();
                return;
            }

            HoursRemaining = (int)timeRemaining.TotalHours;
            MinutesRemaining = timeRemaining.Minutes;
            SecondsRemaining = timeRemaining.Seconds;
            CountdownText = $"{HoursRemaining:D2}:{MinutesRemaining:D2}:{SecondsRemaining:D2}";
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

        [RelayCommand]
        private void ApplyFilter()
        {
            // TODO: Implementar filtrado por categoría
            System.Diagnostics.Debug.WriteLine($"Filter by: {SelectedCategory}");
        }

        [RelayCommand]
        private void ApplySort()
        {
            // TODO: Implementar ordenación
            System.Diagnostics.Debug.WriteLine($"Sort by: {SortBy}");
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