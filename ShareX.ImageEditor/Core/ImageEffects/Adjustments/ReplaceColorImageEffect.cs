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

using ShareX.ImageEditor.Core.ImageEffects.Helpers;
using ShareX.ImageEditor.Core.ImageEffects.Parameters;
using ShareX.AvaloniaUI.Theming;
using SkiaSharp;

namespace ShareX.ImageEditor.Core.ImageEffects.Adjustments;

public sealed class ReplaceColorImageEffect : AdjustmentImageEffectBase
{
    public override string Id => "replace_color";
    public override string Name => "Replace Color";
    public override string IconKey => LucideIcons.replace;
    public override string Description => "Replaces a specific color.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Color<ReplaceColorImageEffect>("target_color", "Target color", SKColors.White, (effect, value) => effect.TargetColor = value),
        EffectParameters.Color<ReplaceColorImageEffect>("replace_color", "Replace color", SKColors.Black, (effect, value) => effect.ReplaceColor = value),
        EffectParameters.FloatSlider<ReplaceColorImageEffect>("tolerance", "Tolerance", 0, 255, 40, (effect, value) => effect.Tolerance = value)
    ];
    public SKColor TargetColor { get; set; }
    public SKColor ReplaceColor { get; set; }
    public float Tolerance { get; set; }

    public override SKBitmap Apply(SKBitmap source)
    {
        int tol = (int)(Tolerance * 2.55f);
        return ApplyPixelOperation(source, (c) =>
        {
            if (ImageHelpers.ColorsMatch(c, TargetColor, tol))
            {
                return ReplaceColor;
            }
            return c;
        });
    }
}