// Services/LoyaltyService.cs
using System;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class LoyaltyService
    {
        private readonly DatabaseService _databaseService;

        public LoyaltyService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        //public async Task<decimal> CalculateDiscountAsync(int userId)
        //{
        //    var user = await _databaseService.GetUserByIdAsync(userId);
        //    if (user == null)
        //        return 0;

        //    return user.LoyaltyStatus switch
        //    {
        //        "постоянный" => 0.10m, // 10% скидка
        //        "премиум" => 0.15m,   // 15% скидка
        //        _ => 0m
        //    };
        //}

        public async Task UpdateLoyaltyStatusAsync(int userId, decimal totalSpent)
        {
            // Логика обновления статуса лояльности
        }
    }
}