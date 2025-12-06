using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el historial de pedidos del usuario
    /// </summary>
    public partial class OrderHistoryViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Order> _orders = new();

        [ObservableProperty]
        private ObservableCollection<Order> _filteredOrders = new();

        [ObservableProperty]
        private OrderStatus? _selectedStatusFilter;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _hasOrders;

        public List<OrderStatus> AvailableStatuses { get; } = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

        public OrderHistoryViewModel(
            IOrderService orderService,
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _orderService = orderService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Mis Pedidos";
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;

                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    var orders = await _orderService.GetUserOrdersAsync(currentUser.Id);
                    Orders = new ObservableCollection<Order>(orders.OrderByDescending(o => o.OrderDate));
                    FilteredOrders = new ObservableCollection<Order>(Orders);
                    HasOrders = Orders.Any();
                }
            }
            catch (Exception ex)
            {
                // TODO: Mostrar error
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void ViewOrderDetail(Order order)
        {
            if (order == null) return;
            _navigationService.NavigateTo<OrderDetailViewModel>(order.Id);
        }

        [RelayCommand]
        private async Task ReorderAsync(Order order)
        {
            if (order == null) return;

            try
            {
                // TODO: Implementar reorden (agregar items al carrito)
                // foreach (var item in order.Items)
                // {
                //     await _cartService.AddToCartAsync(item.Product, item.Quantity);
                // }

                _navigationService.NavigateTo<CartViewModel>();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private void ApplyFilters()
        {
            if (SelectedStatusFilter.HasValue)
            {
                var filtered = Orders.Where(o => o.Status == SelectedStatusFilter.Value).ToList();
                FilteredOrders = new ObservableCollection<Order>(filtered);
            }
            else
            {
                FilteredOrders = new ObservableCollection<Order>(Orders);
            }
        }

        [RelayCommand]
        private void ClearFilter()
        {
            SelectedStatusFilter = null;
            FilteredOrders = new ObservableCollection<Order>(Orders);
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadOrdersAsync();
        }

        [RelayCommand]
        private void BrowseProducts()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }
    }
}