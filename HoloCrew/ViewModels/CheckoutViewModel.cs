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

// ViewModel de la página de checkout (finalizar compra).
// Tiene 3 pasos: dirección de envío, método de pago, resumen del pedido.
// Al confirmar:
//   1. Procesa el pago (IPaymentService, actualmente simulado).
//   2. Si el pago aprueba, llama a OrderService.CreateOrderFromCartAsync que
//      invoca la RPC create_order_from_cart de Supabase (transaccional).
//   3. Navega al detalle del pedido recién creado.

namespace HoloCrew.ViewModels
{
    public partial class CheckoutViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;
        private readonly IPaymentService _paymentService;

        [ObservableProperty]
        private int _currentStep = 1;

        [ObservableProperty]
        private Address _shippingAddress = new();

        [ObservableProperty]
        private ObservableCollection<Address> _savedAddresses = new();

        [ObservableProperty]
        private Address _selectedSavedAddress;

        [ObservableProperty]
        private PaymentMethod _selectedPaymentMethod;

        [ObservableProperty]
        private ObservableCollection<PaymentMethod> _savedPaymentMethods = new();

        [ObservableProperty]
        private Order _orderSummary;

        [ObservableProperty]
        private bool _isProcessing;

        [ObservableProperty]
        private string _processingMessage = string.Empty;

        [ObservableProperty]
        private decimal _orderTotal;

        [ObservableProperty]
        private ObservableCollection<CartItem> _orderItems = new();

        [ObservableProperty]
        private bool _canCompleteOrder = true;

        public bool IsStep1 => CurrentStep == 1;
        public bool IsStep2 => CurrentStep == 2;
        public bool IsStep3 => CurrentStep == 3;

        public CheckoutViewModel(
            IOrderService orderService,
            ICartService cartService,
            IAuthenticationService authenticationService,
            INavigationService navigationService,
            IPaymentService paymentService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;
            _paymentService = paymentService;

            Title = AppConstants.Checkout.Title;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadCheckoutDataAsync();
        }

        private async Task LoadCheckoutDataAsync()
        {
            await ExecuteAsync(async () =>
            {
                var cartItems = await _cartService.GetCartItemsAsync();
                OrderItems = new ObservableCollection<CartItem>(cartItems);
                OrderTotal = await _cartService.GetCartTotalAsync();

                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    SavedAddresses = new ObservableCollection<Address>(currentUser.Addresses ?? new List<Address>());
                    SavedPaymentMethods = new ObservableCollection<PaymentMethod>(currentUser.PaymentMethods ?? new List<PaymentMethod>());
                }

                SetSuccess();
            });
        }

        [RelayCommand]
        private void NextStep()
        {
            if (CurrentStep < 3 && ValidateCurrentStep())
            {
                CurrentStep++;
                OnPropertyChanged(nameof(IsStep1));
                OnPropertyChanged(nameof(IsStep2));
                OnPropertyChanged(nameof(IsStep3));

                if (CurrentStep == 3) PrepareOrderSummary();
            }
        }

        [RelayCommand]
        private void PreviousStep()
        {
            if (CurrentStep > 1)
            {
                CurrentStep--;
                OnPropertyChanged(nameof(IsStep1));
                OnPropertyChanged(nameof(IsStep2));
                OnPropertyChanged(nameof(IsStep3));
            }
        }

        [RelayCommand]
        private void SelectSavedAddress(Address address)
        {
            if (address == null) return;

            ShippingAddress = new Address
            {
                FullName = address.FullName,
                PhoneNumber = address.PhoneNumber,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country
            };
            SelectedSavedAddress = address;
        }

        [RelayCommand]
        private void AddNewAddress()
        {
            ShippingAddress = new Address();
            SelectedSavedAddress = null;
        }

        [RelayCommand]
        private void SelectPaymentMethod(PaymentMethod paymentMethod)
        {
            SelectedPaymentMethod = paymentMethod;
        }


        // Flujo completo de finalización de compra:
        //   1. Procesa el pago a través de IPaymentService (actualmente simulado).
        //   2. Si el pago se aprueba, crea el pedido en BD vía la RPC transaccional.
        //   3. Navega al detalle del pedido recién creado.
        [RelayCommand]
        private async Task CompleteOrderAsync()
        {
            if (IsProcessing) return;

            try
            {
                IsProcessing = true;
                CanCompleteOrder = false;
                ErrorMessage = string.Empty;

                // ----- PASO 1: PROCESAR PAGO -----
                ProcessingMessage = "Processing payment...";
                var paymentResult = await _paymentService.ProcessPaymentAsync(OrderTotal);

                if (!paymentResult.Success)
                {
                    SetError(paymentResult.Message);
                    CanCompleteOrder = true;
                    return;
                }

                // ----- PASO 2: CREAR PEDIDO EN BD -----
                ProcessingMessage = "Creating your order...";

                var paymentMethodString = SelectedPaymentMethod?.Type switch
                {
                    PaymentType.CreditCard => "credit_card",
                    PaymentType.DebitCard => "debit_card",
                    PaymentType.PayPal => "paypal",
                    _ => "credit_card"
                };

                var couponCode = _cartService.GetAppliedCouponCode();
                var couponToSend = string.IsNullOrEmpty(couponCode) ? null : couponCode;

                var createdOrder = await _orderService.CreateOrderFromCartAsync(
                    ShippingAddress,
                    paymentMethodString,
                    couponToSend);

                if (createdOrder != null)
                {
                    // ----- PASO 3: NAVEGAR AL DETALLE -----
                    ProcessingMessage = "Done!";
                    await Task.Delay(500); // pequeño respiro visual
                    _navigationService.NavigateTo<OrderDetailViewModel>(createdOrder.Id);
                }
                else
                {
                    SetError(AppConstants.Errors.PaymentFailed);
                    CanCompleteOrder = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Checkout] Error: {ex.Message}");
                SetError(AppConstants.Errors.PaymentFailed);
                CanCompleteOrder = true;
            }
            finally
            {
                IsProcessing = false;
                ProcessingMessage = string.Empty;
            }
        }

        [RelayCommand]
        private void CancelCheckout()
        {
            _navigationService.NavigateTo<CartViewModel>();
        }

        private bool ValidateCurrentStep()
        {
            return CurrentStep switch
            {
                1 => !string.IsNullOrWhiteSpace(ShippingAddress?.AddressLine1) &&
                     !string.IsNullOrWhiteSpace(ShippingAddress?.City) &&
                     !string.IsNullOrWhiteSpace(ShippingAddress?.PostalCode),
                2 => true,  // En el paso 2 ya no hay validación de tarjeta porque el pago se simula
                3 => true,
                _ => false
            };
        }

        private void PrepareOrderSummary()
        {
            OrderSummary = new Order
            {
                Items = OrderItems.Select(ci => new OrderItem
                {
                    Product = ci.Product,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList(),
                ShippingAddress = ShippingAddress,
                PaymentMethod = SelectedPaymentMethod,
                Total = OrderTotal
            };
        }
    }
}