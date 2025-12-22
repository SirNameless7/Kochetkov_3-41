using Microsoft.EntityFrameworkCore;
using KPO_Cursovoy.Models;
using Microsoft.Extensions.DependencyInjection;

namespace KPO_Cursovoy.Services
{
    public class DatabaseService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DB Init error: {ex.Message}");
            }
        }

        public async Task<List<PcItem>> GetPcsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await context.Pcs.ToListAsync();
        }

        public async Task<List<ComponentCategory>> GetComponentCategoriesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await context.ComponentCategories.ToListAsync();
        }

        public async Task<List<ComponentItem>> GetComponentsByCategoryAsync(string categoryCode)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await context.Components
                .Where(c => c.CategoryCode == categoryCode)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<List<CompatibilityRule>> GetCompatibilityRulesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // return await context.CompatibilityRules.ToListAsync();
            return new List<CompatibilityRule>();
        }
    }
}
