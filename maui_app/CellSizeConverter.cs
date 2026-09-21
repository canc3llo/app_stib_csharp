using System.Globalization;

namespace maui_app;

public class CellSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not double containerWidth || containerWidth <= 0)
            return 0d;

        var parts = ((string)parameter).Split(':');
        var span = double.Parse(parts[0], CultureInfo.InvariantCulture);
        var spacing = double.Parse(parts[1], CultureInfo.InvariantCulture);

        return (containerWidth - spacing * (span - 1)) / span;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
