using HoloCrew.ViewModels.Base;

namespace HoloCrew.Services.Interfaces
{
    /// <summary>
    /// Servicio de navegación entre vistas
    /// Permite navegar sin code-behind siguiendo MVVM puro
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Navega a un ViewModel específico
        /// </summary>
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;

        /// <summary>
        /// Navega a un ViewModel con parámetro
        /// </summary>
        void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase;

        /// <summary>
        /// Navega hacia atrás en el historial
        /// </summary>
        void GoBack();

        /// <summary>
        /// Indica si se puede navegar hacia atrás
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Inicializa el servicio de navegación con el setter de la vista actual
        /// </summary>
        void Initialize(Action<object> setCurrentView);
    }
}