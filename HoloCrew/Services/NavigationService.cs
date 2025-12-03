using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace HoloCrew.Services
{
    /// <summary>
    /// Implementación del servicio de navegación
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<ViewModelBase> _navigationStack;
        private Action<object> _setCurrentView;

        public bool CanGoBack => _navigationStack.Count > 1;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _navigationStack = new Stack<ViewModelBase>();
        }

        public void Initialize(Action<object> setCurrentView)
        {
            _setCurrentView = setCurrentView;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            NavigateTo<TViewModel>(null);
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            // Resolver ViewModel desde DI
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // Notificar al ViewModel anterior que se está saliendo
            if (_navigationStack.Count > 0)
            {
                var previousViewModel = _navigationStack.Peek();
                previousViewModel.OnNavigatedFrom();
            }

            // Notificar al nuevo ViewModel que se está navegando a él
            if (parameter != null)
            {
                viewModel.OnNavigatedTo(parameter);
            }
            else
            {
                viewModel.OnNavigatedTo(null);
            }

            // Agregar al historial
            _navigationStack.Push(viewModel);

            // Actualizar vista actual
            _setCurrentView?.Invoke(viewModel);
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            // Remover vista actual
            var currentViewModel = _navigationStack.Pop();
            currentViewModel.OnNavigatedFrom();

            // Obtener vista anterior
            var previousViewModel = _navigationStack.Peek();
            previousViewModel.OnNavigatedTo(null);

            // Actualizar vista actual
            _setCurrentView?.Invoke(previousViewModel);
        }
    }
}