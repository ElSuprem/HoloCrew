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
                oldVm.PropertyChanged -= ViewModel_PropertyChanged;

            if (e.NewValue is INotifyPropertyChanged newVm)
                newVm.PropertyChanged += ViewModel_PropertyChanged;
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

                    if (_mainScrollViewer == null)
                        _mainScrollViewer = FindMainScrollViewer();

                    if (_mainScrollViewer != null)
                    {
                        if (anyModalOpen)
                        {
                            _mainScrollViewer.ScrollToTop();
                            _mainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        }
                        else
                        {
                            _mainScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        }
                    }
                }
            }
        }

        private ScrollViewer? FindMainScrollViewer()
        {
            DependencyObject? current = this;
            while (current != null && current is not Window)
                current = VisualTreeHelper.GetParent(current);

            if (current is Window window)
                return FindChildByName<ScrollViewer>(window, "MainScrollViewer");

            return null;
        }

        private T? FindChildByName<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                    return element;

                var result = FindChildByName<T>(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }
    }
}