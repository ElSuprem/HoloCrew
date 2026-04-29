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

// ViewModel del historial de pedidos del usuario.
// Muestra todos los pedidos del usuario actual desde Supabase, permite filtrar por estado
// y rango de fechas, y ver el detalle de cada pedido. También calcula stats globales
// (total pedidos, total gastado, valor medio).

namespace HoloCrew.ViewModels
{
    public partial class OrderHistoryViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        // Lista completa cargada desde BD (no se filtra)
        private List<Order> _allOrders = new();

        // Lista que se muestra en la UI (filtrada)
        [ObservableProperty]
        private ObservableCollection<Order> _orders = new();

        [ObservableProperty]
        private bool _hasOrders;

        // ====== FILTROS ======

        // Opciones del desplegable de estado: "All", "Pending", "Confirmed", etc.
        public ObservableCollection<string> FilterOptions { get; } = new()
        {
            "All",
            "Pending",
            "Confirmed",
            "Processing",
            "Shipped",
            "Delivered",
            "Cancelled",
            "Refunded"
        };

        [ObservableProperty]
        private string _selectedFilter = "All";

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        // ====== STATS ======

        [ObservableProperty]
        private int _totalOrders;

        [ObservableProperty]
        private decimal _totalSpent;

        [ObservableProperty]
        private decimal _averageOrderValue;

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
                if (currentUser == null)
                {
                    SetEmpty();
                    return;
                }

                var orders = await _orderService.GetUserOrdersAsync(currentUser.Id);
                _allOrders = orders.OrderByDescending(o => o.OrderDate).ToList();

                ApplyFiltersInternal();
                CalculateStats();

                if (HasOrders)
                    SetSuccess();
                else
                    SetEmpty();
            });
        }


        [RelayCommand]
        private void ViewOrderDetail(Order order)
        {
            if (order == null) return;
            _navigationService.NavigateTo<OrderDetailViewModel>(order.Id);
        }


        [RelayCommand]
        private void ApplyFilters()
        {
            ApplyFiltersInternal();
        }


        [RelayCommand]
        private void ClearFilter()
        {
            SelectedFilter = "All";
            StartDate = null;
            EndDate = null;
            ApplyFiltersInternal();
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


        // ==================== HELPERS ====================

        // Aplica los filtros activos sobre _allOrders y actualiza Orders.
        private void ApplyFiltersInternal()
        {
            IEnumerable<Order> filtered = _allOrders;

            // Filtro por estado
            if (!string.IsNullOrEmpty(SelectedFilter) && SelectedFilter != "All")
            {
                if (Enum.TryParse<OrderStatus>(SelectedFilter, true, out var status))
                {
                    filtered = filtered.Where(o => o.Status == status);
                }
            }

            // Filtro por rango de fechas
            if (StartDate.HasValue)
                filtered = filtered.Where(o => o.OrderDate.Date >= StartDate.Value.Date);

            if (EndDate.HasValue)
                filtered = filtered.Where(o => o.OrderDate.Date <= EndDate.Value.Date);

            Orders = new ObservableCollection<Order>(filtered);
            HasOrders = Orders.Any();
        }


        // Calcula stats sobre TODOS los pedidos (no filtrados)
        private void CalculateStats()
        {
            TotalOrders = _allOrders.Count;
            TotalSpent = _allOrders.Sum(o => o.Total);
            AverageOrderValue = TotalOrders > 0
                ? TotalSpent / TotalOrders
                : 0m;
        }
    }
}