#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class RapidShare : FileUploader
    {
        private const string rapidshareURL = "https://api.rapidshare.com/cgi-bin/rsapi.cgi";
        private const string rapidshareUploadURL = "https://rs{0}.rapidshare.com/cgi-bin/rsapi.cgi";

        public string Username { get; set; }

        public string Password { get; set; }

        public string FolderID { get; set; }

        public RapidShare(string username, string password, string folderID = null)
        {
            Username = username;
            Password = password;
            FolderID = folderID;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Errors.Add("RapidShare account username or password is empty.");
                return null;
            }

            string url = NextUploadServer();

            if (string.IsNullOrEmpty(url))
            {
                Errors.Add("RapidShare next upload server URL is empty.");
                return null;
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sub", "upload");
            args.Add("login", Username);
            args.Add("password", Password);

            if (!string.IsNullOrEmpty(FolderID))
            {
                args.Add("folder", FolderID);
            }

            UploadResult result = SendRequestFile(url, stream, fileName, "filecontent", args);

            if (result.IsSuccess)
            {
                if (result.Response.StartsWith("ERROR: "))
                {
                    Errors.Add(result.Response.Substring(7));
                }
                else if (result.Response.StartsWith("COMPLETE\n"))
                {
                    RapidShareUploadInfo info = new RapidShareUploadInfo(result.Response);
                    result.URL = info.URL;
                }
            }

            return result;
        }

        private string NextUploadServer()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sub", "nextuploadserver");

            string response = SendRequest(HttpMethod.GET, rapidshareURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                return string.Format(rapidshareUploadURL, response);
            }

            return "";
        }

        public RapidShareFolderInfo GetRootFolderWithChilds()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("sub", "listrealfolders");
            args.Add("login", Username);
            args.Add("password", Password);

            string response = SendRequest(HttpMethod.GET, rapidshareURL, args);

            return RapidShareFolderInfo.GetRootFolderWithChilds(response);
        }
    }

    public class RapidShareUploadInfo
    {
        public string FileID { get; private set; }

        public string FileName { get; private set; }

        public string FileSize { get; private set; }

        public string MD5 { get; private set; }

        public string URL
        {
            get
            {
                if (!string.IsNullOrEmpty(FileID) && !string.IsNullOrEmpty(FileName))
                {
                    return string.Format("https://rapidshare.com/files/{0}/{1}", FileID, FileName);
                }

                return null;
            }
        }

        public RapidShareUploadInfo(string response)
        {
            string[] split = response.Substring(9).Trim('\n').Split(',');

            if (split.Length > 3)
            {
                FileID = split[0];
                FileName = split[1];
                FileSize = split[2];
                MD5 = split[3];
            }
        }
    }

    public class RapidShareFolderInfo
    {
        public string RealFolderID { get; private set; }

        public string ParentRealFolderID { get; private set; }

        public string FolderName { get; private set; }

        public string BrowseACL { get; private set; }

        public string UploadACL { get; private set; }

        public string DownloadACL { get; private set; }

        public List<RapidShareFolderInfo> ChildFolders = new List<RapidShareFolderInfo>();

        public RapidShareFolderInfo(string id, string name)
        {
            RealFolderID = id;
            FolderName = name;
        }

        public RapidShareFolderInfo(string response)
        {
            string[] split = response.Split(',');

            if (split.Length > 5)
            {
                RealFolderID = split[0];
                ParentRealFolderID = split[1];
                FolderName = split[2];
                BrowseACL = split[3];
                UploadACL = split[4];
                DownloadACL = split[5];
            }
        }

        public static List<RapidShareFolderInfo> GetFolderInfos(string response)
        {
            List<RapidShareFolderInfo> list = new List<RapidShareFolderInfo>();

            if (!string.IsNullOrEmpty(response) && response != "NONE")
            {
                string[] split = response.Trim('\n').Split('\n');

                list.AddRange(split.Select(folderInfo => new RapidShareFolderInfo(folderInfo)));
            }

            return list;
        }

        public static RapidShareFolderInfo GetRootFolderWithChilds(string response)
        {
            RapidShareFolderInfo root = new RapidShareFolderInfo("0", "root");

            List<RapidShareFolderInfo> list = GetFolderInfos(response);
            list.Add(root);

            foreach (RapidShareFolderInfo folderInfo in list)
            {
                if (folderInfo.RealFolderID != "0")
                {
                    foreach (RapidShareFolderInfo folderInfo2 in list)
                    {
                        if (folderInfo.ParentRealFolderID == folderInfo2.RealFolderID)
                        {
                            folderInfo2.ChildFolders.Add(folderInfo);
                            folderInfo2.ChildFolders = folderInfo2.ChildFolders.OrderBy(x => x.FolderName).ToList();
                        }
                    }
                }
            }

            root.ChildFolders = root.ChildFolders.OrderBy(x => x.FolderName).ToList();

            return root;
        }
    }
}