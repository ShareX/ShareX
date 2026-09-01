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

namespace ShareX.ImageEditor.Core.ImageEffects.Manipulations;

public sealed class RoundedCornersImageEffect : ImageEffectBase
{
    public override string Id => "rounded_corners";
    public override string Name => "Rounded Corners";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.square_round_corner;
    public override string Description => "Rounds the corners of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.IntSlider<RoundedCornersImageEffect>("corner_radius", "Corner radius", 0, 500, 20, (e, v) => e.CornerRadius = v)
    ];

    public int CornerRadius { get; set; } = 20;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (CornerRadius <= 0) return source.Copy();

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using SKCanvas canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        using SKPath clipPath = new SKPath();
        clipPath.AddRoundRect(new SKRect(0, 0, source.Width, source.Height), CornerRadius, CornerRadius);
        canvas.ClipPath(clipPath, SKClipOperation.Intersect, true);

        canvas.DrawBitmap(source, 0, 0);
        return result;
    }
}