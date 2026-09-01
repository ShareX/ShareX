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

using Avalonia.Layout;
using Avalonia.Media;
using ShareX.AvaloniaUI.Theming;
using ShareX.ImageEditor.Core.Annotations;
using ShareX.ImageEditor.Localization;

namespace ShareX.ImageEditor.Presentation.Helpers;

public static class TextHorizontalAlignmentHelper
{
    public static string GetDisplayName(TextHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TextHorizontalAlignment.Left => Strings.TextHorizontalAlignmentHelper_Left,
            TextHorizontalAlignment.Center => Strings.TextHorizontalAlignmentHelper_Center,
            TextHorizontalAlignment.Right => Strings.TextHorizontalAlignmentHelper_Right,
            _ => Strings.TextHorizontalAlignmentHelper_Center
        };
    }

    public static string GetIcon(TextHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TextHorizontalAlignment.Left => LucideIcons.text_align_start,
            TextHorizontalAlignment.Center => LucideIcons.text_align_center,
            TextHorizontalAlignment.Right => LucideIcons.text_align_end,
            _ => LucideIcons.text_align_center
        };
    }

    public static TextAlignment ToAvaloniaTextAlignment(TextHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TextHorizontalAlignment.Left => TextAlignment.Left,
            TextHorizontalAlignment.Center => TextAlignment.Center,
            TextHorizontalAlignment.Right => TextAlignment.Right,
            _ => TextAlignment.Center
        };
    }

    public static HorizontalAlignment ToHorizontalContentAlignment(TextHorizontalAlignment alignment)
    {
        return alignment switch
        {
            TextHorizontalAlignment.Left => HorizontalAlignment.Left,
            TextHorizontalAlignment.Center => HorizontalAlignment.Center,
            TextHorizontalAlignment.Right => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Center
        };
    }
}
