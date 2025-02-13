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
    public class FTPAccount : ICloneable
    {
        [Category("FTP"), Description("Shown in the list as: Name - Server:Port")]
        public string Name { get; set; }

        [Category("Account"), Description("Connection protocol"), DefaultValue(FTPProtocol.FTP)]
        public FTPProtocol Protocol { get; set; }

        [Category("FTP"), Description("Host, e.g. google.com")]
        public string Host { get; set; }

        [Category("FTP"), Description("Port number"), DefaultValue(21)]
        public int Port { get; set; }

        [Category("FTP")]
        public string Username { get; set; }

        [Category("FTP"), PasswordPropertyText(true), JsonEncrypt]
        public string Password { get; set; }

        [Category("FTP"), Description("Set true for active or false for passive"), DefaultValue(false)]
        public bool IsActive { get; set; }

        [Category("FTP"), Description("FTP sub folder path, example: Screenshots.\r\nYou can use name parsing: %y = year, %mo = month.")]
        public string SubFolderPath { get; set; }

        [Category("FTP"), Description("Choose an appropriate protocol to be accessed by the browser"), DefaultValue(BrowserProtocol.http)]
        public BrowserProtocol BrowserProtocol { get; set; }

        [Category("FTP"), Description("URL = HttpHomePath + SubFolderPath + FileName\r\nIf HttpHomePath is empty then URL = Host + SubFolderPath + FileName\r\n%host = Host")]
        public string HttpHomePath { get; set; }

        [Category("FTP"), Description("Automatically add sub folder path to end of http home path"), DefaultValue(false)]
        public bool HttpHomePathAutoAddSubFolderPath { get; set; }

        [Category("FTP"), Description("Don't add file extension to URL"), DefaultValue(false)]
        public bool HttpHomePathNoExtension { get; set; }

        [Category("FTP"), Description("Protocol://Host:Port"), Browsable(false)]
        public string FTPAddress
        {
            get
            {
                if (string.IsNullOrEmpty(Host))
                {
                    return "";
                }

                string serverProtocol;

                switch (Protocol)
                {
                    default:
                    case FTPProtocol.FTP:
                        serverProtocol = "ftp://";
                        break;
                    case FTPProtocol.FTPS:
                        serverProtocol = "ftps://";
                        break;
                    case FTPProtocol.SFTP:
                        serverProtocol = "sftp://";
                        break;
                }

                return string.Format("{0}{1}:{2}", serverProtocol, Host, Port);
            }
        }

        private string exampleFileName = "example.png";

        [Category("FTP"), Description("Preview of the FTP path based on the settings above")]
        public string PreviewFtpPath => GetFtpPath(exampleFileName);

        [Category("FTP"), Description("Preview of the HTTP path based on the settings above")]
        public string PreviewHttpPath
        {
            get
            {
                try
                {
                    return GetUriPath(exampleFileName);
                }
                catch
                {
                    return "";
                }
            }
        }

        [Category("FTPS"), Description("Type of SSL to use. Explicit is TLS, Implicit is SSL."), DefaultValue(FTPSEncryption.Explicit)]
        public FTPSEncryption FTPSEncryption { get; set; }

        [Category("FTPS"), Description("Certificate file location. Optional setting.")]
        [Editor(typeof(CertFileNameEditor), typeof(UITypeEditor))]
        public string FTPSCertificateLocation { get; set; }

        [Category("SFTP"), Description("Key location")]
        [Editor(typeof(KeyFileNameEditor), typeof(UITypeEditor))]
        public string Keypath { get; set; }

        [Category("SFTP"), Description("OpenSSH key passphrase"), PasswordPropertyText(true), JsonEncrypt]
        public string Passphrase { get; set; }

        public FTPAccount()
        {
            Name = "New account";
            Protocol = FTPProtocol.FTP;
            Host = "";
            Port = 21;
            IsActive = false;
            SubFolderPath = "";
            BrowserProtocol = BrowserProtocol.http;
            HttpHomePath = "";
            HttpHomePathAutoAddSubFolderPath = true;
            HttpHomePathNoExtension = false;
            FTPSEncryption = FTPSEncryption.Explicit;
            FTPSCertificateLocation = "";
        }

        public string GetSubFolderPath(string fileName = null, NameParserType nameParserType = NameParserType.URL)
        {
            string path = NameParser.Parse(nameParserType, SubFolderPath.Replace("%host", Host));
            return URLHelpers.CombineURL(path, fileName);
        }

        public string GetHttpHomePath()
        {
            string homePath = HttpHomePath.Replace("%host", Host);

            ShareXCustomUploaderSyntaxParser parser = new ShareXCustomUploaderSyntaxParser();
            parser.UseNameParser = true;
            parser.NameParserType = NameParserType.URL;
            return parser.Parse(homePath);
        }

        public string GetUriPath(string fileName, string subFolderPath = null)
        {
            if (string.IsNullOrEmpty(Host))
            {
                return "";
            }

            if (HttpHomePathNoExtension)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName);
            }

            fileName = URLHelpers.URLEncode(fileName);

            if (subFolderPath == null)
            {
                subFolderPath = GetSubFolderPath();
            }

            UriBuilder httpHomeUri;

            string httpHomePath = GetHttpHomePath();

            if (string.IsNullOrEmpty(httpHomePath))
            {
                string url = Host;

                if (url.StartsWith("ftp."))
                {
                    url = url.Substring(4);
                }

                if (HttpHomePathAutoAddSubFolderPath)
                {
                    url = URLHelpers.CombineURL(url, subFolderPath);
                }

                url = URLHelpers.CombineURL(url, fileName);

                httpHomeUri = new UriBuilder(url);
                httpHomeUri.Port = -1; //Since httpHomePath is not set, it's safe to erase UriBuilder's assumed port number
            }
            else
            {
                //Parse HttpHomePath in to host, port, path and query components
                int firstSlash = httpHomePath.IndexOf('/');
                string httpHome = firstSlash >= 0 ? httpHomePath.Substring(0, firstSlash) : httpHomePath;
                int portSpecifiedAt = httpHome.LastIndexOf(':');

                string httpHomeHost = portSpecifiedAt >= 0 ? httpHome.Substring(0, portSpecifiedAt) : httpHome;
                int httpHomePort = -1;
                string httpHomePathAndQuery = firstSlash >= 0 ? httpHomePath.Substring(firstSlash + 1) : "";
                int querySpecifiedAt = httpHomePathAndQuery.LastIndexOf('?');
                string httpHomeDir = querySpecifiedAt >= 0 ? httpHomePathAndQuery.Substring(0, querySpecifiedAt) : httpHomePathAndQuery;
                string httpHomeQuery = querySpecifiedAt >= 0 ? httpHomePathAndQuery.Substring(querySpecifiedAt + 1) : "";

                if (portSpecifiedAt >= 0)
                    int.TryParse(httpHome.Substring(portSpecifiedAt + 1), out httpHomePort);

                //Build URI
                httpHomeUri = new UriBuilder { Host = httpHomeHost, Path = httpHomeDir, Query = httpHomeQuery };
                if (portSpecifiedAt >= 0)
                {
                    httpHomeUri.Port = httpHomePort;
                }

                if (httpHomeUri.Query.EndsWith("="))
                {
                    //Setting URIBuilder.Query automatically prepends a ? so we must trim it first.
                    if (HttpHomePathAutoAddSubFolderPath)
                    {
                        httpHomeUri.Query = URLHelpers.CombineURL(httpHomeUri.Query.Substring(1), subFolderPath, fileName);
                    }
                    else
                    {
                        httpHomeUri.Query = httpHomeUri.Query.Substring(1) + fileName;
                    }
                }
                else
                {
                    if (HttpHomePathAutoAddSubFolderPath)
                    {
                        httpHomeUri.Path = URLHelpers.CombineURL(httpHomeUri.Path, subFolderPath);
                    }

                    httpHomeUri.Path = URLHelpers.CombineURL(httpHomeUri.Path, fileName);
                }
            }

            httpHomeUri.Scheme = BrowserProtocol.GetDescription();
            return httpHomeUri.Uri.OriginalString;
        }

        public string GetFtpPath(string fileName)
        {
            if (string.IsNullOrEmpty(FTPAddress))
            {
                return "";
            }

            return URLHelpers.CombineURL(FTPAddress, GetSubFolderPath(fileName, NameParserType.FilePath));
        }

        public override string ToString()
        {
            return $"{Name} ({Host}:{Port})";
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