#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class DarkColorTable : ProfessionalColorTable
    {
        public override Color ButtonSelectedHighlight => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonSelectedHighlightBorder => ShareXResources.Theme.MenuHighlightBorderColor;
        public override Color ButtonPressedHighlight => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonPressedHighlightBorder => ShareXResources.Theme.MenuHighlightBorderColor;
        public override Color ButtonCheckedHighlight => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color ButtonCheckedHighlightBorder => ShareXResources.Theme.MenuHighlightBorderColor;
        public override Color ButtonPressedBorder => ShareXResources.Theme.MenuHighlightBorderColor;
        public override Color ButtonSelectedBorder => ShareXResources.Theme.MenuHighlightBorderColor;
        public override Color ButtonCheckedGradientBegin => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color ButtonCheckedGradientMiddle => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color ButtonCheckedGradientEnd => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color ButtonSelectedGradientBegin => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonSelectedGradientMiddle => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonSelectedGradientEnd => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonPressedGradientBegin => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonPressedGradientMiddle => ShareXResources.Theme.MenuHighlightColor;
        public override Color ButtonPressedGradientEnd => ShareXResources.Theme.MenuHighlightColor;
        public override Color CheckBackground => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color CheckSelectedBackground => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color CheckPressedBackground => ShareXResources.Theme.MenuCheckBackgroundColor;
        public override Color GripDark => ShareXResources.Theme.SeparatorDarkColor;
        public override Color GripLight => ShareXResources.Theme.SeparatorLightColor;
        public override Color ImageMarginGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color ImageMarginGradientMiddle => ShareXResources.Theme.BackgroundColor;
        public override Color ImageMarginGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color ImageMarginRevealedGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color ImageMarginRevealedGradientMiddle => ShareXResources.Theme.BackgroundColor;
        public override Color ImageMarginRevealedGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color MenuStripGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color MenuStripGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color MenuItemSelected => ShareXResources.Theme.MenuHighlightColor;
        public override Color MenuItemBorder => ShareXResources.Theme.MenuBorderColor;
        public override Color MenuBorder => ShareXResources.Theme.MenuBorderColor;
        public override Color MenuItemSelectedGradientBegin => ShareXResources.Theme.MenuHighlightColor;
        public override Color MenuItemSelectedGradientEnd => ShareXResources.Theme.MenuHighlightColor;
        public override Color MenuItemPressedGradientBegin => ShareXResources.Theme.MenuHighlightColor;
        public override Color MenuItemPressedGradientMiddle => ShareXResources.Theme.MenuHighlightColor;
        public override Color MenuItemPressedGradientEnd => ShareXResources.Theme.MenuHighlightColor;
        public override Color RaftingContainerGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color RaftingContainerGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color SeparatorDark => ShareXResources.Theme.SeparatorDarkColor;
        public override Color SeparatorLight => ShareXResources.Theme.SeparatorLightColor;
        public override Color StatusStripGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color StatusStripGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripBorder => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripDropDownBackground => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripGradientMiddle => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripContentPanelGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripContentPanelGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripPanelGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color ToolStripPanelGradientEnd => ShareXResources.Theme.BackgroundColor;
        public override Color OverflowButtonGradientBegin => ShareXResources.Theme.BackgroundColor;
        public override Color OverflowButtonGradientMiddle => ShareXResources.Theme.BackgroundColor;
        public override Color OverflowButtonGradientEnd => ShareXResources.Theme.BackgroundColor;
    }
}