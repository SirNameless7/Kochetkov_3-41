using System;
using System.Globalization;
using KPO_Cursovoy.Models;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Converters
{
    public class OrderStatusToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.New => Colors.Blue,
                    OrderStatus.WaitingPayment => Colors.Orange,
                    OrderStatus.Paid => Colors.Green,
                    OrderStatus.Processing => Colors.Purple,
                    OrderStatus.Completed => Colors.DarkGreen,
                    OrderStatus.Cancelled => Colors.Red,
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}