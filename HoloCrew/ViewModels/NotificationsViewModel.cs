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
            foreach (var notification in _allNotifications)
            {
                notification.IsRead = true;
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
            if (notification != null)
            {
                notification.IsRead = true;
                UpdateCounts();
            }
        }

        [RelayCommand]
        private void DeleteNotification(Notification notification)
        {
            if (notification != null)
            {
                _allNotifications.Remove(notification);
                Notifications.Remove(notification);
                UpdateCounts();
            }
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

                // TODO: Mostrar mensaje de éxito
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