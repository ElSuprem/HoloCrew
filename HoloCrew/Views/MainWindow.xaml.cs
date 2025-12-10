using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HoloCrew.Views
{
    /// <summary>
    /// Ventana principal de la aplicación con SCROLL INTELIGENTE
    /// Permite que vistas internas tengan su propio scroll
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // El DataContext se establece por Dependency Injection en App.xaml.cs
        }

        /// <summary>
        /// Evento de scroll inteligente para MainScrollViewer
        /// Detecta si el ratón está sobre un ScrollViewer interno y NO captura el evento
        /// </summary>
        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            // Obtener el elemento bajo el ratón
            var mousePosition = e.GetPosition(scrollViewer);
            var elementUnderMouse = scrollViewer.InputHitTest(mousePosition) as DependencyObject;

            // Buscar si hay un ScrollViewer interno en la jerarquía
            var innerScrollViewer = FindParentScrollViewer(elementUnderMouse, scrollViewer);

            if (innerScrollViewer != null)
            {
                // HAY un ScrollViewer interno
                // Verificar si puede hacer scroll
                bool canScrollUp = innerScrollViewer.VerticalOffset > 0;
                bool canScrollDown = innerScrollViewer.VerticalOffset < innerScrollViewer.ScrollableHeight;

                if ((e.Delta > 0 && canScrollUp) || (e.Delta < 0 && canScrollDown))
                {
                    // El ScrollViewer interno PUEDE hacer scroll
                    // NO capturamos el evento, dejamos que el interno lo maneje
                    return;
                }
            }

            // NO hay ScrollViewer interno O ya llegó al límite
            // El MainScrollViewer maneja el evento normalmente
            if (e.Delta > 0)
            {
                // Scroll hacia arriba
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 120);
            }
            else
            {
                // Scroll hacia abajo
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 120);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Busca un ScrollViewer padre en la jerarquía visual
        /// Excluye el scrollViewerToExclude (MainScrollViewer)
        /// </summary>
        private ScrollViewer FindParentScrollViewer(DependencyObject child, ScrollViewer scrollViewerToExclude)
        {
            if (child == null) return null;

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                // Si encontramos el MainScrollViewer, paramos (no queremos encontrarnos a nosotros mismos)
                if (parent == scrollViewerToExclude)
                {
                    return null;
                }

                // Si encontramos un ScrollViewer, lo devolvemos
                if (parent is ScrollViewer scrollViewer)
                {
                    return scrollViewer;
                }

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}