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

public sealed class StripeOverlayImageEffect : ImageEffectBase
{
    public override string Id => "stripe_overlay";
    public override string Name => "Stripe overlay";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.blinds;
    public override string Description => "Overlays diagonal stripes on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<StripeOverlayImageEffect>("width", "Stripe width", 2, 50, 10, (e, v) => e.StripeWidth = v),
        EffectParameters.IntSlider<StripeOverlayImageEffect>("gap", "Gap width", 2, 50, 10, (e, v) => e.GapWidth = v),
        EffectParameters.IntSlider<StripeOverlayImageEffect>("angle", "Angle", 0, 180, 45, (e, v) => e.Angle = v),
        EffectParameters.FloatSlider<StripeOverlayImageEffect>("opacity", "Opacity", 0, 100, 40, (e, v) => e.Opacity = v),
        EffectParameters.Color<StripeOverlayImageEffect>("color", "Color", SkiaSharp.SKColors.Black, (e, v) => e.Color = v)
    ];

    public int StripeWidth { get; set; } = 10;
    public int GapWidth { get; set; } = 10;
    public int Angle { get; set; } = 45;
    public float Opacity { get; set; } = 40f;
    public SKColor Color { get; set; } = SKColors.Black;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        int period = Math.Max(4, StripeWidth + GapWidth);
        int w = source.Width, h = source.Height;

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        byte a = (byte)(Color.Alpha * alpha);
        using SKPaint paint = new() { Color = Color.WithAlpha(a), IsAntialias = true, Style = SKPaintStyle.Fill };

        float diagonal = MathF.Sqrt(w * w + h * h);
        int count = (int)(diagonal / period) + 2;

        canvas.Save();
        canvas.RotateDegrees(Angle, w / 2f, h / 2f);

        float startX = -diagonal / 2f + w / 2f;
        float startY = -diagonal / 2f + h / 2f;

        for (int i = 0; i < count; i++)
        {
            float x = startX + i * period;
            canvas.DrawRect(x, startY, StripeWidth, diagonal * 2, paint);
        }

        canvas.Restore();
        return result;
    }
}