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

namespace ShareX.OCR;

// One detected piece of text. Points are the corners of its box in image coordinates: top-left, top-right,
// bottom-right, bottom-left.
public sealed record OCRTextRegion(SKPoint[] Points, string Text, float Score)
{
    public SKRect Bounds { get; } = Geometry.GetBounds(Points);
}

public sealed class OCRResult
{
    // Two regions are on the same line when they share at least this much of the shorter one's height.
    private const float SameLineOverlap = 0.5f;

    public IReadOnlyList<OCRTextRegion> Regions { get; }
    public IReadOnlyList<string> Lines { get; }

    public OCRResult(IReadOnlyList<OCRTextRegion> regions)
    {
        Regions = regions;
        Lines = GroupLines(regions);
    }

    public string GetText(bool singleLine = false)
    {
        return string.Join(singleLine ? " " : Environment.NewLine, Lines);
    }

    public override string ToString() => GetText();

    // Joins regions that sit side by side into lines, read top to bottom and left to right.
    private static List<string> GroupLines(IReadOnlyList<OCRTextRegion> regions)
    {
        List<List<OCRTextRegion>> lines = [];

        foreach (OCRTextRegion region in regions.OrderBy(region => region.Bounds.MidY))
        {
            List<OCRTextRegion>? line = lines.LastOrDefault();

            if (line != null && IsSameLine(line, region))
            {
                line.Add(region);
            }
            else
            {
                lines.Add([region]);
            }
        }

        return [.. lines.Select(line => string.Join(" ", line.OrderBy(region => region.Bounds.Left).Select(region => region.Text)))];
    }

    private static bool IsSameLine(List<OCRTextRegion> line, OCRTextRegion region)
    {
        float top = line.Min(item => item.Bounds.Top);
        float bottom = line.Max(item => item.Bounds.Bottom);
        float overlap = Math.Min(bottom, region.Bounds.Bottom) - Math.Max(top, region.Bounds.Top);
        float height = Math.Min(bottom - top, region.Bounds.Height);
        return height > 0 && overlap >= height * SameLineOverlap;
    }
}
