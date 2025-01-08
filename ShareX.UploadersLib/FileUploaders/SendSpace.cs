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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public class SendSpaceFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.SendSpace;

        public override Icon ServiceIcon => Resources.SendSpace;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.SendSpaceAccountType == AccountType.Anonymous ||
                (!string.IsNullOrEmpty(config.SendSpaceUsername) && !string.IsNullOrEmpty(config.SendSpacePassword));
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new SendSpace(APIKeys.SendSpaceKey)
            {
                AccountType = config.SendSpaceAccountType,
                Username = config.SendSpaceUsername,
                Password = config.SendSpacePassword
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpSendSpace;
    }

    public sealed class SendSpace : FileUploader
    {
        private string APIKey;

        private const string APIURL = "http://api.sendspace.com/rest/";
        private const string APIVersion = "1.0";

        public AccountType AccountType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Upload speed limit in kilobytes, 0 for unlimited
        /// </summary>
        public int SpeedLimit = 0;

        public string AppVersion = "1.0";

        public SendSpace(string apiKey)
        {
            APIKey = apiKey;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (AccountType == AccountType.User)
            {
                SendSpaceManager.PrepareUploadInfo(APIKey, Username, Password);
            }
            else
            {
                SendSpaceManager.PrepareUploadInfo(APIKey);
            }

            return Upload(stream, fileName, SendSpaceManager.UploadInfo);
        }

        #region Helpers

        public class ResponsePacket
        {
            public string Method { get; set; }

            public string Status { get; set; }

            public bool Error { get; set; }

            public string ErrorCode { get; set; }

            public string ErrorText { get; set; }

            public XElement Result { get; set; }
        }

        private ResponsePacket ParseResponse(string response)
        {
            ResponsePacket packet = new ResponsePacket();

            XDocument xml = XDocument.Parse(response);
            packet.Result = xml.Element("result");
            packet.Method = packet.Result.Attribute("method").Value;
            packet.Status = packet.Result.Attribute("status").Value;
            packet.Error = packet.Status == "fail";

            if (packet.Error)
            {
                XElement error = packet.Result.Element("error");
                packet.ErrorCode = error.Attribute("code").Value;
                packet.ErrorText = error.Attribute("text").Value;

                Errors.Add(string.Format("Code: {0}, Method: {1}\r\nText: {2}", packet.ErrorCode, packet.Method, packet.ErrorText));
            }

            return packet;
        }

        private UploadResponsePacket ParseUploadResponse(string response)
        {
            XDocument xml = XDocument.Parse(response);
            XElement xe = xml.Root;
            if (xe.GetElementValue("status") == "ok")
            {
                UploadResponsePacket urp = new UploadResponsePacket
                {
                    DownloadURL = xe.GetElementValue("download_url"),
                    DeleteURL = xe.GetElementValue("delete_url")
                };

                return urp;
            }

            return null;
        }

        public class UploadResponsePacket
        {
            public string DownloadURL { get; set; }

            public string DeleteURL { get; set; }
        }

        public class LoginInfo
        {
            /// <summary>
            /// Session key to be sent with all method calls, user information, including the user account's capabilities
            /// </summary>
            public string SessionKey { get; set; }

            public string Username { get; set; }

            public string EMail { get; set; }

            public string MembershipType { get; set; }

            public string MembershipEnds { get; set; }

            public bool CapableUpload { get; set; }

            public bool CapableDownload { get; set; }

            public bool CapableFolders { get; set; }

            public bool CapableFiles { get; set; }

            public bool CapableHTTPS { get; set; }

            public bool CapableAddressBook { get; set; }

            public string BandwidthLeft { get; set; }

            public string DiskSpaceLeft { get; set; }

            public string DiskSpaceUsed { get; set; }

            public string Points { get; set; }

            public LoginInfo()
            {
            }

            public LoginInfo(XElement element)
            {
                SessionKey = element.GetElementValue("session_key");
                Username = element.GetElementValue("user_name");
                EMail = element.GetElementValue("email");
                MembershipType = element.GetElementValue("membership_type");
                MembershipEnds = element.GetElementValue("membership_ends");
                CapableUpload = element.GetElementValue("capable_upload") != "0";
                CapableDownload = element.GetElementValue("capable_download") != "0";
                CapableFolders = element.GetElementValue("capable_folders") != "0";
                CapableFiles = element.GetElementValue("capable_files") != "0";
                CapableHTTPS = element.GetElementValue("capable_https") != "0";
                CapableAddressBook = element.GetElementValue("capable_addressbook") != "0";
                BandwidthLeft = element.GetElementValue("bandwidth_left");
                DiskSpaceLeft = element.GetElementValue("diskspace_left");
                DiskSpaceUsed = element.GetElementValue("diskspace_used");
                Points = element.GetElementValue("points");
            }
        }

        public class UploadInfo
        {
            public string URL { get; set; }

            public string ProgressURL { get; set; }

            public string MaxFileSize { get; set; }

            public string UploadIdentifier { get; set; }

            public string ExtraInfo { get; set; }

            public UploadInfo()
            {
            }

            public UploadInfo(XElement element)
            {
                XElement upload = element.Element("upload");
                URL = upload.GetAttributeValue("url");
                ProgressURL = upload.GetAttributeValue("progress_url");
                MaxFileSize = upload.GetAttributeValue("max_file_size");
                UploadIdentifier = upload.GetAttributeValue("upload_identifier");
                ExtraInfo = upload.GetAttributeValue("extra_info");
            }
        }

        #endregion Helpers

        #region Authentication

        /// <summary>
        /// Creates a new user account. An activation/validation email will be sent automatically to the user.
        /// http://www.sendspace.com/dev_method.html?method=auth.register
        /// </summary>
        /// <param name="username">a-z/A-Z/0-9, 3-20 chars</param>
        /// <param name="fullname">a-z/A-Z/space, 3-20 chars</param>
        /// <param name="email">Valid email address required</param>
        /// <param name="password">Can be left empty and the API will create a unique password or enter one with 4-20 chars</param>
        /// <returns>true = success, false = error</returns>
        public bool AuthRegister(string username, string fullname, string email, string password)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "auth.register");
            args.Add("api_key", APIKey);
            args.Add("user_name", username);
            args.Add("full_name", fullname);
            args.Add("email", email);
            args.Add("password", password);

            string response = SendRequestMultiPart(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                return !ParseResponse(response).Error;
            }

            return false;
        }

        /// <summary>
        /// Obtains a new and random token per session. Required for login.
        /// http://www.sendspace.com/dev_method.html?method=auth.createToken
        /// </summary>
        /// <returns>A token to be used with the auth.login method</returns>
        public string AuthCreateToken()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "auth.createToken");
            args.Add("api_key", APIKey); // Received from sendspace
            args.Add("api_version", APIVersion); // Value must be: 1.0
            args.Add("app_version", AppVersion); // Application specific, formatting / style is up to you
            args.Add("response_format", "xml"); // Value must be: XML

            string response = SendRequestMultiPart(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                ResponsePacket packet = ParseResponse(response);

                if (!packet.Error)
                {
                    return packet.Result.GetElementValue("token");
                }
            }

            return null;
        }

        /// <summary>
        /// Starts a session and returns user API method capabilities -- which features the given user can and cannot use.
        /// http://www.sendspace.com/dev_method.html?method=auth.login
        /// </summary>
        /// <param name="token">Received on create token</param>
        /// <param name="username">Registered user name</param>
        /// <param name="password">Registered password</param>
        /// <returns>Account informations including session key</returns>
        public LoginInfo AuthLogin(string token, string username, string password)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "auth.login");
            args.Add("token", token);
            args.Add("user_name", username);
            // lowercase(md5(token+lowercase(md5(password)))) - md5 values should always be lowercase.
            string passwordHash = TranslatorHelper.TextToHash(password, HashType.MD5);
            args.Add("tokened_password", TranslatorHelper.TextToHash(token + passwordHash, HashType.MD5));

            string response = SendRequestMultiPart(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                ResponsePacket packet = ParseResponse(response);

                if (!packet.Error)
                {
                    LoginInfo loginInfo = new LoginInfo(packet.Result);
                    return loginInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if a session is valid or not.
        /// http://www.sendspace.com/dev_method.html?method=auth.checksession
        /// </summary>
        /// <param name="sessionKey">Received from auth.login</param>
        /// <returns>true = success, false = error</returns>
        public bool AuthCheckSession(string sessionKey)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "auth.checkSession");
            args.Add("session_key", sessionKey);

            string response = SendRequestMultiPart(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                ResponsePacket packet = ParseResponse(response);

                if (!packet.Error)
                {
                    string session = packet.Result.GetElementValue("session");

                    if (!string.IsNullOrEmpty(session))
                    {
                        return session == "ok";
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Logs out from a session.
        /// http://www.sendspace.com/dev_method.html?method=auth.logout
        /// </summary>
        /// <param name="sessionKey">Received from auth.login</param>
        /// <returns>true = success, false = error</returns>
        public bool AuthLogout(string sessionKey)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "auth.logout");
            args.Add("session_key", sessionKey);

            string response = SendRequestMultiPart(APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                return !ParseResponse(response).Error;
            }

            return false;
        }

        #endregion Authentication

        #region Upload

        /// <summary>
        /// Obtains the information needed to perform an upload.
        /// http://www.sendspace.com/dev_method.html?method=upload.getInfo
        /// </summary>
        /// <param name="sessionKey">Received from auth.login</param>
        /// <returns>URL to upload the file to, progress_url for real-time progress information, max_file_size for max size current user can upload, upload_identifier & extra_info to be passed with the upload form</returns>
        public UploadInfo UploadGetInfo(string sessionKey)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "upload.getInfo");
            args.Add("session_key", sessionKey);
            args.Add("speed_limit", SpeedLimit.ToString());

            string response = SendRequest(HttpMethod.GET, APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                ResponsePacket packet = ParseResponse(response);

                if (!packet.Error)
                {
                    UploadInfo uploadInfo = new UploadInfo(packet.Result);
                    return uploadInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtains the basic information needed to make an anonymous upload. This method does not require authentication or login.
        /// </summary>
        /// <returns>URL to upload the file to, progress_url for real-time progress information, max_file_size for max size current user can upload, upload_identifier & extra_info to be passed in the upload form</returns>
        public UploadInfo AnonymousUploadGetInfo()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("method", "anonymous.uploadGetInfo");
            args.Add("speed_limit", SpeedLimit.ToString());
            args.Add("api_key", APIKey);
            args.Add("api_version", APIVersion);
            args.Add("app_version", AppVersion);

            string response = SendRequest(HttpMethod.GET, APIURL, args);

            if (!string.IsNullOrEmpty(response))
            {
                ResponsePacket packet = ParseResponse(response);

                if (!packet.Error)
                {
                    UploadInfo uploadInfo = new UploadInfo(packet.Result);
                    return uploadInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// http://www.sendspace.com/dev_method.html?method=upload.getInfo
        /// </summary>
        /// <param name="max_file_size">max_file_size value received in UploadGetInfo response</param>
        /// <param name="upload_identifier">upload_identifier value received in UploadGetInfo response</param>
        /// <param name="extra_info">extra_info value received in UploadGetInfo response</param>
        /// <param name="description"></param>
        /// <param name="password"></param>
        /// <param name="folder_id"></param>
        /// <param name="recipient_email">an email (or emails separated with ,) of recipient/s to receive information about the upload</param>
        /// <param name="notify_uploader">0/1 - should the uploader be notified?</param>
        /// <param name="redirect_url">page to redirect after upload will be attached upload_status=ok/fail&file_id=XXXX</param>
        /// <returns></returns>
        public Dictionary<string, string> PrepareArguments(string max_file_size, string upload_identifier, string extra_info,
            string description, string password, string folder_id, string recipient_email, string notify_uploader, string redirect_url)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("MAX_FILE_SIZE", max_file_size);
            args.Add("UPLOAD_IDENTIFIER", upload_identifier);
            args.Add("extra_info", extra_info);

            // Optional fields

            if (!string.IsNullOrEmpty(description)) args.Add("description", description);
            if (!string.IsNullOrEmpty(password)) args.Add("password", password);
            if (!string.IsNullOrEmpty(folder_id)) args.Add("folder_id", folder_id);
            if (!string.IsNullOrEmpty(recipient_email)) args.Add("recipient_email", recipient_email);
            if (!string.IsNullOrEmpty(notify_uploader)) args.Add("notify_uploader", notify_uploader);
            if (!string.IsNullOrEmpty(redirect_url)) args.Add("redirect_url", redirect_url);

            return args;
        }

        public Dictionary<string, string> PrepareArguments(string max_file_size, string upload_identifier, string extra_info)
        {
            return PrepareArguments(max_file_size, upload_identifier, extra_info, null, null, null, null, null, null);
        }

        public UploadResult Upload(Stream stream, string fileName, UploadInfo uploadInfo)
        {
            UploadResult result = null;

            if (uploadInfo != null)
            {
                Dictionary<string, string> args = PrepareArguments(uploadInfo.MaxFileSize, uploadInfo.UploadIdentifier, uploadInfo.ExtraInfo);

                result = SendRequestFile(uploadInfo.URL, stream, fileName, "userfile", args);

                if (result.IsSuccess)
                {
                    if (result.Response.StartsWith("upload_status=ok")) // User
                    {
                        string fileid = Regex.Match(result.Response, @"file_id=(\w+)").Groups[1].Value;
                        result.URL = "http://www.sendspace.com/file/" + fileid;
                    }
                    else // Anonymous
                    {
                        UploadResponsePacket urp = ParseUploadResponse(result.Response);
                        result.URL = urp.DownloadURL;
                        result.DeletionURL = urp.DeleteURL;
                    }
                }
            }

            return result;
        }

        public class CheckProgress : IDisposable
        {
            private SendSpace sendSpace;
            private string url;
            private int interval = 1000;
            private CancellationTokenSource cts;

            public CheckProgress(string progressURL, SendSpace sendSpace)
            {
                url = progressURL;
                this.sendSpace = sendSpace;

                cts = new CancellationTokenSource();
                Task.Run(() => DoWork(cts.Token), cts.Token);
            }

            private void DoWork(CancellationToken ct)
            {
                Thread.Sleep(1000);
                ProgressInfo progressInfo = new ProgressInfo();
                DateTime time;
                while (!ct.IsCancellationRequested)
                {
                    time = DateTime.Now;
                    try
                    {
                        string response = sendSpace.SendRequest(HttpMethod.POST, url);

                        progressInfo.ParseResponse(response);

                        if (progressInfo.Status != "fail" && !string.IsNullOrEmpty(progressInfo.Meter))
                        {
                            if (int.TryParse(progressInfo.Meter, out int progress))
                            {
                                //sendSpace.OnProgressChanged(0, 0);
                            }
                        }
                    }
                    catch
                    {
                    }
                    int elapsed = (int)(DateTime.Now - time).TotalMilliseconds;
                    if (elapsed < interval)
                    {
                        Thread.Sleep(interval - elapsed);
                    }
                }
            }

            private class ProgressInfo
            {
                public string Status { get; set; }
                public string ETA { get; set; }
                public string Speed { get; set; }
                public string UploadedBytes { get; set; }
                public string TotalSize { get; set; }
                public string Elapsed { get; set; }
                public string Meter { get; set; }

                public ProgressInfo()
                {
                }

                public ProgressInfo(string response)
                {
                    ParseResponse(response);
                }

                public void ParseResponse(string response)
                {
                    XDocument xml = XDocument.Parse(response);
                    XElement element = xml.Element("progress");

                    Status = element.GetElementValue("status");
                    ETA = element.GetElementValue("eta");
                    Speed = element.GetElementValue("speed");
                    UploadedBytes = element.GetElementValue("uploaded_bytes");
                    TotalSize = element.GetElementValue("total_size");
                    Elapsed = element.GetElementValue("elapsed");
                    Meter = element.GetElementValue("meter");
                }

                public override string ToString()
                {
                    return string.Format("Status: {0}, ETA: {1}, Speed: {2}\r\nBytes: {3}/{4}, Elapsed: {5}, Meter: {6}%",
                        Status, ETA, Speed, UploadedBytes, TotalSize, Elapsed, Meter);
                }
            }

            public void Dispose()
            {
                cts?.Cancel();
                cts?.Dispose();
            }
        }

        #endregion Upload
    }
}