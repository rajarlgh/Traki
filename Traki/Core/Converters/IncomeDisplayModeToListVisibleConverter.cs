using Core.Enum; // or wherever your enum is
using System.Globalization;

namespace Core.Converters
{
    public class IncomeDisplayModeToChartVisibleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IncomeDisplayMode mode)
                return mode == IncomeDisplayMode.Chart;
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class IncomeDisplayModeToListVisibleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IncomeDisplayMode mode)
                return mode == IncomeDisplayMode.List;
            return false;
        }

        public object ConvertBack(object?  value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
