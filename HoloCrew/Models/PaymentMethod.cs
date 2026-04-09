using System;

// Cómo se puede pagar un pedido.
// Método de pago guardado del usuario. No se guarda el número entero por seguridad, solo los últimos 4 dígitos.

namespace HoloCrew.Models
{
    public enum PaymentType
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        BankTransfer = 3,
        CashOnDelivery = 4   // pago contra reembolso
    }

    public class PaymentMethod
    {
        public int Id { get; set; }
        public PaymentType Type { get; set; }
        public string CardholderName { get; set; }
        public string CardNumberMasked { get; set; } // solo los últimos 4 dígitos (ej: "**** **** **** 1234")
        public string ExpirationDate { get; set; }
        public string BillingAddress { get; set; }
        public bool IsDefault { get; set; } // si es el método por defecto, se selecciona automáticamente
    }
}