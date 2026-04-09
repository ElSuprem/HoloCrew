using System.Windows;
using System.Windows.Controls;

namespace HoloCrew.Views
{
    // Vista de inicio de sesión (code-behind).
    // Sincroniza el PasswordBox con la propiedad Password del ViewModel.

    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // cuando el usuario escribe en el campo de contraseña, se actualiza la propiedad en el ViewModel
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.LoginViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}