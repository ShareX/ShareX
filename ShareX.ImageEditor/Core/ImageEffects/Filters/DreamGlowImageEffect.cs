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

public sealed class DreamGlowImageEffect : ImageEffectBase
{
    public override string Id => "dream_glow";
    public override string Name => "Dream glow";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.cloud;
    public override string Description => "Creates a dreamy soft glow by blending a blurred copy over the original.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<DreamGlowImageEffect>("radius", "Blur radius", 1, 50, 15, (e, v) => e.Radius = v),
        EffectParameters.FloatSlider<DreamGlowImageEffect>("opacity", "Blend opacity", 0, 100, 50, (e, v) => e.Opacity = v)
    ];

    public int Radius { get; set; } = 15;
    public float Opacity { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float alpha = Math.Clamp(Opacity / 100f, 0f, 1f);
        if (alpha <= 0f || Radius <= 0) return source.Copy();

        int w = source.Width, h = source.Height;

        // Create blurred version
        SKBitmap blurred = new(w, h, source.ColorType, source.AlphaType);
        using (SKCanvas blurCanvas = new(blurred))
        {
            using SKPaint blurPaint = new() { ImageFilter = SKImageFilter.CreateBlur(Radius, Radius) };
            blurCanvas.DrawBitmap(source, 0, 0, blurPaint);
        }

        // Blend: original + blurred with Screen mode
        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);
        using SKPaint blendPaint = new()
        {
            Color = new SKColor(255, 255, 255, (byte)(255 * alpha)),
            BlendMode = SKBlendMode.Screen
        };
        canvas.DrawBitmap(blurred, 0, 0, blendPaint);
        blurred.Dispose();

        return result;
    }
}