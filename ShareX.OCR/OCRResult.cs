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
using System.Text;

namespace ShareX.OCR;

// One detected piece of text. Points are the corners of its box in image coordinates: top-left, top-right,
// bottom-right, bottom-left.
public sealed record OCRTextRegion(SKPoint[] Points, string Text, float Score)
{
    public SKRect Bounds { get; } = Geometry.GetBounds(Points);
}

// Regions that sit side by side, joined in reading order. Height is the typical text height of the line.
// StartsParagraph is true when the line is set apart from the one above it, so it begins a new block of text.
public sealed record OCRTextLine(string Text, SKRect Bounds, float Height, bool StartsParagraph);

public sealed class OCRResult
{
    // Two regions are on the same line when they share at least this much of the shorter one's height.
    private const float SameLineOverlap = 0.5f;

    // Line spacing assumed inside a paragraph, as a share of the text height, when there are too few lines to measure it.
    private const float DefaultLineGap = 0.35f;

    // A gap splits a paragraph when it is at least twice the line spacing and wider by at least this share of the text height.
    private const float ParagraphGapMargin = 0.35f;

    // Lines whose text heights differ this much, such as a heading and the text under it, are in different paragraphs.
    private const float ParagraphHeightRatio = 1.6f;

    public IReadOnlyList<OCRTextRegion> Regions { get; }
    public IReadOnlyList<OCRTextLine> Lines { get; }

    public OCRResult(IReadOnlyList<OCRTextRegion> regions)
    {
        Regions = regions;
        Lines = GroupLines(regions);
    }

    private OCRResult(IReadOnlyList<string> lines)
    {
        Regions = [];
        Lines = lines.Select(line => new OCRTextLine(line, default, 0, false)).ToList();
    }

    // Text from an engine that does not report where it found it.
    public static OCRResult FromLines(IReadOnlyList<string> lines) => new(lines);

    // Lines are joined with a new line, and with an empty line between paragraphs when separateParagraphs is set.
    public string GetText(bool singleLine = false, bool separateParagraphs = true)
    {
        if (singleLine)
        {
            return string.Join(" ", Lines.Select(line => line.Text));
        }

        StringBuilder text = new();

        foreach (OCRTextLine line in Lines)
        {
            if (text.Length > 0)
            {
                text.Append(Environment.NewLine);

                if (separateParagraphs && line.StartsParagraph)
                {
                    text.Append(Environment.NewLine);
                }
            }

            text.Append(line.Text);
        }

        return text.ToString();
    }

    public override string ToString() => GetText();

    // Joins regions that sit side by side into lines, read top to bottom and left to right.
    private static List<OCRTextLine> GroupLines(IReadOnlyList<OCRTextRegion> regions)
    {
        List<List<OCRTextRegion>> groups = [];

        foreach (OCRTextRegion region in regions.OrderBy(region => region.Bounds.MidY))
        {
            List<OCRTextRegion>? group = groups.LastOrDefault();

            if (group != null && IsSameLine(group, region))
            {
                group.Add(region);
            }
            else
            {
                groups.Add([region]);
            }
        }

        List<OCRTextLine> lines = [.. groups.Select(CreateLine)];
        return MarkParagraphs(lines);
    }

    private static OCRTextLine CreateLine(List<OCRTextRegion> group)
    {
        List<OCRTextRegion> ordered = [.. group.OrderBy(region => region.Bounds.Left)];
        string text = string.Join(" ", ordered.Select(region => region.Text));

        SKRect bounds = ordered[0].Bounds;
        foreach (OCRTextRegion region in ordered)
        {
            bounds.Union(region.Bounds);
        }

        // The median keeps one tall or short box, such as a symbol, from deciding the height of the whole line.
        float height = Median(ordered.Select(region => region.Bounds.Height));
        return new OCRTextLine(text, bounds, height, false);
    }

    private static bool IsSameLine(List<OCRTextRegion> line, OCRTextRegion region)
    {
        float top = line.Min(item => item.Bounds.Top);
        float bottom = line.Max(item => item.Bounds.Bottom);
        float overlap = Math.Min(bottom, region.Bounds.Bottom) - Math.Max(top, region.Bounds.Top);
        float height = Math.Min(bottom - top, region.Bounds.Height);
        return height > 0 && overlap >= height * SameLineOverlap;
    }

    // Decides where paragraphs begin. Text is set in blocks whose lines are evenly spaced, so the usual line spacing is
    // measured from the image and a line starts a paragraph when it is clearly further from the line above than that.
    // A change of text size, such as after a heading, or a line that does not sit under the one above it at all,
    // such as text in another part of the screen, starts a paragraph too.
    private static List<OCRTextLine> MarkParagraphs(List<OCRTextLine> lines)
    {
        if (lines.Count < 2)
        {
            return lines;
        }

        float lineGap = GetLineGap(lines);
        List<OCRTextLine> result = [lines[0]];

        for (int i = 1; i < lines.Count; i++)
        {
            OCRTextLine previous = lines[i - 1];
            OCRTextLine line = lines[i];
            float height = Math.Min(previous.Height, line.Height);

            bool startsParagraph = height > 0 &&
                (GetGap(previous, line) / height > Math.Max(lineGap * 2, lineGap + ParagraphGapMargin) ||
                Math.Max(previous.Height, line.Height) / height >= ParagraphHeightRatio ||
                !OverlapsHorizontally(previous, line));

            result.Add(line with { StartsParagraph = startsParagraph });
        }

        return result;
    }

    // The line spacing inside paragraphs, as a share of the text height. Most gaps between lines are inside a
    // paragraph, and the gaps between paragraphs are the wide ones, so a low percentile of the gaps is the spacing.
    // Only pairs of lines that could be in one paragraph, under each other and of a similar size, are measured.
    private static float GetLineGap(List<OCRTextLine> lines)
    {
        List<float> gaps = [];

        for (int i = 1; i < lines.Count; i++)
        {
            OCRTextLine previous = lines[i - 1];
            OCRTextLine line = lines[i];
            float height = Math.Min(previous.Height, line.Height);

            if (height > 0 && Math.Max(previous.Height, line.Height) / height < ParagraphHeightRatio && OverlapsHorizontally(previous, line))
            {
                gaps.Add(Math.Max(GetGap(previous, line), 0) / height);
            }
        }

        if (gaps.Count < 2)
        {
            return DefaultLineGap;
        }

        gaps.Sort();
        return gaps[(gaps.Count - 1) / 4];
    }

    private static float GetGap(OCRTextLine upper, OCRTextLine lower)
    {
        return lower.Bounds.Top - upper.Bounds.Bottom;
    }

    private static bool OverlapsHorizontally(OCRTextLine a, OCRTextLine b)
    {
        return Math.Min(a.Bounds.Right, b.Bounds.Right) > Math.Max(a.Bounds.Left, b.Bounds.Left);
    }

    private static float Median(IEnumerable<float> values)
    {
        List<float> sorted = [.. values.Order()];
        int middle = sorted.Count / 2;
        return sorted.Count % 2 == 1 ? sorted[middle] : (sorted[middle - 1] + sorted[middle]) / 2;
    }
}
