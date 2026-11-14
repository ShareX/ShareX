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
using System.Globalization;

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

internal static class DrawingEffectHelpers
{
    public static string ExpandVariables(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        return Environment.ExpandEnvironmentVariables(path.Trim());
    }

    public static string ExpandTextVariables(string? text, SKSizeI imageSize)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        string expanded = Environment.ExpandEnvironmentVariables(text);
        DateTime now = DateTime.Now;
        bool useTwelveHourClock = expanded.Contains("%pm", StringComparison.OrdinalIgnoreCase);
        int hour = now.Hour % 12;
        if (hour == 0)
        {
            hour = 12;
        }

        string currentMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(now.Month);
        string invariantMonth = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(now.Month);
        string currentDay = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(now.DayOfWeek);
        string invariantDay = CultureInfo.InvariantCulture.DateTimeFormat.GetDayName(now.DayOfWeek);

        (string Token, string Value)[] replacements =
        [
            ("%width", imageSize.Width.ToString(CultureInfo.InvariantCulture)),
            ("%height", imageSize.Height.ToString(CultureInfo.InvariantCulture)),
            ("%unix", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            ("%mon2", invariantMonth),
            ("%mon", currentMonth),
            ("%yy", now.ToString("yy", CultureInfo.InvariantCulture)),
            ("%y", now.Year.ToString(CultureInfo.InvariantCulture)),
            ("%mo", now.Month.ToString("00", CultureInfo.InvariantCulture)),
            ("%mi", now.Minute.ToString("00", CultureInfo.InvariantCulture)),
            ("%ms", now.Millisecond.ToString("000", CultureInfo.InvariantCulture)),
            ("%pm", now.Hour >= 12 ? "PM" : "AM"),
            ("%w2", invariantDay),
            ("%w", currentDay),
            ("%d", now.Day.ToString("00", CultureInfo.InvariantCulture)),
            ("%h", (useTwelveHourClock ? hour : now.Hour).ToString("00", CultureInfo.InvariantCulture)),
            ("%s", now.Second.ToString("00", CultureInfo.InvariantCulture)),
            ("%un", Environment.UserName),
            ("%uln", Environment.UserDomainName),
            ("%cn", Environment.MachineName),
            ("%n", Environment.NewLine)
        ];

        foreach ((string token, string value) in replacements)
        {
            expanded = expanded.Replace(token, value, StringComparison.OrdinalIgnoreCase);
        }

        return expanded;
    }

    public static SKPointI GetPosition(DrawingPlacement placement, SKPointI offset, SKSizeI backgroundSize, SKSizeI objectSize)
    {
        int midX = (int)Math.Round((backgroundSize.Width / 2f) - (objectSize.Width / 2f));
        int midY = (int)Math.Round((backgroundSize.Height / 2f) - (objectSize.Height / 2f));
        int right = backgroundSize.Width - objectSize.Width;
        int bottom = backgroundSize.Height - objectSize.Height;

        return placement switch
        {
            DrawingPlacement.TopCenter => new SKPointI(midX, offset.Y),
            DrawingPlacement.TopRight => new SKPointI(right - offset.X, offset.Y),
            DrawingPlacement.MiddleLeft => new SKPointI(offset.X, midY),
            DrawingPlacement.MiddleCenter => new SKPointI(midX, midY),
            DrawingPlacement.MiddleRight => new SKPointI(right - offset.X, midY),
            DrawingPlacement.BottomLeft => new SKPointI(offset.X, bottom - offset.Y),
            DrawingPlacement.BottomCenter => new SKPointI(midX, bottom - offset.Y),
            DrawingPlacement.BottomRight => new SKPointI(right - offset.X, bottom - offset.Y),
            _ => new SKPointI(offset.X, offset.Y)
        };
    }

    public static SKSizeI ApplyAspectRatio(int width, int height, SKBitmap bitmap)
    {
        int newWidth;
        int newHeight;

        if (width == 0)
        {
            newWidth = (int)Math.Round((float)height / bitmap.Height * bitmap.Width);
            newHeight = height;
        }
        else if (height == 0)
        {
            newWidth = width;
            newHeight = (int)Math.Round((float)width / bitmap.Width * bitmap.Height);
        }
        else
        {
            newWidth = width;
            newHeight = height;
        }

        return new SKSizeI(newWidth, newHeight);
    }

    public static SKBlendMode GetBlendMode(DrawingCompositingMode compositingMode)
    {
        return compositingMode == DrawingCompositingMode.SourceCopy
            ? SKBlendMode.Src
            : SKBlendMode.SrcOver;
    }

    public static SKRectI Inflate(SKRectI rect, int amount)
    {
        return new SKRectI(rect.Left - amount, rect.Top - amount, rect.Right + amount, rect.Bottom + amount);
    }

    public static bool Contains(SKRectI outerRect, SKRectI innerRect)
    {
        return innerRect.Left >= outerRect.Left &&
               innerRect.Top >= outerRect.Top &&
               innerRect.Right <= outerRect.Right &&
               innerRect.Bottom <= outerRect.Bottom;
    }

    public static bool Intersects(SKRectI left, SKRectI right)
    {
        return left.Left < right.Right &&
               left.Right > right.Left &&
               left.Top < right.Bottom &&
               left.Bottom > right.Top;
    }

    public static SKBitmap RotateFlip(SKBitmap source, DrawingImageRotateFlipType rotateFlip)
    {
        return rotateFlip switch
        {
            DrawingImageRotateFlipType.Rotate90 => Rotate90(source),
            DrawingImageRotateFlipType.Rotate180 => Rotate180(source),
            DrawingImageRotateFlipType.Rotate270 => Rotate270(source),
            DrawingImageRotateFlipType.FlipX => FlipHorizontal(source),
            DrawingImageRotateFlipType.FlipY => FlipVertical(source),
            DrawingImageRotateFlipType.Rotate90FlipX => Rotate90FlipHorizontal(source),
            DrawingImageRotateFlipType.Rotate90FlipY => Rotate90FlipVertical(source),
            _ => source.Copy()
        };
    }

    private static SKBitmap Rotate90(SKBitmap source)
    {
        SKBitmap result = new SKBitmap(source.Height, source.Width, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Translate(source.Height, 0);
        canvas.RotateDegrees(90);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap Rotate180(SKBitmap source)
    {
        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Translate(source.Width, source.Height);
        canvas.RotateDegrees(180);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap Rotate270(SKBitmap source)
    {
        SKBitmap result = new SKBitmap(source.Height, source.Width, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Translate(0, source.Width);
        canvas.RotateDegrees(270);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap FlipHorizontal(SKBitmap source)
    {
        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Translate(source.Width, 0);
        canvas.Scale(-1, 1);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap FlipVertical(SKBitmap source)
    {
        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Translate(0, source.Height);
        canvas.Scale(1, -1);
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }

    private static SKBitmap Rotate90FlipHorizontal(SKBitmap source)
    {
        using SKBitmap rotated = Rotate90(source);
        return FlipHorizontal(rotated);
    }

    private static SKBitmap Rotate90FlipVertical(SKBitmap source)
    {
        using SKBitmap rotated = Rotate90(source);
        return FlipVertical(rotated);
    }
}