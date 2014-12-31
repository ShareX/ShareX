#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Net;
using System.Web;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public abstract class UpdateChecker
    {
        public UpdateStatus Status { get; set; }
        public Version CurrentVersion { get; set; }
        public Version LatestVersion { get; set; }
        public ReleaseChannelType ReleaseType { get; set; }
        public bool IsBeta { get; set; }
        public IWebProxy Proxy { get; set; }

        private string filename;

        public string Filename
        {
            get
            {
                if (string.IsNullOrEmpty(filename))
                {
                    return HttpUtility.UrlDecode(DownloadURL.Substring(DownloadURL.LastIndexOf('/') + 1));
                }

                return filename;
            }
            set
            {
                filename = value;
            }
        }

        public string DownloadURL { get; set; }

        private const bool forceUpdate = false; // For testing purposes

        public void RefreshStatus()
        {
            if (CurrentVersion == null)
            {
                CurrentVersion = Version.Parse(Application.ProductVersion);
            }

            if (Status != UpdateStatus.UpdateCheckFailed && CurrentVersion != null && LatestVersion != null && !string.IsNullOrEmpty(DownloadURL) &&
                (forceUpdate || Helpers.CompareVersion(CurrentVersion, LatestVersion) < 0 || (IsBeta && Helpers.CompareVersion(CurrentVersion, LatestVersion) == 0)))
            {
                Status = UpdateStatus.UpdateAvailable;
            }
            else
            {
                Status = UpdateStatus.UpToDate;
            }
        }

        public abstract void CheckUpdate();
    }
}