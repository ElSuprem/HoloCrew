using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace HoloCrew.Views
{
    // Ventana principal (code-behind).
    // Maneja el mega menú desplegable, la barra de búsqueda con foco automático,
    // el scroll inteligente del contenido y la navegación entre vistas.
    // Se conecta con MainWindowViewModel.

    public partial class MainWindow : Window
    {
        private bool _isMenuOpen = false;

        public MainWindow()
        {
            InitializeComponent();

            DataContextChanged += MainWindow_DataContextChanged;
        }

        // cuando cambia el DataContext, suscribirse a los cambios del ViewModel
        private void MainWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is INotifyPropertyChanged oldVm)
            {
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;
            }

            if (e.NewValue is INotifyPropertyChanged newVm)
            {
                newVm.PropertyChanged += ViewModel_PropertyChanged;
            }
        }

        // cuando cambia CurrentView, resetear el scroll al principio de la página
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentView")
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainScrollViewer.ScrollToTop();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        // muestra el mega menú cuando el ratón entra en "SHOP"
        private void ShopMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            MegaMenuDropdown.Visibility = Visibility.Visible;
            _isMenuOpen = true;
        }

        // oculta el mega menú cuando el ratón sale (con un pequeño retraso para evitar cierre accidental)
        private void ShopMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, args) =>
            {
                timer.Stop();

                if (!IsMouseOverElement(MegaMenuDropdown) && !IsMouseOverElement(ShopMenuTrigger))
                {
                    MegaMenuDropdown.Visibility = Visibility.Collapsed;
                    _isMenuOpen = false;
                }
            };
            timer.Start();
        }

        // comprueba si el ratón está sobre un elemento de la interfaz
        private bool IsMouseOverElement(UIElement element)
        {
            if (element == null) return false;

            Point mousePos = Mouse.GetPosition(element);
            return mousePos.X >= 0 && mousePos.Y >= 0 &&
                   mousePos.X <= ((FrameworkElement)element).ActualWidth &&
                   mousePos.Y <= ((FrameworkElement)element).ActualHeight;
        }

        // cuando la barra de búsqueda se hace visible, poner el foco automáticamente
        private void SearchBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Border border && border.Visibility == Visibility.Visible)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SearchTextBox.Focus();
                    SearchTextBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        // scroll inteligente: si el scroll interno puede moverse, no pasa el evento al exterior
        private void MainScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null) return;

            var mousePosition = e.GetPosition(scrollViewer);
            var elementUnderMouse = scrollViewer.InputHitTest(mousePosition) as DependencyObject;

            if (elementUnderMouse != null && (elementUnderMouse is Visual || elementUnderMouse is Visual3D))
            {
                var innerScrollViewer = FindParentScrollViewer(elementUnderMouse, scrollViewer);

                if (innerScrollViewer != null)
                {
                    bool canScrollUp = innerScrollViewer.VerticalOffset > 0;
                    bool canScrollDown = innerScrollViewer.VerticalOffset < innerScrollViewer.ScrollableHeight;

                    if ((e.Delta > 0 && canScrollUp) || (e.Delta < 0 && canScrollDown))
                    {
                        return;
                    }
                }
            }

            if (e.Delta > 0)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 100);
            }
            else
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 100);
            }

            e.Handled = true;
        }

        // busca un ScrollViewer padre en el árbol visual (excluyendo el actual)
        private ScrollViewer? FindParentScrollViewer(DependencyObject child, ScrollViewer scrollViewerToExclude)
        {
            if (child == null) return null;

            try
            {
                DependencyObject parent = VisualTreeHelper.GetParent(child);

                while (parent != null)
                {
                    if (parent == scrollViewerToExclude)
                    {
                        return null;
                    }

                    if (parent is ScrollViewer scrollViewer)
                    {
                        return scrollViewer;
                    }

                    parent = VisualTreeHelper.GetParent(parent);
                }
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }

            return null;
        }

        // limpiar suscripciones al cerrar la ventana
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is INotifyPropertyChanged vm)
            {
                vm.PropertyChanged -= ViewModel_PropertyChanged;
            }
            base.OnClosed(e);
        }
    }
}