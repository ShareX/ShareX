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

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class ThresholdImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "threshold";
    public override string Name => "Threshold";
    public override string IconKey => LucideIcons.binary;
    public override string Description => "Applies a contrast threshold.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<ThresholdImageEffect>("value", "Threshold", 0, 255, 128, (effect, value) => effect.Value = value)
    ];

    public int Value { get; set; } = 128;

    public override SKBitmap Apply(SKBitmap source)
    {
        int threshold = Math.Clamp(Value, 0, 255);

        return ApplyPixelOperation(source, c =>
        {
            int luma = ((c.Red * 77) + (c.Green * 150) + (c.Blue * 29)) >> 8;
            byte bw = (byte)(luma >= threshold ? 255 : 0);
            return new SKColor(bw, bw, bw, c.Alpha);
        });
    }
}