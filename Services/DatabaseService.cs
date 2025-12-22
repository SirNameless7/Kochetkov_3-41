using Microsoft.EntityFrameworkCore;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class DatabaseService
    {
        private readonly AppDbContext _context;

        public DatabaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PcItem>> GetPcsAsync()
        {
            return await _context.Pcs.ToListAsync();
        }

        public async Task<List<ComponentCategory>> GetComponentCategoriesAsync()
        {
            return await _context.ComponentCategories.ToListAsync();
        }

        public async Task<List<ComponentItem>> GetComponentsByCategoryAsync(string categoryCode)
        {
            return await _context.Components
                .Where(c => c.CategoryCode == categoryCode)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<List<CompatibilityRule>> GetCompatibilityRulesAsync()
        {
            //return await _context.CompatibilityRules.ToListAsync();
            return new List<CompatibilityRule>();
        }
    }
}
