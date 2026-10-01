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

using System;

namespace ShareX.HelpersLib
{
    // Portable stand-ins for WinForms and GDI+ enums used in saved settings. Member names and values match the originals,
    // because settings and image effect presets store them by name.

    /// <summary>Sides of a rectangle, like System.Windows.Forms.AnchorStyles.</summary>
    [Flags]
    public enum AnchorSides
    {
        None = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    /// <summary>Line dash pattern, like System.Drawing.Drawing2D.DashStyle.</summary>
    public enum LineDashStyle
    {
        Solid = 0,
        Dash = 1,
        Dot = 2,
        DashDot = 3,
        DashDotDot = 4,
        Custom = 5
    }

    /// <summary>Gradient direction, like System.Drawing.Drawing2D.LinearGradientMode.</summary>
    public enum GradientDirection
    {
        Horizontal = 0,
        Vertical = 1,
        ForwardDiagonal = 2,
        BackwardDiagonal = 3
    }

    /// <summary>Where to place something inside an image, like System.Drawing.ContentAlignment (same names and values).</summary>
    public enum ImageAlignment
    {
        TopLeft = 1,
        TopCenter = 2,
        TopRight = 4,
        MiddleLeft = 16,
        MiddleCenter = 32,
        MiddleRight = 64,
        BottomLeft = 256,
        BottomCenter = 512,
        BottomRight = 1024
    }

    /// <summary>Like System.Drawing.Drawing2D.CompositingMode.</summary>
    public enum ImageCompositingMode
    {
        SourceOver = 0,
        SourceCopy = 1
    }

    /// <summary>Like System.Drawing.Text.TextRenderingHint.</summary>
    public enum TextRenderingMode
    {
        SystemDefault = 0,
        SingleBitPerPixelGridFit = 1,
        SingleBitPerPixel = 2,
        AntiAliasGridFit = 3,
        AntiAlias = 4,
        ClearTypeGridFit = 5
    }
}
