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

using ShareX.HelpersLib;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;

namespace ShareX.UploadersLib
{
    public class LocalhostAccount : ICloneable
    {
        [Category("Localhost"), Description("Shown in the list as: Name - LocalhostRoot:Port")]
        public string Name { get; set; }

        [Category("Localhost"), Description(@"Root folder, e.g. C:\Inetpub\wwwroot")]
        [Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string LocalhostRoot { get; set; }

        [Category("Localhost"), Description("Port Number"), DefaultValue(80)]
        public int Port { get; set; }

        [Category("Localhost")]
        public string UserName { get; set; }

        [Category("Localhost"), PasswordPropertyText(true), JsonEncrypt]
        public string Password { get; set; }

        [Category("Localhost"), Description("Localhost Sub-folder Path, e.g. screenshots, %y = year, %mo = month. SubFolderPath will be automatically appended to HttpHomePath if HttpHomePath does not start with @")]
        public string SubFolderPath { get; set; }

        [Category("Localhost"), Description("HTTP Home Path, %host = Host e.g. google.com without http:// because you choose that in Remote Protocol.\nURL = HttpHomePath + SubFolderPath + FileName\nURL = Host + SubFolderPath + FileName (if HttpHomePath is empty)")]
        public string HttpHomePath { get; set; }

        [Category("Localhost"), Description("Automatically add sub folder path to end of http home path"), DefaultValue(true)]
        public bool HttpHomePathAutoAddSubFolderPath { get; set; }

        [Category("Localhost"), Description("Don't add file extension to URL"), DefaultValue(false)]
        public bool HttpHomePathNoExtension { get; set; }

        [Category("Localhost"), Description("Choose an appropriate protocol to be accessed by the browser. Use 'file' for Shared Folders. RemoteProtocol will always be 'file' if HTTP Home Path is empty. "), DefaultValue(BrowserProtocol.file)]
        public BrowserProtocol RemoteProtocol { get; set; }

        [Category("Localhost"), Description("file://Host:Port"), Browsable(false)]
        public string LocalUri
        {
            get
            {
                if (string.IsNullOrEmpty(LocalhostRoot))
                {
                    return "";
                }

                return new Uri(FileHelpers.ExpandFolderVariables(LocalhostRoot)).AbsoluteUri;
            }
        }

        private string exampleFileName = "screenshot.jpg";

        [Category("Localhost"), Description("Preview of the Localhost Path based on the settings above")]
        public string PreviewLocalPath
        {
            get
            {
                return GetLocalhostUri(exampleFileName);
            }
        }

        [Category("Localhost"), Description("Preview of the HTTP Path based on the settings above")]
        public string PreviewRemotePath
        {
            get
            {
                return GetUriPath(exampleFileName);
            }
        }

        public LocalhostAccount()
        {
            Name = "New account";
            LocalhostRoot = "";
            Port = 80;
            SubFolderPath = "";
            HttpHomePath = "";
            HttpHomePathAutoAddSubFolderPath = true;
            HttpHomePathNoExtension = false;
            RemoteProtocol = BrowserProtocol.file;
        }

        public string GetSubFolderPath()
        {
            return NameParser.Parse(NameParserType.URL, SubFolderPath.Replace("%host", FileHelpers.ExpandFolderVariables(LocalhostRoot)));
        }

        public string GetHttpHomePath()
        {
            // @ deprecated
            if (HttpHomePath.StartsWith("@"))
            {
                HttpHomePath = HttpHomePath.Substring(1);
                HttpHomePathAutoAddSubFolderPath = false;
            }

            HttpHomePath = URLHelpers.RemovePrefixes(HttpHomePath);

            return NameParser.Parse(NameParserType.URL, HttpHomePath.Replace("%host", FileHelpers.ExpandFolderVariables(LocalhostRoot)));
        }

        public string GetUriPath(string fileName)
        {
            if (string.IsNullOrEmpty(LocalhostRoot))
            {
                return "";
            }

            if (HttpHomePathNoExtension)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            fileName = URLHelpers.URLEncode(fileName);

            string subFolderPath = GetSubFolderPath();
            subFolderPath = URLHelpers.URLEncode(subFolderPath, true);

            string httpHomePath = GetHttpHomePath();

            string path;

            if (string.IsNullOrEmpty(httpHomePath))
            {
                RemoteProtocol = BrowserProtocol.file;
                path = LocalUri.Replace("file://", "");
            }
            else
            {
                path = URLHelpers.URLEncode(httpHomePath, true);
            }

            if (Port != 80)
            {
                path = string.Format("{0}:{1}", path, Port);
            }

            if (HttpHomePathAutoAddSubFolderPath)
            {
                path = URLHelpers.CombineURL(path, subFolderPath);
            }

            path = URLHelpers.CombineURL(path, fileName);

            string remoteProtocol = RemoteProtocol.GetDescription();

            if (!path.StartsWith(remoteProtocol))
            {
                path = remoteProtocol + path;
            }

            return path;
        }

        public string GetLocalhostPath(string fileName)
        {
            if (string.IsNullOrEmpty(LocalhostRoot))
            {
                return "";
            }

            return Path.Combine(Path.Combine(FileHelpers.ExpandFolderVariables(LocalhostRoot), GetSubFolderPath()), fileName);
        }

        public string GetLocalhostUri(string fileName)
        {
            string localhostAddress = LocalUri;

            if (string.IsNullOrEmpty(localhostAddress))
            {
                return "";
            }

            return URLHelpers.CombineURL(localhostAddress, GetSubFolderPath(), fileName);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2}", Name, FileHelpers.GetVariableFolderPath(LocalhostRoot), Port);
        }

        public LocalhostAccount Clone()
        {
            return MemberwiseClone() as LocalhostAccount;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}