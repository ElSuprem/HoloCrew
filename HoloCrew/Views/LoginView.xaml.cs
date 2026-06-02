using System.Windows;
using System.Windows.Controls;

namespace HoloCrew.Views
{
    // Vista de inicio de sesión (code-behind).
    // Sincroniza el PasswordBox con la propiedad Password del ViewModel
    // y controla el placeholder (puntitos) que se muestra cuando el campo está vacío.
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        // Cuando el usuario escribe en el campo de contraseña, se actualiza la propiedad
        // en el ViewModel y se oculta/muestra el placeholder según haya texto o no.
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;

            if (DataContext is ViewModels.LoginViewModel viewModel)
            {
                viewModel.Password = passwordBox.Password;
            }

            // Oculta el placeholder en cuanto hay algo escrito; lo muestra si se vacía.
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }
    }
}