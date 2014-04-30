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
using System.Windows.Forms;
using UploadersLib.HelperClasses;

namespace UploadersLib
{
    public class FTPAccount : ICloneable
    {
        [Category("Account"), Description("Connection protocol"), DefaultValue(FTPProtocol.FTP)]
        public FTPProtocol Protocol { get; set; }

        [Category("FTP"), Description("Shown in the list as: Name - Server:Port")]
        public string Name { get; set; }

        [Category("FTP"), Description("Host, e.g. google.com")]
        public string Host { get; set; }

        [Category("FTP"), Description("Port number"), DefaultValue(21)]
        public int Port { get; set; }

        [Category("FTP")]
        public string Username { get; set; }

        [Category("FTP"), PasswordPropertyText(true)]
        public string Password { get; set; }

        [Category("FTP"), Description("Choose an appropriate protocol to be accessed by the server. This affects the server address."), DefaultValue(ServerProtocol.Ftp)]
        public ServerProtocol ServerProtocol { get; set; }

        [Category("FTP"), Description("FTP sub folder path, example: Screenshots.\r\nYou can use name parsing: %y = year, %mo = month."), DefaultValue("")]
        public string SubFolderPath { get; set; }

        [Category("FTP"), Description("Choose an appropriate protocol to be accessed by the browser"), DefaultValue(BrowserProtocol.Http)]
        public BrowserProtocol BrowserProtocol { get; set; }

        [Category("FTP"), Description("URL = HttpHomePath + SubFolderPath + FileName\r\nIf HttpHomePath is empty then URL = Host + SubFolderPath + FileName\r\n%host = Host"), DefaultValue("")]
        public string HttpHomePath { get; set; }

        [Category("FTP"), Description("Automatically add sub folder path to end of http home path"), DefaultValue(true)]
        public bool HttpHomePathAutoAddSubFolderPath { get; set; }

        [Category("FTP"), Description("Don't add file extension to URL"), DefaultValue(false)]
        public bool HttpHomePathNoExtension { get; set; }

        [Category("FTP"), Description("Set true for active or false for passive"), DefaultValue(false)]
        public bool IsActive { get; set; }

        [Category("FTP"), Description("ftp://Host:Port"), Browsable(false)]
        public string FTPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(Host))
                {
                    return string.Empty;
                }

                return string.Format("{0}{1}:{2}", ServerProtocol.GetDescription(), Host, Port);
            }
        }

        private string exampleFilename = "screenshot.jpg";

        [Category("FTP"), Description("Preview of the FTP path based on the settings above")]
        public string PreviewFtpPath
        {
            get
            {
                return GetFtpPath(exampleFilename);
            }
        }

        [Category("FTP"), Description("Preview of the HTTP path based on the settings above")]
        public string PreviewHttpPath
        {
            get
            {
                return GetUriPath(exampleFilename);
            }
        }

        [Category("FTPS"), Description("Certification location")]
        [Editor(typeof(CertFileNameEditor), typeof(UITypeEditor))]
        public string FtpsCertLocation { get; set; }

        [Category("FTPS"), Description("Security protocol"), DefaultValue(FtpSecurityProtocol.Ssl2Explicit)]
        public FtpSecurityProtocol FtpsSecurityProtocol { get; set; }

        [Category("SFTP"), Description("OpenSSH key passphrase"), PasswordPropertyText(true)]
        public string Passphrase { get; set; }

        [Category("SFTP"), Description("Key location")]
        [Editor(typeof(KeyFileNameEditor), typeof(UITypeEditor))]
        public string Keypath { get; set; }

        private static bool warning1Showed = false;

        public FTPAccount()
        {
            Protocol = FTPProtocol.FTP;
            Name = "New Account";
            Host = "host";
            Port = 21;
            Username = "username";
            Password = "password";
            ServerProtocol = ServerProtocol.Ftp;
            SubFolderPath = string.Empty;
            BrowserProtocol = BrowserProtocol.Http;
            HttpHomePath = string.Empty;
            HttpHomePathNoExtension = false;
            IsActive = false;
            FtpsSecurityProtocol = FtpSecurityProtocol.Ssl2Explicit;
        }

        public FTPAccount(string name)
            : this()
        {
            Name = name;
        }

        public string GetSubFolderPath()
        {
            NameParser parser = new NameParser(NameParserType.URL);
            return parser.Parse(SubFolderPath.Replace("%host", Host));
        }

        public string GetSubFolderPath(string filename)
        {
            return Helpers.CombineURL(GetSubFolderPath(), filename);
        }

        public string GetHttpHomePath()
        {
            HttpHomePath = FTPHelpers.RemovePrefixes(HttpHomePath);
            NameParser nameParser = new NameParser(NameParserType.URL);
            return nameParser.Parse(HttpHomePath.Replace("%host", Host));
        }

        public string GetUriPath(string filename)
        {
            if (string.IsNullOrEmpty(Host))
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

            if (httpHomePath.StartsWith("@"))
            {
                if (!warning1Showed)
                {
                    // TODO: Remove this warning 2 release later.
                    MessageBox.Show("Please use 'HttpHomePathAutoAddSubFolderPath' setting instead adding @ character in beginning of 'HttpHomePath' setting.", "ShareX - FTP account problem",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    warning1Showed = true;
                }

                httpHomePath = httpHomePath.Substring(1);
            }

            httpHomePath = Helpers.URLPathEncode(httpHomePath);

            string path;

            if (string.IsNullOrEmpty(httpHomePath))
            {
                string host = Host;

                if (host.StartsWith("ftp."))
                {
                    host = host.Substring(4);
                }

                path = Helpers.CombineURL(host, subFolderPath, filename);
            }
            else
            {
                if (HttpHomePathAutoAddSubFolderPath)
                {
                    path = Helpers.CombineURL(httpHomePath, subFolderPath);
                }
                else
                {
                    path = httpHomePath;
                }

                if (path.EndsWith("="))
                {
                    path += filename;
                }
                else
                {
                    path = Helpers.CombineURL(path, filename);
                }
            }

            string browserProtocol = BrowserProtocol.GetDescription();

            if (!path.StartsWith(browserProtocol))
            {
                path = browserProtocol + path;
            }

            return path;
        }

        public string GetFtpPath(string filemame)
        {
            if (string.IsNullOrEmpty(FTPAddress))
            {
                return string.Empty;
            }

            return Helpers.CombineURL(FTPAddress, GetSubFolderPath(filemame));
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}:{2}", Name, Host, Port);
        }

        public FTPAccount Clone()
        {
            return MemberwiseClone() as FTPAccount;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}