namespace HoloCrew.Models
{
    // ===== ENUM =====
    public enum PaymentType
    {
        CreditCard = 0,
        DebitCard = 1,
        PayPal = 2,
        BankTransfer = 3,
        CashOnDelivery = 4
    }

    // ===== CLASE PRINCIPAL =====
    public class PaymentMethod
    {
        public int Id { get; set; }
        public PaymentType Type { get; set; }
        public string CardholderName { get; set; }
        public string CardNumberMasked { get; set; } // Últimos 4 dígitos
        public string ExpirationDate { get; set; }
        public string BillingAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}