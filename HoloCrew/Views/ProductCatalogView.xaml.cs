using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HoloCrew.Views
{
    /// <summary>
    /// Vista de catálogo de productos con filtros y grid moderno
    /// </summary>
    public partial class ProductCatalogView : UserControl
    {
        public ProductCatalogView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Scroll para el área de PRODUCTOS (derecha)
        /// Propaga el scroll al padre cuando llega al límite
        /// </summary>
        private void ProductsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var canScrollUp = scrollViewer.VerticalOffset > 0;
                var canScrollDown = scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight;

                // Si no puede seguir scrolleando, propagar al padre
                if ((e.Delta > 0 && !canScrollUp) || (e.Delta < 0 && !canScrollDown))
                {
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = this
                    };

                    var parent = ((FrameworkElement)this).Parent as UIElement;
                    parent?.RaiseEvent(eventArg);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Scroll para el área de FILTROS (izquierda)
        /// Scroll independiente que no propaga al padre
        /// </summary>
        private void FiltersScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                // El sidebar de filtros hace scroll independiente
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}