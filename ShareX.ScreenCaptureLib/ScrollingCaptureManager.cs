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

using ShareX.Platform;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Bitmap = SkiaSharp.SKBitmap;

namespace ShareX.ScreenCaptureLib
{
    internal class ScrollingCaptureManager : IDisposable
    {
        public ScrollingCaptureOptions Options { get; private set; }
        public SKBitmap? Result { get; private set; }
        public bool IsCapturing { get; private set; }

        private SKBitmap? lastScreenshot;
        private SKBitmap? previousScreenshot;
        private bool stopRequested;
        private ScrollingCaptureStatus status;
        private int bestMatchCount, bestMatchIndex, bestIgnoreBottomOffset;
        private PlatformWindow? selectedWindow;
        private Rectangle selectedRectangle;

        public ScrollingCaptureManager(ScrollingCaptureOptions options)
        {
            Options = options;
        }

        public void Dispose()
        {
            Reset();
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

        public async Task<ScrollingCaptureStatus> StartCapture()
        {
            if (!IsCapturing && selectedWindow != null && !selectedRectangle.IsEmpty)
            {
                IsCapturing = true;
                stopRequested = false;
                status = ScrollingCaptureStatus.Failed;
                bestMatchCount = 0;
                bestMatchIndex = 0;
                bestIgnoreBottomOffset = 0;
                Reset();

                ScrollingCaptureRegionWindow? regionWindow = null;

                if (Options.ShowRegion)
                {
                    regionWindow = new ScrollingCaptureRegionWindow(selectedRectangle);
                    regionWindow.Show();
                }

                IWindowService windows = PlatformServices.Current.Windows;
                IInputService input = PlatformServices.Current.Input;

                try
                {
                    windows.ActivateWindow(selectedWindow.Handle);

                    await Task.Delay(Options.StartDelay);

                    if (Options.AutoScrollTop)
                    {
                        input.SendKeyPress(VirtualKeys.Home);
                        input.ScrollWindow(selectedWindow.Handle, WindowScrollCommand.Top);

                        await Task.Delay(Options.ScrollDelay);
                    }

                    Screenshot screenshot = new Screenshot()
                    {
                        CaptureCursor = false
                    };

                    while (!stopRequested)
                    {
                        lastScreenshot = await screenshot.CaptureRectangleAsync(selectedRectangle);

                        if (CompareLastTwoImages())
                        {
                            break;
                        }

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
                            await Task.Delay(delay);
                        }
                    }
                }
                finally
                {
                    regionWindow?.Close();

                    Reset(true);
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
            var selection = await RegionCaptureTasks.GetRectangleRegionAsync(new RegionCaptureOptions());
            if (selection == null)
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
