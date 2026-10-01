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
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace ShareX.HelpersLib;

internal sealed class PrintTextHelper
{
    private string text = "";
    private int offset, page;
    public string Text { get => text; set => text = value.Replace("\r\n", "\n").Replace('\r', '\n'); }
    public ImageFont Font { get; set; }
    public void BeginPrint() { offset = 0; page = 1; }

    public void PrintPage(PrintPageEventArgs args)
    {
        using SKBitmap bitmap = RenderPage(args.PageBounds.Size, args.MarginBounds, out bool morePages);
        if (bitmap != null) WindowsPrintInterop.DrawImage(args, bitmap, args.PageBounds);
        args.HasMorePages = morePages;
    }

    // Page and margin bounds are in hundredths of an inch; rasterize at 300 dpi.
    public SKBitmap RenderPage(Size pageSize, Rectangle margin, out bool morePages)
    {
        using ImageFont settings = new(Font.Name, Font.PixelSize * 300 / 96, Font.Style, ImageFontUnit.Pixel);
        using SKFont font = settings.CreateFont();
        float lineHeight = font.Spacing;
        float width = margin.Width * 3f, height = margin.Height * 3f - lineHeight * 3;
        if (width <= 0 || height < lineHeight)
        {
            morePages = false;
            return null;
        }
        SKBitmap bitmap = SkiaImageHelpers.CreateBitmap(pageSize.Width * 3, pageSize.Height * 3);
        using SKCanvas canvas = new(bitmap);
        using SKPaint paint = new() { Color = SKColors.Black, IsAntialias = true };
        canvas.Clear(SKColors.White);
        canvas.ClipRect(SKRect.Create(margin.Left * 3f, margin.Top * 3f, width, margin.Height * 3f));
        int lineCount = Math.Max(1, (int)(height / lineHeight));
        for (int row = 0; row < lineCount && offset < text.Length; row++)
        {
            int start = offset, lastWhitespace = -1;
            StringBuilder line = new();
            while (offset < text.Length)
            {
                char character = text[offset++];
                if (character == '\n') break;
                line.Append(character);
                if (char.IsWhiteSpace(character)) lastWhitespace = line.Length - 1;
                if (MeasureLine(font, line.ToString()) <= width || line.Length == 1) continue;
                if (lastWhitespace >= 0)
                {
                    offset = start + lastWhitespace + 1;
                    line.Length = lastWhitespace;
                }
                else { offset--; line.Length--; }
                break;
            }
            DrawLine(canvas, font, paint, line.ToString(), margin.Left * 3f,
                margin.Top * 3f + row * lineHeight - font.Metrics.Ascent, Font.Style);
        }
        string footer = page.ToString();
        canvas.DrawText(footer, margin.Left * 3f + (width - font.MeasureText(footer)) / 2,
            margin.Top * 3f + height + lineHeight * 2 - font.Metrics.Ascent, font, paint);
        page++;
        morePages = offset < text.Length;
        return bitmap;
    }

    private static float MeasureLine(SKFont font, string text)
    {
        float width = 0;
        string[] parts = text.Split('\t');
        for (int i = 0; i < parts.Length; i++)
        {
            width += font.MeasureText(parts[i]);
            if (i < parts.Length - 1) width = (MathF.Floor(width / 300) + 1) * 300;
        }
        return width;
    }

    private static void DrawLine(SKCanvas canvas, SKFont font, SKPaint paint, string text, float x, float baseline, ImageFontStyle style)
    {
        float width = 0;
        string[] parts = text.Split('\t');
        for (int i = 0; i < parts.Length; i++)
        {
            canvas.DrawText(parts[i], x + width, baseline, font, paint);
            using SKPath decorations = SkiaDrawing.TextDecorations(parts[i], font, style, x + width, baseline);
            canvas.DrawPath(decorations, paint);
            width += font.MeasureText(parts[i]);
            if (i < parts.Length - 1) width = (MathF.Floor(width / 300) + 1) * 300;
        }
    }
}
