using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el proceso de checkout (pago)
    /// Maneja un wizard de 3 pasos: Dirección, Pago, Confirmación
    /// </summary>
    public partial class CheckoutViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private int _currentStep = 1; // 1: Dirección, 2: Pago, 3: Confirmación

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
        private string _cardNumber;

        [ObservableProperty]
        private string _cardholderName;

        [ObservableProperty]
        private string _expirationDate;

        [ObservableProperty]
        private string _cvv;

        [ObservableProperty]
        private Order _orderSummary;

        [ObservableProperty]
        private bool _isProcessing;

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
            INavigationService navigationService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Checkout";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadCheckoutDataAsync();
        }

        private async Task LoadCheckoutDataAsync()
        {
            try
            {
                IsBusy = true;

                // Cargar items del carrito
                var cartItems = await _cartService.GetCartItemsAsync();
                OrderItems = new ObservableCollection<CartItem>(cartItems);
                OrderTotal = await _cartService.GetCartTotalAsync();

                // Cargar direcciones guardadas del usuario
                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    SavedAddresses = new ObservableCollection<Address>(currentUser.Addresses ?? new List<Address>());
                    SavedPaymentMethods = new ObservableCollection<PaymentMethod>(currentUser.PaymentMethods ?? new List<PaymentMethod>());
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void NextStep()
        {
            if (CurrentStep < 3)
            {
                if (ValidateCurrentStep())
                {
                    CurrentStep++;
                    OnPropertyChanged(nameof(IsStep1));
                    OnPropertyChanged(nameof(IsStep2));
                    OnPropertyChanged(nameof(IsStep3));

                    if (CurrentStep == 3)
                    {
                        PrepareOrderSummary();
                    }
                }
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

        [RelayCommand]
        private async Task CompleteOrderAsync()
        {
            if (IsProcessing) return;

            try
            {
                IsProcessing = true;
                CanCompleteOrder = false;

                var order = new Order
                {
                    UserId = _authenticationService.GetCurrentUser()?.Id ?? 0,
                    OrderNumber = GenerateOrderNumber(),
                    Items = OrderItems.Select(ci => new OrderItem
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.UnitPrice,
                        SelectedVariant = ci.SelectedVariant
                    }).ToList(),
                    ShippingAddress = ShippingAddress,
                    PaymentMethod = SelectedPaymentMethod,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.Now,
                    Total = OrderTotal
                };

                var createdOrder = await _orderService.CreateOrderAsync(order);

                if (createdOrder != null)
                {
                    // Limpiar carrito
                    await _cartService.ClearCartAsync();

                    // Navegar a confirmación del pedido
                    _navigationService.NavigateTo<OrderDetailViewModel>(createdOrder.Id);
                }
            }
            catch (Exception ex)
            {
                // TODO: Mostrar error
                CanCompleteOrder = true;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void CancelCheckout()
        {
            _navigationService.NavigateTo<CartViewModel>();
        }

        private bool ValidateCurrentStep()
        {
            switch (CurrentStep)
            {
                case 1: // Validar dirección
                    return !string.IsNullOrWhiteSpace(ShippingAddress?.AddressLine1) &&
                           !string.IsNullOrWhiteSpace(ShippingAddress?.City) &&
                           !string.IsNullOrWhiteSpace(ShippingAddress?.PostalCode);

                case 2: // Validar método de pago
                    return SelectedPaymentMethod != null ||
                           (!string.IsNullOrWhiteSpace(CardNumber) &&
                            !string.IsNullOrWhiteSpace(CardholderName));

                case 3: // Paso de confirmación, siempre válido
                    return true;

                default:
                    return false;
            }
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

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyyyMMdd}-{DateTime.Now.Ticks % 100000:D5}";
        }
    }
}