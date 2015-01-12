#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using ShareX.UploadersLib.HelperClasses;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Web;

namespace ShareX.UploadersLib
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

        [Category("FTP"), Description("Choose an appropriate protocol to be accessed by the server. This affects the server address."), DefaultValue(ServerProtocol.ftp)]
        public ServerProtocol ServerProtocol { get; set; }

        [Category("FTP"), Description("FTP sub folder path, example: Screenshots.\r\nYou can use name parsing: %y = year, %mo = month.")]
        public string SubFolderPath { get; set; }

        [Category("FTP"), Description("Choose an appropriate protocol to be accessed by the browser"), DefaultValue(BrowserProtocol.http)]
        public BrowserProtocol BrowserProtocol { get; set; }

        [Category("FTP"), Description("URL = HttpHomePath + SubFolderPath + FileName\r\nIf HttpHomePath is empty then URL = Host + SubFolderPath + FileName\r\n%host = Host")]
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

        [Category("FTPS"), Description("Type of SSL to use. Explicit is TLS, Implicit is SSL."), DefaultValue(FTPSEncryption.Explicit)]
        public FTPSEncryption FTPSEncryption { get; set; }

        [Category("FTPS"), Description("Certificate file location. Optional setting.")]
        [Editor(typeof(CertFileNameEditor), typeof(UITypeEditor))]
        public string FTPSCertificateLocation { get; set; }

        [Category("SFTP"), Description("OpenSSH key passphrase"), PasswordPropertyText(true)]
        public string Passphrase { get; set; }

        [Category("SFTP"), Description("Key location")]
        [Editor(typeof(KeyFileNameEditor), typeof(UITypeEditor))]
        public string Keypath { get; set; }

        public FTPAccount()
        {
            Protocol = FTPProtocol.FTP;
            Name = "New account";
            Host = "host";
            Port = 21;
            ServerProtocol = ServerProtocol.ftp;
            SubFolderPath = string.Empty;
            BrowserProtocol = BrowserProtocol.http;
            HttpHomePath = string.Empty;
            HttpHomePathAutoAddSubFolderPath = true;
            HttpHomePathNoExtension = false;
            IsActive = false;
            FTPSEncryption = FTPSEncryption.Explicit;
            FTPSCertificateLocation = string.Empty;
        }

        public string GetSubFolderPath(string filename = null, NameParserType nameParserType = NameParserType.URL)
        {
            string path = NameParser.Parse(nameParserType, SubFolderPath.Replace("%host", Host));
            return URLHelpers.CombineURL(path, filename);
        }

        public string GetHttpHomePath()
        {
            // @ deprecated
            if (HttpHomePath.StartsWith("@"))
            {
                HttpHomePath = HttpHomePath.Substring(1);
                HttpHomePathAutoAddSubFolderPath = false;
            }

            return NameParser.Parse(NameParserType.URL, HttpHomePath.Replace("%host", Host));
        }

        public string GetUriPath(string filename, string subFolderPath = null)
        {
            if (string.IsNullOrEmpty(Host))
            {
                return string.Empty;
            }

            if (HttpHomePathNoExtension)
            {
                filename = Path.GetFileNameWithoutExtension(filename);
            }

            filename = HttpUtility.UrlEncode(filename);

            if (subFolderPath == null)
            {
                subFolderPath = GetSubFolderPath();
            }

            UriBuilder httpHomeUri;

            var httpHomePath = GetHttpHomePath();

            if (string.IsNullOrEmpty(httpHomePath))
            {
                var host = Host;

                if (host.StartsWith("ftp."))
                {
                    host = host.Substring(4);
                }

                httpHomeUri = new UriBuilder(URLHelpers.CombineURL(host, subFolderPath, filename));
                httpHomeUri.Port = -1; //Since httpHomePath is not set, it's safe to erase UriBuilder's assumed port number
            }
            else
            {
                //Parse HttpHomePath in to host, port, path and query components
                var firstSlash = httpHomePath.IndexOf('/');
                var httpHome = firstSlash >= 0 ? httpHomePath.Substring(0, firstSlash) : httpHomePath;
                var portSpecifiedAt = httpHome.LastIndexOf(':');

                var httpHomeHost = portSpecifiedAt >= 0 ? httpHome.Substring(0, portSpecifiedAt) : httpHome;
                var httpHomePort = -1;
                var httpHomePathAndQuery = firstSlash >= 0 ? httpHomePath.Substring(firstSlash + 1) : "";
                var querySpecifiedAt = httpHomePathAndQuery.LastIndexOf('?');
                var httpHomeDir = querySpecifiedAt >= 0 ? httpHomePathAndQuery.Substring(0, querySpecifiedAt) : httpHomePathAndQuery;
                var httpHomeQuery = querySpecifiedAt >= 0 ? httpHomePathAndQuery.Substring(querySpecifiedAt + 1) : "";

                if (portSpecifiedAt >= 0)
                    int.TryParse(httpHome.Substring(portSpecifiedAt + 1), out httpHomePort);

                //Build URI
                httpHomeUri = new UriBuilder { Host = httpHomeHost, Path = httpHomeDir, Query = httpHomeQuery };
                if (portSpecifiedAt >= 0)
                    httpHomeUri.Port = httpHomePort;

                if (httpHomeUri.Query.EndsWith("="))
                {
                    //Setting URIBuilder.Query automatically prepends a ? so we must trim it first.
                    if (HttpHomePathAutoAddSubFolderPath)
                        httpHomeUri.Query = URLHelpers.CombineURL(httpHomeUri.Query.Substring(1), subFolderPath, filename);
                    else
                        httpHomeUri.Query = httpHomeUri.Query.Substring(1) + filename;
                }
                else
                {
                    if (HttpHomePathAutoAddSubFolderPath)
                        httpHomeUri.Path = URLHelpers.CombineURL(httpHomeUri.Path, subFolderPath);

                    httpHomeUri.Path = URLHelpers.CombineURL(httpHomeUri.Path, filename);
                }
            }

            httpHomeUri.Scheme = BrowserProtocol.GetDescription();
            return httpHomeUri.Uri.ToString();
        }

        public string GetFtpPath(string filemame)
        {
            if (string.IsNullOrEmpty(FTPAddress))
            {
                return string.Empty;
            }

            return URLHelpers.CombineURL(FTPAddress, GetSubFolderPath(filemame, NameParserType.FolderPath));
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