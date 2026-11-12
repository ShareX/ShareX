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
using ShareX.AvaloniaUI.Imaging;
using SkiaSharp;

namespace ShareX.ImageEditor.Presentation.Rendering.AnnotationVisuals;

internal sealed class ResampledImageVisual : Image
{
    private const int MaxResampledDimension = 16384;
    private const long MaxResampledPixelCount = 16_777_216;

    private SKBitmap? _sourceBitmap;
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

        if (_hasSourceState &&
            ReferenceEquals(_sourceBitmap, sourceBitmap) &&
            _resampledWidth == resampledWidth &&
            _resampledHeight == resampledHeight &&
            _resamplingMode == resamplingMode)
        {
            return;
        }

        SKSamplingOptions samplingOptions = ImageResampling.GetSamplingOptions(resamplingMode);
        Bitmap? nextSource = null;
        if (sourceBitmap != null)
        {
            SKBitmap? resampledBitmap = null;
            try
            {
                SKBitmap displayBitmap = sourceBitmap;
                if (resampledWidth != sourceBitmap.Width || resampledHeight != sourceBitmap.Height)
                {
                    SKImageInfo imageInfo = new(resampledWidth, resampledHeight, sourceBitmap.ColorType, sourceBitmap.AlphaType, sourceBitmap.ColorSpace);
                    resampledBitmap = sourceBitmap.Resize(imageInfo, samplingOptions)
                        ?? throw new InvalidOperationException("Failed to resample image annotation bitmap.");
                    displayBitmap = resampledBitmap;
                }

                nextSource = BitmapConversionHelpers.ToAvaloniBitmap(displayBitmap);
            }
            finally
            {
                resampledBitmap?.Dispose();
            }
        }

        IImage? previousSource = Source;
        Source = nextSource;
        (previousSource as IDisposable)?.Dispose();

        _sourceBitmap = sourceBitmap;
        _resampledWidth = resampledWidth;
        _resampledHeight = resampledHeight;
        _resamplingMode = resamplingMode;
        _hasSourceState = true;
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (!_hasSourceState || e.NewSize.Width <= 0 || e.NewSize.Height <= 0)
        {
            return;
        }

        SetSource(_sourceBitmap, e.NewSize.Width, e.NewSize.Height, _resamplingMode);
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
