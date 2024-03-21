#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class NextCloudFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.NextCloud;

        public override Image ServiceImage => Resources.NextCloud;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.NextCloudHost) && !string.IsNullOrEmpty(config.NextCloudUsername) && !string.IsNullOrEmpty(config.NextCloudPassword);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new NextCloud(config.NextCloudHost, config.NextCloudUsername, config.NextCloudPassword)
            {
                Path = config.NextCloudPath,
                CreateShare = config.NextCloudCreateShare,
                DirectLink = config.NextCloudDirectLink,
                PreviewLink = config.NextCloudUsePreviewLinks,
                AppendFileNameToURL = config.NextCloudAppendFileNameToURL,
                IsCompatibility81 = config.NextCloud81Compatibility,
                AutoExpireTime = config.NextCloudExpiryTime,
                AutoExpire = config.NextCloudAutoExpire
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpNextCloud;
    }

    public sealed class NextCloud : FileUploader
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public int AutoExpireTime { get; set; }
        public bool CreateShare { get; set; }
        public bool AppendFileNameToURL { get; set; }
        public bool DirectLink { get; set; }
        public bool PreviewLink { get; set; }
        public bool IsCompatibility81 { get; set; }
        public bool AutoExpire { get; set; }

        public NextCloud(string host, string username, string password)
        {
            Host = host;
            Username = username;
            Password = password;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (string.IsNullOrEmpty(Host))
            {
                throw new Exception("nextCloud Host is empty.");
            }

            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                throw new Exception("nextCloud Username or Password is empty.");
            }

            if (string.IsNullOrEmpty(Path))
            {
                Path = "/";
            }

            // Original, unencoded path. Necessary for shared files
            string path = URLHelpers.CombineURL(Path, fileName);
            // Encoded path, necessary when sent in the URL
            string encodedPath = URLHelpers.CombineURL(Path, URLHelpers.URLEncode(fileName));

            string url = URLHelpers.CombineURL(Host, "remote.php/dav", encodedPath);
            url = URLHelpers.FixPrefix(url);

            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(Username, Password);
            headers["OCS-APIREQUEST"] = "true";

            string response = SendRequest(HttpMethod.PUT, url, stream, MimeTypes.GetMimeTypeFromFileName(fileName), null, headers);

            UploadResult result = new UploadResult(response);

            if (!IsError)
            {
                if (CreateShare)
                {
                    AllowReportProgress = false;
                    result.URL = ShareFile(path, fileName);
                }
                else
                {
                    result.IsURLExpected = false;
                }
            }

            return result;
        }

        public string ShareFile(string path, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("path", path); // path to the file/folder which should be shared
            args.Add("shareType", "3"); // ‘0’ = user; ‘1’ = group; ‘3’ = public link
            // args.Add("shareWith", ""); // user / group id with which the file should be shared
            // args.Add("publicUpload", "false"); // allow public upload to a public shared folder (true/false)
            // args.Add("password", ""); // password to protect public link Share with
            args.Add("permissions", "1"); // 1 = read; 2 = update; 4 = create; 8 = delete; 16 = share; 31 = all (default: 31, for public shares: 1)

            if (AutoExpire)
            {
                if (AutoExpireTime == 0)
                {
                    throw new Exception("nextCloud Auto Epxire Time is not valid.");
                }
                else
                {
                    try
                    {
                        DateTime expireTime = DateTime.UtcNow.AddDays(AutoExpireTime);
                        args.Add("expireDate", $"{expireTime.Year}-{expireTime.Month}-{expireTime.Day}");
                    }
                    catch
                    {
                        throw new Exception("nextCloud Auto Expire time is invalid");
                    }
                }
            }

            string url = URLHelpers.CombineURL(Host, "ocs/v1.php/apps/files_sharing/api/v1/shares?format=json");
            url = URLHelpers.FixPrefix(url);

            NameValueCollection headers = RequestHelpers.CreateAuthenticationHeader(Username, Password);
            headers["OCS-APIREQUEST"] = "true";

            string response = SendRequestMultiPart(url, args, headers);

            if (!string.IsNullOrEmpty(response))
            {
                NextCloudShareResponse result = JsonConvert.DeserializeObject<NextCloudShareResponse>(response);

                if (result != null && result.ocs != null && result.ocs.meta != null)
                {
                    if (result.ocs.data != null && result.ocs.meta.statuscode == 100)
                    {
                        NextCloudShareResponseData data = ((JObject)result.ocs.data).ToObject<NextCloudShareResponseData>();
                        string link = data.url;

                        if (PreviewLink && FileHelpers.IsImageFile(path))
                        {
                            link += "/preview";
                        }
                        else if (DirectLink)
                        {
                            if (IsCompatibility81)
                            {
                                link += "/download";
                            }
                            else
                            {
                                link += "&download";
                            }

                            if (AppendFileNameToURL)
                            {
                                link = URLHelpers.CombineURL(link, URLHelpers.URLEncode(fileName));
                            }
                        }

                        return link;
                    }
                    else
                    {
                        Errors.Add(string.Format("Status: {0}\r\nStatus code: {1}\r\nMessage: {2}", result.ocs.meta.status, result.ocs.meta.statuscode, result.ocs.meta.message));
                    }
                }
            }

            return null;
        }

        public class NextCloudShareResponse
        {
            public NextCloudShareResponseOcs ocs { get; set; }
        }

        public class NextCloudShareResponseOcs
        {
            public NextCloudShareResponseMeta meta { get; set; }
            public object data { get; set; }
        }

        public class NextCloudShareResponseMeta
        {
            public string status { get; set; }
            public int statuscode { get; set; }
            public string message { get; set; }
        }

        public class NextCloudShareResponseData
        {
            public int id { get; set; }
            public string url { get; set; }
            public string token { get; set; }
        }
    }
}
