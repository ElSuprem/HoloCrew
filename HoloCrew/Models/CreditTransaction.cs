using System;

// Transacción de créditos (ganancia o canje).
// Viene de la tabla credit_transactions de Supabase.

namespace HoloCrew.Models
{
    public class CreditTransaction
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty; // "earn", "redeem", "bonus", etc.
        public int Amount { get; set; }                              // positivo o negativo
        public int BalanceAfter { get; set; }
        public int? OrderId { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}