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

#nullable enable

using ShareX.HelpersLib;
using ShareX.ImageEditor.Integration;
using ShareX.ScreenCaptureLib.Presentation.RegionCapture;
using SkiaSharp;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib;

public static class RegionCaptureTasks
{
    public static async Task<Bitmap?> GetRegionImageAsync(RegionCaptureOptions? options = null)
    {
        AvaloniaRegionCaptureResult? result = await CaptureAsync(options);
        if (result == null)
        {
            return null;
        }

        using (result.Image)
        {
            return GdiSkiaBitmapConverter.ToGdiBitmap(result.Image);
        }
    }

    public static async Task<(Bitmap Image, Rectangle Rectangle)?> GetRegionImageWithRectangleAsync(
        RegionCaptureOptions? options = null)
    {
        AvaloniaRegionCaptureResult? result = await CaptureAsync(options);
        if (result == null)
        {
            return null;
        }

        using (result.Image)
        {
            return (GdiSkiaBitmapConverter.ToGdiBitmap(result.Image), result.ScreenRectangle);
        }
    }

    public static async Task<(Rectangle Rectangle, WindowInfo? WindowInfo)?> GetRectangleRegionAsync(
        RegionCaptureOptions? options = null)
    {
        AvaloniaRegionCaptureResult? result = await CaptureAsync(options);
        if (result == null)
        {
            return null;
        }

        result.Image.Dispose();
        return (result.ScreenRectangle, result.WindowInfo);
    }

    private static async Task<AvaloniaRegionCaptureResult?> CaptureAsync(RegionCaptureOptions? options)
    {
        options ??= new RegionCaptureOptions();

        Screenshot screenshot = new Screenshot
        {
            CaptureCursor = false
        };

        Rectangle screenBounds = options.ActiveMonitorMode
            ? CaptureHelpers.GetActiveScreenBounds()
            : CaptureHelpers.GetScreenBounds();

        SKBitmap frozenScreenshot;
        using (Bitmap canvas = options.ActiveMonitorMode
            ? screenshot.CaptureActiveMonitor()
            : screenshot.CaptureFullscreen())
        {
            frozenScreenshot = GdiSkiaBitmapConverter.ToSKBitmap(canvas);
        }

        AvaloniaRegionCaptureRequest request = new AvaloniaRegionCaptureRequest
        {
            Screenshot = frozenScreenshot,
            ScreenBounds = screenBounds,
            RegionCaptureOptions = options,
            ImageEditorOptions = new ImageEditorOptions(),
            EnableAnnotations = false
        };

        return await RegionCaptureIntegration.CaptureAsync(request);
    }
}
