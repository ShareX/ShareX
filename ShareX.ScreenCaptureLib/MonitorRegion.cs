#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using ShareX.ScreenCaptureLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class MonitorRegion
    {
        public string MonitorIdentifier { get; private set; }

        public Rectangle Bounds { get; private set; }

        public MonitorRegion(Screen monitor, int monitorNumber)
        {
            Bounds = monitor.Bounds;
            CreateTheNameFromBoundsAndMonitorNumber(monitorNumber);
        }

        private void CreateTheNameFromBoundsAndMonitorNumber(int monitorNumber)
        {
            MonitorIdentifier = String.Format(Resources.ScreenRegion_Name_Monitor_0___X__1__Y__2__Width__3__Height__4_, monitorNumber, Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        }

        public override string ToString()
        {
            return MonitorIdentifier;
        }
    }
}