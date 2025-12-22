using System;
using System.Globalization;
using KPO_Cursovoy.Models;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Converters
{
    public class OrderStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.New => "Новый",
                    OrderStatus.WaitingPayment => "Ожидает оплаты",
                    OrderStatus.Paid => "Оплачен",
                    OrderStatus.Processing => "В обработке",
                    OrderStatus.Completed => "Завершен",
                    OrderStatus.Cancelled => "Отменен",
                    _ => "Неизвестный статус"
                };
            }
            return "Неизвестный статус";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}