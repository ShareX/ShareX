#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
        public override Color ButtonSelectedHighlight
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return ShareXResources.Theme.MenuHighlightBorderColor; }
        }
        public override Color ButtonPressedHighlight
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return ShareXResources.Theme.MenuHighlightBorderColor; }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return ShareXResources.Theme.MenuHighlightBorderColor; }
        }
        public override Color ButtonPressedBorder
        {
            get { return ShareXResources.Theme.MenuHighlightBorderColor; }
        }
        public override Color ButtonSelectedBorder
        {
            get { return ShareXResources.Theme.MenuHighlightBorderColor; }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color CheckBackground
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color CheckSelectedBackground
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color CheckPressedBackground
        {
            get { return ShareXResources.Theme.MenuCheckBackgroundColor; }
        }
        public override Color GripDark
        {
            get { return ShareXResources.Theme.SeparatorDarkColor; }
        }
        public override Color GripLight
        {
            get { return ShareXResources.Theme.SeparatorLightColor; }
        }
        public override Color ImageMarginGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ImageMarginGradientMiddle
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ImageMarginGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ImageMarginRevealedGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ImageMarginRevealedGradientMiddle
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ImageMarginRevealedGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color MenuStripGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color MenuStripGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color MenuItemSelected
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color MenuItemBorder
        {
            get { return ShareXResources.Theme.MenuBorderColor; }
        }
        public override Color MenuBorder
        {
            get { return ShareXResources.Theme.MenuBorderColor; }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return ShareXResources.Theme.MenuHighlightColor; }
        }
        public override Color RaftingContainerGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color RaftingContainerGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color SeparatorDark
        {
            get { return ShareXResources.Theme.SeparatorDarkColor; }
        }
        public override Color SeparatorLight
        {
            get { return ShareXResources.Theme.SeparatorLightColor; }
        }
        public override Color StatusStripGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color StatusStripGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripBorder
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripDropDownBackground
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripContentPanelGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripContentPanelGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripPanelGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color ToolStripPanelGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return ShareXResources.Theme.BackgroundColor; }
        }
    }
}