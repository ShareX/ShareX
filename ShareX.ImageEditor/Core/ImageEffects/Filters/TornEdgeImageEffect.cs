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

using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class TornEdgeImageEffect : ImageEffectBase
{
    public override string Id => "torn_edge";
    public override string Name => "Torn edge";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.scissors_line_dashed;
    public override string Description => "Adds a torn edge border effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<TornEdgeImageEffect>("depth", "Depth", 1, 100, 20, (effect, value) => effect.Depth = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<TornEdgeImageEffect>("range", "Range", 1, 100, 20, (effect, value) => effect.Range = value, isSnapToTickEnabled: false),
        EffectParameters.Bool<TornEdgeImageEffect>("top", "Top", true, (effect, value) => effect.Top = value),
        EffectParameters.Bool<TornEdgeImageEffect>("right", "Right", true, (effect, value) => effect.Right = value),
        EffectParameters.Bool<TornEdgeImageEffect>("bottom", "Bottom", true, (effect, value) => effect.Bottom = value),
        EffectParameters.Bool<TornEdgeImageEffect>("left", "Left", true, (effect, value) => effect.Left = value),
        EffectParameters.Bool<TornEdgeImageEffect>("curved", "Curved edges", false, (effect, value) => effect.Curved = value)
    ];

    public int Depth { get; set; } = 20;
    public int Range { get; set; } = 20;
    public bool Top { get; set; } = true;
    public bool Right { get; set; } = true;
    public bool Bottom { get; set; } = true;
    public bool Left { get; set; } = true;
    public bool Curved { get; set; }

    public TornEdgeImageEffect()
    {
    }

    public TornEdgeImageEffect(int depth, int range, bool top, bool right, bool bottom, bool left, bool curved)
    {
        Depth = depth;
        Range = range;
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
        Curved = curved;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Depth < 1 || Range < 1) return source.Copy();
        if (!Top && !Right && !Bottom && !Left) return source.Copy();

        int horizontalTornCount = source.Width / Range;
        int verticalTornCount = source.Height / Range;

        if (horizontalTornCount < 2 && verticalTornCount < 2)
        {
            return source.Copy();
        }

        List<SKPoint> points = [];
        Random rand = Random.Shared;

        if (Top && horizontalTornCount > 1)
        {
            int startX = (Left && verticalTornCount > 1) ? Depth : 0;
            int endX = (Right && verticalTornCount > 1) ? source.Width - Depth : source.Width;
            for (int x = startX; x < endX; x += Range)
            {
                points.Add(new SKPoint(x, rand.Next(0, Depth + 1)));
            }
        }
        else
        {
            points.Add(new SKPoint(0, 0));
            points.Add(new SKPoint(source.Width, 0));
        }

        if (Right && verticalTornCount > 1)
        {
            int startY = (Top && horizontalTornCount > 1) ? Depth : 0;
            int endY = (Bottom && horizontalTornCount > 1) ? source.Height - Depth : source.Height;
            for (int y = startY; y < endY; y += Range)
            {
                points.Add(new SKPoint(source.Width - Depth + rand.Next(0, Depth + 1), y));
            }
        }
        else
        {
            points.Add(new SKPoint(source.Width, 0));
            points.Add(new SKPoint(source.Width, source.Height));
        }

        if (Bottom && horizontalTornCount > 1)
        {
            int startX = (Right && verticalTornCount > 1) ? source.Width - Depth : source.Width;
            int endX = (Left && verticalTornCount > 1) ? Depth : 0;
            for (int x = startX; x >= endX; x -= Range)
            {
                points.Add(new SKPoint(x, source.Height - Depth + rand.Next(0, Depth + 1)));
            }
        }
        else
        {
            points.Add(new SKPoint(source.Width, source.Height));
            points.Add(new SKPoint(0, source.Height));
        }

        if (Left && verticalTornCount > 1)
        {
            int startY = (Bottom && horizontalTornCount > 1) ? source.Height - Depth : source.Height;
            int endY = (Top && horizontalTornCount > 1) ? Depth : 0;
            for (int y = startY; y >= endY; y -= Range)
            {
                points.Add(new SKPoint(rand.Next(0, Depth + 1), y));
            }
        }
        else
        {
            points.Add(new SKPoint(0, source.Height));
            points.Add(new SKPoint(0, 0));
        }

        List<SKPoint> distinctPoints = [];
        if (points.Count > 0)
        {
            distinctPoints.Add(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i] != points[i - 1])
                {
                    distinctPoints.Add(points[i]);
                }
            }

            if (distinctPoints.Count > 1 && distinctPoints[^1] == distinctPoints[0])
            {
                distinctPoints.RemoveAt(distinctPoints.Count - 1);
            }
        }

        SKPoint[] pts = distinctPoints.ToArray();
        SKBitmap result = new(source.Width, source.Height);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        using SKShader shader = SKShader.CreateBitmap(source, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
        using SKPaint paint = new()
        {
            Shader = shader,
            IsAntialias = true
        };

        using SKPath path = new();
        if (pts.Length > 2)
        {
            if (Curved)
            {
                SKPoint lastPoint = pts[^1];
                SKPoint firstPoint = pts[0];
                SKPoint currentMid = new((lastPoint.X + firstPoint.X) / 2, (lastPoint.Y + firstPoint.Y) / 2);

                path.MoveTo(currentMid);

                for (int i = 0; i < pts.Length; i++)
                {
                    SKPoint current = pts[i];
                    SKPoint next = pts[(i + 1) % pts.Length];
                    SKPoint nextMid = new((current.X + next.X) / 2, (current.Y + next.Y) / 2);

                    path.QuadTo(current, nextMid);
                    currentMid = nextMid;
                }

                path.Close();
            }
            else
            {
                path.AddPoly(pts, true);
            }

            canvas.DrawPath(path, paint);
        }

        return result;
    }
}