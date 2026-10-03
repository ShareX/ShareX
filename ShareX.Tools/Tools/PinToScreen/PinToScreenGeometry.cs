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
using ShareX.HelpersLib;
using ShareX.Platform;

namespace ShareX.Tools;

/// <summary>Converts logical pin-window measurements to physical desktop positions.</summary>
internal static class PinToScreenGeometry
{
    public static ScreenInfo? FindScreen(IReadOnlyList<ScreenInfo> screens, PlatformPoint? point)
    {
        if (point is PlatformPoint position)
        {
            ScreenInfo? containing = screens.FirstOrDefault(screen =>
                position.X >= screen.Bounds.X && position.X < screen.Bounds.Right &&
                position.Y >= screen.Bounds.Y && position.Y < screen.Bounds.Bottom);
            if (containing != null) return containing;
            return screens.OrderBy(screen => DistanceToScreen(position, screen.Bounds))
                .ThenByDescending(screen => screen.IsPrimary).FirstOrDefault();
        }
        return screens.FirstOrDefault(screen => screen.IsPrimary) ?? screens.FirstOrDefault();
    }

    public static PixelPoint GetInitialPosition(ScreenInfo? screen, Size logicalSize, System.Drawing.Point? imageOrigin,
        int logicalInset, ImageContentAlignment placement, int offset)
    {
        double scaling = NormalizeScaling(screen?.ScaleFactor ?? 1);
        if (imageOrigin is System.Drawing.Point origin)
        {
            int inset = (int)Math.Round(logicalInset * scaling);
            return new PixelPoint(origin.X - inset, origin.Y - inset);
        }
        if (screen == null) return default;

        System.Drawing.Size size = new(
            Math.Max(1, (int)Math.Ceiling(logicalSize.Width * scaling)),
            Math.Max(1, (int)Math.Ceiling(logicalSize.Height * scaling)));
        PlatformRectangle area = screen.WorkingArea;
        System.Drawing.Point point = Helpers.GetPosition(placement, offset,
            new System.Drawing.Rectangle(area.X, area.Y, area.Width, area.Height), size);
        return new PixelPoint(point.X, point.Y);
    }

    public static PixelPoint KeepCenter(PixelPoint position, Size previousSize, Size newSize, double renderScaling)
    {
        double scaling = NormalizeScaling(renderScaling);
        return new PixelPoint(
            position.X + (int)Math.Round((previousSize.Width - newSize.Width) * scaling / 2),
            position.Y + (int)Math.Round((previousSize.Height - newSize.Height) * scaling / 2));
    }

    private static double NormalizeScaling(double scaling) => double.IsFinite(scaling) && scaling > 0 ? scaling : 1;

    private static double DistanceToScreen(PlatformPoint point, PlatformRectangle bounds)
    {
        double dx = Math.Max((double)bounds.X - point.X, Math.Max((double)point.X - (bounds.Right - 1), 0));
        double dy = Math.Max((double)bounds.Y - point.Y, Math.Max((double)point.Y - (bounds.Bottom - 1), 0));
        return dx * dx + dy * dy;
    }
}
