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

public class FlipImageEffect : ImageEffectBase
{
    public enum FlipDirection { Horizontal, Vertical }

    private readonly FlipDirection? _direction;
    private readonly string _name;

    public bool Horizontally { get; set; }
    public bool Vertically { get; set; }

    public override string Id => "flip";
    public override string Name => _name;
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.flip_horizontal;
    public override string Description => "Flips the image horizontally or vertically.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Bool<FlipImageEffect>("horizontally", "Horizontally", false, (e, v) => e.Horizontally = v),
        EffectParameters.Bool<FlipImageEffect>("vertically", "Vertically", false, (e, v) => e.Vertically = v)
    ];

    public FlipImageEffect()
    {
        _name = "Flip";
    }

    private FlipImageEffect(FlipDirection direction, string name)
    {
        _direction = direction;
        _name = name;
    }

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        bool flipHorizontal = _direction == FlipDirection.Horizontal || (_direction == null && Horizontally);
        bool flipVertical = _direction == FlipDirection.Vertical || (_direction == null && Vertically);

        if (!flipHorizontal && !flipVertical)
        {
            return source.Copy();
        }

        SKBitmap result = new SKBitmap(source.Width, source.Height, source.ColorType, source.AlphaType);
        using (SKCanvas canvas = new SKCanvas(result))
        {
            canvas.Clear(SKColors.Transparent);
            if (flipHorizontal && flipVertical)
            {
                canvas.Scale(-1, -1, source.Width / 2f, source.Height / 2f);
            }
            else if (flipHorizontal)
            {
                canvas.Scale(-1, 1, source.Width / 2f, source.Height / 2f);
            }
            else
            {
                canvas.Scale(1, -1, source.Width / 2f, source.Height / 2f);
            }
            canvas.DrawBitmap(source, 0, 0);
        }
        return result;
    }

    public static FlipImageEffect Horizontal => new FlipImageEffect(FlipDirection.Horizontal, "Flip horizontal");
    public static FlipImageEffect Vertical => new FlipImageEffect(FlipDirection.Vertical, "Flip vertical");
}