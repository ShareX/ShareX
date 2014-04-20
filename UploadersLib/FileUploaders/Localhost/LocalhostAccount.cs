#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Web;
using System.Windows.Forms;

namespace UploadersLib
{
    public class LocalhostAccount
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

        [Category("Localhost"), PasswordPropertyText(true)]
        public string Password { get; set; }

        [Category("Localhost"), Description("Localhost Sub-folder Path, e.g. screenshots, %y = year, %mo = month. SubFolderPath will be automatically appended to HttpHomePath if HttpHomePath does not start with @"), DefaultValue("")]
        public string SubFolderPath { get; set; }

        [Category("Localhost"), Description("HTTP Home Path, %host = Host e.g. google.com without http:// because you choose that in Remote Protocol.\nURL = HttpHomePath + SubFolderPath + FileName\nURL = Host + SubFolderPath + FileName (if HttpHomePath is empty)"), DefaultValue("")]
        public string HttpHomePath { get; set; }

        [Category("Localhost"), Description("Automatically add sub folder path to end of http home path"), DefaultValue(true)]
        public bool HttpHomePathAutoAddSubFolderPath { get; set; }

        [Category("Localhost"), Description("Don't add file extension to URL"), DefaultValue(false)]
        public bool HttpHomePathNoExtension { get; set; }

        [Category("Localhost"), Description("Choose an appropriate protocol to be accessed by the browser. Use 'file' for Shared Folders. RemoteProtocol will always be 'file' if HTTP Home Path is empty. "), DefaultValue(BrowserProtocol.File)]
        public BrowserProtocol RemoteProtocol { get; set; }

        [Category("Localhost"), Description("file://Host:Port"), Browsable(false)]
        public string LocalUri
        {
            get
            {
                if (string.IsNullOrEmpty(LocalhostRoot))
                {
                    return string.Empty;
                }

                return new Uri(LocalhostRoot).AbsoluteUri;
            }
        }

        private string exampleFilename = "screenshot.jpg";

        [Category("Localhost"), Description("Preview of the Localhost Path based on the settings above")]
        public string PreviewLocalPath
        {
            get
            {
                return GetLocalhostUri(exampleFilename);
            }
        }

        [Category("Localhost"), Description("Preview of the HTTP Path based on the settings above")]
        public string PreviewRemotePath
        {
            get
            {
                return GetUriPath(exampleFilename);
            }
        }

        private static bool warning1Showed = false;

        public LocalhostAccount()
        {
            this.ApplyDefaultPropertyValues();
        }

        public LocalhostAccount(string name)
            : this()
        {
            Name = name;
        }

        public string GetSubFolderPath()
        {
            NameParser parser = new NameParser(NameParserType.URL);
            return parser.Parse(SubFolderPath.Replace("%host", LocalhostRoot));
        }

        public string GetHttpHomePath()
        {
            NameParser parser = new NameParser(NameParserType.URL);
            HttpHomePath = FTPHelpers.RemovePrefixes(HttpHomePath.Replace("%host", LocalhostRoot));
            return parser.Parse(HttpHomePath);
        }

        public string GetUriPath(string filename)
        {
            if (string.IsNullOrEmpty(LocalhostRoot))
            {
                return string.Empty;
            }

            if (HttpHomePathNoExtension)
            {
                filename = Path.GetFileNameWithoutExtension(filename);
            }

            filename = Helpers.URLEncode(filename);

            string subFolderPath = GetSubFolderPath();
            subFolderPath = Helpers.URLPathEncode(subFolderPath);

            string httpHomePath = GetHttpHomePath();

            string path;

            if (string.IsNullOrEmpty(httpHomePath))
            {
                RemoteProtocol = BrowserProtocol.File;
                path = LocalUri.Replace("file://", "");
            }
            else
            {
                if (httpHomePath.StartsWith("@"))
                {
                    if (!warning1Showed)
                    {
                        // TODO: Remove this warning 2 release later.
                        MessageBox.Show("Please use 'HttpHomePathAutoAddSubFolderPath' setting instead adding @ character in beginning of 'HttpHomePath' setting.", "ShareX - Localhost account problem",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        warning1Showed = true;
                    }

                    httpHomePath = httpHomePath.Substring(1);
                }

                httpHomePath = Helpers.URLPathEncode(httpHomePath);
                path = httpHomePath;
            }

            if (Port != 80)
            {
                path = string.Format("{0}:{1}", path, Port);
            }

            if (HttpHomePathAutoAddSubFolderPath)
            {
                path = Helpers.CombineURL(path, subFolderPath);
            }

            path = Helpers.CombineURL(path, filename);

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
                return string.Empty;
            }
            return Path.Combine(Path.Combine(LocalhostRoot, GetSubFolderPath()), fileName);
        }

        public string GetLocalhostUri(string fileName)
        {
            string localhostAddress = LocalUri;

            if (string.IsNullOrEmpty(localhostAddress))
            {
                return string.Empty;
            }

            return Helpers.CombineURL(localhostAddress, GetSubFolderPath(), fileName);
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2}", Name, LocalhostRoot, Port);
        }
    }
}