using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels;
using HoloCrew.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

// Registro de repositorios necesarios
using HoloCrew.Repositories;
using HoloCrew.Repositories.Interfaces;

// Archivo principal de la aplicación (code-behind).
// Configura la inyección de dependencias (DI) registrando repositorios, servicios, ViewModels y vistas.
// Al iniciar, aplica el tema guardado, crea la ventana principal y navega a HomeViewModel.

namespace HoloCrew
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // ========== REPOSITORIOS ==========
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IWishlistRepository, WishlistRepository>();

            // ========== SERVICIOS ==========
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<IThemeService, ThemeService>();

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddSingleton<IWishlistService, WishlistService>();

            // ========== VIEWMODELS ==========
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

            services.AddTransient<FlashSaleViewModel>();
            services.AddTransient<BlackWeekViewModel>();
            services.AddTransient<MembersClubViewModel>();

            // ========== VISTAS ==========
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // aplicar tema guardado (si falla, continuar igual)
                try
                {
                    var themeService = _serviceProvider.GetRequiredService<IThemeService>();
                    themeService.ApplySavedTheme();
                }
                catch
                {
                    // si falla el tema, se sigue sin él
                }

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                var mainWindowViewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();

                mainWindow.DataContext = mainWindowViewModel;

                var navigationService = _serviceProvider.GetRequiredService<INavigationService>();

                // configura el servicio de navegación para que actualice CurrentView
                navigationService.Initialize(view => mainWindowViewModel.CurrentView = view);

                navigationService.NavigateTo<HomeViewModel>();

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