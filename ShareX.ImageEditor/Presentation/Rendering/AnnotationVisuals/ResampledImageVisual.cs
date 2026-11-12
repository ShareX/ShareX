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

using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;
using System.Diagnostics;

namespace ShareX.ImageEditor.Presentation.Rendering.AnnotationVisuals;

internal sealed class ResampledImageVisual : Image
{
    private static readonly TimeSpan MinimumResampleInterval = TimeSpan.FromSeconds(1d / 12);
    private const int MaxResampledDimension = 16384;
    private const long MaxResampledPixelCount = 16_777_216;

    private readonly object _resampleLock = new();
    private PendingResample? _pendingResample;
    private bool _resampleWorkerRunning;
    private long _lastResampleTimestamp;
    private long _requestVersion;
    private SKBitmap? _sourceBitmap;
    private SKBitmap? _fullResolutionSourceBitmap;
    // Keep an original-resolution preview so an upscale never has to stretch a reduced preview.
    private Bitmap? _fullResolutionSource;
    private int _displayWidth;
    private int _displayHeight;
    private int _resampledWidth;
    private int _resampledHeight;
    private ImageResamplingMode _resamplingMode;
    private bool _hasSourceState;

    public ResampledImageVisual()
    {
        SizeChanged += OnSizeChanged;
    }

    public void SetSource(SKBitmap? sourceBitmap, double width, double height, ImageResamplingMode resamplingMode)
    {
        (int resampledWidth, int resampledHeight) = GetResampledSize(width, height);

        if (_hasSourceState && !ReferenceEquals(_sourceBitmap, sourceBitmap))
        {
            ClearFullResolutionSource();
        }

        if (_hasSourceState &&
            ReferenceEquals(_sourceBitmap, sourceBitmap) &&
            _resampledWidth == resampledWidth &&
            _resampledHeight == resampledHeight &&
            _resamplingMode == resamplingMode)
        {
            return;
        }

        _sourceBitmap = sourceBitmap;
        _resampledWidth = resampledWidth;
        _resampledHeight = resampledHeight;
        _resamplingMode = resamplingMode;
        _hasSourceState = true;
        long requestVersion = Interlocked.Increment(ref _requestVersion);

        if (sourceBitmap != null &&
            ReferenceEquals(_fullResolutionSourceBitmap, sourceBitmap) &&
            _fullResolutionSource != null)
        {
            if (resampledWidth == sourceBitmap.Width && resampledHeight == sourceBitmap.Height)
            {
                ReplaceSource(_fullResolutionSource, sourceBitmap.Width, sourceBitmap.Height);
                return;
            }

            if (resampledWidth > _displayWidth || resampledHeight > _displayHeight)
            {
                ReplaceSource(_fullResolutionSource, sourceBitmap.Width, sourceBitmap.Height);
            }
        }

        if (sourceBitmap == null)
        {
            lock (_resampleLock)
            {
                _pendingResample?.Dispose();
                _pendingResample = null;
            }

            ReplaceSource(null);
            return;
        }

        // Keep the source pixels alive while the background worker uses them.
        SKImage sourceImage = SKImage.FromBitmap(sourceBitmap);
        PendingResample request = new(
            requestVersion,
            sourceImage,
            sourceBitmap,
            sourceBitmap.Info,
            resampledWidth,
            resampledHeight,
            ImageResampling.GetSamplingOptions(resamplingMode));
        bool startWorker = false;

        lock (_resampleLock)
        {
            // Resize events can outpace sampling; work only on the latest requested size.
            _pendingResample?.Dispose();
            _pendingResample = request;

            if (!_resampleWorkerRunning)
            {
                _resampleWorkerRunning = true;
                startWorker = true;
            }
        }

        if (startWorker)
        {
            _ = ProcessResampleQueueAsync();
        }
    }

    private async Task ProcessResampleQueueAsync()
    {
        while (true)
        {
            PendingResample? request;
            lock (_resampleLock)
            {
                request = _pendingResample;
                _pendingResample = null;
                if (request == null)
                {
                    _resampleWorkerRunning = false;
                    return;
                }
            }

            TimeSpan timeSinceLastResample = _lastResampleTimestamp == 0
                ? MinimumResampleInterval
                : Stopwatch.GetElapsedTime(_lastResampleTimestamp);
            if (timeSinceLastResample < MinimumResampleInterval)
            {
                await Task.Delay(MinimumResampleInterval - timeSinceLastResample);

                lock (_resampleLock)
                {
                    if (_pendingResample is { } newerRequest)
                    {
                        request.Dispose();
                        request = newerRequest;
                        _pendingResample = null;
                    }
                }
            }

            Bitmap? nextSource = null;
            try
            {
                if (request.Version != Volatile.Read(ref _requestVersion))
                {
                    continue;
                }

                nextSource = await Task.Run(() => Resample(request));
                _lastResampleTimestamp = Stopwatch.GetTimestamp();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (request.Version == Volatile.Read(ref _requestVersion))
                    {
                        if (request.Width == request.SourceInfo.Width &&
                            request.Height == request.SourceInfo.Height &&
                            ReferenceEquals(request.SourceBitmap, _sourceBitmap))
                        {
                            _fullResolutionSource = nextSource;
                            _fullResolutionSourceBitmap = request.SourceBitmap;
                        }

                        ReplaceSource(nextSource, request.Width, request.Height);
                        nextSource = null;
                    }
                }, DispatcherPriority.Render);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Failed to resample image annotation preview: {0}", ex);
            }
            finally
            {
                nextSource?.Dispose();
                request.Dispose();
            }
        }
    }

    private static Bitmap Resample(PendingResample request)
    {
        SKImageInfo imageInfo = new(
            request.Width,
            request.Height,
            request.SourceInfo.ColorType,
            request.SourceInfo.AlphaType,
            request.SourceInfo.ColorSpace);
        using SKBitmap resampledBitmap = new(imageInfo);
        using SKPixmap pixmap = resampledBitmap.PeekPixels();

        if (!request.SourceImage.ScalePixels(pixmap, request.SamplingOptions))
        {
            throw new InvalidOperationException("Failed to resample image annotation bitmap.");
        }

        return BitmapConversionHelpers.ToAvaloniBitmap(resampledBitmap);
    }

    private void ReplaceSource(Bitmap? nextSource, int width = 0, int height = 0)
    {
        IImage? previousSource = Source;
        Source = nextSource;
        _displayWidth = width;
        _displayHeight = height;

        if (!ReferenceEquals(previousSource, _fullResolutionSource) &&
            !ReferenceEquals(previousSource, nextSource))
        {
            (previousSource as IDisposable)?.Dispose();
        }
    }

    private void ClearFullResolutionSource()
    {
        Bitmap? fullResolutionSource = _fullResolutionSource;
        _fullResolutionSource = null;
        _fullResolutionSourceBitmap = null;

        if (ReferenceEquals(Source, fullResolutionSource))
        {
            ReplaceSource(null);
        }
        else
        {
            fullResolutionSource?.Dispose();
        }
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (!_hasSourceState || e.NewSize.Width <= 0 || e.NewSize.Height <= 0)
        {
            return;
        }

        SetSource(_sourceBitmap, e.NewSize.Width, e.NewSize.Height, _resamplingMode);
    }

    private sealed class PendingResample : IDisposable
    {
        public PendingResample(
            long version,
            SKImage sourceImage,
            SKBitmap sourceBitmap,
            SKImageInfo sourceInfo,
            int width,
            int height,
            SKSamplingOptions samplingOptions)
        {
            Version = version;
            SourceImage = sourceImage;
            SourceBitmap = sourceBitmap;
            SourceInfo = sourceInfo;
            Width = width;
            Height = height;
            SamplingOptions = samplingOptions;
        }

        public long Version { get; }
        public SKImage SourceImage { get; }
        public SKBitmap SourceBitmap { get; }
        public SKImageInfo SourceInfo { get; }
        public int Width { get; }
        public int Height { get; }
        public SKSamplingOptions SamplingOptions { get; }

        public void Dispose() => SourceImage.Dispose();
    }

    private static (int Width, int Height) GetResampledSize(double width, double height)
    {
        if (!double.IsFinite(width)) throw new ArgumentOutOfRangeException(nameof(width));
        if (!double.IsFinite(height)) throw new ArgumentOutOfRangeException(nameof(height));

        width = Math.Max(1, Math.Ceiling(width));
        height = Math.Max(1, Math.Ceiling(height));
        // Bound intermediate rasters; Avalonia scales the result to the annotation bounds.
        double scale = Math.Min(
            1,
            Math.Min(
                Math.Min((double)MaxResampledDimension / width, (double)MaxResampledDimension / height),
                Math.Sqrt((double)MaxResampledPixelCount / width / height)));

        return (
            Math.Max(1, (int)Math.Floor(width * scale)),
            Math.Max(1, (int)Math.Floor(height * scale)));
    }
}
