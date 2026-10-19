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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using System;
using System.Threading;
using System.Threading.Tasks;
using DrawingBitmap = SkiaSharp.SKBitmap;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ScreenCaptureLib;

/// <summary>
/// Captures a region while the user scrolls its content by hand, stitching each frame onto the image. The toolbar is
/// placed outside the region so it never appears in the capture.
/// </summary>
public partial class ManualScrollingCaptureWindow : Window
{
    private const int FrameInterval = 100;
    // The region selection overlay fades out after the selection, so the first frame waits for the screen to settle.
    private const int SettleDelay = 300;
    private const int SettleAttempts = 10;
    // If even the settled first frame cannot be continued, the capture starts over from the current frame.
    private const int RestartAfterNoOverlapFrames = 3;
    private const int ToolbarGap = 8;
    // Room for the region outline, which is drawn just outside the region.
    private const int OutlineMargin = 2;

    private readonly DrawingRectangle _region;
    private ManualScrollingCaptureStitcher _stitcher = new();
    private readonly CancellationTokenSource _stop = new();
    private readonly TaskCompletionSource<DrawingBitmap?> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private ScrollingCaptureRegionWindow? _regionWindow;
    private Task? _captureLoop;
    private bool _finishing;

    /// <summary>The stitched image after Finish, or null when the capture is canceled.</summary>
    public Task<DrawingBitmap?> Completion => _completion.Task;

    public ManualScrollingCaptureWindow() : this(new DrawingRectangle(0, 0, 640, 420))
    {
    }

    public ManualScrollingCaptureWindow(DrawingRectangle region)
    {
        _region = region;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();

        FinishButton.Click += (_, _) => _ = FinishAsync();
        CancelButton.Click += (_, _) => _ = CancelAsync();
        Opened += OnOpened;
        Closed += OnClosed;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        PlaceOutsideRegion();

        _regionWindow = new ScrollingCaptureRegionWindow(_region) { Title = Title };
        _regionWindow.Show();

        _captureLoop = Task.Run(() => CaptureLoopAsync(_stop.Token));
    }

    /// <summary>Puts the toolbar below the region, or above it, or beside it, whichever fits on the region's screen.</summary>
    private void PlaceOutsideRegion()
    {
        Screen? screen = Screens.ScreenFromBounds(new PixelRect(_region.X, _region.Y, _region.Width, _region.Height)) ?? Screens.Primary;
        double scaling = screen?.Scaling ?? 1;
        PixelRect bounds = screen?.Bounds ?? new PixelRect(_region.X, _region.Y, _region.Width, _region.Height);
        int width = (int)Math.Ceiling(Width * scaling);
        int height = (int)Math.Ceiling(Height * scaling);
        int border = OutlineMargin;
        int centeredX = Math.Clamp(_region.X + (_region.Width - width) / 2, bounds.X, bounds.Right - width);

        PixelPoint position;
        if (_region.Bottom + border + ToolbarGap + height <= bounds.Bottom)
        {
            position = new(centeredX, _region.Bottom + border + ToolbarGap);
        }
        else if (_region.Top - border - ToolbarGap - height >= bounds.Y)
        {
            position = new(centeredX, _region.Top - border - ToolbarGap - height);
        }
        else if (_region.Right + border + ToolbarGap + width <= bounds.Right)
        {
            position = new(_region.Right + border + ToolbarGap, _region.Bottom - height);
        }
        else if (_region.X - border - ToolbarGap - width >= bounds.X)
        {
            position = new(_region.X - border - ToolbarGap - width, _region.Bottom - height);
        }
        else
        {
            // The region fills the screen; the toolbar has to cover its bottom corner.
            position = new(bounds.Right - width - ToolbarGap, bounds.Bottom - height - ToolbarGap);
        }

        Position = position;
    }

    private async Task CaptureLoopAsync(CancellationToken token)
    {
        Screenshot screenshot = new() { CaptureCursor = false };
        DrawingBitmap? previousFrame = null;
        int frameCount = 0, noOverlapFrames = 0;

        try
        {
            previousFrame = await CaptureSettledFrameAsync(screenshot, token);
            if (previousFrame != null)
            {
                _stitcher.AddFrame(previousFrame);
                frameCount = 1;
                int height = _stitcher.Height;
                Dispatcher.UIThread.Post(() => ShowStatus(ManualScrollingFrameResult.Appended, height));
            }

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(FrameInterval, token);

                DrawingBitmap? frame = screenshot.CaptureRectangle(_region);

                if (frame != null)
                {
                    // An unchanged frame means the user is not scrolling; there is nothing to stitch.
                    if (previousFrame != null && SkiaImageHelpers.CompareImages(frame, previousFrame))
                    {
                        frame.Dispose();
                    }
                    else
                    {
                        ManualScrollingFrameResult result = _stitcher.AddFrame(frame);
                        noOverlapFrames = result == ManualScrollingFrameResult.NoOverlap ? noOverlapFrames + 1 : 0;
                        if (result == ManualScrollingFrameResult.Appended) frameCount++;

                        if (frameCount == 1 && noOverlapFrames >= RestartAfterNoOverlapFrames)
                        {
                            // Nothing was stitched yet, so the first frame was probably taken before the screen settled.
                            _stitcher.Dispose();
                            _stitcher = new ManualScrollingCaptureStitcher();
                            result = _stitcher.AddFrame(frame);
                            noOverlapFrames = 0;
                        }

                        previousFrame?.Dispose();
                        previousFrame = frame;
                        int height = _stitcher.Height;
                        Dispatcher.UIThread.Post(() => ShowStatus(result, height));

                        if (height >= ManualScrollingCaptureStitcher.MaxHeight)
                        {
                            Dispatcher.UIThread.Post(() => _ = FinishAsync());
                            return;
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            DebugHelper.WriteException(e, "Manual scrolling capture failed.");
        }
        finally
        {
            previousFrame?.Dispose();
        }
    }

    /// <summary>Returns the first frame that matches the frame before it, or the last one tried if the screen keeps changing.</summary>
    private async Task<DrawingBitmap?> CaptureSettledFrameAsync(Screenshot screenshot, CancellationToken token)
    {
        await Task.Delay(SettleDelay, token);
        DrawingBitmap? frame = screenshot.CaptureRectangle(_region);

        for (int i = 0; i < SettleAttempts && frame != null; i++)
        {
            await Task.Delay(FrameInterval, token);
            DrawingBitmap? next = screenshot.CaptureRectangle(_region);
            if (next == null) break;

            bool settled = SkiaImageHelpers.CompareImages(frame, next);
            frame.Dispose();
            frame = next;
            if (settled) break;
        }

        return frame;
    }

    private void ShowStatus(ManualScrollingFrameResult result, int height)
    {
        if (_finishing) return;

        if (height >= ManualScrollingCaptureStitcher.MaxHeight)
        {
            StatusIndicator.Background = Brushes.Goldenrod;
            StatusText.Text = Localization.Strings.ManualScrollingCaptureWindow_MaxHeight;
        }
        else if (result == ManualScrollingFrameResult.NoOverlap)
        {
            StatusIndicator.Background = Brushes.Goldenrod;
            StatusText.Text = Localization.Strings.ManualScrollingCaptureWindow_LostTrack;
        }
        else
        {
            StatusIndicator.Background = Brushes.LimeGreen;
            StatusText.Text = string.Format(Localization.Strings.ManualScrollingCaptureWindow_Captured, height);
        }
    }

    public async Task FinishAsync()
    {
        if (_finishing) return;
        _finishing = true;
        await StopCaptureAsync();
        _completion.TrySetResult(_stitcher.TakeResult());
        Close();
    }

    public async Task CancelAsync()
    {
        if (_finishing) return;
        _finishing = true;
        await StopCaptureAsync();
        _completion.TrySetResult(null);
        Close();
    }

    private async Task StopCaptureAsync()
    {
        _stop.Cancel();
        if (_captureLoop != null) await _captureLoop;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _stop.Cancel();
        _regionWindow?.Close();
        _regionWindow = null;
        // Closing the window any other way discards the capture.
        _completion.TrySetResult(null);
        _stitcher.Dispose();
    }
}
