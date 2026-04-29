using HoloCrew.Services;
using HoloCrew.Services.Interfaces;
using HoloCrew.ViewModels;
using HoloCrew.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;

// Registro de repositorios necesarios
using HoloCrew.Repositories;
using HoloCrew.Repositories.Interfaces;

// Cliente de Supabase
using Supabase;

// Archivo principal de la aplicación (code-behind).
// Configura la inyección de dependencias (DI) registrando repositorios, servicios, ViewModels y vistas.
// Inicializa el cliente de Supabase con las credenciales de appsettings.json.
// Al iniciar, aplica el tema guardado, crea la ventana principal y navega a HomeViewModel.

namespace HoloCrew
{
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;
        private IConfiguration _configuration;

        public App()
        {
            // Cargar configuración desde appsettings.json (en la carpeta del ejecutable)
            _configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // ========== CONFIGURACIÓN ==========
            services.AddSingleton(_configuration);

            // ========== SUPABASE ==========
            // Se registra como Singleton: una sola instancia compartida en toda la app.
            // El cliente gestiona internamente la sesión, los tokens y la conexión.
            services.AddSingleton<Supabase.Client>(provider =>
            {
                var url = _configuration["Supabase:Url"]
                    ?? throw new InvalidOperationException("Falta Supabase:Url en appsettings.json");
                var anonKey = _configuration["Supabase:AnonKey"]
                    ?? throw new InvalidOperationException("Falta Supabase:AnonKey en appsettings.json");

                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = false,           // Realtime apagado (lo activaremos solo si hace falta)
                    AutoRefreshToken = true                // Renueva el JWT automáticamente al expirar
                };

                var client = new Supabase.Client(url, anonKey, options);

                // Inicializar de forma síncrona en el arranque (es UNA sola vez al abrir la app)
                client.InitializeAsync().GetAwaiter().GetResult();

                return client;
            });

            // ========== REPOSITORIOS ==========
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IOrderRepository, OrderRepository>();
            services.AddSingleton<IWishlistRepository, WishlistRepository>();
            services.AddSingleton<ICartRepository, CartRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();

            // ========== SERVICIOS ==========
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IAvatarService, AvatarService>();
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
                    "1. appsettings.json existe y tiene las credenciales de Supabase\n" +
                    "2. Todos los repositorios estén registrados en ConfigureServices()\n" +
                    "3. Todos los servicios tengan sus dependencias registradas\n" +
                    "4. Los using están correctamente añadidos",
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