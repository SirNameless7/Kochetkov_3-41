using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace KPO_Cursovoy.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status.ToLower() switch
                {
                    "совместим" => Colors.Green,
                    "несовместим" => Colors.Red,
                    "предупреждение" => Colors.Orange,
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