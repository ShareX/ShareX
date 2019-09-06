#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonSelectedHighlightBorder
        {
            get { return Color.FromArgb(255, 116, 129, 152); }
        }
        public override Color ButtonPressedHighlight
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonPressedHighlightBorder
        {
            get { return Color.FromArgb(255, 116, 129, 152); }
        }
        public override Color ButtonCheckedHighlight
        {
            get { return Color.FromArgb(255, 55, 63, 74); }
        }
        public override Color ButtonCheckedHighlightBorder
        {
            get { return Color.FromArgb(255, 116, 129, 152); }
        }
        public override Color ButtonPressedBorder
        {
            get { return Color.FromArgb(255, 116, 129, 152); }
        }
        public override Color ButtonSelectedBorder
        {
            get { return Color.FromArgb(255, 116, 129, 152); }
        }
        public override Color ButtonCheckedGradientBegin
        {
            get { return Color.FromArgb(255, 55, 63, 74); }
        }
        public override Color ButtonCheckedGradientMiddle
        {
            get { return Color.FromArgb(255, 55, 63, 74); }
        }
        public override Color ButtonCheckedGradientEnd
        {
            get { return Color.FromArgb(255, 55, 63, 74); }
        }
        public override Color ButtonSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonSelectedGradientMiddle
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonPressedGradientBegin
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color ButtonPressedGradientEnd
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color CheckBackground
        {
            get { return Color.FromArgb(255, 74, 83, 100); }
        }
        public override Color CheckSelectedBackground
        {
            get { return Color.FromArgb(255, 74, 83, 100); }
        }
        public override Color CheckPressedBackground
        {
            get { return Color.FromArgb(255, 74, 83, 100); }
        }
        public override Color GripDark
        {
            get { return Color.FromArgb(255, 22, 26, 31); }
        }
        public override Color GripLight
        {
            get { return Color.FromArgb(255, 74, 83, 100); }
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
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color MenuItemBorder
        {
            get { return Color.FromArgb(255, 22, 26, 31); }
        }
        public override Color MenuBorder
        {
            get { return Color.FromArgb(255, 22, 26, 31); }
        }
        public override Color MenuItemSelectedGradientBegin
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color MenuItemSelectedGradientEnd
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color MenuItemPressedGradientBegin
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color MenuItemPressedGradientMiddle
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
        }
        public override Color MenuItemPressedGradientEnd
        {
            get { return Color.FromArgb(255, 30, 34, 40); }
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
            get { return Color.FromArgb(255, 22, 26, 31); }
        }
        public override Color SeparatorLight
        {
            get { return Color.FromArgb(255, 56, 64, 75); }
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