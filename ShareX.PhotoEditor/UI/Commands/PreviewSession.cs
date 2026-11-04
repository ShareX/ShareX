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

using Avalonia.Threading;

namespace ShareX.PhotoEditor.UI.Commands;

/// <summary>
/// Live preview of a pixel operation on the active raster layer, restricted to the selection.
/// Work runs off the UI thread; Cancel restores the original pixels.
/// </summary>
public sealed class PreviewSession
{
    private readonly PhotoEditorWindow _editor;
    private readonly PhotoDocument _doc;
    private readonly RasterLayer _layer;
    private readonly SKBitmap _backup;
    private readonly SKRectI _region;
    private readonly DispatcherTimer _debounce;
    private Func<SKBitmap, SKBitmap>? _pending;
    private int _version;
    private bool _finished;

    public PreviewSession(PhotoEditorWindow editor, RasterLayer layer, int margin = 64)
    {
        _editor = editor;
        _doc = editor.Document!;
        _layer = layer;
        layer.EnsureCovers(_doc.Bounds);
        PixelSurface surface = layer.GetWritableSurface();
        _backup = surface.Bitmap.Copy();
        SKRectI region = _doc.SelectionBounds;
        region.Offset(-layer.Offset.X, -layer.Offset.Y);
        Target = region;
        region.Inflate(margin, margin);
        region.Intersect(new SKRectI(0, 0, surface.Width, surface.Height));
        _region = region;
        _debounce = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        _debounce.Tick += (_, _) =>
        {
            _debounce.Stop();
            if (_pending != null) _ = RunAsync(_pending);
        };
    }

    /// <summary>The selection bounds in layer coordinates.</summary>
    public SKRectI Target { get; }

    public bool PreviewEnabled { get; set; } = true;

    /// <summary>Schedules a preview of a filter that maps the region bitmap to a new bitmap of the same size.</summary>
    public void Preview(Func<SKBitmap, SKBitmap> filter)
    {
        _pending = filter;
        if (!PreviewEnabled)
        {
            Restore();
            return;
        }
        _debounce.Stop();
        _debounce.Start();
    }

    private async Task RunAsync(Func<SKBitmap, SKBitmap> filter)
    {
        int version = ++_version;
        SKBitmap? result = await Task.Run(() => Compute(filter));
        if (result == null) return;
        if (version != _version || _finished)
        {
            result.Dispose();
            return;
        }
        WriteBack(result);
        result.Dispose();
    }

    private SKBitmap? Compute(Func<SKBitmap, SKBitmap> filter)
    {
        try
        {
            using SKBitmap input = ImageOps.Crop(_backup, _region);
            SKBitmap output = filter(input);
            if (output.Width != input.Width || output.Height != input.Height || output.ColorType != SKColorType.Bgra8888 || output.AlphaType != SKAlphaType.Premul)
            {
                SKBitmap converted = ImageOps.CreateBitmap(input.Width, input.Height);
                converted.Erase(SKColors.Transparent);
                using (SKCanvas canvas = new(converted)) canvas.DrawBitmap(output, SKRect.Create(output.Width, output.Height), SKRect.Create(input.Width, input.Height));
                output.Dispose();
                output = converted;
            }
            return output;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>Blends the filtered region into the layer weighted by the selection and the layer's transparency lock.</summary>
    private unsafe void WriteBack(SKBitmap filtered)
    {
        PixelSurface surface = _layer.Surface;
        if (surface.IsDisposed) return;
        uint* dst = (uint*)surface.Bitmap.GetPixels();
        uint* src = (uint*)_backup.GetPixels();
        uint* f = (uint*)filtered.GetPixels();
        int stride = surface.RowBytes / 4, fStride = filtered.RowBytes / 4;
        PixelSurface? selection = _doc.Selection;
        SKPointI offset = _layer.Offset;
        bool lockAlpha = _layer.LockTransparency;
        SKRectI target = Target;
        target.Intersect(_region);
        for (int y = target.Top; y < target.Bottom; y++)
        {
            for (int x = target.Left; x < target.Right; x++)
            {
                float w = selection != null ? MaskOps.Get(selection, x + offset.X, y + offset.Y) / 255f : 1;
                uint original = src[y * stride + x];
                uint value = f[(y - _region.Top) * fStride + (x - _region.Left)];
                if (lockAlpha)
                {
                    ColorMath.Unpack(value, out float r, out float g, out float b, out _);
                    value = ColorMath.PackUnpremul(r, g, b, (original >> 24) / 255f);
                }
                dst[y * stride + x] = w >= 1 ? value : ColorMath.LerpPremul(original, value, w);
            }
        }
        SKRectI dirty = target;
        dirty.Offset(offset.X, offset.Y);
        _editor.InvalidateCanvas(dirty);
    }

    public void Restore()
    {
        // The layer may have been replaced or disposed while the dialog was open (for example by closing the document).
        if (_layer.Surface.IsDisposed || _backup.Handle == IntPtr.Zero) return;
        using SKCanvas canvas = new(_layer.Surface.Bitmap);
        using SKPaint paint = new() { BlendMode = SKBlendMode.Src };
        canvas.DrawBitmap(_backup, 0, 0, paint);
        SKRectI dirty = _region;
        dirty.Offset(_layer.Offset.X, _layer.Offset.Y);
        _editor.InvalidateCanvas(dirty);
    }

    /// <summary>Applies the final result synchronously and records history.</summary>
    public async Task CommitAsync(Func<SKBitmap, SKBitmap> filter, string historyName)
    {
        _debounce.Stop();
        _version++;
        SKBitmap? result = null;
        await _editor.RunBusyAsync(historyName + "...", _ => Task.Run(() => result = Compute(filter)));
        _finished = true;
        if (result != null)
        {
            WriteBack(result);
            result.Dispose();
        }
        _backup.Dispose();
        _editor.Commit(historyName);
    }

    public void Cancel()
    {
        _debounce.Stop();
        _version++;
        _finished = true;
        Restore();
        _backup.Dispose();
    }
}
