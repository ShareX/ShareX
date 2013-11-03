/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

using System;
using System.Windows.Forms;

namespace Greenshot.Helpers
{
    /// <summary>
    /// Enables or disables toolstrip items, taking care of the hierarchy.
    /// (parent) OwnerItems are ENabled with ToolStripItems,
    /// (child) DropDownItems are ENabled and DISabled with ToolStripItems.
    /// </summary>
    public static class ToolStripItemEndisabler
    {
        [Flags]
        private enum PropagationMode { NONE = 0, CHILDREN = 1, ANCESTORS = 2 };

        /// <summary>
        /// Enables all of a ToolStrip's children (recursively),
        /// but not the ToolStrip itself
        /// </summary>
        public static void Enable(ToolStrip ts)
        {
            Endisable(ts, true, PropagationMode.CHILDREN);
        }

        /// <summary>
        /// Disables all of a ToolStrip's children (recursively),
        /// but not the ToolStrip itself
        /// </summary>
        public static void Disable(ToolStrip ts)
        {
            Endisable(ts, false, PropagationMode.CHILDREN);
        }

        /// <summary>
        /// Enables the ToolStripItem, including children (ToolStripDropDownItem)
        /// and ancestor (OwnerItem)
        /// </summary>
        public static void Enable(ToolStripItem tsi)
        {
            Endisable(tsi, true, PropagationMode.CHILDREN | PropagationMode.ANCESTORS);
        }

        /// <summary>
        /// Disables the ToolStripItem, including children (ToolStripDropDownItem),
        /// but NOT the ancestor (OwnerItem)
        /// </summary>
        public static void Disable(ToolStripItem tsi)
        {
            Endisable(tsi, false, PropagationMode.CHILDREN);
        }

        private static void Endisable(ToolStrip ts, bool enable, PropagationMode mode)
        {
            if ((mode & PropagationMode.CHILDREN) == PropagationMode.CHILDREN)
            {
                foreach (ToolStripItem tsi in ts.Items)
                {
                    Endisable(tsi, enable, PropagationMode.CHILDREN);
                }
            }
        }

        private static void Endisable(ToolStripItem tsi, bool enable, PropagationMode mode)
        {
            if (tsi is ToolStripDropDownItem)
            {
                Endisable(tsi as ToolStripDropDownItem, enable, mode);
            }
            else
            {
                tsi.Enabled = enable;
            }
            if ((mode & PropagationMode.ANCESTORS) == PropagationMode.ANCESTORS)
            {
                if (tsi.OwnerItem != null) Endisable(tsi.OwnerItem, enable, PropagationMode.ANCESTORS);
            }
        }

        private static void Endisable(ToolStripDropDownItem tsddi, bool enable, PropagationMode mode)
        {
            if ((mode & PropagationMode.CHILDREN) == PropagationMode.CHILDREN)
            {
                foreach (ToolStripItem tsi in tsddi.DropDownItems)
                {
                    Endisable(tsi, enable, PropagationMode.CHILDREN);
                }
            }
            tsddi.Enabled = enable;
        }
    }
}