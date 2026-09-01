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

public enum SymmetryDirection
{
    LeftToRight,
    RightToLeft,
    TopToBottom,
    BottomToTop
}

public sealed class SymmetryImageEffect : ImageEffectBase
{
    public override string Id => "symmetry";
    public override string Name => "Symmetry";
    public override ImageEffectCategory Category => ImageEffectCategory.Manipulations;
    public override string IconKey => LucideIcons.flip_horizontal_2;
    public override string Description => "Creates bilateral symmetry by mirroring one half of the image.";
    public override IReadOnlyList<EffectParameter> Parameters =>
    [
        EffectParameters.Enum<SymmetryImageEffect, SymmetryDirection>(
            "direction", "Direction", SymmetryDirection.LeftToRight, (e, v) => e.Direction = v,
            new (string, SymmetryDirection)[]
            {
                ("Left \u2192 Right", SymmetryDirection.LeftToRight),
                ("Right \u2192 Left", SymmetryDirection.RightToLeft),
                ("Top \u2192 Bottom", SymmetryDirection.TopToBottom),
                ("Bottom \u2192 Top", SymmetryDirection.BottomToTop)
            })
    ];

    public SymmetryDirection Direction { get; set; } = SymmetryDirection.LeftToRight;

    public override SKBitmap Apply(SKBitmap source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        SKBitmap result = source.Copy();
        using SKCanvas canvas = new(result);

        int w = source.Width, h = source.Height;
        SKRect srcRect, dstRect;

        switch (Direction)
        {
            case SymmetryDirection.LeftToRight:
                srcRect = new SKRect(0, 0, w / 2f, h);
                dstRect = new SKRect(w, 0, w / 2f, h);
                canvas.Save();
                canvas.Scale(-1, 1, w / 2f, 0);
                canvas.DrawBitmap(source, srcRect, srcRect);
                canvas.Restore();
                break;
            case SymmetryDirection.RightToLeft:
                srcRect = new SKRect(w / 2f, 0, w, h);
                canvas.Save();
                canvas.Scale(-1, 1, w / 2f, 0);
                canvas.DrawBitmap(source, srcRect, srcRect);
                canvas.Restore();
                break;
            case SymmetryDirection.TopToBottom:
                srcRect = new SKRect(0, 0, w, h / 2f);
                canvas.Save();
                canvas.Scale(1, -1, 0, h / 2f);
                canvas.DrawBitmap(source, srcRect, srcRect);
                canvas.Restore();
                break;
            case SymmetryDirection.BottomToTop:
                srcRect = new SKRect(0, h / 2f, w, h);
                canvas.Save();
                canvas.Scale(1, -1, 0, h / 2f);
                canvas.DrawBitmap(source, srcRect, srcRect);
                canvas.Restore();
                break;
        }

        return result;
    }
}