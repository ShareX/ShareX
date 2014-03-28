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

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace UploadersLib.FileUploaders
{
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

            string argsJSON = JsonConvert.SerializeObject(args);

            string response = SendPostRequestJSON("https://open.ge.tt/1/users/login", argsJSON);

            return JsonConvert.DeserializeObject<Ge_ttLogin>(response);
        }

        public Ge_ttShare CreateShare(string accessToken)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("accesstoken", accessToken);

            string url = CreateQuery("https://open.ge.tt/1/shares/create", args);
            string response = SendPostRequest(url);

            return JsonConvert.DeserializeObject<Ge_ttShare>(response);
        }

        public Ge_ttFile CreateFile(string accessToken, string shareName, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("accesstoken", accessToken);

            string url = CreateQuery(string.Format("https://open.ge.tt/1/files/{0}/create", shareName), args);

            Dictionary<string, string> args2 = new Dictionary<string, string>();
            args2.Add("filename", fileName);

            string argsJSON = JsonConvert.SerializeObject(args2);

            string response = SendPostRequestJSON(url, argsJSON);

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