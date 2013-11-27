#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib;
using System;
using System.Text;
using System.Web;

namespace HelpersLib
{
    public class UpdateInfo
    {
        public UpdateStatus Status { get; set; }
        public Version CurrentVersion { get; set; }
        public Version LatestVersion { get; set; }

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
        public ReleaseChannelType ReleaseChannel { get; set; }

        private bool forceUpdate = false; // For testing purposes

        public UpdateInfo()
        {
            ReleaseChannel = ReleaseChannelType.Stable;
        }

        public void RefreshStatus()
        {
            if (Status != UpdateStatus.UpdateCheckFailed && CurrentVersion != null && LatestVersion != null &&
                !string.IsNullOrEmpty(DownloadURL) && (forceUpdate || Helpers.CheckVersion(CurrentVersion, LatestVersion)))
            {
                Status = UpdateStatus.UpdateAvailable;
            }
            else
            {
                Status = UpdateStatus.UpToDate;
            }
        }
    }
}