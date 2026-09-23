#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

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
