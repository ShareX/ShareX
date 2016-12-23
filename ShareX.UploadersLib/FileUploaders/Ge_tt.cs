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
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Ge_ttFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Ge_tt;

        public override Icon ServiceIcon => Resources.Gett;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.Ge_ttLogin != null && !string.IsNullOrEmpty(config.Ge_ttLogin.AccessToken);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Ge_tt(APIKeys.Ge_ttKey)
            {
                AccessToken = config.Ge_ttLogin.AccessToken
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGe_tt;
    }

    public sealed class Ge_tt : FileUploader
    {
        public string APIKey { get; private set; }
        public string AccessToken { get; set; }

        public Ge_tt(string apiKey)
        {
            APIKey = apiKey;
        }

        public Ge_ttLogin Login(string email, string password)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("apikey", APIKey);
            args.Add("email", email);
            args.Add("password", password);

            string json = JsonConvert.SerializeObject(args);

            string response = SendRequest(HttpMethod.POST, "https://open.ge.tt/1/users/login", json, ContentTypeJSON);

            return JsonConvert.DeserializeObject<Ge_ttLogin>(response);
        }

        public Ge_ttShare CreateShare(string accessToken)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("accesstoken", accessToken);

            string url = CreateQuery("https://open.ge.tt/1/shares/create", args);
            string response = SendRequest(HttpMethod.POST, url);

            return JsonConvert.DeserializeObject<Ge_ttShare>(response);
        }

        public Ge_ttFile CreateFile(string accessToken, string shareName, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("accesstoken", accessToken);

            Dictionary<string, string> args2 = new Dictionary<string, string>();
            args2.Add("filename", fileName);

            string json = JsonConvert.SerializeObject(args2);

            string response = SendRequest(HttpMethod.POST, $"https://open.ge.tt/1/files/{shareName}/create", json, ContentTypeJSON, args);

            return JsonConvert.DeserializeObject<Ge_ttFile>(response);
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = null;

            Ge_ttShare share = CreateShare(AccessToken);

            if (share != null)
            {
                Ge_ttFile file = CreateFile(AccessToken, share.ShareName, fileName);

                if (file != null)
                {
                    result = UploadData(stream, file.Upload.PostURL, fileName);

                    if (result.IsSuccess)
                    {
                        result.URL = file.GettURL;
                    }
                }
            }

            return result;
        }
    }

    public class Ge_ttLogin
    {
        public string Expires { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class Ge_ttShare
    {
        public string Created { get; set; }
        public string UserID { get; set; }
        public string ShareName { get; set; }
        public string ReadyState { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string GettURL { get; set; }
    }

    public class Ge_ttFile
    {
        public string GettURL { get; set; }
        public Ge_ttUpload Upload { get; set; }
    }

    public class Ge_ttUpload
    {
        public string PostURL { get; set; }
        public string PutURL { get; set; }
    }
}