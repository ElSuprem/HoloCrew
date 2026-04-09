using System;
using System.Collections.Generic;

// Información de seguimiento de un pedido: número, transportista, fecha de envío, eventos.
// Cada evento es un punto en el recorrido (ej: "10:30 - Llega a Madrid").

namespace HoloCrew.Models
{
    public class TrackingInfo
    {
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }          // empresa de paquetería (Correos, DHL, etc.)
        public DateTime ShipDate { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string CurrentStatus { get; set; }    // estado actual (ej: "En reparto")
        public string CurrentLocation { get; set; }  // dónde está ahora el paquete
        public List<TrackingEvent> Events { get; set; } // historial del recorrido
    }

    public class TrackingEvent
    {
        public DateTime Timestamp { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }      // explicación larga del evento
    }
}