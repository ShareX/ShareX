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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Linq;

namespace UploadersLib.FileUploaders
{
    public sealed class Box : FileUploader
    {
        private string APIKey;
        private const string APIURL = "https://www.box.net/api/1.0/rest";
        private const string AuthURL = "https://www.box.net/api/1.0/auth/{0}";
        private const string UploadURL = "https://upload.box.net/api/1.0/upload/{0}/{1}";
        private const string ShareURL = "http://www.box.com/s/{0}";

        public string Ticket { get; set; }

        public string AuthToken { get; set; }

        public string FolderID { get; set; }

        public bool Share { get; set; }

        public Box(string apiKey)
        {
            APIKey = apiKey;
            FolderID = "0";
            Share = true;
        }

        public string GetTicket()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("action", "get_ticket");
            args.Add("api_key", APIKey);

            string response = SendGetRequest(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                XDocument xd = XDocument.Parse(response);
                XElement xe = xd.GetElement("response");

                if (xe != null && xe.GetElementValue("status") == "get_ticket_ok")
                {
                    Ticket = xe.GetElementValue("ticket");
                    return Ticket;
                }
            }

            return null;
        }

        public string GetAuthorizationURL()
        {
            string ticket = GetTicket();

            if (!string.IsNullOrEmpty(ticket))
            {
                return string.Format(AuthURL, ticket);
            }

            return null;
        }

        public string GetAuthToken(string ticket)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("action", "get_auth_token");
            args.Add("api_key", APIKey);
            args.Add("ticket", ticket);

            string response = SendGetRequest(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                XDocument xd = XDocument.Parse(response);
                XElement xe = xd.GetElement("response");

                if (xe != null && xe.GetElementValue("status") == "get_auth_token_ok")
                {
                    AuthToken = xe.GetElementValue("auth_token");
                    return AuthToken;
                }
            }

            return null;
        }

        public string GetAuthToken()
        {
            return GetAuthToken(Ticket);
        }

        public BoxFolder GetAccountTree(string folderID = "0", bool onelevel = false, bool nofiles = false, bool nozip = true, bool simple = false)
        {
            NameValueCollection args = new NameValueCollection();
            args.Add("action", "get_account_tree");
            args.Add("api_key", APIKey);
            args.Add("auth_token", AuthToken);
            args.Add("folder_id", folderID);

            if (onelevel) // Make a tree of one level depth, so you will get only the files and folders stored in the folder of the folder_id you have provided.
            {
                args.Add("params", "onelevel");
            }

            if (nofiles) // Only include the folders in the user account tree, and ignore the files.
            {
                args.Add("params", "nofiles");
            }

            if (nozip) // Do not zip the tree xml.
            {
                args.Add("params", "nozip");
            }

            if (simple) // Display the full tree with a limited list of attributes to make for smaller, more efficient output (folders only contain the 'name' and 'id' attributes, and files will contain the 'name', 'id', 'created', and 'size' attributes)
            {
                args.Add("params", "simple");
            }

            string url = CreateQuery(APIURL, args);

            string response = SendGetRequest(url);

            if (!string.IsNullOrEmpty(response))
            {
                XDocument xd = XDocument.Parse(response);
                XElement xe = xd.GetElement("response");

                if (xe != null && xe.GetElementValue("status") == "listing_ok")
                {
                    XElement xeTree = xe.Element("tree");

                    if (xeTree != null)
                    {
                        return ParseFolder(xeTree.Element("folder"));
                    }
                }
            }

            return null;
        }

        private BoxFolder ParseFolder(XElement xe)
        {
            if (xe != null && xe.Name == "folder")
            {
                BoxFolder folder = new BoxFolder();
                folder.ID = xe.GetAttributeValue("id");
                folder.Name = xe.GetAttributeValue("name");

                XElement xeFolders = xe.Element("folders");

                if (xeFolders != null)
                {
                    foreach (XElement xeFolder in xeFolders.Elements())
                    {
                        BoxFolder childFolder = ParseFolder(xeFolder);

                        if (childFolder != null)
                        {
                            folder.Folders.Add(childFolder);
                        }
                    }
                }

                return folder;
            }

            return null;
        }

        public BoxFolder GetFolderList()
        {
            return GetAccountTree("0", false, true, true, true);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(AuthToken))
            {
                Errors.Add("Login is required.");
                return null;
            }

            Dictionary<string, string> args = new Dictionary<string, string>();
            if (Share) args.Add("share", "1");

            string url = string.Format(UploadURL, AuthToken, FolderID);
            UploadResult result = UploadData(stream, url, fileName, "file", args);

            if (result.IsSuccess)
            {
                XDocument xd = XDocument.Parse(result.Response);
                XElement xe = xd.GetElement("response");

                if (xe != null && xe.GetElementValue("status") == "upload_ok")
                {
                    XElement xeFile = xe.GetElement("files", "file");

                    if (xeFile != null)
                    {
                        string publicName = xeFile.GetAttributeValue("public_name");

                        if (!string.IsNullOrEmpty(publicName))
                        {
                            result.URL = string.Format(ShareURL, publicName);
                        }
                    }
                }
            }

            return result;
        }
    }

    public class BoxFolder
    {
        public string ID;
        public string Name;
        public string User_id;
        public string Description;
        public string Shared;
        public string Shared_link;
        public string Permissions;

        //public List<BoxTag> Tags;
        //public List<BoxFile> Files;
        public List<BoxFolder> Folders = new List<BoxFolder>();
    }
}