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

using ShareX.Platform;

namespace ShareX.AvaloniaUI.Windows
{
    /// <summary>Converts between screen coordinates, which place windows, and a window's logical units.</summary>
    public static class WindowScaling
    {
        /// <summary>
        /// Screen coordinate units per logical unit of a window that renders at <paramref name="renderScaling"/>: the render scaling
        /// on Windows and Linux, which place windows in device pixels, and 1 on macOS, which places them in points even on Retina.
        /// </summary>
        public static double GetPositionScaling(double renderScaling)
        {
            if (PlatformServices.IsInitialized && PlatformServices.Current.Windows.PositionsWindowsInPoints)
            {
                return 1;
            }

            return double.IsFinite(renderScaling) && renderScaling > 0 ? renderScaling : 1;
        }
    }
}
