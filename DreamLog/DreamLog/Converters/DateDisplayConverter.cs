using System.Globalization;
using Xamarin.Forms;
using System;

namespace DreamLog.Converters
{
    public class DateDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime date)
            {
                if(!(parameter is null) && parameter is string str)
                {
                    return date.ToString(CultureInfo.GetCultureInfo(str).DateTimeFormat);
                }
                return date.ToString("dd.MM.yyyy");
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
