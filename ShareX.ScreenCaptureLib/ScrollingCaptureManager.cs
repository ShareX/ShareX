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
using ShareX.Platform;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX.ScreenCaptureLib
{
    /// <summary>What scrolling capture needs from the desktop. Tests replace it; the default uses the platform services.</summary>
    internal class ScrollingCaptureHost
    {
        public virtual FeatureSupport CaptureSupport => PlatformServices.Current.ScreenCapture.Support;
        public virtual IInputService Input => PlatformServices.Current.Input;
        public virtual void ActivateWindow(long handle) => PlatformServices.Current.Windows.ActivateWindow(handle);
        public virtual Task<SKBitmap?> CaptureAsync(Rectangle rectangle) => new Screenshot { CaptureCursor = false }.CaptureRectangleAsync(rectangle);
        public virtual Task<(Rectangle Rectangle, PlatformWindow? Window)?> SelectAsync() => RegionCaptureTasks.GetRectangleRegionAsync(new RegionCaptureOptions());
        public virtual Task Delay(int milliseconds, CancellationToken cancellationToken) => Task.Delay(Math.Max(0, milliseconds), cancellationToken);

        public virtual IDisposable? ShowRegion(Rectangle rectangle)
        {
            ScrollingCaptureRegionWindow window = new ScrollingCaptureRegionWindow(rectangle);
            window.Show();
            return new CloseOnDispose(window);
        }

        private sealed class CloseOnDispose(ScrollingCaptureRegionWindow window) : IDisposable
        {
            public void Dispose() => window.Close();
        }
    }

    internal class ScrollingCaptureManager : IDisposable
    {
        public ScrollingCaptureOptions Options { get; private set; }
        public SKBitmap? Result { get; private set; }
        public bool IsCapturing { get; private set; }

        /// <summary>Why the last capture stopped early because the desktop no longer allowed it, or null.</summary>
        public string? FailureReason { get; private set; }

        private readonly ScrollingCaptureHost host;
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private bool disposed;

        private SKBitmap? lastScreenshot;
        private SKBitmap? previousScreenshot;
        private bool stopRequested;
        private ScrollingCaptureStatus status;
        private int bestMatchCount, bestMatchIndex, bestIgnoreBottomOffset;
        private PlatformWindow? selectedWindow;
        private Rectangle selectedRectangle;

        public ScrollingCaptureManager(ScrollingCaptureOptions options) : this(options, new ScrollingCaptureHost())
        {
        }

        internal ScrollingCaptureManager(ScrollingCaptureOptions options, ScrollingCaptureHost host)
        {
            Options = options;
            this.host = host;
        }

        /// <summary>Stops a running capture at its next step (delays end at once) and releases the images.</summary>
        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            stopRequested = true;
            lifetime.Cancel();

            if (!IsCapturing)
            {
                Reset();
            }
        }

        private void Reset(bool keepResult = false)
        {
            lastScreenshot?.Dispose();
            lastScreenshot = null;

            previousScreenshot?.Dispose();
            previousScreenshot = null;

            if (!keepResult)
            {
                Result?.Dispose();
                Result = null;
            }
        }

        /// <summary>The input a scroll method needs, so it is checked right before each use.</summary>
        internal static FeatureSupport GetMethodSupport(IInputService input, ScrollMethod method) => method switch
        {
            ScrollMethod.MouseWheel => input.MouseWheelSupport,
            ScrollMethod.DownArrow or ScrollMethod.PageDown => input.KeyboardSupport,
            ScrollMethod.ScrollMessage => input.WindowScrollSupport,
            _ => FeatureSupport.NotSupported("Unknown scroll method.")
        };

        /// <summary>Capture and the scroll method's input must both still work; the desktop can change while ShareX waits.</summary>
        private bool CheckPrerequisites()
        {
            FeatureSupport capture = host.CaptureSupport;
            FeatureSupport method = capture.IsSupported ? GetMethodSupport(host.Input, Options.ScrollMethod) : capture;

            if (method.IsSupported)
            {
                return true;
            }

            FailureReason = method.Reason;
            DebugHelper.WriteLine("Scrolling capture stopped: " + method.Reason);
            return false;
        }

        public async Task<ScrollingCaptureStatus> StartCapture()
        {
            if (!disposed && !IsCapturing && selectedWindow != null && !selectedRectangle.IsEmpty)
            {
                IsCapturing = true;
                stopRequested = false;
                status = ScrollingCaptureStatus.Failed;
                FailureReason = null;
                bestMatchCount = 0;
                bestMatchIndex = 0;
                bestIgnoreBottomOffset = 0;
                Reset();

                CancellationToken cancellationToken = lifetime.Token;
                IDisposable? regionWindow = Options.ShowRegion ? host.ShowRegion(selectedRectangle) : null;

                try
                {
                    host.ActivateWindow(selectedWindow.Handle);

                    await host.Delay(Options.StartDelay, cancellationToken);

                    if (!CheckPrerequisites())
                    {
                        return status;
                    }

                    if (Options.AutoScrollTop)
                    {
                        // Send each way of reaching the top that this desktop allows; Windows allows both, as before.
                        IInputService input = host.Input;

                        if (input.KeyboardSupport.IsSupported)
                        {
                            input.SendKeyPress(VirtualKeys.Home);
                        }

                        if (input.WindowScrollSupport.IsSupported)
                        {
                            input.ScrollWindow(selectedWindow.Handle, WindowScrollCommand.Top);
                        }

                        await host.Delay(Options.ScrollDelay, cancellationToken);
                    }

                    while (!stopRequested)
                    {
                        if (!CheckPrerequisites())
                        {
                            break;
                        }

                        lastScreenshot = await host.CaptureAsync(selectedRectangle);

                        if (stopRequested || CompareLastTwoImages())
                        {
                            break;
                        }

                        if (!CheckPrerequisites())
                        {
                            break;
                        }

                        IInputService input = host.Input;

                        switch (Options.ScrollMethod)
                        {
                            case ScrollMethod.MouseWheel:
                                input.SendMouseWheel(-Options.ScrollAmount);
                                break;
                            case ScrollMethod.DownArrow:
                                for (int i = 0; i < Options.ScrollAmount; i++)
                                {
                                    input.SendKeyPress(VirtualKeys.Down);
                                }
                                break;
                            case ScrollMethod.PageDown:
                                input.SendKeyPress(VirtualKeys.PageDown);
                                break;
                            case ScrollMethod.ScrollMessage:
                                for (int i = 0; i < Options.ScrollAmount; i++)
                                {
                                    input.ScrollWindow(selectedWindow.Handle, WindowScrollCommand.LineDown);
                                }
                                break;
                        }

                        Stopwatch timer = Stopwatch.StartNew();

                        if (lastScreenshot != null)
                        {
                            SKBitmap? newResult = await CombineImagesAsync(Result, lastScreenshot);

                            if (newResult != null)
                            {
                                Result?.Dispose();
                                Result = newResult;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (stopRequested)
                        {
                            break;
                        }

                        if (lastScreenshot != null)
                        {
                            previousScreenshot?.Dispose();
                            previousScreenshot = lastScreenshot;
                            lastScreenshot = null;
                        }

                        int delay = Options.ScrollDelay - (int)timer.ElapsedMilliseconds;

                        if (delay > 0)
                        {
                            await host.Delay(delay, cancellationToken);
                        }
                    }
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Disposed while waiting: stop quietly.
                }
                finally
                {
                    regionWindow?.Dispose();

                    // A disposed manager keeps nothing; otherwise the result stays for the caller.
                    Reset(!disposed);
                    IsCapturing = false;
                }
            }

            return status;
        }

        public void StopCapture()
        {
            if (IsCapturing)
            {
                stopRequested = true;
            }
        }

        public async Task<bool> SelectWindowAsync()
        {
            var selection = await host.SelectAsync();

            // The selector cannot be cancelled from here; if the capture was closed meanwhile, ignore what it returns.
            if (selection == null || disposed)
            {
                return false;
            }

            selectedRectangle = selection.Value.Rectangle;
            selectedWindow = selection.Value.Window;
            return selectedWindow != null;
        }

        private bool CompareLastTwoImages()
        {
            if (lastScreenshot != null && previousScreenshot != null &&
                lastScreenshot.Width == previousScreenshot.Width && lastScreenshot.Height == previousScreenshot.Height)
            {
                return lastScreenshot.GetPixelSpan().SequenceEqual(previousScreenshot.GetPixelSpan());
            }

            return false;
        }

        private Task<SKBitmap?> CombineImagesAsync(SKBitmap? result, SKBitmap currentImage)
        {
            return Task.Run(() => CombineImages(result, currentImage));
        }

        private SKBitmap? CombineImages(SKBitmap? result, SKBitmap currentImage)
        {
            if (result == null)
            {
                status = ScrollingCaptureStatus.Successful;

                return currentImage.Copy();
            }

            int matchCount = 0;
            int matchIndex = 0;
            int matchLimit = currentImage.Height / 2;

            int ignoreSideOffset = Math.Max(50, currentImage.Width / 20);
            ignoreSideOffset = Math.Min(ignoreSideOffset, currentImage.Width / 3);

            Rectangle rect = new Rectangle(ignoreSideOffset, result.Height - currentImage.Height, currentImage.Width - ignoreSideOffset * 2, currentImage.Height);

            // Both bitmaps are 32 bit BGRA captures of the same width, so rows compare byte for byte.
            int pixelSize = result.BytesPerPixel;
            int rowStart = pixelSize * ignoreSideOffset;
            int compareLength = pixelSize * rect.Width;

            bool RowsEqual(int resultRow, int currentRow) =>
                result.GetPixelSpan().Slice(rowStart + resultRow * result.RowBytes, compareLength)
                    .SequenceEqual(currentImage.GetPixelSpan().Slice(rowStart + currentRow * currentImage.RowBytes, compareLength));

            int ignoreBottomOffsetMax = currentImage.Height / 3;
            int ignoreBottomOffset = Math.Max(50, currentImage.Height / 10);

            if (Options.AutoIgnoreBottomEdge)
            {
                for (int i = 0; i <= ignoreBottomOffsetMax; i++)
                {
                    if (!RowsEqual(result.Height - 1 - i, currentImage.Height - 1 - i))
                    {
                        ignoreBottomOffset += i;
                        break;
                    }
                }

                ignoreBottomOffset = Math.Max(ignoreBottomOffset, bestIgnoreBottomOffset);
            }

            ignoreBottomOffset = Math.Min(ignoreBottomOffset, ignoreBottomOffsetMax);

            int rectBottom = rect.Bottom - ignoreBottomOffset - 1;

            for (int currentImageY = currentImage.Height - 1; currentImageY >= 0 && matchCount < matchLimit; currentImageY--)
            {
                int currentMatchCount = 0;

                for (int y = 0; currentImageY - y >= 0 && currentMatchCount < matchLimit; y++)
                {
                    if (RowsEqual(rectBottom - y, currentImageY - y))
                    {
                        currentMatchCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (currentMatchCount > matchCount)
                {
                    matchCount = currentMatchCount;
                    matchIndex = currentImageY;
                }
            }

            bool bestGuess = false;

            if (matchCount == 0 && bestMatchCount > 0)
            {
                matchCount = bestMatchCount;
                matchIndex = bestMatchIndex;
                ignoreBottomOffset = bestIgnoreBottomOffset;
                bestGuess = true;
            }

            if (matchCount > 0)
            {
                int matchHeight = currentImage.Height - matchIndex - 1;

                if (matchHeight > 0)
                {
                    if (matchCount > bestMatchCount)
                    {
                        bestMatchCount = matchCount;
                        bestMatchIndex = matchIndex;
                        bestIgnoreBottomOffset = ignoreBottomOffset;
                    }

                    SKBitmap newResult = new SKBitmap(new SKImageInfo(result.Width, result.Height - ignoreBottomOffset + matchHeight, result.ColorType, result.AlphaType));

                    using (SKCanvas canvas = new SKCanvas(newResult))
                    using (SKPaint paint = new SKPaint { BlendMode = SKBlendMode.Src })
                    {
                        SKRect top = SKRect.Create(0, 0, result.Width, result.Height - ignoreBottomOffset);
                        canvas.DrawBitmap(result, top, top, paint);
                        canvas.DrawBitmap(currentImage, SKRect.Create(0, matchIndex + 1, currentImage.Width, matchHeight),
                            SKRect.Create(0, result.Height - ignoreBottomOffset, currentImage.Width, matchHeight), paint);
                    }

                    if (bestGuess)
                    {
                        status = ScrollingCaptureStatus.PartiallySuccessful;
                    }
                    else if (status != ScrollingCaptureStatus.PartiallySuccessful)
                    {
                        status = ScrollingCaptureStatus.Successful;
                    }

                    return newResult;
                }
            }

            status = ScrollingCaptureStatus.Failed;

            return null;
        }
    }
}
