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
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public abstract class UpdateChecker
    {
        /// <summary>For testing purposes.</summary>
        public static bool ForceUpdate { get; private set; } = false;

        public UpdateStatus Status { get; set; }
        public Version CurrentVersion { get; set; }
        public Version LatestVersion { get; set; }
        public ReleaseChannelType ReleaseType { get; set; }
        public bool IsDev { get; set; }
        public bool IsPortable { get; set; }
        public bool IgnoreRevision { get; set; }

        private string fileName;

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    return HttpUtility.UrlDecode(DownloadURL.Substring(DownloadURL.LastIndexOf('/') + 1));
                }

                return fileName;
            }
            set
            {
                fileName = value;
            }
        }

        public string DownloadURL { get; set; }

        public void RefreshStatus()
        {
            if (CurrentVersion == null)
            {
                CurrentVersion = Version.Parse(Application.ProductVersion);
            }

            if (Status != UpdateStatus.UpdateCheckFailed && CurrentVersion != null && LatestVersion != null && !string.IsNullOrEmpty(DownloadURL) &&
                (ForceUpdate || Helpers.CompareVersion(CurrentVersion, LatestVersion, IgnoreRevision) < 0))
            {
                Status = UpdateStatus.UpdateAvailable;
            }
            else
            {
                Status = UpdateStatus.UpToDate;
            }
        }

        public abstract Task CheckUpdateAsync();

        public void DownloadUpdate()
        {
            DebugHelper.WriteLine("Updating ShareX from version {0} to {1}", CurrentVersion, LatestVersion);

            if (IsPortable)
            {
                URLHelpers.OpenURL(DownloadURL);
            }
            else
            {
                using (DownloaderForm updaterForm = new DownloaderForm(this))
                {
                    updaterForm.ShowDialog();

                    if (updaterForm.Status == DownloaderFormStatus.InstallStarted)
                    {
                        Application.Exit();
                    }
                }
            }
        }
    }
}