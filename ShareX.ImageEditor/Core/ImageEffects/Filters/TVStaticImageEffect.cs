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

public sealed class TVStaticImageEffect : ImageEffectBase
{
    public override string Id => "tv_static";
    public override string Name => "TV static";
    public override ImageEffectCategory Category => ImageEffectCategory.Filters;
    public override string IconKey => LucideIcons.tv;
    public override string Description => "Blends black-and-white TV static noise over the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.FloatSlider<TVStaticImageEffect>("amount", "Amount", 0, 100, 40, (e, v) => e.Amount = v)
    ];

    public float Amount { get; set; } = 40f;
    public int Seed { get; set; } = 42;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        float blend = Math.Clamp(Amount / 100f, 0f, 1f);
        if (blend <= 0f) return source.Copy();

        int w = source.Width, h = source.Height;
        SKColor[] src = source.Pixels;
        SKColor[] dst = new SKColor[src.Length];
        Random rng = new(Seed);

        for (int i = 0; i < src.Length; i++)
        {
            byte noise = (byte)rng.Next(256);
            SKColor c = src[i];
            byte r = (byte)(c.Red + (noise - c.Red) * blend);
            byte g = (byte)(c.Green + (noise - c.Green) * blend);
            byte b = (byte)(c.Blue + (noise - c.Blue) * blend);
            dst[i] = new SKColor(r, g, b, c.Alpha);
        }

        return new SKBitmap(w, h, source.ColorType, source.AlphaType) { Pixels = dst };
    }
}