using HoloCrew.Models;
using System.Threading.Tasks;

// Servicio de pago. Abstrae la pasarela de pago real (Stripe, PayPal, etc.).
// La implementación actual es simulada; sustituyendo esta clase por otra que
// implemente la misma interfaz se podría conectar Stripe sin tocar el resto.

namespace HoloCrew.Services.Interfaces
{
    public interface IPaymentService
    {
        // Procesa un pago por el importe indicado.
        // Devuelve un PaymentResult con éxito/fallo y datos de la transacción.
        Task<PaymentResult> ProcessPaymentAsync(decimal amount);
    }
}