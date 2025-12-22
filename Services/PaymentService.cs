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
                    Status = PaymentStatus.Pending
                };
                payment.Status = await SimulatePaymentProcessingAsync(payment);

                await SavePaymentAsync(payment);
                return payment.Status == PaymentStatus.Paid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<PaymentStatus> SimulatePaymentProcessingAsync(Payment payment)
        {
            await Task.Delay(1000);
            return new Random().Next(0, 2) == 0 ? PaymentStatus.Paid : PaymentStatus.Failed;
        }

        private async Task<decimal> GetOrderAmountAsync(int orderId)
        {
            return 10000;
        }

        private async Task SavePaymentAsync(Payment payment)
        {
        }
    }
}