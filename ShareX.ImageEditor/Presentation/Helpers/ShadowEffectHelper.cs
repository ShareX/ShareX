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

using Avalonia.Media;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Integration;

namespace ShareX.ImageEditor.Presentation.Helpers;

public static class ShadowEffectHelper
{
    private const double CanvasShadowOffsetY = 10;
    private const byte CanvasShadowDefaultAlpha = 80;

    public static DropShadowEffect CreateDropShadow(string? shadowColorHex)
    {
        return CreateDropShadow(
            shadowColorHex,
            Annotation.DefaultShadowBlurRadius,
            Annotation.DefaultShadowOpacity,
            Annotation.DefaultShadowOffsetX,
            Annotation.DefaultShadowOffsetY);
    }

    public static DropShadowEffect CreateDropShadow(Annotation annotation)
    {
        return CreateDropShadow(
            annotation.ShadowColor,
            annotation.ShadowBlurRadius,
            annotation.ShadowOpacity,
            annotation.ShadowOffsetX,
            annotation.ShadowOffsetY);
    }

    public static DropShadowEffect CreateDropShadow(string? shadowColorHex, double blurRadius, double opacity, double offsetX, double offsetY)
    {
        return new DropShadowEffect
        {
            OffsetX = offsetX,
            OffsetY = offsetY,
            BlurRadius = Math.Max(0, blurRadius),
            Color = ParseShadowColor(shadowColorHex),
            Opacity = Math.Clamp(opacity, 0, 1)
        };
    }

    public static BoxShadow CreateCanvasShadow(Color shadowColor, double blurRadius)
    {
        return new BoxShadow
        {
            Blur = blurRadius,
            Color = CreateCanvasShadowColor(shadowColor),
            OffsetX = 0,
            OffsetY = CanvasShadowOffsetY
        };
    }

    public static Color ParseShadowColor(string? shadowColorHex)
    {
        return !string.IsNullOrWhiteSpace(shadowColorHex) && Color.TryParse(shadowColorHex, out Color shadowColor)
            ? shadowColor
            : ImageEditorOptions.DefaultShadowColor;
    }

    private static Color CreateCanvasShadowColor(Color shadowColor)
    {
        byte alpha = (byte)Math.Clamp(
            (int)Math.Round(shadowColor.A * (CanvasShadowDefaultAlpha / (double)ImageEditorOptions.DefaultShadowColor.A)),
            0,
            255);

        return Color.FromArgb(alpha, shadowColor.R, shadowColor.G, shadowColor.B);
    }
}
