using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HoloCrew.Constants;
using System;
using System.Threading.Tasks;

// Estados de carga y clase base para todos los ViewModels.
// Maneja estados (cargando, éxito, vacío, error), reintentos automáticos,
// y métodos de navegación (OnNavigatedTo, OnNavigatedFrom).
// Los mensajes de error y textos vacíos se sacan de AppConstants.

namespace HoloCrew.ViewModels.Base
{
    public enum LoadingState
    {
        Idle,
        Loading,
        Refreshing,
        LoadingMore,
        Success,
        Empty,
        Error
    }

    public abstract partial class ViewModelBase : ObservableObject
    {
        #region Properties

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsIdle))]
        [NotifyPropertyChangedFor(nameof(IsLoading))]
        [NotifyPropertyChangedFor(nameof(IsRefreshing))]
        [NotifyPropertyChangedFor(nameof(IsLoadingMore))]
        [NotifyPropertyChangedFor(nameof(IsSuccess))]
        [NotifyPropertyChangedFor(nameof(IsEmpty))]
        [NotifyPropertyChangedFor(nameof(IsError))]
        [NotifyPropertyChangedFor(nameof(IsBusy))]
        [NotifyPropertyChangedFor(nameof(ShowContent))]
        [NotifyPropertyChangedFor(nameof(ShowLoading))]
        [NotifyPropertyChangedFor(nameof(ShowEmpty))]
        [NotifyPropertyChangedFor(nameof(ShowError))]
        private LoadingState _state = LoadingState.Idle;

        // IsBusy se calcula desde State (true si está cargando, refrescando o cargando más)
        public bool IsBusy => State == LoadingState.Loading ||
                              State == LoadingState.Refreshing ||
                              State == LoadingState.LoadingMore;

        [ObservableProperty]
        private string _title = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private string _emptyTitle = AppConstants.Empty.ProductsTitle;

        [ObservableProperty]
        private string _emptySubtitle = AppConstants.Empty.ProductsSubtitle;

        [ObservableProperty]
        private string _emptyActionText = AppConstants.Empty.ProductsAction;

        [ObservableProperty]
        private string _loadingMessage = AppConstants.UI.Loading;

        #endregion

        #region Computed Properties

        public bool IsIdle => State == LoadingState.Idle;
        public bool IsLoading => State == LoadingState.Loading;
        public bool IsRefreshing => State == LoadingState.Refreshing;
        public bool IsLoadingMore => State == LoadingState.LoadingMore;
        public bool IsSuccess => State == LoadingState.Success;
        public bool IsEmpty => State == LoadingState.Empty;
        public bool IsError => State == LoadingState.Error;

        public bool ShowContent => State == LoadingState.Success ||
                                   State == LoadingState.Refreshing ||
                                   State == LoadingState.LoadingMore;
        public bool ShowLoading => State == LoadingState.Loading;
        public bool ShowEmpty => State == LoadingState.Empty;
        public bool ShowError => State == LoadingState.Error;

        #endregion

        #region Retry Logic

        private Func<Task>? _lastOperation;  // guarda la última operación para poder reintentar
        private int _retryCount = 0;
        public int RetryCount => _retryCount;

        [RelayCommand]
        private async Task RetryAsync()
        {
            if (_lastOperation != null)
            {
                await ExecuteWithRetryAsync(_lastOperation);
            }
        }

        [RelayCommand(AllowConcurrentExecutions = false)]
        protected virtual async Task RefreshDataAsync()
        {
            if (_lastOperation != null)
            {
                State = LoadingState.Refreshing;
                try
                {
                    await _lastOperation();
                    State = LoadingState.Success;
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
            }
        }

        #endregion

        #region Protected Methods

        // ejecuta una operación asíncrona y maneja estados de carga
        protected async Task ExecuteAsync(Func<Task> operation, bool isRefresh = false, bool isLoadMore = false)
        {
            _lastOperation = operation;

            if (isLoadMore) State = LoadingState.LoadingMore;
            else if (isRefresh) State = LoadingState.Refreshing;
            else State = LoadingState.Loading;

            try
            {
                await operation();
                _retryCount = 0;
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        // ejecuta una operación con reintentos automáticos (hasta 3 intentos)
        protected async Task ExecuteWithRetryAsync(Func<Task> operation, int maxRetries = 3)
        {
            _lastOperation = operation;
            State = LoadingState.Loading;
            _retryCount = 0;

            while (_retryCount < maxRetries)
            {
                try
                {
                    await operation();
                    _retryCount = 0;
                    return;
                }
                catch (Exception ex)
                {
                    _retryCount++;
                    if (_retryCount >= maxRetries)
                    {
                        HandleError(ex);
                        return;
                    }
                    await Task.Delay(AppConstants.RetryDelayMilliseconds * _retryCount);
                }
            }
        }

        // ejecuta una operación que devuelve un resultado
        protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation, bool isRefresh = false)
        {
            State = isRefresh ? LoadingState.Refreshing : LoadingState.Loading;
            try
            {
                var result = await operation();
                _retryCount = 0;
                return result;
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return default;
            }
        }

        protected void SetSuccess() => State = LoadingState.Success;

        protected void SetEmpty(string? title = null, string? subtitle = null, string? actionText = null)
        {
            if (title != null) EmptyTitle = title;
            if (subtitle != null) EmptySubtitle = subtitle;
            if (actionText != null) EmptyActionText = actionText;
            State = LoadingState.Empty;
        }

        protected void SetError(string message)
        {
            ErrorMessage = message;
            State = LoadingState.Error;
        }

        // traduce la excepción a un mensaje amigable para el usuario
        protected virtual void HandleError(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in {GetType().Name}: {ex.Message}");

            ErrorMessage = ex switch
            {
                TimeoutException => AppConstants.Errors.TimeoutError,
                UnauthorizedAccessException => AppConstants.Errors.Unauthorized,
                InvalidOperationException => ex.Message,
                _ when ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase) => AppConstants.Errors.NetworkError,
                _ when ex.Message.Contains("server", StringComparison.OrdinalIgnoreCase) => AppConstants.Errors.ServerError,
                _ => AppConstants.Errors.UnexpectedError
            };

            State = LoadingState.Error;
        }

        #endregion

        #region Navigation

        public virtual void OnNavigatedTo(object? parameter) { }  // cuando se navega a esta pantalla
        public virtual void OnNavigatedFrom() { }                 // cuando se sale de esta pantalla
        public virtual void Cleanup() => _lastOperation = null;   // limpiar referencias

        #endregion
    }
}