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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GreenshotPlugin.UnmanagedHelpers
{
    /// <summary>
    /// Description of PsAPI.
    /// </summary>
    public class PsAPI
    {
        [DllImport("psapi", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);

        [DllImport("psapi", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetProcessImageFileName(IntPtr hProcess, StringBuilder lpImageFileName, uint nSize);

        [DllImport("psapi")]
        private static extern int EmptyWorkingSet(IntPtr hwProc);

        /// <summary>
        /// Make the process use less memory by emptying the working set
        /// </summary>
        public static void EmptyWorkingSet()
        {
            LOG.Info("Calling EmptyWorkingSet");
            using (Process currentProcess = Process.GetCurrentProcess())
            {
                EmptyWorkingSet(currentProcess.Handle);
            }
        }
    }
}