using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

// Servicio de navegación entre pantallas (MVVM puro).
// En lugar de navegar a vistas, navega a ViewModels.
// Guarda un historial para poder volver atrás (GoBack).

namespace HoloCrew.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<ViewModelBase> _navigationStack;  // historial de pantallas
        private Action<object> _setCurrentView;

        public bool CanGoBack => _navigationStack.Count > 1;  // se puede volver atrás si hay más de una pantalla

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _navigationStack = new Stack<ViewModelBase>();
        }

        public void Initialize(Action<object> setCurrentView)
        {
            _setCurrentView = setCurrentView;  // se guarda la función que cambia la vista actual
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            NavigateTo<TViewModel>(null);
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();

            // avisar a la pantalla actual que nos vamos
            if (_navigationStack.Count > 0)
            {
                var previousViewModel = _navigationStack.Peek();
                previousViewModel.OnNavigatedFrom();
            }

            // avisar a la nueva pantalla que ha llegado (con o sin parámetro)
            if (parameter != null)
            {
                viewModel.OnNavigatedTo(parameter);
            }
            else
            {
                viewModel.OnNavigatedTo(null);
            }

            _navigationStack.Push(viewModel);
            _setCurrentView?.Invoke(viewModel);
        }

        public void GoBack()
        {
            if (!CanGoBack) return;

            var currentViewModel = _navigationStack.Pop();
            currentViewModel.OnNavigatedFrom();

            var previousViewModel = _navigationStack.Peek();
            previousViewModel.OnNavigatedTo(null);

            _setCurrentView?.Invoke(previousViewModel);
        }
    }
}