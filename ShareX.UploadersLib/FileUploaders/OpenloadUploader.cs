#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class OpenloadUploader : FileUploader
    {

        public string APILogin { get; set; }
        public string APIKey { get; set; }
        public bool UploadToFolder { get; set; }
        public string FolderID { get; set; }

        public bool IsStatusOK(int statusCode)
        {
            return (statusCode / 100 == 2);
        }

        public string GetUploadURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(APILogin) && !string.IsNullOrEmpty(APIKey))
            {
                args.Add("login", APILogin);
                args.Add("key", APIKey);
                if (UploadToFolder && !string.IsNullOrEmpty(FolderID))
                    args.Add("folder", FolderID);
            }

            string response = SendRequest(HttpMethod.POST, "https://api.openload.co/1/file/ul", args);
            if (!string.IsNullOrEmpty(response))
            {
                var uploadResponse = JsonConvert.DeserializeObject<OpenloadResponse<OpenloadUploadResult>>(response);
                if (IsStatusOK(uploadResponse.status))
                    return uploadResponse.result.url;
                Errors.Add(string.Format("Can't retrieve the upload URL: {0}", uploadResponse.message));
                return null;
            }
            Errors.Add("Can't retrieve the upload URL");
            return null;
        }

        public OpenloadFolderNode GetFolderTree(string folderID)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(APILogin) || string.IsNullOrEmpty(APIKey))
                return null;

            args.Add("login", APILogin);
            args.Add("key", APIKey);
            if (!string.IsNullOrEmpty(folderID))
                args.Add("folder", folderID);

            string response = SendRequest(HttpMethod.POST, "https://api.openload.co/1/file/listfolder", args);
            if (string.IsNullOrEmpty(response))
                return null;
            var folderResponse = JsonConvert.DeserializeObject<OpenloadResponse<OpenloadFolderResult>>(response);
            if (!IsStatusOK(folderResponse.status))
                return null;

            OpenloadFolderNode folderTree = new OpenloadFolderNode();
            foreach (OpenloadFolder folder in folderResponse.result.folders)
            {
                OpenloadFolderNode folderNode = GetFolderTree(folder.id);
                if (folderNode == null)
                    folderNode = new OpenloadFolderNode();
                folderNode.folder = folder;
                folderTree.subNodes.Add(folderNode);
            }
            return folderTree;
        }

        public UploadResult UploadToURL(Stream stream, string fileName, string uploadURL)
        {
            UploadResult result = UploadData(stream, uploadURL, fileName, "image");
            if (!string.IsNullOrEmpty(result.Response))
            {
                var uploadResponse = JsonConvert.DeserializeObject<OpenloadResponse<OpenloadImageUploadResult>>(result.Response);
                if (IsStatusOK(uploadResponse.status))
                {
                    result.URL = uploadResponse.result.url;
                    return result;
                }
            }
            Errors.Add("Upload to openload.co failed");
            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string uploadURL = GetUploadURL();
            return (string.IsNullOrEmpty(uploadURL)) ? null : UploadToURL(stream, fileName, uploadURL);
        }

    }

    public class OpenloadFolderNode
    {
        public OpenloadFolderNode()
        {
            subNodes = new List<OpenloadFolderNode>();
        }

        public OpenloadFolder folder { get; set; }
        public List<OpenloadFolderNode> subNodes { get; set; }
    }

    public class OpenloadResponse<ResultType>
    {
        public int status { get; set; }
        public string message { get; set; }
        public ResultType result { get; set; }
    }

    public class OpenloadUploadResult
    {
        public string url { get; set; }
        public string valid_until { get; set; }
    }

    public class OpenloadImageUploadResult
    {
        public string name { get; set; }
        public string size { get; set; }
        public string sha1 { get; set; }
        public string content_type { get; set; }
        public string id { get; set; }
        public string url { get; set; }
    }

    public class OpenloadFolderResult
    {
        public OpenloadFolder[] folders { get; set; }
    }

    public class OpenloadFolder
    {
        public string id { get; set; }
        public string name { get; set; }
    }

}
