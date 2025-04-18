using System.Globalization;

namespace Core.Converters
{
    class BooleanToTextConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var status = string.Empty;
            if (value is bool isExpanded)
            {
                status = isExpanded ? "Collapse" : "Expand";
            }
            return status;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
