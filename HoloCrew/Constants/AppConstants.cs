namespace HoloCrew.Constants
{
    /// <summary>
    /// Constantes globales de la aplicación
    /// Centraliza todos los textos para facilitar traducción futura
    /// </summary>
    public static class AppConstants
    {
        // ========== INFORMACIÓN DE LA APP ==========
        public const string AppName = "HoloCrew";
        public const string AppVersion = "1.0.0";
        public const string CompanyName = "HoloCrew Inc.";
        public const string AppTagline = "Rise with the Crew";

        // ========== API ==========
        public const string ApiBaseUrl = "https://api.holocrew.com/v1/";
        public const int ApiTimeoutSeconds = 30;
        public const int MaxRetryAttempts = 3;
        public const int RetryDelayMilliseconds = 1000;

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
        public const int MinSearchLength = 2;

        // ========== FORMATOS ==========
        public const string DateFormat = "dd/MM/yyyy";
        public const string DateTimeFormat = "dd/MM/yyyy HH:mm";
        public const string TimeFormat = "HH:mm";
        public const string CurrencyFormat = "€{0:N2}";
        public const string CurrencySymbol = "€";

        // ========== ENVÍO ==========
        public const decimal FreeShippingThreshold = 50.00m;
        public const decimal DefaultShippingCost = 5.99m;
        public const decimal ExpressShippingCost = 9.99m;
        public const decimal TaxRate = 0.21m; // IVA 21%

        // ========== LÍMITES ==========
        public const int MaxCartItemQuantity = 99;
        public const int MaxWishlistItems = 100;
        public const int MaxRecentSearches = 10;
        public const int MaxNotifications = 100;
        public const int LowStockThreshold = 10;

        // ========== NOTIFICACIONES ==========
        public const int NotificationDisplayDurationSeconds = 5;
        public const int ToastDurationMilliseconds = 3000;

        // ========== RUTAS ==========
        public static class Paths
        {
            public const string Images = "/Resources/Images/";
            public const string Icons = "/Resources/Icons/";
            public const string Fonts = "/Resources/Fonts/";
            public const string Placeholder = "/Resources/Images/placeholder.png";
            public const string PlaceholderProduct = "/Resources/Images/product-placeholder.png";
            public const string PlaceholderAvatar = "/Resources/Images/avatar-placeholder.png";
        }

        // ========== REGEX PATTERNS ==========
        public static class RegexPatterns
        {
            public const string Email = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            public const string Phone = @"^(\+34|0034|34)?[ -]?[6-9]\d{2}[ -]?\d{3}[ -]?\d{3}$";
            public const string PostalCode = @"^(?:0[1-9]|[1-4]\d|5[0-2])\d{3}$";
            public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$";
        }

        // ========== UI - TEXTOS GENERALES ==========
        public static class UI
        {
            // Navegación
            public const string Home = "Home";
            public const string Shop = "Shop";
            public const string Cart = "Cart";
            public const string Wishlist = "Wishlist";
            public const string Profile = "Profile";
            public const string Settings = "Settings";
            public const string Notifications = "Notifications";
            public const string Orders = "Orders";
            public const string Checkout = "Checkout";

            // Acciones comunes
            public const string Add = "Add";
            public const string Remove = "Remove";
            public const string Edit = "Edit";
            public const string Delete = "Delete";
            public const string Save = "Save";
            public const string Cancel = "Cancel";
            public const string Confirm = "Confirm";
            public const string Close = "Close";
            public const string Back = "Back";
            public const string Next = "Next";
            public const string Continue = "Continue";
            public const string Submit = "Submit";
            public const string Apply = "Apply";
            public const string Clear = "Clear";
            public const string ClearAll = "Clear All";
            public const string Retry = "Retry";
            public const string Refresh = "Refresh";
            public const string LoadMore = "Load More";
            public const string ViewAll = "View All";
            public const string SeeMore = "See More";
            public const string ShowLess = "Show Less";

            // Filtros y ordenación
            public const string Filters = "Filters";
            public const string SortBy = "Sort by";
            public const string Featured = "Featured";
            public const string Newest = "Newest";
            public const string PriceLowToHigh = "Price: Low to High";
            public const string PriceHighToLow = "Price: High to Low";
            public const string BestSelling = "Best Selling";
            public const string TopRated = "Top Rated";

            // Estados
            public const string Loading = "Loading...";
            public const string Saving = "Saving...";
            public const string Processing = "Processing...";
            public const string Searching = "Searching...";
            public const string NoResults = "No results found";
            public const string Empty = "Nothing here yet";
        }

        // ========== MENSAJES DE ÉXITO ==========
        public static class Success
        {
            // Carrito
            public const string AddedToCart = "Product added to cart";
            public const string RemovedFromCart = "Product removed from cart";
            public const string CartUpdated = "Cart updated";
            public const string CartCleared = "Cart cleared";

            // Wishlist
            public const string AddedToWishlist = "Added to wishlist";
            public const string RemovedFromWishlist = "Removed from wishlist";

            // Pedidos
            public const string OrderPlaced = "Order placed successfully!";
            public const string OrderCancelled = "Order cancelled";
            public const string OrderUpdated = "Order updated";

            // Usuario
            public const string LoginSuccess = "Welcome back!";
            public const string RegisterSuccess = "Account created successfully!";
            public const string LogoutSuccess = "You have been logged out";
            public const string ProfileUpdated = "Profile updated successfully";
            public const string PasswordChanged = "Password changed successfully";
            public const string EmailVerified = "Email verified successfully";

            // Configuración
            public const string SettingsSaved = "Settings saved";
            public const string PreferencesSaved = "Preferences saved";

            // General
            public const string ChangesSaved = "Changes saved";
            public const string ActionCompleted = "Action completed";
        }

        // ========== MENSAJES DE ERROR ==========
        public static class Errors
        {
            // Conexión
            public const string NetworkError = "Connection error. Please check your internet.";
            public const string ServerError = "Server error. Please try again later.";
            public const string TimeoutError = "Request timed out. Please try again.";
            public const string UnexpectedError = "An unexpected error occurred.";

            // Autenticación
            public const string InvalidCredentials = "Invalid email or password";
            public const string EmailInUse = "Email is already in use";
            public const string WeakPassword = "Password is too weak";
            public const string SessionExpired = "Your session has expired. Please log in again.";
            public const string Unauthorized = "You don't have permission to do this.";

            // Validación
            public const string RequiredField = "This field is required";
            public const string InvalidEmail = "Please enter a valid email address";
            public const string InvalidPhone = "Please enter a valid phone number";
            public const string InvalidPostalCode = "Please enter a valid postal code";
            public const string PasswordMismatch = "Passwords do not match";
            public const string PasswordTooShort = "Password must be at least 8 characters";
            public const string NameTooShort = "Name must be at least 2 characters";

            // Carrito
            public const string CartEmpty = "Your cart is empty";
            public const string ProductNotAvailable = "This product is no longer available";
            public const string InsufficientStock = "Insufficient stock available";
            public const string MaxQuantityReached = "Maximum quantity reached";

            // Pedidos
            public const string OrderNotFound = "Order not found";
            public const string OrderCannotBeCancelled = "This order cannot be cancelled";
            public const string PaymentFailed = "Payment failed. Please try again.";
            public const string InvalidPromoCode = "Invalid promo code";
            public const string PromoCodeExpired = "This promo code has expired";

            // Carga de datos
            public const string LoadingFailed = "Failed to load data";
            public const string ProductsLoadFailed = "Failed to load products";
            public const string OrdersLoadFailed = "Failed to load orders";
            public const string ProfileLoadFailed = "Failed to load profile";
        }

        // ========== MENSAJES VACÍOS / PLACEHOLDER ==========
        public static class Empty
        {
            public const string CartTitle = "Your cart is empty";
            public const string CartSubtitle = "Add items to your cart to checkout";
            public const string CartAction = "Start Shopping";

            public const string WishlistTitle = "Your wishlist is empty";
            public const string WishlistSubtitle = "Save your favorite items here";
            public const string WishlistAction = "Explore Products";

            public const string OrdersTitle = "No orders yet";
            public const string OrdersSubtitle = "Your order history will appear here";
            public const string OrdersAction = "Start Shopping";

            public const string NotificationsTitle = "No notifications";
            public const string NotificationsSubtitle = "You're all caught up!";

            public const string SearchTitle = "No results found";
            public const string SearchSubtitle = "Try adjusting your search or filters";
            public const string SearchAction = "Clear Filters";

            public const string ProductsTitle = "No products found";
            public const string ProductsSubtitle = "Try adjusting your filters";
            public const string ProductsAction = "Clear Filters";

            public const string ReviewsTitle = "No reviews yet";
            public const string ReviewsSubtitle = "Be the first to review this product";
            public const string ReviewsAction = "Write a Review";
        }

        // ========== TEXTOS DE PRODUCTO ==========
        public static class Product
        {
            public const string AddToCart = "Add to Cart";
            public const string AddedToCart = "Added to Cart";
            public const string BuyNow = "Buy Now";
            public const string QuickAdd = "Quick Add";
            public const string OutOfStock = "Out of Stock";
            public const string LowStock = "Low Stock";
            public const string InStock = "In Stock";
            public const string PreOrder = "Pre-Order";
            public const string ComingSoon = "Coming Soon";

            public const string NewArrival = "NEW";
            public const string Sale = "SALE";
            public const string BestSeller = "BEST SELLER";
            public const string Limited = "LIMITED";

            public const string SelectSize = "Select Size";
            public const string SelectColor = "Select Color";
            public const string SizeGuide = "Size Guide";
            public const string Quantity = "Quantity";

            public const string Description = "Description";
            public const string Details = "Details";
            public const string Care = "Care Instructions";
            public const string Shipping = "Shipping & Returns";
            public const string Reviews = "Reviews";

            public const string FreeShipping = "Free shipping on orders over €50";
            public const string EasyReturns = "14-day return policy";
            public const string ShipsWithin = "Ships within 24h";
        }

        // ========== TEXTOS DE CHECKOUT ==========
        public static class Checkout
        {
            public const string Title = "Checkout";
            public const string ShippingAddress = "Shipping Address";
            public const string BillingAddress = "Billing Address";
            public const string PaymentMethod = "Payment Method";
            public const string OrderSummary = "Order Summary";

            public const string Subtotal = "Subtotal";
            public const string Shipping = "Shipping";
            public const string Tax = "Tax";
            public const string Discount = "Discount";
            public const string Total = "Total";

            public const string FreeShipping = "FREE";
            public const string CalculatedAtCheckout = "Calculated at checkout";

            public const string PromoCode = "Promo Code";
            public const string ApplyCode = "Apply";

            public const string PlaceOrder = "Place Order";
            public const string Processing = "Processing your order...";

            public const string SecureCheckout = "Secure Checkout";
            public const string EncryptedPayment = "Your payment information is encrypted";
        }

        // ========== TEXTOS DE AUTENTICACIÓN ==========
        public static class Auth
        {
            public const string Login = "Log In";
            public const string Logout = "Log Out";
            public const string Register = "Create Account";
            public const string ForgotPassword = "Forgot Password?";
            public const string ResetPassword = "Reset Password";

            public const string Email = "Email";
            public const string Password = "Password";
            public const string ConfirmPassword = "Confirm Password";
            public const string FirstName = "First Name";
            public const string LastName = "Last Name";
            public const string FullName = "Full Name";
            public const string Phone = "Phone";

            public const string RememberMe = "Remember me";
            public const string KeepLoggedIn = "Keep me logged in";

            public const string NoAccount = "Don't have an account?";
            public const string HasAccount = "Already have an account?";
            public const string SignUpHere = "Sign up here";
            public const string LoginHere = "Log in here";

            public const string TermsAgreement = "I agree to the Terms and Conditions";
            public const string NewsletterOptIn = "Subscribe to our newsletter";
        }

        // ========== TEXTOS DE PERFIL ==========
        public static class Profile
        {
            public const string Title = "My Account";
            public const string PersonalInfo = "Personal Information";
            public const string Addresses = "Addresses";
            public const string PaymentMethods = "Payment Methods";
            public const string OrderHistory = "Order History";
            public const string Preferences = "Preferences";
            public const string Security = "Security";

            public const string EditProfile = "Edit Profile";
            public const string ChangePassword = "Change Password";
            public const string DeleteAccount = "Delete Account";

            public const string MemberSince = "Member since";
            public const string TotalOrders = "Total Orders";
            public const string TotalSpent = "Total Spent";
        }

        // ========== TEXTOS DE PEDIDOS ==========
        public static class Orders
        {
            public const string Title = "My Orders";
            public const string OrderNumber = "Order #";
            public const string OrderDate = "Order Date";
            public const string Status = "Status";
            public const string TrackOrder = "Track Order";
            public const string ViewDetails = "View Details";
            public const string Reorder = "Reorder";
            public const string CancelOrder = "Cancel Order";

            // Estados
            public const string StatusPending = "Pending";
            public const string StatusProcessing = "Processing";
            public const string StatusShipped = "Shipped";
            public const string StatusDelivered = "Delivered";
            public const string StatusCancelled = "Cancelled";
            public const string StatusRefunded = "Refunded";
        }

        // ========== TEXTOS DE NOTIFICACIONES ==========
        public static class Notifications
        {
            public const string Title = "Notifications";
            public const string MarkAllRead = "Mark all as read";
            public const string ClearAll = "Clear all";

            public const string OrderShipped = "Your order has been shipped!";
            public const string OrderDelivered = "Your order has been delivered!";
            public const string NewArrival = "New arrivals are here!";
            public const string SaleAlert = "Sale starts now!";
            public const string BackInStock = "Back in stock!";
            public const string PriceDropAlert = "Price drop alert!";
        }

        // ========== TEXTOS DE FOOTER/LEGALES ==========
        public static class Footer
        {
            public const string About = "About Us";
            public const string Contact = "Contact";
            public const string FAQ = "FAQ";
            public const string Careers = "Careers";
            public const string Press = "Press";

            public const string Shipping = "Shipping";
            public const string Returns = "Returns";
            public const string SizeGuide = "Size Guide";
            public const string TrackOrder = "Track Order";

            public const string Terms = "Terms & Conditions";
            public const string Privacy = "Privacy Policy";
            public const string Cookies = "Cookie Policy";

            public const string Newsletter = "Newsletter";
            public const string NewsletterSubtitle = "Subscribe for exclusive offers";
            public const string EmailPlaceholder = "Enter your email";
            public const string Subscribe = "Subscribe";

            public const string Copyright = "© 2024 HoloCrew. All rights reserved.";
        }
    }
}