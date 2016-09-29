#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using ShareX.HelpersLib;
using System;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace ShareX
{
    public class UpdateManager : IDisposable
    {
        public double UpdateCheckInterval { get; private set; } = 1; // Hour

        private bool firstUpdateCheck = true;
        private Timer updateTimer = null;
        private readonly object updateTimerLock = new object();

        public UpdateManager()
        {
        }

        public UpdateManager(double updateCheckInterval)
        {
            UpdateCheckInterval = updateCheckInterval;
        }

        public void ConfigureAutoUpdate()
        {
            lock (updateTimerLock)
            {
                if (Program.Settings.AutoCheckUpdate && !Program.PortableApps)
                {
                    if (updateTimer == null)
                    {
                        updateTimer = new Timer(state => CheckUpdate(), null, TimeSpan.Zero, TimeSpan.FromHours(UpdateCheckInterval));
                    }
                }
                else
                {
                    Dispose();
                }
            }
        }

        private void CheckUpdate()
        {
            if (!UpdateMessageBox.DontShow && !UpdateMessageBox.IsOpen)
            {
                UpdateChecker updateChecker = CreateUpdateChecker();
                updateChecker.CheckUpdate();

                if (UpdateMessageBox.Start(updateChecker, firstUpdateCheck) != DialogResult.Yes)
                {
                    TimeSpan interval = TimeSpan.FromHours(24);
                    updateTimer.Change(interval, interval);
                }

                firstUpdateCheck = false;
            }
        }

        public static UpdateChecker CreateUpdateChecker()
        {
            return new GitHubUpdateChecker("ShareX", "ShareX")
            {
                IsBeta = Program.Beta,
                IsPortable = Program.Portable,
                IncludePreRelease = Program.Settings.CheckPreReleaseUpdates,
                Proxy = HelpersOptions.CurrentProxy.GetWebProxy()
            };
        }

        public void Dispose()
        {
            if (updateTimer != null)
            {
                updateTimer.Dispose();
                updateTimer = null;
            }
        }
    }
}