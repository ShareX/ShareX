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
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;

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
            return result.Image.Copy();
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
            return (result.Image.Copy(), result.ScreenRectangle);
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

    /// <summary>
    /// Returns a copy of <paramref name="source"/> that keeps only the pixels inside <paramref name="regions"/>
    /// and makes everything else transparent. Regions are in <paramref name="source"/> pixel coordinates.
    /// </summary>
    public static Bitmap ApplyRegionMask(Bitmap source, IReadOnlyList<Rectangle> regions)
    {
        Bitmap result = new Bitmap(new SKImageInfo(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul));
        result.Erase(SKColors.Transparent);

        using SKCanvas canvas = new SKCanvas(result);
        using SKPath path = new SKPath { FillType = SKPathFillType.Winding };

        foreach (Rectangle region in regions)
        {
            path.AddRect(SKRect.Create(region.X, region.Y, region.Width, region.Height));
        }

        canvas.ClipPath(path, SKClipOperation.Intersect, false);
        canvas.DrawBitmap(source, 0, 0);
        return result;
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
            frozenScreenshot = canvas.Copy();
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
