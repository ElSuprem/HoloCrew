using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels;
using HoloCrew.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

// ⭐ IMPORTANTE: Agregar estos using para los repositorios
using HoloCrew.Repositories; // Para UserRepository, ProductRepository, etc.
using HoloCrew.Repositories.Interfaces; // Para IUserRepository, IProductRepository, etc.

namespace HoloCrew
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            // Configurar el contenedor de dependencias
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // ========== REPOSITORIES ==========
            // ⭐ REGISTRAR TODOS LOS REPOSITORIOS QUE EXISTEN
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
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

            // NUEVOS ViewModels agregados
            services.AddTransient<FlashSaleViewModel>();
            services.AddTransient<BlackWeekViewModel>();
            services.AddTransient<MembersClubViewModel>();

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
                    $"Error al iniciar la aplicación: {ex.Message}\n\n" +
                    $"Detalles: {ex.InnerException?.Message}\n\n" +
                    "Verifica que:\n" +
                    "1. Todos los repositorios estén registrados en ConfigureServices()\n" +
                    "2. Todos los servicios tengan sus dependencias registradas\n" +
                    "3. Los using están correctamente añadidos",
                    "Error de Inicio",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Application.Current.Shutdown();
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Excepción no controlada: {e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }
    }
}