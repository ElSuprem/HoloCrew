using System;

// Resultado de procesar un pago a través de IPaymentService.
// Incluye éxito/fallo, identificador de transacción y mensaje legible.

namespace HoloCrew.Models
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }

        // Factoría: pago aprobado.
        public static PaymentResult Approved(string transactionId, string message)
        {
            return new PaymentResult
            {
                Success = true,
                TransactionId = transactionId,
                Message = message,
                ProcessedAt = DateTime.UtcNow
            };
        }

        // Factoría: pago rechazado.
        public static PaymentResult Declined(string message)
        {
            return new PaymentResult
            {
                Success = false,
                TransactionId = string.Empty,
                Message = message,
                ProcessedAt = DateTime.UtcNow
            };
        }
    }
}