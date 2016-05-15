/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Controls
{
    /// <summary>
    /// Prevent having a gradient background in the toolstrip, and the overflow button
    /// See: http://stackoverflow.com/a/16926979
    /// </summary>
    internal class CustomProfessionalColorTable : ProfessionalColorTable
    {
        public override Color ToolStripGradientBegin
        {
            get { return SystemColors.Control; }
        }
        public override Color ToolStripGradientMiddle
        {
            get { return SystemColors.Control; }
        }
        public override Color ToolStripGradientEnd
        {
            get { return SystemColors.Control; }
        }
        public override Color OverflowButtonGradientBegin
        {
            get { return SystemColors.Control; }
        }
        public override Color OverflowButtonGradientMiddle
        {
            get { return SystemColors.Control; }
        }
        public override Color OverflowButtonGradientEnd
        {
            get { return SystemColors.Control; }
        }
    }

    /// <summary>
    /// ToolStripProfessionalRenderer without having a visual artifact
    /// See: http://stackoverflow.com/a/16926979 and http://stackoverflow.com/a/13418840
    /// </summary>
    internal class CustomToolStripProfessionalRenderer : ToolStripProfessionalRenderer
    {
        public CustomToolStripProfessionalRenderer() : base(new CustomProfessionalColorTable())
        {
            RoundedEdges = false;
        }

        /// <summary>
        /// By overriding the OnRenderToolStripBorder we can make the ToolStrip without border
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Don't draw a border
        }
    }
}