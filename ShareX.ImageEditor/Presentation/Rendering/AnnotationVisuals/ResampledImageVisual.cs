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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ShareX.AvaloniaUI.Imaging;
using ShareX.ImageEditor.Core.Annotations;
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
    private SKBitmap? _cachedSourceBitmap;
    private CachedSourceImage? _cachedSourceImage;
    private SKBitmap? _fullResolutionSourceBitmap;
    private Bitmap? _fullResolutionSource;
    private double _logicalWidth;
    private double _logicalHeight;
    private int _displayWidth;
    private int _displayHeight;
    private int _resampledWidth;
    private int _resampledHeight;
    private ImageResamplingMode _resamplingMode;
    private bool _hasSourceState;

    public ResampledImageVisual()
    {
        SizeChanged += OnSizeChanged;
        DataContextChanged += (_, _) => RefreshPhysicalTarget();
        AttachedToVisualTree += OnAttachedToVisualTree;
        DetachedFromVisualTree += OnDetachedFromVisualTree;
    }

    public void SetSource(SKBitmap? sourceBitmap, double width, double height, ImageResamplingMode resamplingMode)
    {
        _logicalWidth = width;
        _logicalHeight = height;
        UpdateSource(sourceBitmap, resamplingMode);
    }

    public void RefreshPhysicalTarget()
    {
        if (_hasSourceState)
        {
            UpdateSource(_sourceBitmap, _resamplingMode);
        }
        else
        {
            UpdateBitmapInterpolationMode(Source);
        }
    }

    public IDisposable BeginSnapshotRender()
    {
        if (Tag is not ImageAnnotation annotation)
        {
            throw new InvalidOperationException("Image annotation visual is missing its annotation model.");
        }

        SKBitmap sourceBitmap = annotation.ImageBitmap
            ?? throw new InvalidOperationException("Image annotation has no source bitmap for snapshot rendering.");
        SKRect bounds = annotation.GetBounds();
        int width = GetSnapshotPixelDimension(bounds.Width);
        int height = GetSnapshotPixelDimension(bounds.Height);
        bool sourceWasUnavailable = !_hasSourceState || !ReferenceEquals(_sourceBitmap, sourceBitmap);
        Bitmap snapshotSource = CreateSnapshotBitmap(sourceBitmap, width, height, annotation.ResamplingMode);
        if (sourceWasUnavailable)
        {
            ClearCachedSourceImage();
        }

        IImage? previousSource = Source;
        UpdateBitmapInterpolationMode(snapshotSource, width, height, hasPhysicalTarget: true);
        Source = snapshotSource;
        return new SnapshotSourceLease(this, previousSource, snapshotSource);
    }

    public void ReleaseDisplayBitmaps()
    {
        Interlocked.Increment(ref _requestVersion);

        lock (_resampleLock)
        {
            _pendingResample?.Dispose();
            _pendingResample = null;
        }

        ClearCachedSourceImage();

        IImage? source = Source;
        Source = null;
        Bitmap? fullResolutionSource = _fullResolutionSource;
        _fullResolutionSource = null;
        _fullResolutionSourceBitmap = null;

        if (!ReferenceEquals(source, fullResolutionSource))
        {
            (source as IDisposable)?.Dispose();
        }

        fullResolutionSource?.Dispose();
        _sourceBitmap = null;
        _hasSourceState = false;
        UpdateBitmapInterpolationMode(Source);
        _displayWidth = 0;
        _displayHeight = 0;
        _resampledWidth = 0;
        _resampledHeight = 0;
    }

    private void UpdateSource(SKBitmap? sourceBitmap, ImageResamplingMode resamplingMode)
    {
        bool hasPhysicalTarget = TryGetPhysicalTargetSize(
            _logicalWidth,
            _logicalHeight,
            out double physicalWidth,
            out double physicalHeight);
        (int resampledWidth, int resampledHeight) = GetResampledSize(
            sourceBitmap,
            physicalWidth,
            physicalHeight,
            hasPhysicalTarget);
        UpdateBitmapInterpolationMode(Source, physicalWidth, physicalHeight, hasPhysicalTarget);

        if (_hasSourceState && !ReferenceEquals(_sourceBitmap, sourceBitmap))
        {
            ClearFullResolutionSource();
            ClearCachedSourceImage();
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
                ReplaceSource(_fullResolutionSource);
                return;
            }

            if (resampledWidth > _displayWidth || resampledHeight > _displayHeight)
            {
                ReplaceSource(_fullResolutionSource);
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
        CachedSourceImage.SourceImageLease sourceImage = GetSourceImageLease(sourceBitmap);
        // Natural-size images are copied without filtering; the selected mode applies when their bounds change.
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

                        ReplaceSource(nextSource);
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

        bool succeeded = request.Width == request.SourceInfo.Width &&
            request.Height == request.SourceInfo.Height
                ? request.SourceImage.Image.ReadPixels(pixmap)
                : request.SourceImage.Image.ScalePixels(pixmap, request.SamplingOptions);
        if (!succeeded)
        {
            throw new InvalidOperationException("Failed to copy or resample image annotation bitmap.");
        }

        return BitmapConversionHelpers.ToAvaloniBitmap(resampledBitmap);
    }

    private Bitmap CreateSnapshotBitmap(
        SKBitmap sourceBitmap,
        int width,
        int height,
        ImageResamplingMode resamplingMode)
    {
        if (width == sourceBitmap.Width && height == sourceBitmap.Height)
        {
            return BitmapConversionHelpers.ToAvaloniBitmap(sourceBitmap);
        }

        SKImageInfo imageInfo = new(
            width,
            height,
            sourceBitmap.ColorType,
            sourceBitmap.AlphaType,
            sourceBitmap.ColorSpace);
        using SKBitmap resampledBitmap = new(imageInfo);
        using SKPixmap pixmap = resampledBitmap.PeekPixels();
        using CachedSourceImage.SourceImageLease sourceImage = GetSourceImageLease(sourceBitmap);

        if (!sourceImage.Image.ScalePixels(pixmap, ImageResampling.GetSamplingOptions(resamplingMode)))
        {
            throw new InvalidOperationException("Failed to resample image annotation for snapshot.");
        }

        return BitmapConversionHelpers.ToAvaloniBitmap(resampledBitmap);
    }

    private static int GetSnapshotPixelDimension(double size)
    {
        if (!double.IsFinite(size) || size > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(size));
        }

        return Math.Max(1, (int)Math.Ceiling(size));
    }

    private CachedSourceImage.SourceImageLease GetSourceImageLease(SKBitmap sourceBitmap)
    {
        if (!ReferenceEquals(_cachedSourceBitmap, sourceBitmap) || _cachedSourceImage == null)
        {
            ClearCachedSourceImage();
            _cachedSourceBitmap = sourceBitmap;
            _cachedSourceImage = new CachedSourceImage(SKImage.FromBitmap(sourceBitmap));
        }

        return _cachedSourceImage!.Acquire();
    }

    private void ClearCachedSourceImage()
    {
        CachedSourceImage? cachedSourceImage = _cachedSourceImage;
        _cachedSourceImage = null;
        _cachedSourceBitmap = null;
        cachedSourceImage?.Dispose();
    }

    private bool TryGetPhysicalTargetSize(
        double width,
        double height,
        out double physicalWidth,
        out double physicalHeight)
    {
        physicalWidth = 0;
        physicalHeight = 0;

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        Matrix? transform = topLevel == null ? null : this.TransformToVisual(topLevel);
        if (transform == null || !double.IsFinite(topLevel!.RenderScaling) || topLevel.RenderScaling <= 0)
        {
            return false;
        }

        Point origin = transform.Value.Transform(new Point(0, 0));
        Point horizontal = transform.Value.Transform(new Point(1, 0));
        Point vertical = transform.Value.Transform(new Point(0, 1));
        double scaleX = Math.Sqrt(
            Math.Pow(horizontal.X - origin.X, 2) +
            Math.Pow(horizontal.Y - origin.Y, 2)) * topLevel.RenderScaling;
        double scaleY = Math.Sqrt(
            Math.Pow(vertical.X - origin.X, 2) +
            Math.Pow(vertical.Y - origin.Y, 2)) * topLevel.RenderScaling;

        double visualWidth = Bounds.Width > 0 && double.IsFinite(Bounds.Width) ? Bounds.Width : width;
        double visualHeight = Bounds.Height > 0 && double.IsFinite(Bounds.Height) ? Bounds.Height : height;
        physicalWidth = visualWidth * scaleX;
        physicalHeight = visualHeight * scaleY;
        return double.IsFinite(physicalWidth) && double.IsFinite(physicalHeight) &&
            physicalWidth > 0 && physicalHeight > 0;
    }

    private void UpdateBitmapInterpolationMode(IImage? image)
    {
        bool hasPhysicalTarget = TryGetPhysicalTargetSize(
            _logicalWidth,
            _logicalHeight,
            out double physicalWidth,
            out double physicalHeight);
        UpdateBitmapInterpolationMode(image, physicalWidth, physicalHeight, hasPhysicalTarget);
    }

    private void UpdateBitmapInterpolationMode(
        IImage? image,
        double physicalWidth,
        double physicalHeight,
        bool hasPhysicalTarget)
    {
        bool isPixelExact = hasPhysicalTarget &&
            image is Bitmap bitmap &&
            Math.Abs(physicalWidth - bitmap.PixelSize.Width) < 1 &&
            Math.Abs(physicalHeight - bitmap.PixelSize.Height) < 1;
        RenderOptions.SetBitmapInterpolationMode(
            this,
            isPixelExact ? BitmapInterpolationMode.None : BitmapInterpolationMode.HighQuality);
    }

    private void ReplaceSource(Bitmap? nextSource)
    {
        IImage? previousSource = Source;
        UpdateBitmapInterpolationMode(nextSource);
        Source = nextSource;
        _displayWidth = nextSource?.PixelSize.Width ?? 0;
        _displayHeight = nextSource?.PixelSize.Height ?? 0;

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

        _logicalWidth = e.NewSize.Width;
        _logicalHeight = e.NewSize.Height;
        UpdateSource(_sourceBitmap, _resamplingMode);
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (Tag is ImageAnnotation annotation)
        {
            SKRect bounds = annotation.GetBounds();
            SetSource(annotation.ImageBitmap, bounds.Width, bounds.Height, annotation.ResamplingMode);
        }
        else
        {
            RefreshPhysicalTarget();
        }
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        ReleaseDisplayBitmaps();
    }

    private sealed class SnapshotSourceLease : IDisposable
    {
        private ResampledImageVisual? _owner;
        private readonly IImage? _previousSource;
        private readonly Bitmap _snapshotSource;

        public SnapshotSourceLease(ResampledImageVisual owner, IImage? previousSource, Bitmap snapshotSource)
        {
            _owner = owner;
            _previousSource = previousSource;
            _snapshotSource = snapshotSource;
        }

        public void Dispose()
        {
            ResampledImageVisual? owner = Interlocked.Exchange(ref _owner, null);
            if (owner == null)
            {
                return;
            }

            if (ReferenceEquals(owner.Source, _snapshotSource))
            {
                owner.Source = _previousSource;
                owner.UpdateBitmapInterpolationMode(owner.Source);
            }

            _snapshotSource.Dispose();
        }
    }

    private sealed class CachedSourceImage : IDisposable
    {
        private readonly object _sync = new();
        private SKImage? _image;
        private int _referenceCount = 1;
        private bool _ownerDisposed;

        public CachedSourceImage(SKImage image)
        {
            _image = image;
        }

        public SourceImageLease Acquire()
        {
            lock (_sync)
            {
                if (_ownerDisposed || _image == null)
                {
                    throw new ObjectDisposedException(nameof(CachedSourceImage));
                }

                _referenceCount++;
                return new SourceImageLease(this, _image);
            }
        }

        public void Dispose()
        {
            ReleaseReference(owner: true);
        }

        private void ReleaseReference(bool owner = false)
        {
            SKImage? imageToDispose = null;
            lock (_sync)
            {
                if (owner)
                {
                    if (_ownerDisposed)
                    {
                        return;
                    }

                    _ownerDisposed = true;
                }

                _referenceCount--;
                if (_referenceCount == 0)
                {
                    imageToDispose = _image;
                    _image = null;
                }
            }

            imageToDispose?.Dispose();
        }

        public sealed class SourceImageLease : IDisposable
        {
            private CachedSourceImage? _owner;

            internal SourceImageLease(CachedSourceImage owner, SKImage image)
            {
                _owner = owner;
                Image = image;
            }

            public SKImage Image { get; }

            public void Dispose()
            {
                Interlocked.Exchange(ref _owner, null)?.ReleaseReference();
            }
        }
    }

    private sealed class PendingResample : IDisposable
    {
        public PendingResample(
            long version,
            CachedSourceImage.SourceImageLease sourceImage,
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
        public CachedSourceImage.SourceImageLease SourceImage { get; }
        public SKBitmap SourceBitmap { get; }
        public SKImageInfo SourceInfo { get; }
        public int Width { get; }
        public int Height { get; }
        public SKSamplingOptions SamplingOptions { get; }

        public void Dispose() => SourceImage.Dispose();
    }

    private static (int Width, int Height) GetResampledSize(
        SKBitmap? sourceBitmap,
        double physicalWidth,
        double physicalHeight,
        bool hasPhysicalTarget)
    {
        if (sourceBitmap == null)
        {
            return (0, 0);
        }

        double width = hasPhysicalTarget ? physicalWidth : sourceBitmap.Width;
        double height = hasPhysicalTarget ? physicalHeight : sourceBitmap.Height;
        if (!double.IsFinite(width) || !double.IsFinite(height))
        {
            throw new ArgumentOutOfRangeException(nameof(physicalWidth));
        }

        if (hasPhysicalTarget)
        {
            if (Math.Abs(width - sourceBitmap.Width) < 1) width = sourceBitmap.Width;
            if (Math.Abs(height - sourceBitmap.Height) < 1) height = sourceBitmap.Height;
        }

        double scale = Math.Min(
            1,
            Math.Min(
                Math.Min((double)MaxResampledDimension / width, (double)MaxResampledDimension / height),
                Math.Sqrt((double)MaxResampledPixelCount / width / height)));

        return (
            Math.Clamp((int)Math.Round(width * scale), 1, MaxResampledDimension),
            Math.Clamp((int)Math.Round(height * scale), 1, MaxResampledDimension));
    }
}
