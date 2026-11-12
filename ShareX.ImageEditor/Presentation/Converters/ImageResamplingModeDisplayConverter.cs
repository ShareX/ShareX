#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

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
using ShareX.AvaloniaUI.Imaging;
using ShareX.ImageEditor.Localization;
using System.Globalization;

namespace ShareX.ImageEditor.Presentation.Converters;

public sealed class ImageResamplingModeDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        ImageResamplingMode mode = value is ImageResamplingMode interpolationMode
            ? interpolationMode
            : ImageResamplingMode.CatmullRom;

        return mode switch
        {
            ImageResamplingMode.Mitchell => Strings.ImageResamplingMode_Mitchell,
            ImageResamplingMode.LinearWithMipmaps => Strings.ImageResamplingMode_LinearWithMipmaps,
            ImageResamplingMode.Linear => Strings.ImageResamplingMode_Linear,
            ImageResamplingMode.NearestNeighbor => Strings.ImageResamplingMode_NearestNeighbor,
            _ => Strings.ImageResamplingMode_CatmullRom
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is ImageResamplingMode mode ? mode : ImageResamplingMode.CatmullRom;
}
