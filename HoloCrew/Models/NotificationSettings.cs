using System;

// Qué tipo de notificaciones quiere recibir el usuario.
// Se guarda en la configuración de cada usuario.

namespace HoloCrew.Models
{
    public class NotificationSettings
    {
        public bool OrderUpdatesEnabled { get; set; }     // cambios en pedidos
        public bool PromotionsEnabled { get; set; }       // ofertas y descuentos
        public bool NewProductsEnabled { get; set; }      // nuevos productos
        public bool PriceAlertsEnabled { get; set; }      // bajada de precio
        public bool EmailNotificationsEnabled { get; set; } // por email
        public bool PushNotificationsEnabled { get; set; }  // en el móvil/app
    }
}