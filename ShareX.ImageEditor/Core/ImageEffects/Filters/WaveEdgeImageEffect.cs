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

public sealed class WaveEdgeImageEffect : ImageEffectBase
{
    public override string Id => "wave_edge";
    public override string Name => "Wave edge";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.waves_ladder;
    public override string Description => "Adds a wavy edge to the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<WaveEdgeImageEffect>("depth", "Depth", 1, 100, 15, (effect, value) => effect.Depth = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<WaveEdgeImageEffect>("range", "Range", 1, 100, 20, (effect, value) => effect.Range = value, isSnapToTickEnabled: false),
        EffectParameters.Bool<WaveEdgeImageEffect>("top", "Top", true, (effect, value) => effect.Top = value),
        EffectParameters.Bool<WaveEdgeImageEffect>("right", "Right", true, (effect, value) => effect.Right = value),
        EffectParameters.Bool<WaveEdgeImageEffect>("bottom", "Bottom", true, (effect, value) => effect.Bottom = value),
        EffectParameters.Bool<WaveEdgeImageEffect>("left", "Left", true, (effect, value) => effect.Left = value)
    ];

    public int Depth { get; set; } = 15;
    public int Range { get; set; } = 20;
    public bool Top { get; set; } = true;
    public bool Right { get; set; } = true;
    public bool Bottom { get; set; } = true;
    public bool Left { get; set; } = true;

    public WaveEdgeImageEffect()
    {
    }

    public WaveEdgeImageEffect(int depth, int range, bool top, bool right, bool bottom, bool left)
    {
        Depth = depth;
        Range = range;
        Top = top;
        Right = right;
        Bottom = bottom;
        Left = left;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Depth < 1 || Range < 1) return source.Copy();
        if (!Top && !Right && !Bottom && !Left) return source.Copy();

        List<SKPoint> points = [];

        int horizontalWaveCount = Math.Max(2, (source.Width / Range + 1) / 2 * 2) - 1;
        int verticalWaveCount = Math.Max(2, (source.Height / Range + 1) / 2 * 2) - 1;
        int horizontalWaveRange = source.Width / horizontalWaveCount;
        int verticalWaveRange = source.Height / verticalWaveCount;

        int step = Math.Min(Math.Max(1, Range / Depth), 10);

        static float WaveFunction(int t, int max, int depth)
        {
            return (float)((1 - Math.Cos(t * Math.PI / max)) * depth / 2d);
        }

        if (Top)
        {
            int startX = Left ? Depth : 0;
            int endX = Right ? source.Width - Depth : source.Width;
            for (int x = startX; x < endX; x += step)
            {
                points.Add(new SKPoint(x, WaveFunction(x, horizontalWaveRange, Depth)));
            }

            points.Add(new SKPoint(endX, WaveFunction(endX, horizontalWaveRange, Depth)));
        }
        else
        {
            points.Add(new SKPoint(0, 0));
        }

        if (Right)
        {
            int startY = Top ? Depth : 0;
            int endY = Bottom ? source.Height - Depth : source.Height;
            for (int y = startY; y < endY; y += step)
            {
                points.Add(new SKPoint(source.Width - Depth + WaveFunction(y, verticalWaveRange, Depth), y));
            }

            points.Add(new SKPoint(source.Width - Depth + WaveFunction(endY, verticalWaveRange, Depth), endY));
        }
        else
        {
            points.Add(new SKPoint(source.Width, points[^1].Y));
        }

        if (Bottom)
        {
            int startX = Right ? source.Width - Depth : source.Width;
            int endX = Left ? Depth : 0;
            for (int x = startX; x >= endX; x -= step)
            {
                points.Add(new SKPoint(x, source.Height - Depth + WaveFunction(x, horizontalWaveRange, Depth)));
            }

            points.Add(new SKPoint(endX, source.Height - Depth + WaveFunction(endX, horizontalWaveRange, Depth)));
        }
        else
        {
            points.Add(new SKPoint(points[^1].X, source.Height));
        }

        if (Left)
        {
            int startY = Bottom ? source.Height - Depth : source.Height;
            int endY = Top ? Depth : 0;
            for (int y = startY; y >= endY; y -= step)
            {
                points.Add(new SKPoint(WaveFunction(y, verticalWaveRange, Depth), y));
            }

            points.Add(new SKPoint(WaveFunction(endY, verticalWaveRange, Depth), endY));
        }
        else
        {
            points.Add(new SKPoint(0, points[^1].Y));
        }

        if (!Top)
        {
            points[0] = new SKPoint(points[^1].X, 0);
        }

        SKBitmap result = new(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        using SKShader shader = SKShader.CreateBitmap(source, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
        using SKPaint paint = new()
        {
            Shader = shader,
            IsAntialias = true
        };

        using SKPath path = new();
        if (points.Count <= 2)
        {
            return source.Copy();
        }

        path.AddPoly(points.ToArray(), true);
        canvas.DrawPath(path, paint);
        return result;
    }
}