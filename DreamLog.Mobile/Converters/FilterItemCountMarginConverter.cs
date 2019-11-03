using System.Collections.Generic;
using System.Globalization;
using DreamLog.ViewModels;
using Xamarin.Forms;
using System.Linq;
using System;

namespace DreamLog.Converters
{
    public class FilterItemCountMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<FilterOptionViewModel> models)
            {
                return new Thickness(25, 50, 25, Math.Max(Application.Current.MainPage.Height - (models.Count() * 50 + 210), 50));
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
