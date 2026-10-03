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

namespace ShareX.HelpersLib
{
    public enum TaskbarProgressBarStatus
    {
        NoProgress = 0,
        Indeterminate = 0x1,
        Normal = 0x2,
        Error = 0x4,
        Paused = 0x8
    }

    /// <summary>Upload progress on the taskbar button or dock icon, through <see cref="ITaskbarService"/>.</summary>
    public static class TaskbarManager
    {
        public static bool Enabled { get; set; }

        public static bool IsPlatformSupported => PlatformServices.IsInitialized && Taskbar.Support.IsSupported;

        private static ITaskbarService Taskbar => PlatformServices.Current.Taskbar;

        public static void SetProgressValue(int currentValue, int maximumValue = 100)
        {
            if (Enabled && IsPlatformSupported)
            {
                Taskbar.SetProgressValue(currentValue, maximumValue);
            }
        }

        public static void SetProgressState(TaskbarProgressBarStatus state)
        {
            if (Enabled && IsPlatformSupported)
            {
                Taskbar.SetProgressState(state switch
                {
                    TaskbarProgressBarStatus.Indeterminate => TaskbarProgressState.Indeterminate,
                    TaskbarProgressBarStatus.Normal => TaskbarProgressState.Normal,
                    TaskbarProgressBarStatus.Error => TaskbarProgressState.Error,
                    TaskbarProgressBarStatus.Paused => TaskbarProgressState.Paused,
                    _ => TaskbarProgressState.None
                });
            }
        }
    }
}
