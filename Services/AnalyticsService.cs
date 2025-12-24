using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.Services
{
    public class AnalyticsService
    {
        //private readonly DatabaseService _databaseService;

        //public AnalyticsService(DatabaseService databaseService)
        //{
        //    _databaseService = databaseService;
        //}

        //public async Task<SalesReport> GetSalesReportAsync(DateTime startDate, DateTime endDate)
        //{
        //    return await _databaseService.GetSalesReportAsync(startDate, endDate);
        //}

        //public async Task<PopularComponentsReport> GetPopularComponentsReportAsync(DateTime startDate, DateTime endDate)
        //{
        //    return await _databaseService.GetPopularComponentsReportAsync(startDate, endDate);
        //}

        //public async Task<UserActivityReport> GetUserActivityReportAsync(DateTime startDate, DateTime endDate)
        //{
        //    return await _databaseService.GetUserActivityReportAsync(startDate, endDate);
        //}
    }

    public class SalesReport
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalSales { get; set; }
        public int OrderCount { get; set; }
        public Dictionary<string, decimal> SalesByCategory { get; set; } = new();
    }

    public class PopularComponentsReport
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public List<ComponentSalesData> TopComponents { get; set; } = new();
    }

    public class ComponentSalesData
    {
        public string ComponentName { get; set; }
        public int UnitsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class UserActivityReport
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public Dictionary<string, int> NewUsersByDay { get; set; } = new();
    }
}