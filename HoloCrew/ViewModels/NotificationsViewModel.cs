using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using System.Threading.Tasks;

namespace HoloCrew.ViewModels
{
    public partial class NotificationsViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        private readonly INavigationService _navigationService;

        // ⭐ PROPIEDADES AGREGADAS
        [ObservableProperty]
        private bool _hasUnreadNotifications = true;

        [ObservableProperty]
        private bool _hasNotifications = true;

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

            Title = "Notificaciones";
        }

        // ⭐ COMANDOS AGREGADOS
        [RelayCommand]
        private void ShowAll()
        {
            // Mostrar todas las notificaciones
        }

        [RelayCommand]
        private void ShowUnread()
        {
            // Mostrar solo no leídas
        }

        [RelayCommand]
        private void ShowOrders()
        {
            // Filtrar por órdenes
        }

        [RelayCommand]
        private void ShowPromotions()
        {
            // Filtrar por promociones
        }

        [RelayCommand]
        private async Task SaveNotificationSettingsAsync()
        {
            try
            {
                IsLoading = true;
                await Task.Delay(1000);
                // Guardar configuración
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
