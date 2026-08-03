using Avalonia.Data.Converters;
using System.Globalization;

namespace ShareX.Tools;

public sealed class LocalizedDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            AIProvider.OpenAI => Localization.Strings.LocalizedDisplayConverter_OpenAI,
            AIProvider.Gemini => Localization.Strings.LocalizedDisplayConverter_Gemini,
            AIProvider.OpenRouter => Localization.Strings.LocalizedDisplayConverter_OpenRouter,
            AIProvider.OpenAILegacy => Localization.Strings.LocalizedDisplayConverter_OpenAI_legacy,
            BackgroundRemovalDevice.Auto => Localization.Strings.LocalizedDisplayConverter_Automatic,
            BackgroundRemovalDevice.GPU => Localization.Strings.LocalizedDisplayConverter_GPU,
            BackgroundRemovalDevice.CPU => Localization.Strings.LocalizedDisplayConverter_CPU,
            MonitorGradientDirection.Horizontal => Localization.Strings.LocalizedDisplayConverter_Horizontal,
            MonitorGradientDirection.Vertical => Localization.Strings.LocalizedDisplayConverter_Vertical,
            MonitorGradientDirection.ForwardDiagonal => Localization.Strings.LocalizedDisplayConverter_Forward_diagonal,
            MonitorGradientDirection.BackwardDiagonal => Localization.Strings.LocalizedDisplayConverter_Backward_diagonal,
            MonitorPattern.HorizontalLines => Localization.Strings.LocalizedDisplayConverter_Horizontal_lines,
            MonitorPattern.VerticalLines => Localization.Strings.LocalizedDisplayConverter_Vertical_lines,
            MonitorPattern.Checkerboard => Localization.Strings.LocalizedDisplayConverter_Checkerboard,
            MonitorMotionDirection.VerticalBars => Localization.Strings.LocalizedDisplayConverter_Vertical_bars,
            MonitorMotionDirection.HorizontalBars => Localization.Strings.LocalizedDisplayConverter_Horizontal_bars,
            "minimal" => Localization.Strings.LocalizedDisplayConverter_Minimal,
            "low" => Localization.Strings.LocalizedDisplayConverter_Low,
            "medium" => Localization.Strings.LocalizedDisplayConverter_Medium,
            "high" => Localization.Strings.LocalizedDisplayConverter_High,
            null => string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
