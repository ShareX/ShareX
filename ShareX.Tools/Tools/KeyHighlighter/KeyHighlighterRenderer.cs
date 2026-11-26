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

using SkiaSharp;

namespace ShareX.Tools;

/// <summary>Draws key lines as rounded labels into premultiplied BGRA pixels. Redraws only when something changed.</summary>
internal sealed class KeyHighlighterRenderer : IDisposable
{
    private readonly KeyHighlighterSettings settings;
    private readonly SKTypeface typeface = SKTypeface.FromFamilyName("Segoe UI", SKFontStyle.Bold);
    private string signature = "";
    private KeyHighlighterImage? image;
    private int version;

    public KeyHighlighterRenderer(KeyHighlighterSettings settings) => this.settings = settings;

    public KeyHighlighterImage? Render(IReadOnlyList<KeyStrokeItem> items, int frameWidth, int frameHeight)
    {
        if (items.Count == 0 || frameWidth < 1 || frameHeight < 1)
        {
            signature = "";
            image = null;
            return null;
        }

        string next = $"{frameWidth}x{frameHeight}|" + string.Join("|", items.Select(item => $"{item.Text}:{item.Count}:{item.Opacity:F2}"));
        if (next == signature && image != null) return image;
        signature = next;

        // Sizes are set for 1080p and scale with the recording, so keys look the same in small and large videos.
        float scale = Math.Clamp(frameHeight / 1080f, 0.5f, 4f);
        using SKFont font = new(typeface, Math.Max(8, settings.FontSize) * scale);
        float padX = font.Size * 0.55f, padY = font.Size * 0.3f, gap = font.Size * 0.3f, radius = font.Size * 0.35f;
        float margin = 20 * scale;
        float lineHeight = font.Spacing + padY * 2;

        string[] texts = items.Select(item => item.Count > 1 ? $"{item.Text}  ×{item.Count}" : item.Text).ToArray();
        float[] widths = texts.Select(text => font.MeasureText(text) + padX * 2).ToArray();
        int width = (int)Math.Ceiling(Math.Min(widths.Max(), frameWidth - margin * 2));
        int height = (int)Math.Ceiling(lineHeight * items.Count + gap * (items.Count - 1));
        if (width < 1 || height < 1 || width > frameWidth || height > frameHeight)
        {
            image = null;
            return null;
        }

        SKImageInfo info = new(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        using SKBitmap bitmap = new(info);
        using (SKCanvas canvas = new(bitmap))
        {
            canvas.Clear(SKColors.Transparent);
            bool right = settings.Position is KeyHighlighterPosition.BottomRight or KeyHighlighterPosition.TopRight;
            bool center = settings.Position == KeyHighlighterPosition.BottomCenter;
            using SKPaint background = new() { IsAntialias = true };
            using SKPaint text = new() { IsAntialias = true };
            for (int i = 0; i < items.Count; i++)
            {
                float itemWidth = Math.Min(widths[i], width);
                float x = right ? width - itemWidth : center ? (width - itemWidth) / 2 : 0;
                float y = i * (lineHeight + gap);
                byte alpha = (byte)Math.Round(255 * items[i].Opacity);
                background.Color = new SKColor(0, 0, 0, (byte)Math.Round(170 * items[i].Opacity));
                canvas.DrawRoundRect(new SKRect(x, y, x + itemWidth, y + lineHeight), radius, radius, background);
                text.Color = new SKColor(settings.TextColor.R, settings.TextColor.G, settings.TextColor.B,
                    (byte)(settings.TextColor.A * alpha / 255));
                canvas.Save();
                canvas.ClipRect(new SKRect(x, y, x + itemWidth, y + lineHeight));
                canvas.DrawText(texts[i], x + padX, y + padY - font.Metrics.Ascent, font, text);
                canvas.Restore();
            }
        }

        int left = settings.Position switch
        {
            KeyHighlighterPosition.BottomRight or KeyHighlighterPosition.TopRight => frameWidth - width - (int)margin,
            KeyHighlighterPosition.BottomCenter => (frameWidth - width) / 2,
            _ => (int)margin
        };
        int top = settings.Position is KeyHighlighterPosition.TopLeft or KeyHighlighterPosition.TopRight
            ? (int)margin
            : frameHeight - height - (int)margin;

        image = new KeyHighlighterImage(bitmap.Bytes, width, height, left, top, ++version);
        return image;
    }

    public void Dispose() => typeface.Dispose();
}
