using System.Globalization;
using System.Windows.Data;

namespace FruVa.Ordering.Ui.Converters
{
    public class InvertBooleanConverter : IValueConverter
    {
        public InvertBooleanConverter()
        {
            
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanValue)
            {
                return false;
            }

            return ! booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanValue)
            {
                return false;
            }

            return ! booleanValue;
        }
    }
}
