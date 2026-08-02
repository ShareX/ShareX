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

using Avalonia;
using Avalonia.Data.Converters;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Localization;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Converters
{
    public class ArrowStyleDisplayNameConverter : IValueConverter
    {
        public static readonly ArrowStyleDisplayNameConverter Instance = new();

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            ArrowStyle style = value is ArrowStyle arrowStyle ? arrowStyle : ArrowStyle.Classic;
            return style switch
            {
                ArrowStyle.Classic => Strings.ArrowStyleDisplayNameConverter_Classic,
                ArrowStyle.Modern => Strings.ArrowStyleDisplayNameConverter_Modern,
                ArrowStyle.Double => Strings.ArrowStyleDisplayNameConverter_Double,
                ArrowStyle.Basic => Strings.ArrowStyleDisplayNameConverter_Basic,
                ArrowStyle.Line => Strings.ArrowStyleDisplayNameConverter_Line,
                _ => Strings.ArrowStyleDisplayNameConverter_Classic
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => ArrowStyle.Classic;
    }

    public class ArrowStylePreviewGeometryConverter : IValueConverter
    {
        public static readonly ArrowStylePreviewGeometryConverter Instance = new();

        private const double PreviewWidth = 36;
        private const double PreviewHeight = 14;
        private const double PreviewPadding = 2;
        private const double PreviewStrokeWidth = 1.75;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            ArrowStyle style = value is ArrowStyle arrowStyle ? arrowStyle : ArrowStyle.Classic;

            var preview = new ArrowAnnotation
            {
                Style = style,
                StrokeWidth = (float)PreviewStrokeWidth
            };

            double headSize = PreviewStrokeWidth * ArrowAnnotation.GetArrowHeadWidthMultiplier(style);

            return preview.CreateArrowGeometry(
                new Point(PreviewPadding, PreviewHeight * 0.5),
                new Point(PreviewWidth - PreviewPadding, PreviewHeight * 0.5),
                headSize);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return ArrowStyle.Classic;
        }
    }
}
