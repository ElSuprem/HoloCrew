using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    public partial class NotificationsViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;
        private ObservableCollection<Notification> _allNotifications;

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications;

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
            INavigationService navigationService)
        {
            _notificationService = notificationService;
            _navigationService = navigationService;

            Title = "Notifications";

            LoadMockNotifications();
            UpdateCounts();
        }

        private void LoadMockNotifications()
        {
            _allNotifications = new ObservableCollection<Notification>
            {
                new Notification
                {
                    Id = 1,
                    Title = "Order Shipped",
                    Message = "Your order #12345 has been shipped and is on its way!",
                    Type = "Order",
                    Icon = "📦",
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    IsRead = false
                },
                new Notification
                {
                    Id = 2,
                    Title = "Flash Sale Alert",
                    Message = "50% OFF on all hoodies! Limited time offer.",
                    Type = "Promotion",
                    Icon = "🔥",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    IsRead = false
                },
                new Notification
                {
                    Id = 3,
                    Title = "Order Delivered",
                    Message = "Your order #12340 has been delivered successfully.",
                    Type = "Order",
                    Icon = "✅",
                    CreatedAt = DateTime.Now.AddHours(-5),
                    IsRead = true
                },
                new Notification
                {
                    Id = 4,
                    Title = "Price Drop Alert",
                    Message = "CARGO PANTS now €59.99 (was €79.99)",
                    Type = "Alert",
                    Icon = "💰",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    IsRead = false
                },
                new Notification
                {
                    Id = 5,
                    Title = "New Arrivals",
                    Message = "Check out our new winter collection!",
                    Type = "Promotion",
                    Icon = "✨",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    IsRead = true
                },
                new Notification
                {
                    Id = 6,
                    Title = "Welcome to HoloCrew",
                    Message = "Thanks for joining! Here's 10% off your first order: WELCOME10",
                    Type = "Info",
                    Icon = "👋",
                    CreatedAt = DateTime.Now.AddDays(-7),
                    IsRead = true
                },
                new Notification
                {
                    Id = 7,
                    Title = "Order Confirmed",
                    Message = "We've received your order #12345. Preparing for shipment.",
                    Type = "Order",
                    Icon = "🛒",
                    CreatedAt = DateTime.Now.AddDays(-8),
                    IsRead = true
                }
            };

            Notifications = new ObservableCollection<Notification>(_allNotifications);
        }

        [RelayCommand]
        private void ShowAll()
        {
            CurrentFilter = "All";
            Notifications = new ObservableCollection<Notification>(_allNotifications);
            UpdateCounts();
        }

        [RelayCommand]
        private void ShowUnread()
        {
            CurrentFilter = "Unread";
            Notifications = new ObservableCollection<Notification>(
                _allNotifications.Where(n => !n.IsRead));
            UpdateCounts();
        }

        [RelayCommand]
        private void ShowOrders()
        {
            CurrentFilter = "Orders";
            Notifications = new ObservableCollection<Notification>(
                _allNotifications.Where(n => n.Type == "Order"));
            UpdateCounts();
        }

        [RelayCommand]
        private void ShowPromotions()
        {
            CurrentFilter = "Promotions";
            Notifications = new ObservableCollection<Notification>(
                _allNotifications.Where(n => n.Type == "Promotion"));
            UpdateCounts();
        }

        [RelayCommand]
        private void MarkAllAsRead()
        {
            // ⭐ CORREGIDO: Crear nuevas instancias para forzar actualización de UI
            var updatedNotifications = new ObservableCollection<Notification>();

            foreach (var notification in _allNotifications)
            {
                // Crear nueva instancia con IsRead = true
                updatedNotifications.Add(new Notification
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    Icon = notification.Icon,
                    CreatedAt = notification.CreatedAt,
                    IsRead = true // ⭐ Marcar como leído
                });
            }

            // Reemplazar la lista completa
            _allNotifications = updatedNotifications;

            // Actualizar la vista según el filtro actual
            switch (CurrentFilter)
            {
                case "Unread":
                    Notifications = new ObservableCollection<Notification>(
                        _allNotifications.Where(n => !n.IsRead));
                    break;
                case "Orders":
                    Notifications = new ObservableCollection<Notification>(
                        _allNotifications.Where(n => n.Type == "Order"));
                    break;
                case "Promotions":
                    Notifications = new ObservableCollection<Notification>(
                        _allNotifications.Where(n => n.Type == "Promotion"));
                    break;
                default:
                    Notifications = new ObservableCollection<Notification>(_allNotifications);
                    break;
            }

            UpdateCounts();
        }

        [RelayCommand]
        private void ClearAll()
        {
            _allNotifications.Clear();
            Notifications.Clear();
            UpdateCounts();
        }

        [RelayCommand]
        private void MarkAsRead(Notification notification)
        {
            if (notification == null) return;

            // Encontrar y actualizar en la lista principal
            var index = _allNotifications.ToList().FindIndex(n => n.Id == notification.Id);
            if (index >= 0)
            {
                var updatedNotification = new Notification
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    Type = notification.Type,
                    Icon = notification.Icon,
                    CreatedAt = notification.CreatedAt,
                    IsRead = true
                };

                _allNotifications[index] = updatedNotification;

                // Actualizar en la vista actual
                var viewIndex = Notifications.ToList().FindIndex(n => n.Id == notification.Id);
                if (viewIndex >= 0)
                {
                    Notifications[viewIndex] = updatedNotification;
                }
            }

            UpdateCounts();
        }

        [RelayCommand]
        private void DeleteNotification(Notification notification)
        {
            if (notification == null) return;

            // Remover de ambas listas
            var toRemoveFromAll = _allNotifications.FirstOrDefault(n => n.Id == notification.Id);
            if (toRemoveFromAll != null)
            {
                _allNotifications.Remove(toRemoveFromAll);
            }

            var toRemoveFromView = Notifications.FirstOrDefault(n => n.Id == notification.Id);
            if (toRemoveFromView != null)
            {
                Notifications.Remove(toRemoveFromView);
            }

            UpdateCounts();
        }

        [RelayCommand]
        private void OpenNotification(Notification notification)
        {
            if (notification == null) return;

            // Marcar como leído
            MarkAsRead(notification);

            // Aquí podrías navegar según el tipo de notificación
            // Por ejemplo, si es una orden, navegar al detalle de la orden
            System.Diagnostics.Debug.WriteLine($"Opened notification: {notification.Title}");
        }

        [RelayCommand]
        private async Task SaveNotificationSettingsAsync()
        {
            try
            {
                IsLoading = true;
                await Task.Delay(500);

                // Aquí guardarías las preferencias
                // await _notificationService.SaveSettingsAsync(...)

                System.Diagnostics.Debug.WriteLine("Notification settings saved!");
                System.Diagnostics.Debug.WriteLine($"Email: {EmailNotifications}, Push: {PushNotifications}");
                System.Diagnostics.Debug.WriteLine($"Orders: {OrderUpdates}, Price: {PriceAlerts}, Promos: {Promotions}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateCounts()
        {
            UnreadCount = _allNotifications.Count(n => !n.IsRead);
            HasUnreadNotifications = UnreadCount > 0;
            HasNotifications = _allNotifications.Any();
        }
    }
}