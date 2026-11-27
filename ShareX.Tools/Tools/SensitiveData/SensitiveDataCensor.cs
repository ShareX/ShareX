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

using ShareX.OCR;
using SkiaSharp;

namespace ShareX.Tools;

// A part of an image to censor. Points are the corners of its box: top-left, top-right, bottom-right, bottom-left.
// TextHeight is the height of the text in it, which sets how coarse pixelation and blur are.
public sealed record SensitiveDataArea(SensitiveDataType Type, SKPoint[] Points, float TextHeight)
{
    public SKRect Bounds => new(Points.Min(point => point.X), Points.Min(point => point.Y), Points.Max(point => point.X), Points.Max(point => point.Y));
}

// Reads the text of a screenshot and hides the sensitive data in it.
public static class SensitiveDataCensor
{
    // Text read with low confidence is still checked: missing a password costs more than censoring a smudge, and every
    // kind of data except labelled passwords has to pass its own checks anyway.
    public const float MinScore = 0.3f;

    // The box of a value is grown by this share of the text height on every side, so a character that sits a little
    // outside the estimated box is still covered.
    private const float Padding = 0.3f;

    // Reads the text in the image and censors in place what the options ask for. Returns the censored areas.
    public static IReadOnlyList<SensitiveDataArea> Censor(SKBitmap bitmap, SensitiveDataOptions options)
    {
        ArgumentNullException.ThrowIfNull(bitmap);
        ArgumentNullException.ThrowIfNull(options);

        SensitiveDataDetector detector = new(options);

        if (!detector.HasRules)
        {
            return [];
        }

        OCRResult result = PaddleOCREngine.Shared.Recognize(bitmap, MinScore);
        IReadOnlyList<SensitiveDataArea> areas = FindAreas(result, detector);
        Apply(bitmap, areas, options.CensorMethod);
        return areas;
    }

    // Finds the sensitive data in each line of text and works out where it is in the image. A value can span several
    // regions of a line, such as a label and its value read as separate pieces, so each region gets its own area.
    public static IReadOnlyList<SensitiveDataArea> FindAreas(OCRResult result, SensitiveDataDetector detector)
    {
        List<SensitiveDataArea> areas = [];

        foreach (OCRTextLine line in result.Lines)
        {
            IReadOnlyList<SensitiveDataMatch> matches = detector.Find(line.Text);

            if (matches.Count == 0)
            {
                continue;
            }

            // The line text is the texts of its regions joined with one space.
            int regionStart = 0;

            foreach (OCRTextRegion region in line.Regions)
            {
                int regionEnd = regionStart + region.Text.Length;
                float textHeight = GetTextHeight(region.Points);

                foreach (SensitiveDataMatch match in matches)
                {
                    int start = Math.Max(match.Index, regionStart);
                    int end = Math.Min(match.End, regionEnd);

                    if (end > start)
                    {
                        SKPoint[] box = region.GetTextBox(start - regionStart, end - start);
                        areas.Add(new SensitiveDataArea(match.Type, Grow(box, Math.Max(2, textHeight * Padding)), textHeight));
                    }
                }

                regionStart = regionEnd + 1;
            }
        }

        return areas;
    }

    public static void Apply(SKBitmap bitmap, IReadOnlyList<SensitiveDataArea> areas, SensitiveDataCensorMethod method)
    {
        if (areas.Count == 0)
        {
            return;
        }

        using SKCanvas canvas = new(bitmap);

        switch (method)
        {
            case SensitiveDataCensorMethod.Pixelate:
                foreach (SensitiveDataArea area in areas)
                {
                    Pixelate(bitmap, canvas, area);
                }
                break;
            case SensitiveDataCensorMethod.Blur:
                Blur(bitmap, canvas, areas);
                break;
            default:
                using (SKPaint paint = new() { Color = SKColors.Black, Style = SKPaintStyle.Fill, IsAntialias = false })
                {
                    foreach (SensitiveDataArea area in areas)
                    {
                        using SKPath path = CreatePath(area.Points);
                        canvas.DrawPath(path, paint);
                    }
                }
                break;
        }

        canvas.Flush();
    }

    // Blocks as big as the text, so each block is the average of a whole character or more and the text cannot be
    // matched back block by block. The blocks replace the pixels, so a see-through block cannot show the text under it.
    private static void Pixelate(SKBitmap bitmap, SKCanvas canvas, SensitiveDataArea area)
    {
        SKRectI rect = SKRectI.Intersect(SKRectI.Ceiling(area.Bounds, true), new SKRectI(0, 0, bitmap.Width, bitmap.Height));

        if (rect.IsEmpty)
        {
            return;
        }

        int blockSize = Math.Max(8, (int)Math.Ceiling(area.TextHeight));
        using SKPath path = CreatePath(area.Points);
        using SKPaint paint = new() { Style = SKPaintStyle.Fill, IsAntialias = false, BlendMode = SKBlendMode.Src };

        canvas.Save();
        canvas.ClipPath(path);

        for (int y = rect.Top; y < rect.Bottom; y += blockSize)
        {
            for (int x = rect.Left; x < rect.Right; x += blockSize)
            {
                SKRectI block = new(x, y, Math.Min(x + blockSize, rect.Right), Math.Min(y + blockSize, rect.Bottom));
                paint.Color = GetAverageColor(bitmap, block);
                canvas.DrawRect(block, paint);
            }
        }

        canvas.Restore();
    }

    private static SKColor GetAverageColor(SKBitmap bitmap, SKRectI rect)
    {
        long r = 0, g = 0, b = 0, a = 0;
        int count = 0;

        for (int y = rect.Top; y < rect.Bottom; y++)
        {
            for (int x = rect.Left; x < rect.Right; x++)
            {
                SKColor color = bitmap.GetPixel(x, y);
                r += color.Red;
                g += color.Green;
                b += color.Blue;
                a += color.Alpha;
                count++;
            }
        }

        return count == 0 ? SKColors.Black : new SKColor((byte)(r / count), (byte)(g / count), (byte)(b / count), (byte)(a / count));
    }

    // A blur as wide as the text. The image is copied first, so each area is blurred from the original pixels. The
    // edges are clamped and the blurred pixels replace the original ones: near the edge of the image a plain blur
    // fades to transparent, and drawn over the original it would let the text show through.
    private static void Blur(SKBitmap bitmap, SKCanvas canvas, IReadOnlyList<SensitiveDataArea> areas)
    {
        using SKBitmap copy = bitmap.Copy();
        using SKImage source = SKImage.FromBitmap(copy);

        foreach (SensitiveDataArea area in areas)
        {
            float sigma = Math.Max(4, area.TextHeight);
            using SKPath path = CreatePath(area.Points);
            using SKImageFilter blur = SKImageFilter.CreateBlur(sigma, sigma, SKShaderTileMode.Clamp);
            using SKPaint paint = new() { ImageFilter = blur, BlendMode = SKBlendMode.Src };

            canvas.Save();
            canvas.ClipPath(path);
            canvas.DrawImage(source, 0, 0, new SKSamplingOptions(SKFilterMode.Linear), paint);
            canvas.Restore();
        }
    }

    private static SKPath CreatePath(SKPoint[] points)
    {
        SKPath path = new();
        path.AddPoly(points, close: true);
        return path;
    }

    // The height of the text in a box: its shorter side, which is across the text for both horizontal and vertical text.
    private static float GetTextHeight(SKPoint[] box)
    {
        float width = SKPoint.Distance(box[0], box[1]);
        float height = SKPoint.Distance(box[0], box[3]);
        return Math.Min(width, height);
    }

    // Moves every side of a box outwards by the given distance.
    private static SKPoint[] Grow(SKPoint[] box, float distance)
    {
        SKPoint across = Normalize(box[1] - box[0], new SKPoint(1, 0));
        SKPoint down = Normalize(box[3] - box[0], new SKPoint(0, 1));
        SKPoint a = new(across.X * distance, across.Y * distance);
        SKPoint d = new(down.X * distance, down.Y * distance);

        return
        [
            box[0] - a - d,
            box[1] + a - d,
            box[2] + a + d,
            box[3] - a + d
        ];
    }

    private static SKPoint Normalize(SKPoint vector, SKPoint fallback)
    {
        float length = vector.Length;
        return length < 1e-3f ? fallback : new SKPoint(vector.X / length, vector.Y / length);
    }
}
