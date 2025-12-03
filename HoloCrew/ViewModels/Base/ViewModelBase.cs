using CommunityToolkit.Mvvm.ComponentModel;

namespace HoloCrew.ViewModels.Base
{
    /// <summary>
    /// Clase base abstracta para todos los ViewModels
    /// </summary>
    public abstract class ViewModelBase : ObservableObject
    {
        private bool _isBusy;
        /// <summary>
        /// Indica si el ViewModel está ocupado realizando una operación
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title;
        /// <summary>
        /// Título del ViewModel (usado para navegación/breadcrumbs)
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Se llama cuando se navega A este ViewModel
        /// </summary>
        /// <param name="parameter">Parámetro opcional de navegación</param>
        public virtual void OnNavigatedTo(object parameter)
        {
            // Los ViewModels hijos pueden override este método
        }

        /// <summary>
        /// Se llama cuando se navega DESDE este ViewModel
        /// </summary>
        public virtual void OnNavigatedFrom()
        {
            // Los ViewModels hijos pueden override este método
        }
    }
}