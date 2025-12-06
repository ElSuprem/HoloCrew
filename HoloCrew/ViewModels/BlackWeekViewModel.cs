using HoloCrew.ViewModels.Base;

namespace HoloCrew.ViewModels
{
    public class BlackWeekViewModel : ViewModelBase
    {
        public BlackWeekViewModel()
        {
            // TODO: Inicializar categorías con ofertas
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            // TODO: Cargar ofertas de Black Week
        }
    }
}
