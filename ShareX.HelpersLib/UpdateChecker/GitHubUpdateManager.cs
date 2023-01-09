#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using Timer = System.Threading.Timer;

namespace ShareX.HelpersLib
{
    public class GitHubUpdateManager : IDisposable
    {
        public bool AllowAutoUpdate { get; set; } // ConfigureAutoUpdate function must be called after change this
        public bool AutoUpdateEnabled { get; set; } = true;
        public TimeSpan UpdateCheckInterval { get; private set; } = TimeSpan.FromHours(1);
        public string GitHubOwner { get; set; }
        public string GitHubRepo { get; set; }
        public bool IsDev { get; set; } // If current build is dev and latest stable release is same version as current build then it will be downloaded
        public bool IsPortable { get; set; } // If current build is portable then download URL will be opened in browser instead of downloading it
        public bool CheckPreReleaseUpdates { get; set; }

        private bool firstUpdateCheck = true;
        private Timer updateTimer = null;
        private readonly object updateTimerLock = new object();

        public GitHubUpdateManager(string owner, string repo)
        {
            GitHubOwner = owner;
            GitHubRepo = repo;
        }

        public GitHubUpdateManager(string owner, string repo, bool dev, bool portable) : this(owner, repo)
        {
            IsDev = dev;
            IsPortable = portable;
        }

        public void ConfigureAutoUpdate()
        {
            lock (updateTimerLock)
            {
                if (AllowAutoUpdate)
                {
                    if (updateTimer == null)
                    {
                        updateTimer = new Timer(state => CheckUpdate(), null, TimeSpan.Zero, UpdateCheckInterval);
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
            if (AutoUpdateEnabled && !UpdateMessageBox.IsOpen)
            {
                UpdateChecker updateChecker = CreateUpdateChecker();
                updateChecker.CheckUpdate();

                if (UpdateMessageBox.Start(updateChecker, firstUpdateCheck) == DialogResult.No)
                {
                    AutoUpdateEnabled = false;
                }

                firstUpdateCheck = false;
            }
        }

        public GitHubUpdateChecker CreateUpdateChecker()
        {
            return new GitHubUpdateChecker(GitHubOwner, GitHubRepo)
            {
                IsDev = IsDev,
                IsPortable = IsPortable,
                IncludePreRelease = CheckPreReleaseUpdates
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