using System.Windows;

namespace HoloCrew.Views
{
    /// <summary>
    /// Ventana principal de la aplicación
    /// NO contiene lógica - todo está en MainWindowViewModel
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // El DataContext se establece por Dependency Injection en App.xaml.cs
        }
    }
}