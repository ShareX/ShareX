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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class SolarizeImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "solarize";
    public override string Name => "Solarize";
    public override string IconKey => LucideIcons.sun;
    public override string Description => "Applies a solarize effect.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<SolarizeImageEffect>("threshold", "Threshold", 0, 255, 128, (effect, value) => effect.Threshold = value)
    ];

    public int Threshold { get; set; } = 128;

    public override SKBitmap Apply(SKBitmap source)
    {
        int threshold = Math.Clamp(Threshold, 0, 255);

        return ApplyPixelOperation(source, c =>
        {
            byte r = c.Red > threshold ? (byte)(255 - c.Red) : c.Red;
            byte g = c.Green > threshold ? (byte)(255 - c.Green) : c.Green;
            byte b = c.Blue > threshold ? (byte)(255 - c.Blue) : c.Blue;
            return new SKColor(r, g, b, c.Alpha);
        });
    }
}