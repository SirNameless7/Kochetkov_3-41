using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class StockService
    {
        private readonly DatabaseService _databaseService;

        public StockService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> CheckAvailabilityAsync(List<OrderComponent> components)
        {
            foreach (var component in components)
            {
                var stock = await GetComponentStockAsync(component.ComponentId);
                if (stock < component.Quantity)
                    return false;
            }
            return true;
        }

        public async Task ReserveComponentsAsync(int orderId, List<OrderComponent> components)
        {
            foreach (var component in components)
            {
                await UpdateStockAsync(component.ComponentId, -component.Quantity);
            }
        }

        public async Task ReleaseReservationAsync(int orderId)
        {
        }

        public async Task<List<ComponentItem>> GetLowStockItemsAsync(int threshold = 5)
        {
            return new List<ComponentItem>();
        }

        private async Task<int> GetComponentStockAsync(int componentId)
        {
             return 10;
        }

        private async Task UpdateStockAsync(int componentId, int delta)
        {
        }
    }
}