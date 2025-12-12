using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
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

            Title = AppConstants.Orders.Title;
            EmptyTitle = AppConstants.Empty.OrdersTitle;
            EmptySubtitle = AppConstants.Empty.OrdersSubtitle;
            EmptyActionText = AppConstants.Empty.OrdersAction;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task LoadOrdersAsync()
        {
            await ExecuteAsync(async () =>
            {
                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    var orders = await _orderService.GetUserOrdersAsync(currentUser.Id);
                    Orders = new ObservableCollection<Order>(orders.OrderByDescending(o => o.OrderDate));
                    FilteredOrders = new ObservableCollection<Order>(Orders);
                    HasOrders = Orders.Any();

                    if (HasOrders)
                        SetSuccess();
                    else
                        SetEmpty();
                }
                else
                {
                    SetEmpty();
                }
            });
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
                // TODO: Add items to cart
                _navigationService.NavigateTo<CartViewModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
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
        private void BrowseProducts()
        {
            _navigationService.NavigateTo<ProductCatalogViewModel>();
        }

        [RelayCommand]
        private void EmptyAction()
        {
            BrowseProducts();
        }
    }
}