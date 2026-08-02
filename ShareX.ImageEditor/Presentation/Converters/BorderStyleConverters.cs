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
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Localization;
using ShareX.ImageEditor.Presentation.Helpers;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Converters
{
    public class BorderStyleDisplayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            BorderStyle borderStyle = value is BorderStyle style ? style : BorderStyle.Solid;

            return borderStyle switch
            {
                BorderStyle.Solid => Strings.BorderStyleDisplayConverter_Solid,
                BorderStyle.Dash => Strings.BorderStyleDisplayConverter_Dash,
                BorderStyle.Dot => Strings.BorderStyleDisplayConverter_Dot,
                BorderStyle.DashDot => Strings.BorderStyleDisplayConverter_DashDot,
                BorderStyle.DashDotDot => Strings.BorderStyleDisplayConverter_DashDotDot,
                _ => Strings.BorderStyleDisplayConverter_Solid
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BorderStyle.Solid;
        }
    }

    public class BorderStylePreviewDashArrayConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            BorderStyle borderStyle = value is BorderStyle style ? style : BorderStyle.Solid;
            return BorderStyleDashHelper.CreateStrokeDashArray(borderStyle);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return BorderStyle.Solid;
        }
    }
}
