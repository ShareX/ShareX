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

public sealed class GradientOverlayImageEffect : ImageEffectBase
{
    public override string Id => "gradient_overlay";
    public override string Name => "Gradient overlay";
    public override ImageEffectCategory Category => ImageEffectCategory.Drawings;
    public override string IconKey => LucideIcons.paintbrush;
    public override string Description => "Blends a two-color gradient overlay on the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<GradientOverlayImageEffect>("color1", "Start color", new SKColor(255, 0, 128, 255), (e, v) => e.Color1 = v),
        EffectParameters.Color<GradientOverlayImageEffect>("color2", "End color", new SKColor(0, 128, 255, 255), (e, v) => e.Color2 = v),
        EffectParameters.IntSlider<GradientOverlayImageEffect>("angle", "Angle", 0, 360, 0, (e, v) => e.Angle = v),
        EffectParameters.FloatSlider<GradientOverlayImageEffect>("opacity", "Opacity", 0, 100, 50, (e, v) => e.Opacity = v)
    ];

    public SKColor Color1 { get; set; } = new SKColor(255, 0, 128);
    public SKColor Color2 { get; set; } = new SKColor(0, 128, 255);
    public int Angle { get; set; } = 0;
    public float Opacity { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        int w = source.Width, h = source.Height;
        float cx = w / 2f, cy = h / 2f;
        float rad = Angle * MathF.PI / 180f;
        float diagonal = MathF.Sqrt(w * w + h * h) / 2f;

        SKPoint start = new(cx - MathF.Cos(rad) * diagonal, cy - MathF.Sin(rad) * diagonal);
        SKPoint end = new(cx + MathF.Cos(rad) * diagonal, cy + MathF.Sin(rad) * diagonal);

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);
        using SKPaint paint = new()
        {
            Shader = SKShader.CreateLinearGradient(
                start, end,
                [Color1.WithAlpha((byte)(Color1.Alpha * alpha)),
                 Color2.WithAlpha((byte)(Color2.Alpha * alpha))],
                [0f, 1f],
                SKShaderTileMode.Clamp),
            BlendMode = SKBlendMode.Overlay
        };

        canvas.DrawRect(0, 0, w, h, paint);
        return result;
    }
}