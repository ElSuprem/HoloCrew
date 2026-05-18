using HoloCrew.Models;
using HoloCrew.Services.Interfaces;
using System;
using System.Threading.Tasks;

// Implementación simulada del servicio de pago.
// 
// Para una pasarela real (Stripe, PayPal, Redsys...), se crearía una nueva clase
// que implementara IPaymentService y se cambiaría el registro en App.xaml.cs.
// El resto de la app (CheckoutViewModel, View, etc.) no se entera del cambio.
// Esto es el principio de inversión de dependencias que ya documentamos en P2
// para el avatar y los repositorios.
//
// Comportamiento actual: simula procesamiento durante 2 segundos y aprueba
// siempre el pago. Devuelve un ID de transacción ficticio para trazabilidad.

namespace HoloCrew.Services
{
    public class SimulatedPaymentService : IPaymentService
    {
        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount)
        {
            // Simulamos la latencia de una pasarela real (Stripe tarda ~1-3s).
            await Task.Delay(2000);

            // Validación mínima: no se cobra un importe negativo o cero.
            if (amount <= 0)
            {
                return PaymentResult.Declined("Invalid payment amount");
            }

            // Generamos un ID de transacción ficticio para trazabilidad.
            // En la integración real lo devolvería la pasarela (ej. Stripe Charge ID).
            var transactionId = $"SIM-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

            System.Diagnostics.Debug.WriteLine(
                $"[Payment] Simulated payment approved: {transactionId} | Amount: €{amount:N2}");

            return PaymentResult.Approved(transactionId, "Payment processed successfully");
        }
    }
}