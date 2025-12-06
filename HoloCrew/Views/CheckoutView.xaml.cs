using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HoloCrew.Views
{
    public partial class CheckoutView : UserControl
    {
        public CheckoutView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Pasar el evento al ScrollViewer padre (MainWindow)
            if (sender is ScrollViewer scrollViewer)
            {
                // Solo pasar si el ScrollViewer interno no puede hacer scroll más
                var canScrollUp = scrollViewer.VerticalOffset > 0;
                var canScrollDown = scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight;

                if ((e.Delta > 0 && !canScrollUp) || (e.Delta < 0 && !canScrollDown))
                {
                    // Crear un nuevo evento y pasarlo al padre
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
    }
}
