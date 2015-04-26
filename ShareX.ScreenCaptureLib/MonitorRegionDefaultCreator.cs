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

using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public static class MonitorRegionDefaultCreator
    {
        private static readonly int firstMonitorNumber = 1;
        private static int monitorCounter;

        /// <summary>
        /// Return the list of screens available on this computer
        /// </summary>
        public static MonitorRegion[] AllMonitorsRegions
        {
            get
            {
                Screen[] screens = Screen.AllScreens;
                monitorCounter = firstMonitorNumber;

                return screens.Select(screen => new MonitorRegion(screen, monitorCounter++)).ToArray();
            }
        }

        /// <summary>
        /// Return the screen region for the primary monitor
        /// </summary>
        public static MonitorRegion DefaultMonitorRegion
        {
            get
            {
                Screen defaultScreen = Screen.PrimaryScreen;

                return new MonitorRegion(defaultScreen, firstMonitorNumber);
            }
        }
    }
}