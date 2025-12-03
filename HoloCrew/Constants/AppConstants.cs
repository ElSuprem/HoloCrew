namespace HoloCrew.Constants
{
    /// <summary>
    /// Constantes globales de la aplicación
    /// </summary>
    public static class AppConstants
    {
        // ========== INFORMACIÓN DE LA APP ==========
        public const string AppName = "HoloCrew";
        public const string AppVersion = "1.0.0";
        public const string CompanyName = "HoloCrew Inc.";

        // ========== API ==========
        public const string ApiBaseUrl = "https://api.holocrew.com/v1/";
        public const int ApiTimeoutSeconds = 30;

        // ========== PAGINACIÓN ==========
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        // ========== CACHÉ ==========
        public const int CacheExpirationMinutes = 10;
        public const int ImageCacheExpirationHours = 24;

        // ========== VALIDACIÓN ==========
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 50;
        public const int MinNameLength = 2;
        public const int MaxNameLength = 100;

        // ========== FORMATOS ==========
        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public const string CurrencyFormat = "€{0:N2}";

        // ========== ENVÍO ==========
        public const decimal FreeShippingThreshold = 50.00m;
        public const decimal DefaultShippingCost = 5.99m;
        public const decimal TaxRate = 0.21m; // IVA 21%

        // ========== LÍMITES ==========
        public const int MaxCartItemQuantity = 99;
        public const int MaxWishlistItems = 100;
        public const int MaxRecentSearches = 10;

        // ========== NOTIFICACIONES ==========
        public const int NotificationDisplayDurationSeconds = 5;
        public const int MaxNotifications = 100;

        // ========== RUTAS ==========
        public static class Paths
        {
            public const string Images = "/Resources/Images/";
            public const string Icons = "/Resources/Icons/";
            public const string Fonts = "/Resources/Fonts/";
            public const string Placeholder = "/Resources/Images/placeholder.png";
        }

        // ========== MENSAJES ==========
        public static class Messages
        {
            public const string AddedToCart = "Producto añadido al carrito";
            public const string RemovedFromCart = "Producto eliminado del carrito";
            public const string AddedToWishlist = "Producto añadido a favoritos";
            public const string OrderPlaced = "Pedido realizado con éxito";
            public const string OrderCancelled = "Pedido cancelado";
            public const string NetworkError = "Error de conexión. Intente nuevamente.";
            public const string UnexpectedError = "Ha ocurrido un error inesperado.";
        }

        // ========== REGEX PATTERNS ==========
        public static class RegexPatterns
        {
            public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            public const string Phone = @"^(\+34|0034|34)?[ -]?[6-9]\d{2}[ -]?\d{3}[ -]?\d{3}$";
            public const string PostalCode = @"^(?:0[1-9]|[1-4]\d|5[0-2])\d{3}$";
        }
    }
}