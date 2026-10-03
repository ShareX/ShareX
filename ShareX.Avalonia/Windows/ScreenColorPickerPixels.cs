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
using Avalonia.Media;
using ShareX.Platform;
using ShareX.Platform.Imaging;

namespace ShareX.AvaloniaUI.Windows;

internal static class ScreenColorPickerPixels
{
    internal static PlatformPoint MapPosition(PixelPoint point, PlatformRectangle desktop, PlatformRectangle samples, bool isLive)
    {
        if (isLive || desktop.IsEmpty || samples.IsEmpty) return new PlatformPoint(point.X, point.Y);
        return new PlatformPoint(
            samples.X + (int)Math.Floor((point.X - desktop.X + 0.5) * samples.Width / desktop.Width),
            samples.Y + (int)Math.Floor((point.Y - desktop.Y + 0.5) * samples.Height / desktop.Height));
    }

    internal static bool TryGetCenterColor(PixelBuffer? pixels, out Color color)
    {
        color = default;
        if (pixels == null) return false;
        int offset = ((pixels.Height / 2) * pixels.Width + pixels.Width / 2) * 4;
        if (pixels.Pixels[offset + 3] == 0) return false;
        color = Color.FromArgb(pixels.Pixels[offset + 3], pixels.Pixels[offset + 2], pixels.Pixels[offset + 1], pixels.Pixels[offset]);
        return true;
    }

    internal static byte[] Premultiply(PixelBuffer pixels)
    {
        byte[] result = new byte[checked(pixels.Width * pixels.Height * 4)];
        for (int i = 0; i < result.Length; i += 4)
        {
            int alpha = pixels.Pixels[i + 3];
            result[i] = (byte)((pixels.Pixels[i] * alpha + 127) / 255);
            result[i + 1] = (byte)((pixels.Pixels[i + 1] * alpha + 127) / 255);
            result[i + 2] = (byte)((pixels.Pixels[i + 2] * alpha + 127) / 255);
            result[i + 3] = (byte)alpha;
        }
        return result;
    }
}
