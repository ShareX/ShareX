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

public sealed class ConfettiImageEffect : ImageEffectBase
{
    public override string Id => "confetti";
    public override string Name => "Confetti";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.party_popper;
    public override string Description => "Scatters colorful confetti pieces over the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ConfettiImageEffect>("count", "Count", 10, 1000, 150, (e, v) => e.Count = v),
        EffectParameters.IntSlider<ConfettiImageEffect>("min_size", "Min size", 2, 40, 4, (e, v) => e.MinSize = v),
        EffectParameters.IntSlider<ConfettiImageEffect>("max_size", "Max size", 4, 80, 12, (e, v) => e.MaxSize = v),
        EffectParameters.FloatSlider<ConfettiImageEffect>("opacity", "Opacity", 0, 100, 90, (e, v) => e.Opacity = v),
        EffectParameters.Bool<ConfettiImageEffect>("background", "Background", false, (e, v) => e.Background = v)
    ];

    public int Count { get; set; } = 150;
    public int MinSize { get; set; } = 4;
    public int MaxSize { get; set; } = 12;
    public float Opacity { get; set; } = 90f;
    public bool Background { get; set; }

    private static readonly SKColor[] ConfettiColors =
    [
        new SKColor(255, 56, 96),   // red-pink
        new SKColor(255, 165, 0),   // orange
        new SKColor(255, 215, 0),   // gold
        new SKColor(0, 200, 83),    // green
        new SKColor(0, 176, 255),   // blue
        new SKColor(156, 39, 176),  // purple
        new SKColor(255, 64, 129),  // pink
        new SKColor(0, 230, 118),   // mint
        new SKColor(255, 234, 0),   // yellow
        new SKColor(29, 233, 182),  // teal
    ];

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Count <= 0) return source.Copy();

        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        int minSize = Math.Max(2, Math.Min(MinSize, MaxSize));
        int maxSize = Math.Max(minSize, MaxSize);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        if (Background)
        {
            canvas.Clear(SKColors.Transparent);
        }

        using SKPaint paint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };

        for (int i = 0; i < Count; i++)
        {
            float x = Random.Shared.Next(0, source.Width);
            float y = Random.Shared.Next(0, source.Height);
            float w = Random.Shared.Next(minSize, maxSize + 1);
            float h = Random.Shared.Next(minSize, maxSize + 1);
            float angle = Random.Shared.Next(0, 360);

            SKColor color = ConfettiColors[Random.Shared.Next(ConfettiColors.Length)];
            paint.Color = color.WithAlpha((byte)(color.Alpha * alpha));

            canvas.Save();
            canvas.Translate(x, y);
            canvas.RotateDegrees(angle);

            int shape = Random.Shared.Next(3);
            switch (shape)
            {
                case 0: // rectangle
                    canvas.DrawRect(-w / 2, -h / 2, w, h, paint);
                    break;
                case 1: // circle
                    canvas.DrawOval(0, 0, w / 2, h / 2, paint);
                    break;
                default: // triangle
                    using (SKPath path = new())
                    {
                        path.MoveTo(0, -h / 2);
                        path.LineTo(w / 2, h / 2);
                        path.LineTo(-w / 2, h / 2);
                        path.Close();
                        canvas.DrawPath(path, paint);
                    }
                    break;
            }

            canvas.Restore();
        }

        if (Background)
        {
            canvas.DrawBitmap(source, 0, 0);
        }

        return result;
    }
}