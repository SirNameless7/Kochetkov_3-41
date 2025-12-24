using Microsoft.EntityFrameworkCore;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ComponentCategory> ComponentCategories { get; set; }
        public DbSet<ComponentItem> Components { get; set; }
        public DbSet<PcItem> Pcs { get; set; }
        public DbSet<ServiceItem> Services { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public DbSet<CompatibilityRule> CompatibilityRules { get; set; }
        public DbSet<OrderComponent> OrderComponents { get; set; }
        public DbSet<OrderService> OrderServices { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
            modelBuilder.Entity<ComponentCategory>().HasKey(c => c.CategoryCode);
            modelBuilder.Entity<ComponentItem>().HasKey(c => c.Id);
            modelBuilder.Entity<PcItem>().HasKey(p => p.Id);
            modelBuilder.Entity<ServiceItem>().HasKey(s => s.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<CompatibilityRule>().HasKey(r => r.RuleId);

            modelBuilder.Entity<Payment>().HasKey(p => p.Id);
            modelBuilder.Entity<Payment>()
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Ignore<ComponentSpecification>();
            modelBuilder.Ignore<DeliveryItem>();
            modelBuilder.Ignore<Specification>();
            modelBuilder.Ignore<SpecificationValue>();

            modelBuilder.Entity<OrderComponent>()
                .HasKey(oc => new { oc.OrderId, oc.ComponentId });

            modelBuilder.Entity<OrderService>()
                .HasKey(os => new { os.OrderId, os.ServiceId });

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Components)
                .WithOne()
                .HasForeignKey(oc => oc.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithOne()
                .HasForeignKey(os => os.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<ComponentItem>()
                .HasOne<ComponentCategory>()
                .WithMany()
                .HasForeignKey(c => c.CategoryCode);

            modelBuilder.Entity<Account>().HasIndex(a => a.Login).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User { UserId = 1, FullName = "Иван Петров", Phone = "+79990001122", LoyaltyStatus = "постоянный" },
                new User { UserId = 2, FullName = "Мария Смирнова", Phone = "+79995553322", LoyaltyStatus = "обычный" }
            );

            modelBuilder.Entity<Account>().HasData(
               new Account { AccountId = 1, UserId = 1, Login = "ivan", PasswordHash = "12345", Role = "client" },
               new Account { AccountId = 2, UserId = 2, Login = "maria", PasswordHash = "54321", Role = "client" }
            );

            modelBuilder.Entity<ComponentCategory>().HasData(
                new ComponentCategory { CategoryCode = "CPU", CategoryName = "Процессор" },
                new ComponentCategory { CategoryCode = "MB", CategoryName = "Материнская плата" },
                new ComponentCategory { CategoryCode = "RAM", CategoryName = "Оперативная память" },
                new ComponentCategory { CategoryCode = "GPU", CategoryName = "Видеокарта" },
                new ComponentCategory { CategoryCode = "SSD", CategoryName = "Накопитель SSD" },
                new ComponentCategory { CategoryCode = "PSU", CategoryName = "Блок питания" },
                new ComponentCategory { CategoryCode = "CASE", CategoryName = "Корпус" }

            );

            modelBuilder.Entity<ComponentItem>().HasData(
                // CPU
                new ComponentItem { Id = 1, Name = "Intel Core i5-11400", CategoryCode = "CPU", Price = 15000, Stock = 10, Socket = "LGA1200", PowerDrawW = 65 },
                new ComponentItem { Id = 2, Name = "AMD Ryzen 5 5600G", CategoryCode = "CPU", Price = 16000, Stock = 15, Socket = "AM4", PowerDrawW = 65 },

                // MB
                new ComponentItem { Id = 3, Name = "Gigabyte B560M", CategoryCode = "MB", Price = 8000, Stock = 17, Socket = "LGA1200", MemoryType = "DDR4", RamSlots = 4 },
                new ComponentItem { Id = 7, Name = "ASUS B550M", CategoryCode = "MB", Price = 9000, Stock = 16, Socket = "AM4", MemoryType = "DDR4", RamSlots = 4 },

                // RAM
                new ComponentItem { Id = 5, Name = "Corsair 16GB DDR4", CategoryCode = "RAM", Price = 7000, Stock = 20, MemoryType = "DDR4" },
                new ComponentItem { Id = 8, Name = "Kingston 8GB DDR4", CategoryCode = "RAM", Price = 3500, Stock = 30, MemoryType = "DDR4" },

                // GPU
                new ComponentItem { Id = 4, Name = "NVIDIA RTX 3060", CategoryCode = "GPU", Price = 25000, Stock = 15, GpuLengthMm = 242, PowerDrawW = 170 },
                new ComponentItem { Id = 9, Name = "NVIDIA RTX 4070", CategoryCode = "GPU", Price = 60000, Stock = 17, GpuLengthMm = 300, PowerDrawW = 200 },

                // SSD
                new ComponentItem { Id = 6, Name = "SSD Samsung 500GB", CategoryCode = "SSD", Price = 5000, Stock = 25 },

                // CASE
                new ComponentItem { Id = 10, Name = "Case Mini Tower 280mm", CategoryCode = "CASE", Price = 4500, Stock = 20, MaxGpuLengthMm = 280 },
                new ComponentItem { Id = 11, Name = "Case Mid Tower 340mm", CategoryCode = "CASE", Price = 6500, Stock = 18, MaxGpuLengthMm = 340 },

                // PSU
                new ComponentItem { Id = 12, Name = "PSU 500W", CategoryCode = "PSU", Price = 4000, Stock = 10, Wattage = 500 },
                new ComponentItem { Id = 13, Name = "PSU 650W", CategoryCode = "PSU", Price = 5500, Stock = 10, Wattage = 650 }
            );

            modelBuilder.Entity<PcItem>().HasData(
                new PcItem
                {
                    Id = 1,
                    Name = "Игровой ПК Start",
                    Description = "Базовый игровой компьютер: i5, 16 ГБ ОЗУ, RTX 3060, SSD 500 ГБ",
                    Price = 60000
                },
                new PcItem
                {
                    Id = 2,
                    Name = "Офисный ПК Lite",
                    Description = "Для работы и учёбы: Ryzen 5, 16 ГБ ОЗУ, встроенная графика, SSD 500 ГБ",
                    Price = 35000
                },
                new PcItem
                {
                    Id = 3,
                    Name = "Игровой ПК Pro",
                    Description = "Для современных игр: i7, 32 ГБ ОЗУ, RTX 4070, SSD 1 ТБ",
                    Price = 120000
                },
                new PcItem
                {
                    Id = 4,
                    Name = "ПК для стриминга",
                    Description = "Многопоточный процессор, 32 ГБ ОЗУ, RTX 4060 Ti, два SSD по 1 ТБ",
                    Price = 135000
                },
                new PcItem
                {
                    Id = 5,
                    Name = "Компактный мини‑ПК",
                    Description = "Маленький корпус, низкий уровень шума, 16 ГБ ОЗУ, SSD 512 ГБ",
                    Price = 45000
                }
            );

            modelBuilder.Entity<ServiceItem>().HasData(
                new ServiceItem { Id = 1, Name = "Чистка ПК", Description = "Удаление пыли", DurationDays = 2, Price = 2000 },
                new ServiceItem { Id = 2, Name = "Диагностика", Description = "Проверка комплектующих", DurationDays = 1, Price = 1500 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
