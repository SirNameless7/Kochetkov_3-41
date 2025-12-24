using System;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class PaymentService
    {
        private readonly DatabaseService _databaseService;

        public PaymentService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentMethod method, PaymentType type)
        {
            try
            {
                var payment = new Payment
                {
                    OrderId = orderId,
                    Method = method,
                    Type = type,
                    Amount = await GetOrderAmountAsync(orderId),
                    Status = PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow
                };

                payment.Status = await SimulatePaymentProcessingAsync(payment);

                await SavePaymentAsync(payment);

                return payment.Status == PaymentStatus.Paid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PAYMENT ERROR (full):");
                System.Diagnostics.Debug.WriteLine(ex.ToString()); // важно!
                return false;
            }

        }

        private async Task<PaymentStatus> SimulatePaymentProcessingAsync(Payment payment)
        {
            await Task.Delay(800);
            return PaymentStatus.Paid;
        }


        private async Task<decimal> GetOrderAmountAsync(int orderId)
        {
            var order = await _databaseService.GetOrderByIdAsync(orderId);
            return order?.TotalAmount ?? 0m;
        }

        private Task SavePaymentAsync(Payment payment)
        {
            return _databaseService.CreatePaymentAsync(payment);
        }
    }
}
