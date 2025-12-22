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
        public DbSet<CompatibilityRule> CompatibilityRules { get; set; }

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
            modelBuilder.Ignore<ComponentSpecification>();
            modelBuilder.Ignore<OrderComponent>();
            modelBuilder.Ignore<OrderService>();
            modelBuilder.Ignore<DeliveryItem>();
            modelBuilder.Ignore<Specification>();
            modelBuilder.Ignore<SpecificationValue>();

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
                new ComponentCategory { CategoryCode = "SSD", CategoryName = "Накопитель SSD" }
            );

            modelBuilder.Entity<ComponentItem>().HasData(
                new ComponentItem { Id = 1, Name = "Intel Core i5-11400", CategoryCode = "CPU", Price = 15000, Stock = 10 },
                new ComponentItem { Id = 2, Name = "AMD Ryzen 5 5600G", CategoryCode = "CPU", Price = 16000, Stock = 5 },
                new ComponentItem { Id = 3, Name = "Gigabyte B560M", CategoryCode = "MB", Price = 8000, Stock = 7 },
                new ComponentItem { Id = 4, Name = "NVIDIA RTX 3060", CategoryCode = "GPU", Price = 25000, Stock = 3 },
                new ComponentItem { Id = 5, Name = "Corsair 16GB DDR4", CategoryCode = "RAM", Price = 7000, Stock = 20 },
                new ComponentItem { Id = 6, Name = "SSD Samsung 500GB", CategoryCode = "SSD", Price = 5000, Stock = 15 }
            );

            modelBuilder.Entity<PcItem>().HasData(
                new PcItem { Id = 1, Name = "Игровой ПК Start", Description = "Базовый игровой компьютер", Price = 60000 },
                new PcItem { Id = 2, Name = "Офисный ПК Lite", Description = "Компьютер для работы", Price = 35000 }
            );

            modelBuilder.Entity<ServiceItem>().HasData(
                new ServiceItem { Id = 1, Name = "Чистка ПК", Description = "Удаление пыли", DurationDays = 2, Price = 2000 },
                new ServiceItem { Id = 2, Name = "Диагностика", Description = "Проверка комплектующих", DurationDays = 1, Price = 1500 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
