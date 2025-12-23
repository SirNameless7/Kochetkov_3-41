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
            if (string.IsNullOrWhiteSpace(categoryCode))
                return new List<ComponentItem>();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return await context.Components
                .Where(c => c.CategoryCode != null && c.CategoryCode == categoryCode)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Orders
                .Include(o => o.Components)
                    .ThenInclude(oc => oc.Component)
                .Include(o => o.Services)
                    .ThenInclude(os => os.Service)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order, List<ComponentItem>? usedComponents = null)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (usedComponents != null && usedComponents.Any())
            {
                foreach (var comp in usedComponents)
                {
                    var dbComp = await context.Components.FirstOrDefaultAsync(c => c.Id == comp.Id);
                    if (dbComp == null)
                        throw new Exception($"Компонент {comp.Name} не найден в базе");

                    if (dbComp.Stock < 1)
                        throw new Exception($"Компонент {dbComp.Name} закончился на складе");

                    dbComp.Stock -= 1; // списываем 1
                }
            }

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Загружаем навигационные свойства после сохранения
            await context.Entry(order).Collection(o => o.Components).Query().Include(oc => oc.Component).LoadAsync();
            await context.Entry(order).Collection(o => o.Services).Query().Include(os => os.Service).LoadAsync();

            return order;
        }


        public async Task<List<CompatibilityRule>> GetCompatibilityRulesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return new List<CompatibilityRule>();
        }

        public async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await context.Database.EnsureCreatedAsync();
            await RemoveDuplicatesAsync(context);
        }

        private async Task RemoveDuplicatesAsync(AppDbContext context)
        {
            var categoryDuplicates = context.ComponentCategories
                .AsEnumerable()
                .GroupBy(c => c.CategoryCode)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (categoryDuplicates.Any())
                context.ComponentCategories.RemoveRange(categoryDuplicates);

            var pcDuplicates = context.Pcs
                .AsEnumerable()
                .GroupBy(p => p.Name)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g.Skip(1))
                .ToList();

            if (pcDuplicates.Any())
                context.Pcs.RemoveRange(pcDuplicates);

            await context.SaveChangesAsync();
        }
    }
}
