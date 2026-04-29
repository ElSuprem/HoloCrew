using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// ViewModel de la página de notificaciones.
// Carga las notificaciones reales del usuario logueado desde Supabase.
// Permite filtrar (todas, no leídas, pedidos, promociones), marcar como leídas,
// borrar, y configurar las preferencias de notificaciones.

namespace HoloCrew.ViewModels
{
    public partial class NotificationsViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthenticationService _authService;
        private readonly INavigationService _navigationService;
        private List<Notification> _allNotifications = new();

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications = new();

        [ObservableProperty]
        private bool _hasUnreadNotifications;

        [ObservableProperty]
        private bool _hasNotifications;

        [ObservableProperty]
        private int _unreadCount;

        [ObservableProperty]
        private string _currentFilter = "All";

        [ObservableProperty]
        private bool _emailNotifications = true;

        [ObservableProperty]
        private bool _pushNotifications = true;

        [ObservableProperty]
        private bool _orderUpdates = true;

        [ObservableProperty]
        private bool _priceAlerts = false;

        [ObservableProperty]
        private bool _promotions = true;

        [ObservableProperty]
        private bool _isLoading = false;

        public NotificationsViewModel(
            INotificationService notificationService,
            IAuthenticationService authService,
            INavigationService navigationService)
        {
            _notificationService = notificationService;
            _authService = authService;
            _navigationService = navigationService;

            Title = "Notifications";
        }


        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadNotificationsAsync();
        }


        // Carga las notificaciones del usuario actual desde Supabase
        private async Task LoadNotificationsAsync()
        {
            try
            {
                IsLoading = true;

                var user = _authService.GetCurrentUser();
                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    _allNotifications = new List<Notification>();
                    Notifications = new ObservableCollection<Notification>();
                    UpdateCounts();
                    return;
                }

                var items = await _notificationService.GetNotificationsAsync(user.Id);
                _allNotifications = items;

                ApplyFilter();
                UpdateCounts();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notifications] Load error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        [RelayCommand]
        private void ShowAll()
        {
            CurrentFilter = "All";
            ApplyFilter();
        }


        [RelayCommand]
        private void ShowUnread()
        {
            CurrentFilter = "Unread";
            ApplyFilter();
        }


        [RelayCommand]
        private void ShowOrders()
        {
            CurrentFilter = "Orders";
            ApplyFilter();
        }


        [RelayCommand]
        private void ShowPromotions()
        {
            CurrentFilter = "Promotions";
            ApplyFilter();
        }


        [RelayCommand]
        private async Task MarkAllAsReadAsync()
        {
            var user = _authService.GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(user.Id)) return;

            await _notificationService.MarkAllAsReadAsync(user.Id);

            // Actualizar caché local para que la UI se refresque al instante
            foreach (var n in _allNotifications)
                n.IsRead = true;

            ApplyFilter();
            UpdateCounts();
        }


        [RelayCommand]
        private async Task ClearAllAsync()
        {
            // Borramos todas en BD una a una
            foreach (var n in _allNotifications.ToList())
            {
                await _notificationService.DeleteNotificationAsync(n.Id);
            }

            _allNotifications.Clear();
            Notifications.Clear();
            UpdateCounts();
        }


        [RelayCommand]
        private async Task MarkAsReadAsync(Notification notification)
        {
            if (notification == null || notification.IsRead) return;

            await _notificationService.MarkAsReadAsync(notification.Id);

            // Actualizar caché local
            var item = _allNotifications.FirstOrDefault(n => n.Id == notification.Id);
            if (item != null)
                item.IsRead = true;

            // Refrescar también la lista visible
            var viewItem = Notifications.FirstOrDefault(n => n.Id == notification.Id);
            if (viewItem != null)
                viewItem.IsRead = true;

            UpdateCounts();
        }


        [RelayCommand]
        private async Task DeleteNotificationAsync(Notification notification)
        {
            if (notification == null) return;

            await _notificationService.DeleteNotificationAsync(notification.Id);

            _allNotifications.RemoveAll(n => n.Id == notification.Id);
            var viewItem = Notifications.FirstOrDefault(n => n.Id == notification.Id);
            if (viewItem != null)
                Notifications.Remove(viewItem);

            UpdateCounts();
        }


        [RelayCommand]
        private async Task OpenNotificationAsync(Notification notification)
        {
            if (notification == null) return;

            // Al abrir, la marcamos como leída automáticamente
            await MarkAsReadAsync(notification);

            // Aquí en el futuro se podría navegar al pedido/producto referenciado
            // según notification.Type (action_type/action_reference en BD).
            System.Diagnostics.Debug.WriteLine($"Opened notification: {notification.Title}");
        }


        [RelayCommand]
        private async Task SaveNotificationSettingsAsync()
        {
            try
            {
                IsLoading = true;
                await Task.Delay(300);

                // Aquí se guardarían las preferencias en BD (en notification_preferences si existe).
                // De momento solo loguea.
                System.Diagnostics.Debug.WriteLine("Notification settings saved");
                System.Diagnostics.Debug.WriteLine($"Email: {EmailNotifications}, Push: {PushNotifications}");
                System.Diagnostics.Debug.WriteLine($"Orders: {OrderUpdates}, Price: {PriceAlerts}, Promos: {Promotions}");
            }
            finally
            {
                IsLoading = false;
            }
        }


        // ==================== HELPERS ====================

        // Aplica el filtro actual a la lista de notificaciones
        private void ApplyFilter()
        {
            IEnumerable<Notification> filtered = CurrentFilter switch
            {
                "Unread" => _allNotifications.Where(n => !n.IsRead),
                "Orders" => _allNotifications.Where(n =>
                    n.Type != null && n.Type.Contains("order", StringComparison.OrdinalIgnoreCase)),
                "Promotions" => _allNotifications.Where(n =>
                    n.Type != null &&
                    (n.Type.Contains("promotion", StringComparison.OrdinalIgnoreCase) ||
                     n.Type.Contains("sale", StringComparison.OrdinalIgnoreCase))),
                _ => _allNotifications
            };

            Notifications = new ObservableCollection<Notification>(filtered);
        }


        private void UpdateCounts()
        {
            UnreadCount = _allNotifications.Count(n => !n.IsRead);
            HasUnreadNotifications = UnreadCount > 0;
            HasNotifications = _allNotifications.Any();
        }
    }
}