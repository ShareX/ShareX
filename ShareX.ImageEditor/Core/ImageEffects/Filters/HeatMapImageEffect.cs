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
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Filters;

public sealed class HeatMapImageEffect : ImageEffectBase
{
    public override string Id => "heat_map";
    public override string Name => "Heat map";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.flame;
    public override string Description => "Maps image luminance to a fire-like color gradient.";
    public override EffectExecutionMode ExecutionMode => EffectExecutionMode.Immediate;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];

        for (int i = 0; i < src.Length; i++)
        {
            SKColor c = src[i];
            float lum = (0.2126f * c.Red + 0.7152f * c.Green + 0.0722f * c.Blue) / 255f;
            dst[i] = HeatColor(lum, c.Alpha);
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }

    private static SKColor HeatColor(float t, byte alpha)
    {
        // Black -> Blue -> Cyan -> Green -> Yellow -> Red -> White
        byte r, g, b;

        if (t < 0.2f)
        {
            float s = t / 0.2f;
            r = 0; g = 0; b = (byte)(s * 255);
        }
        else if (t < 0.4f)
        {
            float s = (t - 0.2f) / 0.2f;
            r = 0; g = (byte)(s * 255); b = 255;
        }
        else if (t < 0.6f)
        {
            float s = (t - 0.4f) / 0.2f;
            r = 0; g = 255; b = (byte)((1f - s) * 255);
        }
        else if (t < 0.8f)
        {
            float s = (t - 0.6f) / 0.2f;
            r = (byte)(s * 255); g = 255; b = 0;
        }
        else
        {
            float s = (t - 0.8f) / 0.2f;
            r = 255; g = (byte)((1f - s) * 255); b = (byte)(s * 255);
        }

        return new SKColor(r, g, b, alpha);
    }
}