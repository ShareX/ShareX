/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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

namespace Greenshot.Controls
{
    /// <summary>
    /// This is an extension of the default ToolStrip and allows us to click it even when the form doesn't have focus.
    /// See: http://blogs.msdn.com/b/rickbrew/archive/2006/01/09/511003.aspx
    /// </summary>
    internal class ToolStripEx : ToolStrip
    {
        private const int WM_MOUSEACTIVATE = 0x21;

        private enum NativeConstants : uint
        {
            MA_ACTIVATE = 1,
            MA_ACTIVATEANDEAT = 2,
        }

        private bool _clickThrough = false;
        /// <summary>
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does not have input focus.
        /// </summary>
        /// <remarks>
        /// Default value is false, which is the same behavior provided by the base ToolStrip class.
        /// </remarks>

        public bool ClickThrough
        {
            get
            {
                return _clickThrough;
            }

            set
            {
                _clickThrough = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (_clickThrough && m.Msg == WM_MOUSEACTIVATE && m.Result == (IntPtr)NativeConstants.MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)NativeConstants.MA_ACTIVATE;
            }
        }
    }
}