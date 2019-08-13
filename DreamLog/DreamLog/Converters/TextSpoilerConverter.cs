using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;
using System.Text;
using System;

namespace DreamLog.Converters
{
    public class TextSpoilerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string str && !(parameter is null) && Int32.TryParse(parameter.ToString(), out int length))
            {
                return str.Length > length ? $"{str.Substring(0, length)}..." : str;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
