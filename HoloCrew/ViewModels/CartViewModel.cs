using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class CartViewModel : ViewModelBase
    {
        private readonly ICartService _cartService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<CartItem> _cartItems = new();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _shippingCost;

        [ObservableProperty]
        private decimal _tax;

        [ObservableProperty]
        private decimal _discount;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private string _couponCode;

        [ObservableProperty]
        private string _couponMessage;

        [ObservableProperty]
        private bool? _isCouponValid;

        [ObservableProperty]
        private bool _isCartEmpty = true;

        [ObservableProperty]
        private bool _canCheckout = true;

        public CartViewModel(
            ICartService cartService,
            INavigationService navigationService)
        {
            _cartService = cartService;
            _navigationService = navigationService;

            Title = AppConstants.UI.Cart;
            EmptyTitle = AppConstants.Empty.CartTitle;
            EmptySubtitle = AppConstants.Empty.CartSubtitle;
            EmptyActionText = AppConstants.Empty.CartAction;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadCartAsync();
        }

        [RelayCommand]
        private async Task LoadCartAsync()
        {
            await ExecuteAsync(async () =>
            {
                var items = await _cartService.GetCartItemsAsync();
                CartItems = new ObservableCollection<CartItem>(items);
                IsCartEmpty = !CartItems.Any();
                CanCheckout = !IsCartEmpty;
                CalculateTotals();

                if (IsCartEmpty)
                    SetEmpty();
                else
                    SetSuccess();
            });
        }

        [RelayCommand]
        private async Task DecreaseQuantityAsync(CartItem item)
        {
            if (item == null || item.Quantity <= 1) return;

            try
            {
                item.Quantity--;
                await _cartService.UpdateQuantityAsync(item.Id, item.Quantity);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task IncreaseQuantityAsync(CartItem item)
        {
            if (item == null) return;

            try
            {
                item.Quantity++;
                await _cartService.UpdateQuantityAsync(item.Id, item.Quantity);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task RemoveFromCartAsync(CartItem item)
        {
            if (item == null) return;

            try
            {
                await _cartService.RemoveFromCartAsync(item.Id);
                CartItems.Remove(item);
                IsCartEmpty = !CartItems.Any();
                CanCheckout = !IsCartEmpty;
                CalculateTotals();

                if (IsCartEmpty) SetEmpty();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ClearCartAsync()
        {
            try
            {
                await _cartService.ClearCartAsync();
                CartItems.Clear();
                IsCartEmpty = true;
                CanCheckout = false;
                CalculateTotals();
                SetEmpty();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ApplyCouponAsync()
        {
            if (string.IsNullOrWhiteSpace(CouponCode))
            {
                CouponMessage = "Please enter a coupon code";
                IsCouponValid = false;
                return;
            }

            try
            {
                // Cupones válidos de ejemplo
                var validCoupons = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
                {
                    { "WELCOME10", 10 },
                    { "SAVE20", 20 },
                    { "HOLOCREW15", 15 }
                };

                if (validCoupons.TryGetValue(CouponCode.Trim(), out decimal discountPercent))
                {
                    Discount = Subtotal * (discountPercent / 100);
                    CouponMessage = $"✓ Coupon applied! {discountPercent}% off";
                    IsCouponValid = true;
                    CalculateTotals();
                }
                else
                {
                    Discount = 0;
                    CouponMessage = "✗ Invalid coupon code";
                    IsCouponValid = false;
                    CalculateTotals();
                }
            }
            catch (Exception ex)
            {
                CouponMessage = "Error applying coupon";
                IsCouponValid = false;
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        [RelayCommand]
        private void CheckoutCommand()
        {
            if (IsCartEmpty) return;
            _navigationService.NavigateTo<CheckoutViewModel>();
        }

        [RelayCommand]
        private void BrowseProducts()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void EmptyAction()
        {
            BrowseProducts();
        }

        private void CalculateTotals()
        {
            Subtotal = CartItems.Sum(item => item.Subtotal);
            ShippingCost = Subtotal > AppConstants.FreeShippingThreshold ? 0 : AppConstants.DefaultShippingCost;
            Tax = Subtotal * AppConstants.TaxRate;
            Total = Subtotal + ShippingCost + Tax - Discount;
        }
    }
}