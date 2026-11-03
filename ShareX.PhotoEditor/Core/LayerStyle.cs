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

namespace ShareX.PhotoEditor.Core;

public sealed class LayerStyle
{
    public bool DropShadow { get; set; }
    public SKColor ShadowColor { get; set; } = SKColors.Black;
    public float ShadowOpacity { get; set; } = 0.75f;
    public float ShadowAngle { get; set; } = 120;
    public float ShadowDistance { get; set; } = 5;
    public float ShadowSize { get; set; } = 5;

    public bool Stroke { get; set; }
    public SKColor StrokeColor { get; set; } = SKColors.Black;
    public float StrokeSize { get; set; } = 3;
    public float StrokeOpacity { get; set; } = 1;
    public StrokePosition StrokePosition { get; set; } = StrokePosition.Outside;

    public bool OuterGlow { get; set; }
    public SKColor GlowColor { get; set; } = new(255, 255, 190);
    public float GlowOpacity { get; set; } = 0.75f;
    public float GlowSize { get; set; } = 10;

    public bool ColorOverlay { get; set; }
    public SKColor OverlayColor { get; set; } = SKColors.Red;
    public float OverlayOpacity { get; set; } = 1;

    public bool InnerShadow { get; set; }
    public SKColor InnerShadowColor { get; set; } = SKColors.Black;
    public float InnerShadowOpacity { get; set; } = 0.75f;
    public float InnerShadowDistance { get; set; } = 5;
    public float InnerShadowSize { get; set; } = 5;

    public bool HasAny => DropShadow || Stroke || OuterGlow || ColorOverlay || InnerShadow;

    public float Extent => Math.Max(DropShadow ? ShadowDistance + ShadowSize * 3 : 0,
        Math.Max(Stroke ? StrokeSize + 2 : 0, OuterGlow ? GlowSize * 3 : 0));

    public LayerStyle Clone() => (LayerStyle)MemberwiseClone();
}

public enum StrokePosition
{
    Outside,
    Inside,
    Center
}
