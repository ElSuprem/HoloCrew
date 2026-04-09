using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HoloCrew.Views
{
    // Vista del perfil de usuario (code-behind).
    // Maneja el evento de scroll para que cuando el scroll interno llegue al final,
    // el evento pase al ScrollViewer padre (MainWindow).

    public partial class ProfileView : UserControl
    {
        public ProfileView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                var canScrollUp = scrollViewer.VerticalOffset > 0;
                var canScrollDown = scrollViewer.VerticalOffset < scrollViewer.ScrollableHeight;

                // si no puede seguir haciendo scroll, pasar el evento al padre
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
    }
}