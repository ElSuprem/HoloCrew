using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Collections.ObjectModel;

namespace HoloCrew.ViewModels
{
    /// <summary>
    /// ViewModel para el centro de notificaciones
    /// </summary>
    public partial class NotificationsViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications = new();

        [ObservableProperty]
        private ObservableCollection<Notification> _filteredNotifications = new();

        [ObservableProperty]
        private int _unreadCount;

        [ObservableProperty]
        private NotificationType? _selectedFilter;

        [ObservableProperty]
        private bool _showOnlyUnread;

        public List<NotificationType> AvailableFilters { get; } = Enum.GetValues(typeof(NotificationType))
            .Cast<NotificationType>()
            .ToList();

        public NotificationsViewModel(
            INotificationService notificationService,
            IAuthenticationService authenticationService,
            INavigationService navigationService)
        {
            _notificationService = notificationService;
            _authenticationService = authenticationService;
            _navigationService = navigationService;

            Title = "Notificaciones";

            // Suscribirse a nuevas notificaciones
            _notificationService.NotificationReceived += OnNotificationReceived;
        }

        public override async void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            await LoadNotificationsAsync();
        }

        [RelayCommand]
        private async Task LoadNotificationsAsync()
        {
            try
            {
                IsBusy = true;

                var currentUser = _authenticationService.GetCurrentUser();
                if (currentUser != null)
                {
                    var notifications = await _notificationService.GetNotificationsAsync(currentUser.Id);
                    Notifications = new ObservableCollection<Notification>(
                        notifications.OrderByDescending(n => n.CreatedAt)
                    );

                    UnreadCount = _notificationService.GetUnreadCount();
                    ApplyFilters();
                }
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task MarkAsReadAsync(Notification notification)
        {
            if (notification == null || notification.IsRead) return;

            try
            {
                await _notificationService.MarkAsReadAsync(notification.Id);
                notification.IsRead = true;
                UnreadCount = _notificationService.GetUnreadCount();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private async Task DeleteNotificationAsync(Notification notification)
        {
            if (notification == null) return;

            try
            {
                await _notificationService.DeleteNotificationAsync(notification.Id);
                Notifications.Remove(notification);
                FilteredNotifications.Remove(notification);

                if (!notification.IsRead)
                {
                    UnreadCount = _notificationService.GetUnreadCount();
                }
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
        }

        [RelayCommand]
        private async Task MarkAllAsReadAsync()
        {
            try
            {
                IsBusy = true;

                foreach (var notification in Notifications.Where(n => !n.IsRead))
                {
                    await _notificationService.MarkAsReadAsync(notification.Id);
                    notification.IsRead = true;
                }

                UnreadCount = 0;
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ClearAllAsync()
        {
            try
            {
                IsBusy = true;

                // Eliminar todas las notificaciones leídas
                var readNotifications = Notifications.Where(n => n.IsRead).ToList();

                foreach (var notification in readNotifications)
                {
                    await _notificationService.DeleteNotificationAsync(notification.Id);
                    Notifications.Remove(notification);
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                // TODO: Manejar error
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void FilterByType()
        {
            ApplyFilters();
        }

        [RelayCommand]
        private void ToggleShowOnlyUnread()
        {
            ApplyFilters();
        }

        [RelayCommand]
        private void ClearFilters()
        {
            SelectedFilter = null;
            ShowOnlyUnread = false;
            ApplyFilters();
        }

        [RelayCommand]
        private void NotificationClicked(Notification notification)
        {
            if (notification == null) return;

            // Marcar como leída
            if (!notification.IsRead)
            {
                MarkAsReadAsync(notification);
            }

            // Navegar según la acción de la notificación
            if (!string.IsNullOrEmpty(notification.ActionUrl))
            {
                // TODO: Parsear ActionUrl y navegar a la vista correspondiente
                // Por ejemplo: "order/123" -> navegar a OrderDetailViewModel con ID 123
            }
        }

        private void ApplyFilters()
        {
            var filtered = Notifications.AsEnumerable();

            // Filtrar por tipo
            if (SelectedFilter.HasValue)
            {
                filtered = filtered.Where(n => n.Type == SelectedFilter.Value);
            }

            // Filtrar solo no leídas
            if (ShowOnlyUnread)
            {
                filtered = filtered.Where(n => !n.IsRead);
            }

            FilteredNotifications = new ObservableCollection<Notification>(filtered);
        }

        private void OnNotificationReceived(object sender, Notification notification)
        {
            // Agregar nueva notificación al principio de la lista
            App.Current.Dispatcher.Invoke(() =>
            {
                Notifications.Insert(0, notification);
                UnreadCount++;
                ApplyFilters();
            });
        }

        public override void OnNavigatedFrom()
        {
            base.OnNavigatedFrom();

            // Desuscribirse de eventos
            _notificationService.NotificationReceived -= OnNotificationReceived;
        }
    }
}