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

using Avalonia.Threading;
using ShareX.AvaloniaUI.Integration;
using ShareX.HelpersLib;
using ShareX.ImageEditor.Integration;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ShareX.ScreenCaptureLib.Presentation.RegionCapture;

/// <summary>UI-thread boundary for the Avalonia region capture experience.</summary>
public static class RegionCaptureIntegration
{
    /// <summary>Bounds of the last captured region, or of all regions when several were captured.</summary>
    public static Rectangle LastRegionRectangle { get; private set; }

    /// <summary>Screen rectangles of the last captured regions. Holds more than one item after a multi region capture.</summary>
    public static IReadOnlyList<Rectangle> LastRegionRectangles { get; private set; } = [];

    internal static void SetLastRegion(Rectangle bounds, IReadOnlyList<Rectangle> regions)
    {
        LastRegionRectangle = bounds;
        LastRegionRectangles = regions;
    }

    public static Task<AvaloniaRegionCaptureResult?> CaptureAsync(AvaloniaRegionCaptureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            AvaloniaBootstrapper.EnsureInitialized();
        }
        catch (Exception ex)
        {
            request.Screenshot.Dispose();
            request.CursorBitmap?.Dispose();
            return Task.FromException<AvaloniaRegionCaptureResult?>(ex);
        }

        TaskCompletionSource<AvaloniaRegionCaptureResult?> completion =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            RegionCaptureWindow? window = null;

            try
            {
                ImageEditorIntegration.Initialize();
                window = new RegionCaptureWindow(request);
                completion.TrySetResult(await window.CaptureAsync());
            }
            catch (Exception ex)
            {
                if (window == null || !window.IsVisible)
                {
                    request.Screenshot.Dispose();
                    request.CursorBitmap?.Dispose();
                }

                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }

    /// <summary>
    /// Builds a region capture window once without showing it, so the first capture after startup does not pay
    /// for JIT, XAML loading, styles, templates, icons and font enumeration. Must run on the UI thread.
    /// Uses default options so the user's settings are never touched.
    /// </summary>
    public static void WarmUp()
    {
        RegionCaptureWindow? window = null;

        try
        {
            AvaloniaBootstrapper.EnsureInitialized();
            ImageEditorIntegration.Initialize();

            const int size = 64;
            SKBitmap screenshot = new SKBitmap(size, size);

            try
            {
                window = new RegionCaptureWindow(new AvaloniaRegionCaptureRequest
                {
                    Screenshot = screenshot,
                    ScreenBounds = new Rectangle(0, 0, size, size),
                    RegionCaptureOptions = new RegionCaptureOptions(),
                    ImageEditorOptions = new ImageEditorOptions()
                });
            }
            catch
            {
                screenshot.Dispose();
                throw;
            }

            window.WarmUpHidden();
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e, "Region capture warm-up failed.");
        }
        finally
        {
            try
            {
                // Closing a window that was never shown releases its native handle and runs OnClosed,
                // which disposes the workspace, the screenshot and the view model.
                window?.Close();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, "Region capture warm-up cleanup failed.");
            }
        }
    }

    /// <summary>
    /// Lets the user draw a rectangle over the live desktop and returns it in screen pixels, or null when cancelled.
    /// The selection window is closed when the task completes, so the caller can capture the screen right after.
    /// </summary>
    public static Task<Rectangle?> SelectTransparentRegionAsync(Rectangle screenBounds, bool lockCursorToWindow)
    {
        AvaloniaBootstrapper.EnsureInitialized();

        TaskCompletionSource<Rectangle?> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                RegionCaptureTransparentWindow window = new(screenBounds, lockCursorToWindow);
                Rectangle? selection = await window.SelectAsync();

                if (selection is Rectangle rectangle)
                {
                    SetLastRegion(rectangle, [rectangle]);
                }

                completion.TrySetResult(selection);
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        });

        return completion.Task;
    }
}
