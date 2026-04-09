using System;

// Los distintos tipos de notificación que puede mostrar la app.
// Cada tipo puede tener un color o icono distinto.

namespace HoloCrew.Models
{
    public enum NotificationType
    {
        Info,       // información general
        Order,      // relacionada con pedidos
        Promotion,  // ofertas y descuentos
        Alert,      // alerta de precio o stock bajo
        Success,    // algo salió bien
        Warning,    // advertencia
        Error       // error o problema
    }
}