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
using Xunit;

namespace ShareX.Tools.Tests;

public sealed class PaddleOCRTests
{
    [Fact]
    public async Task ReadsRenderedTextAndReportsWhereItIs()
    {
        // ONNX Runtime has no Intel Mac build; OCR uses Vision there (OCRHelper).
        if (!PaddleOCREngine.IsAvailable) return;

        using SKBitmap image = new(640, 220);
        using (SKCanvas canvas = new(image))
        {
            using SKFont font = new(SKTypeface.Default, 48);
            using SKPaint paint = new() { Color = SKColors.Black, IsAntialias = true };
            canvas.Clear(SKColors.White);
            canvas.DrawText("SHAREX TEST", 30, 80, font, paint);
            canvas.DrawText("12345", 30, 170, font, paint);
        }

        OCRResult result = await OCRHelper.Recognize(image);

        Assert.Equal(["SHAREX TEST", "12345"], result.Lines.Select(line => line.Text));
        Assert.Equal(2, result.Regions.Count);
        Assert.All(result.Regions, region => Assert.True(image.Info.Rect.Contains(SKRectI.Round(region.Bounds))));
        Assert.Equal("SHAREX TEST 12345", result.GetText(singleLine: true));
    }

    [Fact]
    public void TextFromAnEngineWithoutPositionsKeepsItsLines()
    {
        OCRResult result = OCRResult.FromLines(["first", "second"]);

        Assert.Empty(result.Regions);
        Assert.Equal("first second", result.GetText(singleLine: true));
    }
}
