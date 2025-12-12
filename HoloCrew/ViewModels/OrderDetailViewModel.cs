using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
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
        private ObservableCollection<OrderItem> _orderItems = new();

        [ObservableProperty]
        private TrackingInfo _trackingInfo;

        [ObservableProperty]
        private bool _canBeCancelled;

        [ObservableProperty]
        private string _statusDescription;

        [ObservableProperty]
        private string _statusColor;

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
        }

        private async Task LoadOrderAsync(int orderId)
        {
            await ExecuteAsync(async () =>
            {
                Order = await _orderService.GetOrderByIdAsync(orderId);

                if (Order != null)
                {
                    OrderItems = new ObservableCollection<OrderItem>(Order.Items ?? new List<OrderItem>());
                    CanBeCancelled = Order.Status == OrderStatus.Pending || Order.Status == OrderStatus.Confirmed;
                    UpdateStatusDisplay();

                    if (!string.IsNullOrEmpty(Order.TrackingNumber))
                    {
                        await LoadTrackingInfoAsync(Order.TrackingNumber);
                    }

                    SetSuccess();
                }
                else
                {
                    SetError(AppConstants.Errors.OrderNotFound);
                }
            });
        }

        private async Task LoadTrackingInfoAsync(string trackingNumber)
        {
            try
            {
                TrackingInfo = await _orderService.GetTrackingInfoAsync(trackingNumber);
            }
            catch { /* Ignore tracking errors */ }
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
                    SetSuccess();
                }
            }, isRefresh: true);
        }

        [RelayCommand]
        private void TrackShipment()
        {
            // TODO: Show tracking dialog
        }

        [RelayCommand]
        private void DownloadInvoice()
        {
            // TODO: Download invoice
        }

        [RelayCommand]
        private void ContactSupport()
        {
            // TODO: Contact support
        }

        [RelayCommand]
        private void GoBack()
        {
            _navigationService.NavigateTo<OrderHistoryViewModel>();
        }

        private void UpdateStatusDisplay()
        {
            StatusDescription = Order.Status switch
            {
                OrderStatus.Pending => AppConstants.Orders.StatusPending,
                OrderStatus.Confirmed => "Order confirmed",
                OrderStatus.Processing => AppConstants.Orders.StatusProcessing,
                OrderStatus.Shipped => AppConstants.Orders.StatusShipped,
                OrderStatus.Delivered => AppConstants.Orders.StatusDelivered,
                OrderStatus.Cancelled => AppConstants.Orders.StatusCancelled,
                OrderStatus.Refunded => AppConstants.Orders.StatusRefunded,
                _ => "Unknown"
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
    }
}