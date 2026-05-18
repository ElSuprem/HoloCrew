using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

// DTO que mapea la tabla 'orders' de Supabase.
// Mantiene los campos esenciales que necesita la UI; el resto (notas internas,
// timestamps de cambio de estado, etc.) los gestiona la BD automáticamente vía triggers.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("orders")]
    public class OrderDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("payment_status")]
        public string PaymentStatus { get; set; } = "pending";

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("shipping_cost")]
        public decimal ShippingCost { get; set; }

        [Column("tax")]
        public decimal Tax { get; set; }

        [Column("discount")]
        public decimal Discount { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        [Column("coupon_code")]
        public string? CouponCode { get; set; }

        [Column("shipping_address")]
        public string? ShippingAddressJson { get; set; }

        [Column("billing_address")]
        public string? BillingAddressJson { get; set; }

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("payment_reference")]
        public string? PaymentReference { get; set; }

        [Column("tracking_number")]
        public string? TrackingNumber { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("shipped_at")]
        public DateTime? ShippedAt { get; set; }

        [Column("delivered_at")]
        public DateTime? DeliveredAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        public Order ToOrder(List<OrderItemDto>? items = null)
        {
            var order = new Order
            {
                Id = Id,
                OrderNumber = OrderNumber,
                UserId = UserId.ToString(),
                Status = ParseStatus(Status),
                Subtotal = Subtotal,
                ShippingCost = ShippingCost,
                Tax = Tax,
                Discount = Discount,
                Total = Total,
                OrderDate = OrderDate,
                DeliveredDate = DeliveredAt,
                TrackingNumber = TrackingNumber,
                Items = items?.Select(i => i.ToOrderItem()).ToList() ?? new List<OrderItem>()
            };

            // Deserializar shipping_address (JSONB) al modelo Address.
            // Si está vacío o falla el parsing, dejamos null y la UI mostrará "No address provided".
            if (!string.IsNullOrWhiteSpace(ShippingAddressJson))
            {
                try
                {
                    order.ShippingAddress = Newtonsoft.Json.JsonConvert.DeserializeObject<Address>(ShippingAddressJson);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[OrderDto] Error parsing shipping_address: {ex.Message}");
                }
            }

            // Construir el PaymentMethod a partir de los campos planos de la BBDD.
            // payment_method es el tipo (card, paypal, cash...), payment_reference el ID
            // de transacción (de SimulatedPaymentService) y CardNumberMasked se saca de
            // los últimos 4 dígitos si la referencia es una tarjeta.
            if (!string.IsNullOrWhiteSpace(PaymentMethod))
            {
                order.PaymentMethod = new PaymentMethod
                {
                    Type = ParsePaymentType(PaymentMethod),
                    CardNumberMasked = !string.IsNullOrWhiteSpace(PaymentReference)
                        ? FormatCardNumberMasked(PaymentReference)
                        : string.Empty,
                    CardholderName = string.Empty,
                    ExpirationDate = string.Empty,
                    BillingAddress = string.Empty
                };
            }

            return order;
        }

        // Convierte el enum de la BD a un texto legible: "card" → "Visa", "paypal" → "PayPal".
        // Si en el futuro guardas el brand real (visa, mastercard) cambia esta lógica.
        private static string FormatPaymentType(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            return raw.ToLower() switch
            {
                "card" or "credit_card" or "debit_card" => "Visa",
                "paypal" => "PayPal",
                "cash" or "cash_on_delivery" => "Cash on Delivery",
                _ => char.ToUpper(raw[0]) + raw.Substring(1)
            };
        }

        // Convierte el string que viene de la BBDD (enum Postgres) al enum C# PaymentType.
        // La BBDD usa valores como "card", "paypal", "cash"; el modelo C# espera el enum tipado.
        private static PaymentType ParsePaymentType(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return PaymentType.CreditCard;
            return raw.ToLower() switch
            {
                "card" or "credit_card" => PaymentType.CreditCard,
                "debit_card" => PaymentType.DebitCard,
                "paypal" => PaymentType.PayPal,
                "bank_transfer" or "transfer" => PaymentType.BankTransfer,
                "cash" or "cash_on_delivery" => PaymentType.CashOnDelivery,
                _ => PaymentType.CreditCard
            };
        }

        // Saca los últimos 4 dígitos del payment_reference y los formatea como
        // "•••• 1234". Si no hay nada útil, devuelve string vacío.
        // Se usa para mostrar la tarjeta en el detalle del pedido (ej: "Visa •••• 4242").
        private static string FormatCardNumberMasked(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference)) return string.Empty;
            var digits = new string(reference.Where(char.IsDigit).ToArray());
            if (digits.Length >= 4)
                return $"•••• {digits.Substring(digits.Length - 4)}";
            return $"•••• {reference}";
        }

        // Convierte el string del enum de Postgres al enum C#
        public static OrderStatus ParseStatus(string status) => status?.ToLower() switch
        {
            "pending" => OrderStatus.Pending,
            "confirmed" => OrderStatus.Confirmed,
            "processing" => OrderStatus.Processing,
            "shipped" => OrderStatus.Shipped,
            "delivered" => OrderStatus.Delivered,
            "cancelled" => OrderStatus.Cancelled,
            "refunded" => OrderStatus.Refunded,
            _ => OrderStatus.Pending
        };

        // Convierte el enum C# al string que entiende Postgres
        public static string StatusToString(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "pending",
            OrderStatus.Confirmed => "confirmed",
            OrderStatus.Processing => "processing",
            OrderStatus.Shipped => "shipped",
            OrderStatus.Delivered => "delivered",
            OrderStatus.Cancelled => "cancelled",
            OrderStatus.Refunded => "refunded",
            _ => "pending"
        };
    }
}