using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.ComponentModel;

namespace HoloCrew.Views
{
    /// <summary>
    /// Ventana principal con mega menú desplegable estilo HoloCrew
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isMenuOpen = false;

        public MainWindow()
        {
            InitializeComponent();

            // Suscribirse al cambio de vista para resetear scroll
            DataContextChanged += MainWindow_DataContextChanged;
        }

        /// <summary>
        /// Cuando cambia el DataContext, suscribirse a cambios de CurrentView
        /// </summary>
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

        /// <summary>
        /// Cuando cambia CurrentView, resetear el scroll a la parte superior
        /// </summary>
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentView")
            {
                // Resetear scroll al inicio cuando cambia la vista
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    MainScrollViewer.ScrollToTop();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        /// <summary>
        /// Muestra el mega menú cuando el mouse entra en "SHOP"
        /// </summary>
        private void ShopMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            MegaMenuDropdown.Visibility = Visibility.Visible;
            _isMenuOpen = true;
        }

        /// <summary>
        /// Oculta el mega menú cuando el mouse sale
        /// </summary>
        private void ShopMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            // Pequeño delay para evitar cierre accidental
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, args) =>
            {
                timer.Stop();

                // Verificar si el mouse está sobre el menú o el trigger
                if (!IsMouseOverElement(MegaMenuDropdown) && !IsMouseOverElement(ShopMenuTrigger))
                {
                    MegaMenuDropdown.Visibility = Visibility.Collapsed;
                    _isMenuOpen = false;
                }
            };
            timer.Start();
        }

        /// <summary>
        /// Verifica si el mouse está sobre un elemento
        /// </summary>
        private bool IsMouseOverElement(UIElement element)
        {
            if (element == null) return false;

            Point mousePos = Mouse.GetPosition(element);
            return mousePos.X >= 0 && mousePos.Y >= 0 &&
                   mousePos.X <= ((FrameworkElement)element).ActualWidth &&
                   mousePos.Y <= ((FrameworkElement)element).ActualHeight;
        }

        /// <summary>
        /// Cuando la barra de búsqueda se hace visible, poner el foco automáticamente
        /// </summary>
        private void SearchBar_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Border border && border.Visibility == Visibility.Visible)
            {
                // Usar Dispatcher para asegurar que el focus se aplica después del render
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SearchTextBox.Focus();
                    SearchTextBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        /// <summary>
        /// Scroll inteligente para el contenido principal
        /// </summary>
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

        /// <summary>
        /// Limpiar suscripciones al cerrar
        /// </summary>
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