using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace DreamLog.Converters
{
    public class CreatedAtConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime date)
            {
                if (date == DateTime.MinValue)
                    return "";

                return $"Created at: {date:dd.MM.yyyy HH:mm:ss}";
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string str && str.StartsWith("Created at: "))
            {
                return DateTime.Parse(str.Substring("Created at: ".Length));
            }
            return value;
        }
    }
}
