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

using System;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public class ControlHider : IDisposable
    {
        public Control Control { get; private set; }
        public int AutoHideTime { get; private set; }

        private Timer timer;

        public ControlHider(Control control, int autoHideTime)
        {
            Control = control;
            AutoHideTime = autoHideTime;

            timer = new Timer();
            timer.Interval = AutoHideTime;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();

            if (Control != null && !Control.IsDisposed)
            {
                Control.Visible = false;
            }
        }

        public void Show()
        {
            if (Control != null && !Control.IsDisposed)
            {
                Control.Visible = true;

                timer.Stop();
                timer.Start();
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}