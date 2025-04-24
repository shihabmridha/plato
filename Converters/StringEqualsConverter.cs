using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace plato.Converters
{
    public class StringEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && parameter is string expectedValue)
            {
                return string.Equals(stringValue, expectedValue, StringComparison.OrdinalIgnoreCase);
            }
            
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 