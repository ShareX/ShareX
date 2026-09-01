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

public sealed class ReflectionImageEffect : ImageEffectBase
{
    public override string Id => "reflection";
    public override string Name => "Reflection";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flip_vertical_2;
    public override string Description => "Adds a reflection to the bottom of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ReflectionImageEffect>("percentage", "Percentage", 1, 100, 20, (effect, value) => effect.Percentage = value, isSnapToTickEnabled: false, valueStringFormat: "{}{0:0}%"),
        EffectParameters.IntSlider<ReflectionImageEffect>("max_alpha", "Max Alpha", 0, 255, 255, (effect, value) => effect.MaxAlpha = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<ReflectionImageEffect>("min_alpha", "Min Alpha", 0, 255, 0, (effect, value) => effect.MinAlpha = value, isSnapToTickEnabled: false),
        EffectParameters.IntSlider<ReflectionImageEffect>("offset", "Offset", 0, 100, 0, (effect, value) => effect.Offset = value, isSnapToTickEnabled: false),
        EffectParameters.Bool<ReflectionImageEffect>("skew", "Skew", false, (effect, value) => effect.Skew = value),
        EffectParameters.IntSlider<ReflectionImageEffect>("skew_size", "Skew Size", 1, 100, 25, (effect, value) => effect.SkewSize = value, isSnapToTickEnabled: false)
    ];

    public int Percentage { get; set; } = 20;
    public int MaxAlpha { get; set; } = 255;
    public int MinAlpha { get; set; }
    public int Offset { get; set; }
    public bool Skew { get; set; }
    public int SkewSize { get; set; } = 25;

    public ReflectionImageEffect()
    {
    }

    public ReflectionImageEffect(int percentage, int maxAlpha, int minAlpha, int offset, bool skew, int skewSize)
    {
        Percentage = percentage;
        MaxAlpha = maxAlpha;
        MinAlpha = minAlpha;
        Offset = offset;
        Skew = skew;
        SkewSize = skewSize;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (Percentage <= 0) return source.Copy();

        int reflectionHeight = (int)(source.Height * Percentage / 100f);
        int newHeight = source.Height + Offset + reflectionHeight;

        int newWidth = source.Width;
        if (Skew && SkewSize > 0)
        {
            newWidth += (int)(reflectionHeight * (SkewSize / 100f));
        }

        SKBitmap result = new(newWidth, newHeight);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);
        canvas.DrawBitmap(source, 0, 0);

        using SKBitmap flipped = new(source.Width, reflectionHeight);
        using (SKCanvas reflectionCanvas = new(flipped))
        {
            reflectionCanvas.Translate(0, source.Height);
            reflectionCanvas.Scale(1, -1);
            reflectionCanvas.DrawBitmap(source, 0, 0);
        }

        using SKPaint gradientPaint = new();
        using SKShader gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, reflectionHeight),
            [new SKColor(255, 255, 255, (byte)MaxAlpha), new SKColor(255, 255, 255, (byte)MinAlpha)],
            null,
            SKShaderTileMode.Clamp);
        gradientPaint.Shader = gradient;
        gradientPaint.BlendMode = SKBlendMode.DstIn;

        using SKBitmap reflectionBitmap = new(source.Width, reflectionHeight);
        using (SKCanvas reflectionMaskCanvas = new(reflectionBitmap))
        {
            reflectionMaskCanvas.DrawBitmap(flipped, 0, 0);
            reflectionMaskCanvas.DrawRect(new SKRect(0, 0, source.Width, reflectionHeight), gradientPaint);
        }

        if (Skew && SkewSize > 0)
        {
            canvas.Save();
            canvas.Translate(0, source.Height + Offset);
            canvas.Skew(SkewSize / 100f, 0);
            canvas.DrawBitmap(reflectionBitmap, 0, 0);
            canvas.Restore();
        }
        else
        {
            canvas.DrawBitmap(reflectionBitmap, 0, source.Height + Offset);
        }

        return result;
    }
}