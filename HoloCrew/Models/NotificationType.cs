namespace HoloCrew.Models
{
    /// <summary>
    /// Tipos de notificaciones en la aplicación
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Notificación general o informativa
        /// </summary>
        Info,

        /// <summary>
        /// Notificación relacionada con pedidos
        /// </summary>
        Order,

        /// <summary>
        /// Notificación de promoción o oferta
        /// </summary>
        Promotion,

        /// <summary>
        /// Alerta de precio o stock
        /// </summary>
        Alert,

        /// <summary>
        /// Notificación de éxito
        /// </summary>
        Success,

        /// <summary>
        /// Advertencia
        /// </summary>
        Warning,

        /// <summary>
        /// Error o problema
        /// </summary>
        Error
    }
}