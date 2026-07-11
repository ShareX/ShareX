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

using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public enum LightLeakPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Left,
    Right
}

public sealed class LightLeakImageEffect : ImageEffectBase
{
    public override string Id => "light_leak";
    public override string Name => "Light leak";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.sun;
    public override string Description => "Adds a warm film light leak effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<LightLeakImageEffect>("intensity", "Intensity", 0, 100, 50, (e, v) => e.Intensity = v),
        EffectParameters.Color<LightLeakImageEffect>("color", "Color", new SKColor(255, 160, 50, 255), (e, v) => e.Color = v),
        EffectParameters.Enum<LightLeakImageEffect, LightLeakPosition>(
            "position", "Position", LightLeakPosition.TopRight, (e, v) => e.Position = v,
            new (string, LightLeakPosition)[]
            {
                ("Top left", LightLeakPosition.TopLeft),
                ("Top right", LightLeakPosition.TopRight),
                ("Bottom left", LightLeakPosition.BottomLeft),
                ("Bottom right", LightLeakPosition.BottomRight),
                ("Left", LightLeakPosition.Left),
                ("Right", LightLeakPosition.Right)
            })
    ];

    public float Intensity { get; set; } = 50f;
    public SKColor Color { get; set; } = new SKColor(255, 160, 50);
    public LightLeakPosition Position { get; set; } = LightLeakPosition.TopRight;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float alpha = Math.Clamp(Intensity / 100f, 0f, 1f);
        if (alpha <= 0f) return source.Copy();

        int w = source.Width, h = source.Height;
        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        SKPoint center = Position switch
        {
            LightLeakPosition.TopLeft => new SKPoint(0, 0),
            LightLeakPosition.TopRight => new SKPoint(w, 0),
            LightLeakPosition.BottomLeft => new SKPoint(0, h),
            LightLeakPosition.BottomRight => new SKPoint(w, h),
            LightLeakPosition.Left => new SKPoint(0, h / 2f),
            LightLeakPosition.Right => new SKPoint(w, h / 2f),
            _ => new SKPoint(w, 0)
        };

        float radius = MathF.Sqrt(w * w + h * h) * 0.8f;
        SKColor leakColor = Color.WithAlpha((byte)(255 * alpha));

        using SKPaint paint = new()
        {
            Shader = SKShader.CreateRadialGradient(
                center, radius,
                [leakColor, SKColors.Transparent],
                [0f, 1f],
                SKShaderTileMode.Clamp),
            BlendMode = SKBlendMode.Screen
        };

        canvas.DrawRect(0, 0, w, h, paint);
        return result;
    }
}