using HoloCrew.ViewModels.Base;

namespace HoloCrew.ViewModels
{
    public class FlashSaleViewModel : ViewModelBase
    {
        public FlashSaleViewModel()
        {
            // TODO: Inicializar datos de productos en oferta
        }

        public override void OnNavigatedTo(object parameter)
        {
            base.OnNavigatedTo(parameter);
            // TODO: Cargar ofertas activas
            // TODO: Iniciar temporizador de cuenta regresiva
        }

        public override void OnNavigatedFrom()
        {
            base.OnNavigatedFrom();
            // TODO: Detener temporizador
        }
    }
}
