using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;
using Microsoft.EntityFrameworkCore;
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
            return await context.Pcs.AsNoTracking().ToListAsync();
        }

        public async Task<List<ComponentCategory>> GetComponentCategoriesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var list = await context.ComponentCategories.AsNoTracking().ToListAsync();

            foreach (var c in list)
            {
                if (string.IsNullOrWhiteSpace(c.CategoryCode))
                    continue;

                if (string.IsNullOrWhiteSpace(c.CategoryName))
                    c.CategoryName = c.CategoryCode; // FIX: чтобы не было "пустых дублей" в Picker
            }

            return list
                .Where(c => !string.IsNullOrWhiteSpace(c.CategoryCode))
                .GroupBy(c => c.CategoryCode.Trim().ToUpperInvariant())
                .Select(g => g.First())
                .OrderBy(c => c.CategoryName)
                .ToList();
        }


        public async Task<List<ComponentItem>> GetComponentsByCategoryAsync(string categoryCode)
        {
            if (string.IsNullOrWhiteSpace(categoryCode))
                return new List<ComponentItem>();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Чтобы исключить проблему из-за лишних пробелов/регистра в categoryCode
            var normalized = categoryCode.Trim();

            return await context.Components
                .AsNoTracking()
                .Where(c => c.CategoryCode != null && c.CategoryCode == normalized)
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

        // --- ДОБАВЛЕНО ДЛЯ ОПЛАТЫ ---

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return;

            order.Status = status;
            await context.SaveChangesAsync();
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

            await context.Entry(order).Collection(o => o.Components).Query().Include(oc => oc.Component).LoadAsync();
            await context.Entry(order).Collection(o => o.Services).Query().Include(os => os.Service).LoadAsync();

            return order;
        }

        // --- ЛОЯЛЬНОСТЬ ---

        public async Task<string> UpdateUserLoyaltyFromMetricsAsync(int userId, decimal totalSpentPaid, int paidOrdersCount)
        {
            var newStatus = CalculateLoyaltyStatus(totalSpentPaid, paidOrdersCount);

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return newStatus;

            if (!string.Equals(user.LoyaltyStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                user.LoyaltyStatus = newStatus;
                await context.SaveChangesAsync();
            }

            return user.LoyaltyStatus;
        }

        public async Task<string> RecalculateUserLoyaltyAsync(int userId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var paidStatuses = new[] { OrderStatus.Paid, OrderStatus.Completed };

            var paidOrders = await context.Orders
                .Where(o => o.UserId == userId && paidStatuses.Contains(o.Status))
                .ToListAsync();

            var totalSpentPaid = paidOrders.Sum(o => o.TotalAmount);
            var paidOrdersCount = paidOrders.Count;

            var newStatus = CalculateLoyaltyStatus(totalSpentPaid, paidOrdersCount);

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                return newStatus;

            if (!string.Equals(user.LoyaltyStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            {
                user.LoyaltyStatus = newStatus;
                await context.SaveChangesAsync();
            }

            return user.LoyaltyStatus;
        }

        public static decimal GetDiscountPercentByStatus(string? loyaltyStatus)
        {
            return (loyaltyStatus ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "постоянный" => 10m,
                "премиум" => 15m,
                _ => 0m
            };
        }

        private static string CalculateLoyaltyStatus(decimal totalSpentPaid, int paidOrdersCount)
        {
            if (totalSpentPaid >= 1_000_000m || paidOrdersCount >= 15)
                return "премиум";

            if (totalSpentPaid >= 500_000m || paidOrdersCount >= 5)
                return "постоянный";

            return "обычный";
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

            // Чтобы услуги точно были (для покупки услуг)
            await EnsureDefaultServicesAsync(context);
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

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Set<Payment>().Add(payment);
            await context.SaveChangesAsync();
            return payment;
        }

        public async Task<PcItem?> GetPcByIdAsync(int pcId)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            return await context.Pcs
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == pcId);
        }

        // --- УСЛУГИ ---

        public async Task<List<ServiceItem>> GetServicesAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            await EnsureDefaultServicesAsync(context);

            return await context.Services
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // Делает добавление/обновление услуг "пакетом", без N запросов на каждую услугу
        private static async Task EnsureDefaultServicesAsync(AppDbContext context)
        {
            var defaults = new List<ServiceItem>
            {
                new ServiceItem { Id = 1, Name = "Диагностика", Description = "Проверка ПК и выявление неисправностей", DurationDays = 1, Price = 1500m },
                new ServiceItem { Id = 2, Name = "Чистка от пыли", Description = "Чистка системного блока от пыли/грязи", DurationDays = 1, Price = 1200m },
                new ServiceItem { Id = 3, Name = "Замена термопасты", Description = "Замена термопасты на процессоре", DurationDays = 1, Price = 1000m },
                new ServiceItem { Id = 4, Name = "Сборка ПК", Description = "Профессиональная сборка компьютера", DurationDays = 2, Price = 3000m },
            };

            var ids = defaults.Select(d => d.Id).ToList();

            var existing = await context.Services
                .Where(s => ids.Contains(s.Id))
                .ToListAsync();

            var existingById = existing.ToDictionary(x => x.Id);

            foreach (var d in defaults)
            {
                if (!existingById.TryGetValue(d.Id, out var current))
                {
                    context.Services.Add(d);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(current.Name)) current.Name = d.Name;
                if (string.IsNullOrWhiteSpace(current.Description)) current.Description = d.Description;
                if (current.DurationDays <= 0) current.DurationDays = d.DurationDays;
                if (current.Price <= 0) current.Price = d.Price;
            }

            await context.SaveChangesAsync();
        }
    }
}
