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
    public partial class OrderDetailViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private Order _order;

        [ObservableProperty]
        private ObservableCollection<OrderItemDisplay> _orderItems = new();

        [ObservableProperty]
        private bool _canBeCancelled;

        [ObservableProperty]
        private string _orderNumber;

        [ObservableProperty]
        private DateTime _orderDate;

        [ObservableProperty]
        private string _statusText;

        [ObservableProperty]
        private string _statusColor;

        [ObservableProperty]
        private DateTime _estimatedDelivery;

        [ObservableProperty]
        private string _shippingAddress;

        [ObservableProperty]
        private string _paymentMethod;

        [ObservableProperty]
        private ObservableCollection<TrackingEvent> _trackingHistory = new();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _shippingCost;

        [ObservableProperty]
        private decimal _discountAmount;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private bool _hasDiscount;

        public OrderDetailViewModel(
            IOrderService orderService,
            INavigationService navigationService)
        {
            _orderService = orderService;
            _navigationService = navigationService;
            Title = "Order Details";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);

            if (parameter is int orderId)
            {
                await LoadOrderAsync(orderId);
            }
            else
            {
                LoadMockData();
            }
        }

        private async Task LoadOrderAsync(int orderId)
        {
            await ExecuteAsync(async () =>
            {
                Order = await _orderService.GetOrderByIdAsync(orderId);

                if (Order != null)
                    PopulateFromOrder();
                else
                    LoadMockData();

                SetSuccess();
            });
        }

        private void PopulateFromOrder()
        {
            OrderNumber = Order.OrderNumber ?? $"ORD-{Order.Id:D6}";
            OrderDate = Order.OrderDate;

            var displayItems = Order.Items?.Select(i => new OrderItemDisplay
            {
                ProductName = i.Product?.Name ?? "Product",
                Size = i.SelectedVariant ?? "M",
                Color = "Black",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Subtotal
            }).ToList() ?? new List<OrderItemDisplay>();

            OrderItems = new ObservableCollection<OrderItemDisplay>(displayItems);

            Subtotal = Order.Subtotal;
            ShippingCost = Order.ShippingCost;
            DiscountAmount = Order.Discount;
            HasDiscount = DiscountAmount > 0;
            Total = Order.Total;

            if (Order.ShippingAddress != null)
            {
                var addr = Order.ShippingAddress;
                ShippingAddress = $"{addr.FullName}\n{addr.AddressLine1}\n{addr.City}, {addr.PostalCode}\n{addr.Country}";
            }
            else
            {
                ShippingAddress = "No address provided";
            }

            if (Order.PaymentMethod != null)
            {
                PaymentMethod = $"{Order.PaymentMethod.Type} {Order.PaymentMethod.CardNumberMasked}";
            }
            else
            {
                PaymentMethod = "No payment method";
            }

            UpdateStatusDisplay();
            EstimatedDelivery = Order.EstimatedDeliveryDate ?? Order.OrderDate.AddDays(5);
            LoadTrackingHistoryFromOrder();
            CanBeCancelled = Order.Status == OrderStatus.Pending || Order.Status == OrderStatus.Confirmed;
        }

        private void LoadMockData()
        {
            OrderNumber = "ORD-2024-001234";
            OrderDate = DateTime.Now.AddDays(-2);

            OrderItems = new ObservableCollection<OrderItemDisplay>
            {
                new OrderItemDisplay
                {
                    ProductName = "Premium Hoodie - Black",
                    Size = "M",
                    Color = "Black",
                    Quantity = 1,
                    UnitPrice = 89.99m,
                    TotalPrice = 89.99m
                },
                new OrderItemDisplay
                {
                    ProductName = "Classic T-Shirt - White",
                    Size = "L",
                    Color = "White",
                    Quantity = 2,
                    UnitPrice = 34.99m,
                    TotalPrice = 69.98m
                }
            };

            Subtotal = OrderItems.Sum(i => i.TotalPrice);
            ShippingCost = 0;
            DiscountAmount = 15.00m;
            HasDiscount = true;
            Total = Subtotal + ShippingCost - DiscountAmount;

            ShippingAddress = "Antonio García\nCalle Principal 123, 2º B\nMadrid, 28001\nEspaña";
            PaymentMethod = "Visa •••• 4242";

            StatusText = "SHIPPED";
            StatusColor = "#2196F3";
            EstimatedDelivery = DateTime.Now.AddDays(3);

            TrackingHistory = new ObservableCollection<TrackingEvent>
            {
                new TrackingEvent { EventTitle = "Order Placed", EventDate = DateTime.Now.AddDays(-2), IsCompleted = true },
                new TrackingEvent { EventTitle = "Payment Confirmed", EventDate = DateTime.Now.AddDays(-2).AddHours(1), IsCompleted = true },
                new TrackingEvent { EventTitle = "Processing", EventDate = DateTime.Now.AddDays(-1), IsCompleted = true },
                new TrackingEvent { EventTitle = "Shipped", EventDate = DateTime.Now.AddHours(-6), IsCompleted = true },
                new TrackingEvent { EventTitle = "Out for Delivery", EventDate = null, IsCompleted = false },
                new TrackingEvent { EventTitle = "Delivered", EventDate = null, IsCompleted = false }
            };

            CanBeCancelled = false;
        }

        private void LoadTrackingHistoryFromOrder()
        {
            var events = new List<TrackingEvent>
            {
                new TrackingEvent { EventTitle = "Order Placed", EventDate = OrderDate, IsCompleted = true }
            };

            if (Order.Status >= OrderStatus.Confirmed)
                events.Add(new TrackingEvent { EventTitle = "Payment Confirmed", EventDate = OrderDate.AddMinutes(30), IsCompleted = true });

            if (Order.Status >= OrderStatus.Processing)
                events.Add(new TrackingEvent { EventTitle = "Processing", EventDate = OrderDate.AddHours(2), IsCompleted = true });

            if (Order.Status >= OrderStatus.Shipped)
                events.Add(new TrackingEvent { EventTitle = "Shipped", EventDate = OrderDate.AddDays(1), IsCompleted = true });
            else
                events.Add(new TrackingEvent { EventTitle = "Shipped", EventDate = null, IsCompleted = false });

            if (Order.Status >= OrderStatus.Delivered)
                events.Add(new TrackingEvent { EventTitle = "Delivered", EventDate = Order.DeliveredDate ?? OrderDate.AddDays(3), IsCompleted = true });
            else
                events.Add(new TrackingEvent { EventTitle = "Delivered", EventDate = null, IsCompleted = false });

            TrackingHistory = new ObservableCollection<TrackingEvent>(events);
        }

        private void UpdateStatusDisplay()
        {
            StatusText = Order.Status switch
            {
                OrderStatus.Pending => "PENDING",
                OrderStatus.Confirmed => "CONFIRMED",
                OrderStatus.Processing => "PROCESSING",
                OrderStatus.Shipped => "SHIPPED",
                OrderStatus.Delivered => "DELIVERED",
                OrderStatus.Cancelled => "CANCELLED",
                OrderStatus.Refunded => "REFUNDED",
                _ => "UNKNOWN"
            };

            StatusColor = Order.Status switch
            {
                OrderStatus.Pending => "#FFA500",
                OrderStatus.Confirmed => "#1976D2",
                OrderStatus.Processing => "#1976D2",
                OrderStatus.Shipped => "#2196F3",
                OrderStatus.Delivered => "#4CAF50",
                OrderStatus.Cancelled => "#F44336",
                OrderStatus.Refunded => "#FF9800",
                _ => "#757575"
            };
        }

        [RelayCommand]
        private void BackToHistory()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }

        [RelayCommand]
        private void TrackPackage()
        {
            System.Diagnostics.Debug.WriteLine($"Tracking: {Order?.TrackingNumber ?? "Mock"}");
        }

        [RelayCommand]
        private void ContactSupport()
        {
            System.Diagnostics.Debug.WriteLine("Contact support");
        }

        [RelayCommand]
        private async Task CancelOrderAsync()
        {
            if (Order == null || !CanBeCancelled) return;

            await ExecuteAsync(async () =>
            {
                var cancelled = await _orderService.CancelOrderAsync(Order.Id);
                if (cancelled)
                {
                    Order.Status = OrderStatus.Cancelled;
                    CanBeCancelled = false;
                    UpdateStatusDisplay();
                    LoadTrackingHistoryFromOrder();
                }
            }, isRefresh: true);
        }
    }

    public class OrderItemDisplay
    {
        public string ProductName { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class TrackingEvent
    {
        public string EventTitle { get; set; }
        public DateTime? EventDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}