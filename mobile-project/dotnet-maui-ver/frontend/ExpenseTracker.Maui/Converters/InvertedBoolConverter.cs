using System.Globalization;

namespace ExpenseTracker.Maui.Converters;

// Inverts a boolean value for conditional XAML visibility and enablement
public class InvertedBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolVal)
        {
            return !boolVal;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolVal)
        {
            return !boolVal;
        }

        return false;
    }
}
