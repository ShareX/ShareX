using Avalonia.Data.Converters;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ShareX.Tools;

public sealed partial class MonitorTestEnumDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? string.Empty : WordBoundaryRegex().Replace(value.ToString() ?? string.Empty, " ");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    [GeneratedRegex("(?<=[a-z])(?=[A-Z])")]
    private static partial Regex WordBoundaryRegex();
}
