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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ShareX.PhotoEditor.UI.Panels;

/// <summary>Base class of all dockable panels.</summary>
public abstract class EditorPanel : UserControl
{
    protected EditorPanel(PhotoEditorWindow editor)
    {
        Editor = editor;
    }

    protected PhotoEditorWindow Editor { get; }

    protected PhotoDocument? Doc => Editor.Document;

    public abstract string Id { get; }

    public abstract string Title { get; }

    /// <summary>Rebuilds the panel from the editor state.</summary>
    public abstract void Refresh();

    protected static Button IconButton(string icon, string tip, Action click, double size = 16)
    {
        Button button = new() { Classes = { "icon" }, Content = ToolIcons.Create(icon, size), Padding = new Thickness(5), Width = 28, Height = 26, HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center };
        ToolTip.SetTip(button, tip);
        button.Click += (_, _) => click();
        return button;
    }

    protected static StackPanel Row(params Control[] children)
    {
        StackPanel panel = new() { Orientation = Orientation.Horizontal, Spacing = 6 };
        foreach (Control c in children) panel.Children.Add(c);
        return panel;
    }

    protected static TextBlock Text(string text, double size = 12, bool bold = false) =>
        new() { Text = text, FontSize = size, FontWeight = bold ? FontWeight.SemiBold : FontWeight.Normal, VerticalAlignment = VerticalAlignment.Center };
}

public static class BitmapHelper
{
    /// <summary>Copies a premultiplied BGRA SKBitmap into an Avalonia bitmap.</summary>
    public static unsafe WriteableBitmap ToAvalonia(SKBitmap bitmap)
    {
        WriteableBitmap result = new(new PixelSize(bitmap.Width, bitmap.Height), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        using ILockedFramebuffer buffer = result.Lock();
        for (int y = 0; y < bitmap.Height; y++)
        {
            Buffer.MemoryCopy((byte*)bitmap.GetPixels() + y * bitmap.RowBytes, (byte*)buffer.Address + y * buffer.RowBytes, buffer.RowBytes, bitmap.Width * 4);
        }
        return result;
    }

    /// <summary>Renders a small thumbnail of a layer on a checkerboard.</summary>
    public static SKBitmap LayerThumbnail(PhotoDocument doc, Layer layer, int size)
    {
        float scale = Math.Min(size / (float)doc.Width, size / (float)doc.Height);
        int w = Math.Max(1, (int)(doc.Width * scale)), h = Math.Max(1, (int)(doc.Height * scale));
        SKBitmap result = ImageOps.CreateBitmap(w, h);
        using SKCanvas canvas = new(result);
        DrawChecker(canvas, w, h);
        canvas.Scale(scale);
        RenderContext context = new(doc.Width, doc.Height) { Area = doc.Bounds, Sampling = new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear) };
        switch (layer)
        {
            case GroupLayer group:
                foreach (Layer child in group.Descendants().Where(c => c.Visible && c is not GroupLayer and not AdjustmentLayer)) child.RenderContent(canvas, context);
                break;
            case AdjustmentLayer:
                canvas.ResetMatrix();
                using (SKPaint paint = new() { Color = new SKColor(0x60, 0x60, 0x60) }) canvas.DrawRect(0, 0, w, h, paint);
                using (SKPaint half = new() { Color = SKColors.White }) canvas.DrawRect(0, 0, w / 2f, h, half);
                break;
            case RasterLayer raster:
                using (SKImage image = SKImage.FromBitmap(raster.Surface.Bitmap))
                using (SKPaint paint = new())
                {
                    canvas.DrawImage(image, SKRect.Create(raster.Surface.Width, raster.Surface.Height), SKRect.Create(raster.Offset.X, raster.Offset.Y, raster.Surface.Width, raster.Surface.Height), context.Sampling, paint);
                }
                break;
            default:
                layer.RenderContent(canvas, context);
                break;
        }
        return result;
    }

    public static SKBitmap MaskThumbnail(PixelSurface mask, int size)
    {
        float scale = Math.Min(size / (float)mask.Width, size / (float)mask.Height);
        int w = Math.Max(1, (int)(mask.Width * scale)), h = Math.Max(1, (int)(mask.Height * scale));
        using SKBitmap gray = ImageOps.MaskToGray(mask);
        return ImageOps.Resize(gray, w, h, ResampleMethod.Bilinear);
    }

    public static void DrawChecker(SKCanvas canvas, int w, int h)
    {
        canvas.Clear(SKColors.White);
        using SKPaint paint = new() { Color = new SKColor(0xCC, 0xCC, 0xCC) };
        for (int y = 0; y < h; y += 4)
            for (int x = (y / 4) % 2 * 4; x < w; x += 8)
                canvas.DrawRect(x, y, 4, 4, paint);
    }
}
