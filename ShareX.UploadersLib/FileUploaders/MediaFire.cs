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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class MediaFireFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.MediaFire;

        public override Icon ServiceIcon => Resources.MediaFire;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.MediaFireUsername) && !string.IsNullOrEmpty(config.MediaFirePassword);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new MediaFire(APIKeys.MediaFireAppId, APIKeys.MediaFireApiKey, config.MediaFireUsername, config.MediaFirePassword)
            {
                UploadPath = NameParser.Parse(NameParserType.URL, config.MediaFirePath),
                UseLongLink = config.MediaFireUseLongLink
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpMediaFire;
    }

    public sealed class MediaFire : FileUploader
    {
        public string UploadPath { get; set; }
        public bool UseLongLink { get; set; }

        private static readonly string apiUrl = "https://www.mediafire.com/api/";
        private static readonly int pollInterval = 1000;
        private readonly string appId, apiKey, user, pasw;
        private string sessionToken, signatureTime;
        private int signatureKey;

        public MediaFire(string appId, string apiKey, string user, string pasw)
        {
            this.appId = appId;
            this.apiKey = apiKey;
            this.user = user;
            this.pasw = pasw;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            AllowReportProgress = false;
            GetSessionToken();
            AllowReportProgress = true;
            string key = SimpleUpload(stream, fileName);
            AllowReportProgress = false;
            string url;
            while ((url = PollUpload(key, fileName)) == null)
            {
                Thread.Sleep(pollInterval);
            }
            return new UploadResult() { IsSuccess = true, URL = url };
        }

        private void GetSessionToken()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("email", user);
            args.Add("password", pasw);
            args.Add("application_id", appId);
            args.Add("token_version", "2");
            args.Add("response_format", "json");
            args.Add("signature", GetInitSignature());
            string respStr = SendRequestMultiPart(apiUrl + "user/get_session_token.php", args);
            GetSessionTokenResponse resp = DeserializeResponse<GetSessionTokenResponse>(respStr);
            EnsureSuccess(resp);
            if (resp.session_token == null || resp.time == null || resp.secret_key == null)
                throw new IOException("Invalid response");
            sessionToken = resp.session_token;
            signatureTime = resp.time;
            signatureKey = (int)resp.secret_key;
        }

        private string SimpleUpload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("session_token", sessionToken);
            args.Add("path", UploadPath);
            args.Add("response_format", "json");
            args.Add("signature", GetSignature("upload/simple.php", args));
            string url = URLHelpers.CreateQueryString(apiUrl + "upload/simple.php", args);
            UploadResult res = SendRequestFile(url, stream, fileName, "Filedata");
            if (!res.IsSuccess) throw new IOException(res.ErrorsToString());
            SimpleUploadResponse resp = DeserializeResponse<SimpleUploadResponse>(res.Response);
            EnsureSuccess(resp);
            if (resp.doupload.result != 0 || resp.doupload.key == null) throw new IOException("Invalid response");
            return resp.doupload.key;
        }

        private string PollUpload(string uploadKey, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("session_token", sessionToken);
            args.Add("key", uploadKey);
            args.Add("filename", fileName);
            args.Add("response_format", "json");
            args.Add("signature", GetSignature("upload/poll_upload.php", args));
            string respStr = SendRequestMultiPart(apiUrl + "upload/poll_upload.php", args);
            PollUploadResponse resp = DeserializeResponse<PollUploadResponse>(respStr);
            EnsureSuccess(resp);
            if (resp.doupload.result == null || resp.doupload.status == null) throw new IOException("Invalid response");
            if (resp.doupload.result != 0 || resp.doupload.fileerror != null)
            {
                throw new IOException(string.Format("Couldn't upload the file: {0}", resp.doupload.description ?? "Unknown error"));
            }
            if (resp.doupload.status == 99)
            {
                if (resp.doupload.quickkey == null) throw new IOException("Invalid response");

                string url = URLHelpers.CombineURL("http://www.mediafire.com/view", resp.doupload.quickkey);
                if (UseLongLink) url = URLHelpers.CombineURL(url, URLHelpers.URLEncode(resp.doupload.filename));
                return url;
            }
            return null;
        }

        private void EnsureSuccess(MFResponse resp)
        {
            if (resp.result != "Success")
                throw new IOException(string.Format("Couldn't upload the file: {0}", resp.message ?? "Unknown error"));
            if (resp.new_key == "yes") NextSignatureKey();
        }

        private string GetInitSignature()
        {
            string signatureStr = user + pasw + appId + apiKey;
            byte[] signatureBytes = Encoding.ASCII.GetBytes(signatureStr);
            SHA1 sha1Gen = SHA1.Create();
            byte[] sha1Bytes = sha1Gen.ComputeHash(signatureBytes);
            return BytesToString(sha1Bytes);
        }

        private string GetSignature(string urlSuffix, Dictionary<string, string> args)
        {
            string keyStr = (signatureKey % 256).ToString(CultureInfo.InvariantCulture);
            string urlStr = CreateNonEscapedQuery("/api/" + urlSuffix, args);
            string signatureStr = keyStr + signatureTime + urlStr;
            byte[] signatureBytes = Encoding.ASCII.GetBytes(signatureStr);
            MD5 md5gen = MD5.Create();
            byte[] md5Bytes = md5gen.ComputeHash(signatureBytes);
            return BytesToString(md5Bytes);
        }

        private void NextSignatureKey()
        {
            signatureKey = (int)(((long)signatureKey * 16807) % 2147483647);
        }

        private T DeserializeResponse<T>(string s) where T : new()
        {
            var refObj = new { response = new T() };
            object obj = JsonConvert.DeserializeObject(s, refObj.GetType());
            return (T)obj.GetType().GetProperty("response").GetValue(obj, null);
        }

        private static char IntToChar(int x)
        {
            if (x < 10) return (char)(x + '0');
            return (char)(x - 10 + 'a');
        }

        private static string BytesToString(byte[] b)
        {
            char[] res = new char[b.Length * 2];
            for (int i = 0; i < b.Length; ++i)
            {
                res[2 * i] = IntToChar(b[i] >> 4);
                res[(2 * i) + 1] = IntToChar(b[i] & 0xf);
            }
            return new string(res);
        }

        private static string CreateNonEscapedQuery(string url, Dictionary<string, string> args)
        {
            if (args != null && args.Count > 0)
                return url + "?" + string.Join("&", args.Select(x => x.Key + "=" + x.Value).ToArray());
            return url;
        }

        private class MFResponse
        {
            public string result { get; set; }
            public int? error { get; set; }
            public string message { get; set; }
            public string new_key { get; set; }
        }

        private class GetSessionTokenResponse : MFResponse
        {
            public string session_token { get; set; }
            public int? secret_key { get; set; }
            public string time { get; set; }
        }

        private class SimpleUploadResponse : MFResponse
        {
            public DoUpload doupload { get; set; }

            public class DoUpload
            {
                public int? result { get; set; }
                public string key { get; set; }
            }
        }

        private class PollUploadResponse : MFResponse
        {
            public DoUpload doupload { get; set; }

            public class DoUpload
            {
                public int? result { get; set; }
                public int? status { get; set; }
                public string description { get; set; }
                public int? fileerror { get; set; }
                public string quickkey { get; set; }
                public string filename { get; set; }
            }
        }
    }
}