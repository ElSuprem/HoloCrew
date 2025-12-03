using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para ver los detalles de un pedido específico
    /// </summary>
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

            Title = "Detalle del Pedido";
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
            try
            {
                IsBusy = true;

                Order = await _orderService.GetOrderByIdAsync(orderId);

                if (Order != null)
                {
                    OrderItems = new ObservableCollection<OrderItem>(Order.Items ?? new List<OrderItem>());

                    // Verificar si se puede cancelar (solo si está Pending o Confirmed)
                    CanBeCancelled = Order.Status == OrderStatus.Pending ||
                                     Order.Status == OrderStatus.Confirmed;

                    UpdateStatusDisplay();

                    // Cargar información de tracking si existe
                    if (!string.IsNullOrEmpty(Order.TrackingNumber))
                    {
                        await LoadTrackingInfoAsync(Order.TrackingNumber);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadTrackingInfoAsync(string trackingNumber)
        {
            try
            {
                TrackingInfo = await _orderService.GetTrackingInfoAsync(trackingNumber);
            }
            catch (Exception ex)
            {
                // No mostrar error si no se puede cargar tracking
            }
        }

        [RelayCommand]
        private async Task CancelOrderAsync()
        {
            if (Order == null || !CanBeCancelled) return;

            try
            {
                IsBusy = true;

                var cancelled = await _orderService.CancelOrderAsync(Order.Id);

                if (cancelled)
                {
                    Order.Status = OrderStatus.Cancelled;
                    CanBeCancelled = false;
                    UpdateStatusDisplay();
                    // TODO: Mostrar mensaje de éxito
                }
            }
            catch (Exception ex)
            {
                // TODO: Mostrar error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void TrackShipment()
        {
            if (TrackingInfo != null)
            {
                // TODO: Mostrar ventana/diálogo con información detallada de tracking
            }
        }

        [RelayCommand]
        private void DownloadInvoice()
        {
            // TODO: Implementar descarga de factura
        }

        [RelayCommand]
        private void ContactSupport()
        {
            // TODO: Abrir chat de soporte o formulario de contacto
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
                OrderStatus.Pending => "Tu pedido está siendo procesado",
                OrderStatus.Confirmed => "Tu pedido ha sido confirmado",
                OrderStatus.Processing => "Estamos preparando tu pedido",
                OrderStatus.Shipped => "Tu pedido está en camino",
                OrderStatus.Delivered => "Tu pedido ha sido entregado",
                OrderStatus.Cancelled => "Tu pedido ha sido cancelado",
                OrderStatus.Refunded => "Tu pedido ha sido reembolsado",
                _ => "Estado desconocido"
            };

            StatusColor = Order.Status switch
            {
                OrderStatus.Pending => "#FFA500",      // Naranja
                OrderStatus.Confirmed => "#1976D2",    // Azul
                OrderStatus.Processing => "#1976D2",   // Azul
                OrderStatus.Shipped => "#2196F3",      // Azul claro
                OrderStatus.Delivered => "#4CAF50",    // Verde
                OrderStatus.Cancelled => "#F44336",    // Rojo
                OrderStatus.Refunded => "#FF9800",     // Naranja oscuro
                _ => "#757575"                          // Gris
            };
        }
    }
}