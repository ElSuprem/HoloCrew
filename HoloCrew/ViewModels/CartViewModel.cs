using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
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
        private bool _isCartEmpty = true;

        [ObservableProperty]
        private bool _canCheckout = true;

        [ObservableProperty]
        private string _couponMessage;

        [ObservableProperty]
        private bool _isCouponApplied;

        public CartViewModel(
            ICartService cartService,
            INavigationService navigationService)
        {
            _cartService = cartService;
            _navigationService = navigationService;

            Title = "Carrito de Compras";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadCartAsync();
        }

        [RelayCommand]
        private async Task LoadCartAsync()
        {
            try
            {
                IsBusy = true;

                var items = await _cartService.GetCartItemsAsync();
                CartItems = new ObservableCollection<CartItem>(items);
                IsCartEmpty = !CartItems.Any();
                CanCheckout = !IsCartEmpty;

                // ⭐ CORREGIDO: Cargar el descuento actual del servicio
                Discount = _cartService.GetCurrentDiscount();
                IsCouponApplied = Discount > 0;

                CalculateTotals();
            }
            finally
            {
                IsBusy = false;
            }
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
                System.Diagnostics.Debug.WriteLine($"❌ Error decreasing quantity: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine($"❌ Error increasing quantity: {ex.Message}");
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error removing from cart: {ex.Message}");
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
                Discount = 0;
                IsCouponApplied = false;
                CouponCode = string.Empty;
                CouponMessage = string.Empty;
                CalculateTotals();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error clearing cart: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ApplyCouponAsync()
        {
            if (string.IsNullOrWhiteSpace(CouponCode))
            {
                CouponMessage = "Por favor, introduce un código de cupón.";
                return;
            }

            try
            {
                var applied = await _cartService.ApplyCouponAsync(CouponCode);

                if (applied)
                {
                    // ⭐ CORREGIDO: Actualizar el descuento desde el servicio
                    Discount = _cartService.GetCurrentDiscount();
                    IsCouponApplied = true;
                    CouponMessage = $"¡Cupón aplicado! Descuento: €{Discount:N2}";
                    CalculateTotals();

                    System.Diagnostics.Debug.WriteLine($"✅ Coupon applied: {CouponCode}, Discount: €{Discount}");
                }
                else
                {
                    CouponMessage = "Cupón inválido o expirado.";
                    System.Diagnostics.Debug.WriteLine($"❌ Invalid coupon: {CouponCode}");
                }
            }
            catch (Exception ex)
            {
                CouponMessage = "Error al aplicar el cupón.";
                System.Diagnostics.Debug.WriteLine($"❌ Error applying coupon: {ex.Message}");
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

        private void CalculateTotals()
        {
            Subtotal = CartItems.Sum(item => item.Subtotal);
            ShippingCost = Subtotal > 50 ? 0 : 5.99m; // Envío gratis sobre 50€
            Tax = Subtotal * 0.21m; // IVA 21%
            Total = Subtotal + ShippingCost + Tax - Discount;

            // Asegurar que el total no sea negativo
            if (Total < 0) Total = 0;
        }
    }
}