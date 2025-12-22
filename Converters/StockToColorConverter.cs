using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Converters
{
    public class StockToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int stock)
            {
                if (stock < 5)
                    return Colors.Red;
                if (stock < 10)
                    return Colors.Orange;
                return Colors.Green;
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}