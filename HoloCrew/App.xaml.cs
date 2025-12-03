using HoloCrew.Repositories;
using HoloCrew.Repositories.Interfaces;
using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels;
using HoloCrew.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace HoloCrew
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            // Configurar Dependency Injection al iniciar la aplicación
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Configura todos los servicios de la aplicación
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            // ========== REPOSITORIES ==========
            // Singleton: Una sola instancia durante toda la vida de la app
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IWishlistRepository, WishlistRepository>();

            // ========== SERVICES ==========
            // Singleton: Servicios que mantienen estado global
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IThemeService, ThemeService>();

            // Transient: Nueva instancia cada vez que se solicita
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IWishlistService, WishlistService>();

            // ========== VIEWMODELS ==========
            // Transient: Cada vista obtiene su propia instancia del ViewModel
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ProductCatalogViewModel>();
            services.AddTransient<ProductDetailViewModel>();
            services.AddTransient<CartViewModel>();
            services.AddTransient<CheckoutViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddTransient<OrderHistoryViewModel>();
            services.AddTransient<OrderDetailViewModel>();
            services.AddTransient<WishlistViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<NotificationsViewModel>();

            // ========== VIEWS/WINDOWS ==========
            // Transient: Cada vez que se necesita una ventana, se crea una nueva
            services.AddTransient<MainWindow>();
        }

        /// <summary>
        /// Se ejecuta cuando la aplicación inicia
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Aplicar tema guardado (si ThemeService está implementado)
                try
                {
                    var themeService = _serviceProvider.GetRequiredService<IThemeService>();
                    themeService.ApplySavedTheme();
                }
                catch
                {
                    // Si falla el tema, continuar sin él
                }

                // Obtener MainWindow desde DI
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

                // Obtener MainWindowViewModel desde DI
                var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();

                // Establecer el DataContext
                mainWindow.DataContext = mainWindowViewModel;

                // Obtener NavigationService y configurarlo
                var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

                // Inicializar NavigationService con el callback para actualizar CurrentView
                navigationService.Initialize(view => mainWindowViewModel.CurrentView = view);

                // Navegar a la página inicial (HomeViewModel)
                navigationService.NavigateTo<HomeViewModel>();

                // Mostrar la ventana principal
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al iniciar la aplicación:\n\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                    "Error de Inicio",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // Cerrar la aplicación si hay error crítico
                Shutdown();
            }
        }

        /// <summary>
        /// Se ejecuta cuando la aplicación se cierra
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            // Limpiar recursos
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }

        /// <summary>
        /// Manejo global de excepciones no controladas
        /// </summary>
        private void Application_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Ha ocurrido un error inesperado:\n\n{e.Exception.Message}\n\nStack Trace:\n{e.Exception.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            // Marcar como manejada para evitar que la aplicación se cierre
            e.Handled = true;
        }
    }
}