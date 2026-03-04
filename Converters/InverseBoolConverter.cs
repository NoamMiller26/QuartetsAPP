using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Quartets.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        #region Public Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return true;
        }

        #endregion
    }
}
