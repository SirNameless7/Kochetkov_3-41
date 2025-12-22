using System.Collections.ObjectModel;
using System.Windows.Input;
using KPO_Cursovoy.Models;

namespace KPO_Cursovoy.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        public ObservableCollection<string> ReportPeriods { get; } = new()
        {
            "Сегодня",
            "Вчера",
            "Неделя",
            "Месяц",
            "Квартал",
            "Год"
        };

        private string _selectedPeriod = "Неделя";
        public string SelectedPeriod
        {
            get => _selectedPeriod;
            set => SetProperty(ref _selectedPeriod, value);
        }

        private string _maxSalesDay = "Пятница";
        public string MaxSalesDay
        {
            get => _maxSalesDay;
            set => SetProperty(ref _maxSalesDay, value);
        }

        private decimal _maxSalesAmount = 85000;
        public decimal MaxSalesAmount
        {
            get => _maxSalesAmount;
            set => SetProperty(ref _maxSalesAmount, value);
        }

        private string _minSalesDay = "Воскресенье";
        public string MinSalesDay
        {
            get => _minSalesDay;
            set => SetProperty(ref _minSalesDay, value);
        }

        private decimal _minSalesAmount = 23000;
        public decimal MinSalesAmount
        {
            get => _minSalesAmount;
            set => SetProperty(ref _minSalesAmount, value);
        }

        public ObservableCollection<ComponentReportItem> PopularComponents { get; } = new();

        private int _totalUsers = 152;
        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        private int _activeUsersToday = 28;
        public int ActiveUsersToday
        {
            get => _activeUsersToday;
            set => SetProperty(ref _activeUsersToday, value);
        }
        public ICommand ExportToExcelCommand { get; }
        public ICommand ExportToPdfCommand { get; }

        public ReportsViewModel()
        {
            ExportToExcelCommand = new Command(ExportToExcel);
            ExportToPdfCommand = new Command(ExportToPdf);

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            PopularComponents.Add(new ComponentReportItem
            {
                Name = "NVIDIA RTX 3060",
                SalesPercentage = 0.85
            });

            PopularComponents.Add(new ComponentReportItem
            {
                Name = "Intel Core i5-11400",
                SalesPercentage = 0.78
            });

            PopularComponents.Add(new ComponentReportItem
            {
                Name = "Corsair 16GB DDR4",
                SalesPercentage = 0.72
            });

            PopularComponents.Add(new ComponentReportItem
            {
                Name = "SSD Samsung 500GB",
                SalesPercentage = 0.65
            });
        }

        public void Initialize()
        {
        }

        private void ExportToExcel()
        {
        }

        private void ExportToPdf()
        {
        }
    }

    public class ComponentReportItem
    {
        public string Name { get; set; } = string.Empty;
        public double SalesPercentage { get; set; }
    }
}
