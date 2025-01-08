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

namespace ShareX.HelpersLib
{
    public class DWMManager : IDisposable
    {
        private bool isDWMEnabled;
        private bool autoEnable;

        public DWMManager()
        {
            isDWMEnabled = NativeMethods.IsDWMEnabled();
        }

        public bool AutoDisable()
        {
            if (isDWMEnabled)
            {
                ChangeComposition(false);
                autoEnable = true;
                return true;
            }

            return false;
        }

        public void ChangeComposition(bool enable)
        {
            try
            {
                NativeMethods.DwmEnableComposition(enable ? DWM_EC.DWM_EC_ENABLECOMPOSITION : DWM_EC.DWM_EC_DISABLECOMPOSITION);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public void Dispose()
        {
            if (isDWMEnabled && autoEnable)
            {
                ChangeComposition(true);
                autoEnable = false;
            }
        }
    }
}