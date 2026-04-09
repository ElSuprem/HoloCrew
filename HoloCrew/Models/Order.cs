using System;
using System.Collections.Generic;

// Estados del pedido (pendiente, confirmado, enviado, etc.).
// Pedido completo con sus productos, direcciones, pagos e historial de cambios.

namespace HoloCrew.Models
{
    public enum OrderStatus
    {
        Pending = 0,     // recién creado, esperando confirmación
        Confirmed = 1,   // confirmado por la tienda
        Processing = 2,  // preparando el pedido
        Shipped = 3,     // enviado
        Delivered = 4,   // entregado al cliente
        Cancelled = 5,   // cancelado
        Refunded = 6     // reembolsado
    }

    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<OrderItem> Items { get; set; }          // lista de productos comprados
        public decimal Subtotal { get; set; }               // suma de los productos sin gastos
        public decimal ShippingCost { get; set; }           // gastos de envío
        public decimal Tax { get; set; }                    // impuestos (IVA)
        public decimal Discount { get; set; }               // descuento aplicado
        public decimal Total { get; set; }                  // total a pagar
        public OrderStatus Status { get; set; }
        public Address ShippingAddress { get; set; }        // dirección donde se envía
        public PaymentMethod PaymentMethod { get; set; }    // cómo pagó
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; } // fecha estimada de entrega
        public DateTime? DeliveredDate { get; set; }        // fecha real de entrega
        public string TrackingNumber { get; set; }          // número de seguimiento del envío
        public List<OrderStatusHistory> StatusHistory { get; set; } // historial de cambios de estado
    }

    // Un producto dentro de un pedido (con cantidad y precio al que se compró)
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }      // precio al que se compró (congelado, aunque luego suba)
        public string SelectedVariant { get; set; } // talla o color elegido

        public decimal Subtotal => UnitPrice * Quantity;
    }

    // Registro de cuándo cambió de estado un pedido
    public class OrderStatusHistory
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string Note { get; set; } // nota opcional (ej: "cancelado por falta de stock")
    }
}