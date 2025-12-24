using System;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class OrderProcessingService
    {
        private readonly DatabaseService _databaseService;
        private readonly StockService _stockService;
        private readonly PaymentService _paymentService;

        public OrderProcessingService(
            DatabaseService databaseService,
            StockService stockService,
            PaymentService paymentService)
        {
            _databaseService = databaseService;
            _stockService = stockService;
            _paymentService = paymentService;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            if (!await _stockService.CheckAvailabilityAsync(order.Components))
                throw new Exception("Некоторые компоненты отсутствуют на складе");

            await _stockService.ReserveComponentsAsync(order.Id, order.Components);
            return await _databaseService.CreateOrderAsync(order);
        }

        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentMethod method, PaymentType type)
        {
            var success = await _paymentService.ProcessPaymentAsync(orderId, method, type);
            if (success)
            {
                await UpdateOrderStatusAsync(orderId, OrderStatus.Paid);
                return true;
            }

            return false;
        }

        public async Task CancelOrderAsync(int orderId)
        {
            await _stockService.ReleaseReservationAsync(orderId);
            await UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
        }

        private async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            await _databaseService.UpdateOrderStatusAsync(orderId, status);
        }
    }
}
