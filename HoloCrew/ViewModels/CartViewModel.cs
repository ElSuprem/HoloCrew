using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

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

                CalculateTotals();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task UpdateQuantityAsync(CartItem item)
        {
            if (item == null) return;

            try
            {
                await _cartService.UpdateQuantityAsync(item.Id, item.Quantity);
                CalculateTotals();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private async Task RemoveItemAsync(CartItem item)
        {
            if (item == null) return;

            try
            {
                await _cartService.RemoveFromCartAsync(item.Id);
                CartItems.Remove(item);
                IsCartEmpty = !CartItems.Any();
                CalculateTotals();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
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
                CalculateTotals();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private async Task ApplyCouponAsync()
        {
            if (string.IsNullOrWhiteSpace(CouponCode)) return;

            try
            {
                var applied = await _cartService.ApplyCouponAsync(CouponCode);
                if (applied)
                {
                    // TODO: Mostrar éxito
                    CalculateTotals();
                }
                else
                {
                    // TODO: Mostrar error "Cupón inválido"
                }
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private void ProceedToCheckout()
        {
            if (IsCartEmpty) return;
            _navigationService.NavigateTo<CheckoutViewModel>();
        }

        [RelayCommand]
        private void ContinueShopping()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        private void CalculateTotals()
        {
            Subtotal = CartItems.Sum(item => item.Subtotal);
            ShippingCost = Subtotal > 50 ? 0 : 5.99m; // Envío gratis sobre 50€
            Tax = Subtotal * 0.21m; // IVA 21%
            Total = Subtotal + ShippingCost + Tax - Discount;
        }
    }
}