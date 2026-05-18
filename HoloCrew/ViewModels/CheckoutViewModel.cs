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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ViewModel de la página de checkout (finalizar compra).
// Tiene 3 pasos: dirección de envío, método de pago, resumen del pedido.
// El paso 2 (pago) tiene validación visual real del formulario pero el procesamiento
// es simulado a través de IPaymentService. La arquitectura está preparada para
// integrar Stripe sin tocar el ViewModel.
// Al confirmar:
//   1. Procesa el pago (IPaymentService, actualmente SimulatedPaymentService).
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

        // ========== CAMPOS DEL FORMULARIO DE PAGO ==========
        // Estos campos tienen validación visual pero NO se envían a ninguna pasarela.
        // El SimulatedPaymentService los ignora; solo aprueba el importe.

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPaymentFormValid))]
        private string _cardholderName = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPaymentFormValid))]
        [NotifyPropertyChangedFor(nameof(IsCardNumberValid))]
        private string _cardNumber = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPaymentFormValid))]
        [NotifyPropertyChangedFor(nameof(IsExpirationDateValid))]
        private string _expirationDate = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsPaymentFormValid))]
        [NotifyPropertyChangedFor(nameof(IsCvvValid))]
        private string _cvv = string.Empty;

        // ========== RESTO DE ESTADO ==========

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

        // ========== VALIDACIONES DEL FORMULARIO DE PAGO ==========

        // Número de tarjeta válido: 16 dígitos (ignorando espacios).
        public bool IsCardNumberValid
        {
            get
            {
                var digits = (CardNumber ?? string.Empty).Replace(" ", string.Empty);
                return digits.Length == 16 && digits.All(char.IsDigit);
            }
        }

        // Fecha de caducidad válida: formato MM/YY, mes entre 01-12, no caducada.
        public bool IsExpirationDateValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ExpirationDate)) return false;
                if (!Regex.IsMatch(ExpirationDate, @"^(0[1-9]|1[0-2])/\d{2}$")) return false;

                var parts = ExpirationDate.Split('/');
                var month = int.Parse(parts[0]);
                var year = 2000 + int.Parse(parts[1]);

                // Último día del mes de caducidad
                var lastDayOfMonth = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                return lastDayOfMonth >= DateTime.Today;
            }
        }

        // CVV válido: 3 o 4 dígitos.
        public bool IsCvvValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Cvv)
                    && (Cvv.Length == 3 || Cvv.Length == 4)
                    && Cvv.All(char.IsDigit);
            }
        }

        // El formulario es válido si los 4 campos lo son.
        public bool IsPaymentFormValid =>
            !string.IsNullOrWhiteSpace(CardholderName)
            && CardholderName.Trim().Length >= 2
            && IsCardNumberValid
            && IsExpirationDateValid
            && IsCvvValid;

        // ========== HOOKS DE NOTIFICACIÓN PARA REVALIDAR CanCompleteOrder ==========

        partial void OnCardNumberChanged(string value)
        {
            // Auto-formato: añadir espacios cada 4 dígitos. Quitamos primero los espacios
            // que pudiera haber para no entrar en bucle infinito.
            var digits = (value ?? string.Empty).Replace(" ", string.Empty);
            if (digits.Length > 16) digits = digits.Substring(0, 16);
            if (!digits.All(char.IsDigit) && digits.Length > 0) return;

            var formatted = Regex.Replace(digits, ".{4}", "$0 ").TrimEnd();
            if (formatted != value)
            {
                CardNumber = formatted;
            }
        }

        partial void OnExpirationDateChanged(string value)
        {
            // Auto-formato: insertar la "/" automáticamente al teclear el tercer dígito.
            var digits = (value ?? string.Empty).Replace("/", string.Empty);
            if (digits.Length > 4) digits = digits.Substring(0, 4);
            if (!digits.All(char.IsDigit) && digits.Length > 0) return;

            string formatted = digits.Length >= 3
                ? $"{digits.Substring(0, 2)}/{digits.Substring(2)}"
                : digits;

            if (formatted != value)
            {
                ExpirationDate = formatted;
            }
        }

        partial void OnCvvChanged(string value)
        {
            // Limitar a 4 dígitos numéricos.
            var digits = (value ?? string.Empty).Where(char.IsDigit).ToArray();
            if (digits.Length > 4) digits = digits.Take(4).ToArray();
            var formatted = new string(digits);

            if (formatted != value)
            {
                Cvv = formatted;
            }
        }

        // ========== CONSTRUCTOR ==========

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
                2 => IsPaymentFormValid,
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

        // Helper para mostrar el número enmascarado en el paso 3 (xxxx xxxx xxxx 4242).
        public string CardNumberMasked
        {
            get
            {
                var digits = (CardNumber ?? string.Empty).Replace(" ", string.Empty);
                if (digits.Length < 4) return "•••• •••• •••• ••••";
                var last4 = digits.Substring(digits.Length - 4);
                return $"•••• •••• •••• {last4}";
            }
        }
    }
}