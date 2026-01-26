using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HoloCrew.ViewModels;

namespace HoloCrew.Views
{
    public partial class SettingsView : UserControl
    {
        private ScrollViewer? _mainScrollViewer;

        public SettingsView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsViewModel.ShowPrivacyPolicy) ||
                e.PropertyName == nameof(SettingsViewModel.ShowTermsOfService) ||
                e.PropertyName == nameof(SettingsViewModel.ShowContactSupport))
            {
                if (DataContext is SettingsViewModel vm)
                {
                    bool anyModalOpen = vm.ShowPrivacyPolicy || vm.ShowTermsOfService || vm.ShowContactSupport;
                    
                    // Buscar el MainScrollViewer si no lo tenemos
                    if (_mainScrollViewer == null)
                    {
                        _mainScrollViewer = FindMainScrollViewer();
                    }

                    if (_mainScrollViewer != null)
                    {
                        if (anyModalOpen)
                        {
                            // Modal abierto: scroll arriba y bloquear
                            _mainScrollViewer.ScrollToTop();
                            _mainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        }
                        else
                        {
                            // Modal cerrado: desbloquear scroll
                            _mainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Busca el MainScrollViewer subiendo por el árbol visual hasta encontrarlo
        /// </summary>
        private ScrollViewer? FindMainScrollViewer()
        {
            DependencyObject? current = this;
            
            // Subir hasta encontrar el Window
            while (current != null && !(current is Window))
            {
                current = VisualTreeHelper.GetParent(current);
            }

            if (current is Window window)
            {
                // Buscar el ScrollViewer llamado "MainScrollViewer" dentro del Window
                return FindChildByName<ScrollViewer>(window, "MainScrollViewer");
            }

            return null;
        }

        /// <summary>
        /// Busca un elemento hijo por nombre en el árbol visual
        /// </summary>
        private T? FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                
                if (child is T element && element.Name == name)
                {
                    return element;
                }

                var result = FindChildByName<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}