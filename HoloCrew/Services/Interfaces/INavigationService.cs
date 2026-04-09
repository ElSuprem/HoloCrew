using HoloCrew.ViewModels.Base;

// Servicio para navegar entre pantallas sin usar code-behind (MVVM puro).
// En lugar de navegar directamente a vistas, se navega a ViewModels.

namespace HoloCrew.Services.Interfaces
{
    public interface INavigationService
    {
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;        // ir a una pantalla
        void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase; // ir con un dato extra
        void GoBack();                                                          // volver a la pantalla anterior
        bool CanGoBack { get; }                                                 // si se puede volver atrás
        void Initialize(Action<object> setCurrentView);                         // configurar el servicio (se llama al arrancar)
    }
}