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
using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Converters
{
    /// <summary>
    /// Converts EditorTool comparison to background color for tool button highlighting
    /// </summary>
    public class ActiveToolToBackgroundConverter : IValueConverter
    {
        public static readonly ActiveToolToBackgroundConverter Instance = new();

        // Active tool button color (violet gradient)
        private static readonly IBrush ActiveBrush = new SolidColorBrush(Color.Parse("#8B5CF6"));

        // Inactive tool button color (gray)
        private static readonly IBrush InactiveBrush = new SolidColorBrush(Color.Parse("#374151"));

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EditorTool activeTool && parameter is string toolName)
            {
                if (Enum.TryParse<EditorTool>(toolName, out var tool))
                {
                    return activeTool == tool ? ActiveBrush : InactiveBrush;
                }
            }
            return InactiveBrush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts selected color comparison to border brush for color swatch highlighting
    /// </summary>
    public class SelectedColorToBorderConverter : IValueConverter, IMultiValueConverter
    {
        public static readonly SelectedColorToBorderConverter Instance = new();

        // Selected ring color
        private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.Parse("#FFFFFF"));
        private static readonly IBrush UnselectedBrush = new SolidColorBrush(Colors.Transparent);

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string selectedColor && parameter is string swatchColor)
            {
                return string.Equals(selectedColor, swatchColor, StringComparison.OrdinalIgnoreCase)
                    ? SelectedBrush
                    : UnselectedBrush;
            }
            return UnselectedBrush;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count >= 2 && values[0] is string selectedColor && values[1] is string swatchColor)
            {
                return string.Equals(selectedColor, swatchColor, StringComparison.OrdinalIgnoreCase)
                    ? SelectedBrush
                    : UnselectedBrush;
            }

            return UnselectedBrush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object? ConvertBack(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts stroke width comparison to background for width button highlighting
    /// </summary>
    public class SelectedWidthToBackgroundConverter : IValueConverter, IMultiValueConverter
    {
        public static readonly SelectedWidthToBackgroundConverter Instance = new();

        private static readonly IBrush SelectedBrush = new SolidColorBrush(Color.Parse("#8B5CF6"));
        private static readonly IBrush UnselectedBrush = new SolidColorBrush(Color.Parse("#25263A"));

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int selectedWidth && parameter is string widthStr && int.TryParse(widthStr, out int width))
            {
                return selectedWidth == width ? SelectedBrush : UnselectedBrush;
            }
            return UnselectedBrush;
        }

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count >= 2 && values[0] is int selectedWidth)
            {
                var widthObj = values[1];
                if (widthObj is int width || (widthObj is string widthStr && int.TryParse(widthStr, out width)))
                {
                    return selectedWidth == width ? SelectedBrush : UnselectedBrush;
                }
            }

            return UnselectedBrush;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object? ConvertBack(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}