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

#nullable enable

using System;
using System.Drawing;

namespace ShareX.ScreenCaptureLib;

/// <summary>Where the user dragged the bar, as the offset of its top centre from the region's horizontal centre
/// and from its top or bottom edge, in physical pixels.</summary>
public sealed class RecordingToolbarOffset
{
    public bool FromTop;
    public int X;
    public int Y;
}

/// <summary>Shared by the setup bar and the recording controls so both appear in the same place.</summary>
public static class RecordingToolbarPlacement
{
    /// <summary>Uses the saved offset, else below the region, else above it, else inside its bottom edge.</summary>
    public static Point GetLocation(Rectangle region, Size toolbar, int gap, Rectangle workArea,
        RecordingToolbarOffset? offset, bool allowInside = true)
    {
        int top;
        if (offset != null) top = (offset.FromTop ? region.Top : region.Bottom) + offset.Y;
        else if (region.Bottom + gap + toolbar.Height <= workArea.Bottom) top = region.Bottom + gap;
        else if (region.Top - gap - toolbar.Height >= workArea.Top) top = region.Top - gap - toolbar.Height;
        else if (allowInside) top = Math.Min(region.Bottom, workArea.Bottom) - gap - toolbar.Height;
        else top = region.Bottom + gap;

        return new Point(region.Left + region.Width / 2 + (offset?.X ?? 0) - toolbar.Width / 2, top);
    }

    /// <summary>Keeps the bar in the work area. Vertical clamping is skipped when the bar may not overlap the region.</summary>
    public static Rectangle Clamp(Rectangle toolbar, Rectangle workArea, bool vertical = true)
    {
        int left = Math.Clamp(toolbar.Left, workArea.Left, Math.Max(workArea.Left, workArea.Right - toolbar.Width));
        int top = vertical
            ? Math.Clamp(toolbar.Top, workArea.Top, Math.Max(workArea.Top, workArea.Bottom - toolbar.Height))
            : toolbar.Top;
        return new Rectangle(left, top, toolbar.Width, toolbar.Height);
    }

    public static RecordingToolbarOffset GetOffset(Rectangle region, Rectangle toolbar)
    {
        bool fromTop = toolbar.Top < region.Top + region.Height / 2;
        return new RecordingToolbarOffset
        {
            FromTop = fromTop,
            X = toolbar.Left + toolbar.Width / 2 - (region.Left + region.Width / 2),
            Y = toolbar.Top - (fromTop ? region.Top : region.Bottom)
        };
    }
}
