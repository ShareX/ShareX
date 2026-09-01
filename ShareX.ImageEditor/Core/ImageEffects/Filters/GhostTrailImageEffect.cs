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

public sealed class GhostTrailImageEffect : ImageEffectBase
{
    public override string Id => "ghost_trail";
    public override string Name => "Ghost trail";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.ghost;
    public override string Description => "Creates fading ghost copies offset from the original image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<GhostTrailImageEffect>("copies", "Copies", 1, 8, 3, (e, v) => e.Copies = v),
        EffectParameters.IntSlider<GhostTrailImageEffect>("offset_x", "Offset X", -50, 50, 15, (e, v) => e.OffsetX = v),
        EffectParameters.IntSlider<GhostTrailImageEffect>("offset_y", "Offset Y", -50, 50, 5, (e, v) => e.OffsetY = v),
        EffectParameters.FloatSlider<GhostTrailImageEffect>("fade", "Fade", 10, 90, 50, (e, v) => e.Fade = v)
    ];

    public int Copies { get; set; } = 3;
    public int OffsetX { get; set; } = 15;
    public int OffsetY { get; set; } = 5;
    public float Fade { get; set; } = 50f;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        int copies = Math.Max(1, Copies);

        int w = source.Width, h = source.Height;
        SKBitmap result = new(w, h, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new(result);
        canvas.Clear(SKColors.Transparent);

        float fadeStep = Math.Clamp(Fade / 100f, 0.1f, 0.9f);

        // Draw ghost copies back-to-front (farthest first)
        for (int i = copies; i >= 1; i--)
        {
            float alpha = MathF.Pow(1f - fadeStep, i);
            using SKPaint paint = new() { Color = new SKColor(255, 255, 255, (byte)(255 * alpha)) };
            canvas.DrawBitmap(source, OffsetX * i, OffsetY * i, paint);
        }

        // Draw original on top
        canvas.DrawBitmap(source, 0, 0);
        return result;
    }
}