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

namespace ShareX.PixelEditor.Core;

/// <summary>Flattens the layers of a frame into one buffer.</summary>
public static class Compositor
{
    /// <summary>Composites the visible layers of a frame into an area of the target buffer, which is cleared first.</summary>
    public static void Compose(Sprite sprite, int frame, PixelBuffer target, SKRectI area, bool includeReference = true, Func<Layer, bool>? filter = null)
    {
        area = target.Clip(area);
        if (area.IsEmpty) return;
        for (int y = area.Top; y < area.Bottom; y++) Array.Clear(target.Pixels, y * target.Width + area.Left, area.Width);
        if (frame < 0 || frame >= sprite.Frames.Count) return;
        ComposeGroup(sprite.Root, frame, target, area, includeReference, filter);
    }

    /// <summary>The flattened image of a frame, as it is exported.</summary>
    public static PixelBuffer Flatten(Sprite sprite, int frame, bool includeReference = false, Func<Layer, bool>? filter = null)
    {
        PixelBuffer result = new(sprite.Width, sprite.Height);
        Compose(sprite, frame, result, result.Bounds, includeReference, filter);
        return result;
    }

    private static void ComposeGroup(GroupLayer group, int frame, PixelBuffer target, SKRectI area, bool includeReference, Func<Layer, bool>? filter)
    {
        foreach (Layer child in group.Children)
        {
            if (!child.Visible || filter != null && !filter(child)) continue;
            if (child is PixelLayer layer)
            {
                if (layer.IsReference && !includeReference) continue;
                Cel? cel = (uint)frame < (uint)layer.Cels.Count ? layer.Cels[frame] : null;
                if (cel == null) continue;
                BlendBuffer(target, cel.Buffer, area, (layer.Opacity * cel.Opacity + 127) / 255, layer.Blend);
            }
            else if (child is GroupLayer nested)
            {
                if (nested.Opacity == 255 && nested.Blend == BlendMode.Normal)
                {
                    ComposeGroup(nested, frame, target, area, includeReference, filter);
                }
                else
                {
                    PixelBuffer temp = new(target.Width, target.Height);
                    ComposeGroup(nested, frame, temp, area, includeReference, filter);
                    BlendBuffer(target, temp, area, nested.Opacity, nested.Blend);
                }
            }
        }
    }

    public static void BlendBuffer(PixelBuffer target, PixelBuffer source, SKRectI area, int opacity, BlendMode mode)
    {
        if (opacity <= 0) return;
        int right = Math.Min(area.Right, Math.Min(source.Width, target.Width)), bottom = Math.Min(area.Bottom, Math.Min(source.Height, target.Height));
        uint[] dst = target.Pixels, src = source.Pixels;
        for (int y = Math.Max(0, area.Top); y < bottom; y++)
        {
            int d = y * target.Width + area.Left, s = y * source.Width + area.Left;
            for (int x = area.Left; x < right; x++, d++, s++)
            {
                uint c = src[s];
                if (c >> 24 == 0) continue;
                uint under = dst[d];
                if (mode == BlendMode.Normal || under >> 24 == 0)
                {
                    dst[d] = opacity == 255 && c >> 24 == 255 ? c : Argb.Over(under, c, opacity);
                }
                else
                {
                    dst[d] = Argb.Over(under, BlendColor(under, c, mode), opacity);
                }
            }
        }
    }

    /// <summary>The source color after applying a blend mode against the backdrop; its alpha is unchanged.</summary>
    public static uint BlendColor(uint backdrop, uint source, BlendMode mode)
    {
        int ba = Argb.A(backdrop);
        if (ba == 0 || mode == BlendMode.Normal) return source;
        int br = Argb.R(backdrop), bg = Argb.G(backdrop), bb = Argb.B(backdrop);
        int sr = Argb.R(source), sg = Argb.G(source), sb = Argb.B(source);
        int r, g, b;
        if (mode >= BlendMode.Hue)
        {
            (r, g, b) = NonSeparable(br, bg, bb, sr, sg, sb, mode);
        }
        else
        {
            r = Channel(br, sr, mode);
            g = Channel(bg, sg, mode);
            b = Channel(bb, sb, mode);
        }
        // Where the backdrop is translucent the blended color fades back to the plain source color.
        if (ba < 255)
        {
            r = (r * ba + sr * (255 - ba) + 127) / 255;
            g = (g * ba + sg * (255 - ba) + 127) / 255;
            b = (b * ba + sb * (255 - ba) + 127) / 255;
        }
        return Argb.Make(Argb.A(source), r, g, b);
    }

    private static int Channel(int b, int s, BlendMode mode)
    {
        switch (mode)
        {
            case BlendMode.Darken: return Math.Min(b, s);
            case BlendMode.Lighten: return Math.Max(b, s);
            case BlendMode.Multiply: return (b * s + 127) / 255;
            case BlendMode.Screen: return 255 - ((255 - b) * (255 - s) + 127) / 255;
            case BlendMode.ColorBurn: return b == 255 ? 255 : s == 0 ? 0 : 255 - Math.Min(255, (255 - b) * 255 / s);
            case BlendMode.ColorDodge: return b == 0 ? 0 : s == 255 ? 255 : Math.Min(255, b * 255 / (255 - s));
            case BlendMode.Addition: return Math.Min(255, b + s);
            case BlendMode.Subtract: return Math.Max(0, b - s);
            case BlendMode.Divide: return s == 0 ? 255 : Math.Min(255, b * 255 / s);
            case BlendMode.Difference: return Math.Abs(b - s);
            case BlendMode.Exclusion: return b + s - 2 * b * s / 255;
            case BlendMode.Overlay: return b < 128 ? 2 * b * s / 255 : 255 - 2 * (255 - b) * (255 - s) / 255;
            case BlendMode.HardLight: return s < 128 ? 2 * b * s / 255 : 255 - 2 * (255 - b) * (255 - s) / 255;
            case BlendMode.SoftLight:
            {
                float fb = b / 255f, fs = s / 255f;
                float result = fs <= 0.5f
                    ? fb - (1 - 2 * fs) * fb * (1 - fb)
                    : fb + (2 * fs - 1) * ((fb <= 0.25f ? ((16 * fb - 12) * fb + 4) * fb : MathF.Sqrt(fb)) - fb);
                return Math.Clamp((int)MathF.Round(result * 255), 0, 255);
            }
            default: return s;
        }
    }

    private static (int, int, int) NonSeparable(int br, int bg, int bb, int sr, int sg, int sb, BlendMode mode)
    {
        (float R, float G, float B) backdrop = (br / 255f, bg / 255f, bb / 255f), source = (sr / 255f, sg / 255f, sb / 255f);
        (float R, float G, float B) result = mode switch
        {
            BlendMode.Hue => SetLum(SetSat(source, Sat(backdrop)), Lum(backdrop)),
            BlendMode.Saturation => SetLum(SetSat(backdrop, Sat(source)), Lum(backdrop)),
            BlendMode.Color => SetLum(source, Lum(backdrop)),
            _ => SetLum(backdrop, Lum(source))
        };
        return (Math.Clamp((int)MathF.Round(result.R * 255), 0, 255), Math.Clamp((int)MathF.Round(result.G * 255), 0, 255), Math.Clamp((int)MathF.Round(result.B * 255), 0, 255));
    }

    private static float Lum((float R, float G, float B) c) => 0.3f * c.R + 0.59f * c.G + 0.11f * c.B;

    private static float Sat((float R, float G, float B) c) => Math.Max(c.R, Math.Max(c.G, c.B)) - Math.Min(c.R, Math.Min(c.G, c.B));

    private static (float, float, float) SetLum((float R, float G, float B) c, float lum)
    {
        float d = lum - Lum(c);
        (float R, float G, float B) v = (c.R + d, c.G + d, c.B + d);
        float l = Lum(v), min = Math.Min(v.R, Math.Min(v.G, v.B)), max = Math.Max(v.R, Math.Max(v.G, v.B));
        if (min < 0) v = (l + (v.R - l) * l / (l - min), l + (v.G - l) * l / (l - min), l + (v.B - l) * l / (l - min));
        if (max > 1) v = (l + (v.R - l) * (1 - l) / (max - l), l + (v.G - l) * (1 - l) / (max - l), l + (v.B - l) * (1 - l) / (max - l));
        return v;
    }

    private static (float R, float G, float B) SetSat((float R, float G, float B) c, float sat)
    {
        float max = Math.Max(c.R, Math.Max(c.G, c.B)), min = Math.Min(c.R, Math.Min(c.G, c.B));
        if (max <= min) return (0, 0, 0);
        return ((c.R - min) * sat / (max - min), (c.G - min) * sat / (max - min), (c.B - min) * sat / (max - min));
    }
}
