using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace HoloCrew.Views
{
    // Vista de registro (code-behind).
    // Sincroniza los PasswordBox con las propiedades del ViewModel.
    // Muestra los términos y condiciones en un MessageBox.

    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
        }

        // sincroniza el campo contraseña con el ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        // sincroniza el campo confirmar contraseña con el ViewModel
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.RegisterViewModel viewModel)
            {
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }

        // muestra los términos y condiciones en un cuadro de diálogo
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            MessageBox.Show(
                "TÉRMINOS Y CONDICIONES DE HOLOCREW\n\n" +
                "1. Aceptación de los términos\n" +
                "Al registrarte en HoloCrew, aceptas estos términos de servicio.\n\n" +
                "2. Uso del servicio\n" +
                "Debes tener al menos 18 años para usar nuestro servicio.\n\n" +
                "3. Privacidad\n" +
                "Tus datos personales serán tratados según nuestra política de privacidad.\n\n" +
                "4. Compras\n" +
                "Todas las compras están sujetas a disponibilidad y confirmación del precio.\n\n" +
                "5. Devoluciones\n" +
                "Tienes 30 días para devolver productos en su estado original.\n\n" +
                "6. Contacto\n" +
                "Para cualquier consulta: soporte@holocrew.com",
                "Términos y Condiciones",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            e.Handled = true;
        }
    }
}