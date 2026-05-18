using HoloCrew.Models;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

// DTO que mapea la tabla 'credit_transactions' de Supabase.

namespace HoloCrew.Infraestructure.Supabase.Models
{
    [Table("credit_transactions")]
    public class CreditTransactionDto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("transaction_type")]
        public string TransactionType { get; set; } = string.Empty;

        [Column("amount")]
        public int Amount { get; set; }

        [Column("balance_after")]
        public int BalanceAfter { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }


        public CreditTransaction ToModel()
        {
            return new CreditTransaction
            {
                Id = Id,
                UserId = UserId.ToString(),
                TransactionType = TransactionType,
                Amount = Amount,
                BalanceAfter = BalanceAfter,
                OrderId = OrderId,
                Description = Description ?? string.Empty,
                CreatedAt = CreatedAt
            };
        }
    }
}