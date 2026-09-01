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

namespace ShareX.ImageEditor.Core.ImageEffects.Drawings;

public sealed class FakeCursorImageEffect : ImageEffectBase
{
    public override string Id => "fake_cursor";
    public override string Name => "Fake cursor";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.mouse_pointer;
    public override string Description => "Draws fake mouse cursors at random positions on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<FakeCursorImageEffect>("count", "Count", 1, 200, 1, (e, v) => e.Count = v),
        EffectParameters.FloatSlider<FakeCursorImageEffect>("scale", "Scale", 0.5, 5, 1, (e, v) => e.Scale = v, tickFrequency: 0.5),
        EffectParameters.Bool<FakeCursorImageEffect>("random_size", "Random size", false, (e, v) => e.RandomSize = v),
        EffectParameters.FloatSlider<FakeCursorImageEffect>("random_size_min", "Random size min", 0.5, 5, 0.5, (e, v) => e.RandomSizeMin = v, tickFrequency: 0.5),
        EffectParameters.FloatSlider<FakeCursorImageEffect>("random_size_max", "Random size max", 0.5, 5, 2, (e, v) => e.RandomSizeMax = v, tickFrequency: 0.5),
        EffectParameters.Bool<FakeCursorImageEffect>("random_angle", "Random angle", false, (e, v) => e.RandomAngle = v),
        EffectParameters.IntNumeric<FakeCursorImageEffect>("random_angle_min", "Random angle min", 0, 360, 0, (e, v) => e.RandomAngleMin = v),
        EffectParameters.IntNumeric<FakeCursorImageEffect>("random_angle_max", "Random angle max", 0, 360, 360, (e, v) => e.RandomAngleMax = v),
        EffectParameters.Color<FakeCursorImageEffect>("fill_color", "Fill color", SKColors.White, (e, v) => e.FillColor = v),
        EffectParameters.Color<FakeCursorImageEffect>("border_color", "Border color", SKColors.Black, (e, v) => e.BorderColor = v)
    ];

    public int Count { get; set; } = 1;
    public float Scale { get; set; } = 1f;
    public bool RandomSize { get; set; }
    public float RandomSizeMin { get; set; } = 0.5f;
    public float RandomSizeMax { get; set; } = 2f;
    public bool RandomAngle { get; set; }
    public int RandomAngleMin { get; set; }
    public int RandomAngleMax { get; set; } = 360;
    public SKColor FillColor { get; set; } = SKColors.White;
    public SKColor BorderColor { get; set; } = SKColors.Black;

    // Standard Windows arrow cursor shape (normalized to ~21x25 base)
    private static readonly SKPoint[] CursorOutline =
    [
        new(0, 0),
        new(0, 20.4f),
        new(5.7f, 15.6f),
        new(9.6f, 23.4f),
        new(12.6f, 22.2f),
        new(8.7f, 14.4f),
        new(15.3f, 14.4f),
    ];

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int count = Math.Clamp(Count, 1, 200);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        for (int i = 0; i < count; i++)
        {
            float scale = RandomSize
                ? RandomSizeMin + Random.Shared.NextSingle() * (Math.Max(RandomSizeMin, RandomSizeMax) - RandomSizeMin)
                : Scale;
            scale = Math.Clamp(scale, 0.5f, 5f);

            float cursorWidth = 15.3f * scale;
            float cursorHeight = 23.4f * scale;

            float x = Random.Shared.Next(0, Math.Max(1, source.Width - (int)cursorWidth));
            float y = Random.Shared.Next(0, Math.Max(1, source.Height - (int)cursorHeight));

            canvas.Save();
            canvas.Translate(x, y);

            if (RandomAngle)
            {
                int minA = Math.Min(RandomAngleMin, RandomAngleMax);
                int maxA = Math.Max(RandomAngleMin, RandomAngleMax);
                int angle = minA == maxA ? minA : Random.Shared.Next(minA, maxA);
                float pivotX = cursorWidth / 2f;
                float pivotY = cursorHeight / 2f;
                canvas.Translate(pivotX, pivotY);
                canvas.RotateDegrees(angle);
                canvas.Translate(-pivotX, -pivotY);
            }

            canvas.Scale(scale);

            using SKPath cursorPath = new();
            cursorPath.MoveTo(CursorOutline[0]);
            for (int j = 1; j < CursorOutline.Length; j++)
            {
                cursorPath.LineTo(CursorOutline[j]);
            }
            cursorPath.Close();

            // Draw border/outline
            using (SKPaint borderPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = BorderColor,
                StrokeWidth = 1.5f,
                StrokeJoin = SKStrokeJoin.Round
            })
            {
                canvas.DrawPath(cursorPath, borderPaint);
            }

            // Draw fill
            using (SKPaint fillPaint = new()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = FillColor
            })
            {
                canvas.DrawPath(cursorPath, fillPaint);
            }

            canvas.Restore();
        }

        return result;
    }
}