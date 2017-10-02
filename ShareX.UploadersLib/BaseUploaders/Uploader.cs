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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace ShareX.UploadersLib
{
    public class Uploader
    {
        protected const string ContentTypeMultipartFormData = "multipart/form-data";
        protected const string ContentTypeJSON = "application/json";
        protected const string ContentTypeURLEncoded = "application/x-www-form-urlencoded";
        protected const string ContentTypeOctetStream = "application/octet-stream";

        public delegate void ProgressEventHandler(ProgressManager progress);
        public event ProgressEventHandler ProgressChanged;

        public event Action<string> EarlyURLCopyRequested;

        public bool IsUploading { get; protected set; }
        public List<string> Errors { get; private set; } = new List<string>();
        public bool IsError => !StopUploadRequested && Errors != null && Errors.Count > 0;
        public int BufferSize { get; set; } = 8192;

        protected bool StopUploadRequested { get; set; }
        protected bool AllowReportProgress { get; set; } = true;
        protected bool ReturnResponseOnError { get; set; }

        private HttpWebRequest currentRequest;

        public static void UpdateServicePointManager()
        {
            ServicePointManager.DefaultConnectionLimit = 25;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            try
            {
                // Default value for .NET Framework 4.0 and 4.5: SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
                // Default value for .NET Framework 4.6: SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            }
            catch (NotSupportedException)
            {
                DebugHelper.WriteLine("Unable to configure TLS 1.2 as the default security protocol. .NET Framework 4.5 or newer version must be installed in your system to support it.");
            }
        }

        protected void OnProgressChanged(ProgressManager progress)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(progress);
            }
        }

        protected void OnEarlyURLCopyRequested(string url)
        {
            if (EarlyURLCopyRequested != null && !string.IsNullOrEmpty(url))
            {
                EarlyURLCopyRequested(url);
            }
        }

        public string ToErrorString()
        {
            if (IsError)
            {
                return string.Join(Environment.NewLine, Errors);
            }

            return "";
        }

        public virtual void StopUpload()
        {
            if (IsUploading)
            {
                StopUploadRequested = true;

                if (currentRequest != null)
                {
                    try
                    {
                        currentRequest.Abort();
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                    }
                }
            }
        }

        protected string SendRequest(HttpMethod method, string url, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null, ResponseType responseType = ResponseType.Text)
        {
            return SendRequest(method, url, (Stream)null, null, args, headers, cookies, responseType);
        }

        protected string SendRequest(HttpMethod method, string url, Stream data, string contentType = null, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null, ResponseType responseType = ResponseType.Text)
        {
            using (HttpWebResponse response = GetResponse(method, url, data, contentType, args, headers, cookies))
            {
                return ResponseToString(response, responseType);
            }
        }

        protected string SendRequest(HttpMethod method, string url, string content, string contentType = null, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null, ResponseType responseType = ResponseType.Text)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);

                return SendRequest(method, url, ms, contentType, args, headers, cookies, responseType);
            }
        }

        protected string SendRequestURLEncoded(HttpMethod method, string url, Dictionary<string, string> args, NameValueCollection headers = null, CookieCollection cookies = null,
            ResponseType responseType = ResponseType.Text)
        {
            string query = URLHelpers.CreateQuery(args);

            return SendRequest(method, url, query, ContentTypeURLEncoded, args, headers, cookies, responseType);
        }

        protected NameValueCollection SendRequestGetHeaders(HttpMethod method, string url, Stream data, string contentType, Dictionary<string, string> args,
            NameValueCollection headers = null, CookieCollection cookies = null)
        {
            using (HttpWebResponse response = GetResponse(method, url, data, contentType, null, headers, cookies))
            {
                if (response != null)
                {
                    return response.Headers;
                }

                return null;
            }
        }

        protected bool SendRequestDownload(HttpMethod method, string url, Stream downloadStream, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, string contentType = null)
        {
            using (HttpWebResponse response = GetResponse(method, url, null, contentType, args, headers, cookies))
            {
                if (response != null)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        responseStream.CopyStreamTo(downloadStream, BufferSize);
                    }

                    return true;
                }
            }

            return false;
        }

        protected string SendRequestMultiPart(string url, Dictionary<string, string> args, NameValueCollection headers = null, CookieCollection cookies = null,
            ResponseType responseType = ResponseType.Text)
        {
            string boundary = CreateBoundary();
            string contentType = ContentTypeMultipartFormData + "; boundary=" + boundary;
            byte[] data = MakeInputContent(boundary, args);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);

                using (HttpWebResponse response = GetResponse(HttpMethod.POST, url, stream, contentType, null, headers, cookies))
                {
                    return ResponseToString(response, responseType);
                }
            }
        }

        protected UploadResult SendRequestFile(string url, Stream data, string fileName, string fileFormName = "file", Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, ResponseType responseType = ResponseType.Text, HttpMethod method = HttpMethod.POST,
            string contentType = ContentTypeMultipartFormData, string metadata = null)
        {
            UploadResult result = new UploadResult();

            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                string boundary = CreateBoundary();
                contentType += "; boundary=" + boundary;

                byte[] bytesArguments = MakeInputContent(boundary, args, false);
                byte[] bytesDataOpen;
                byte[] bytesDataDatafile = { };

                if (metadata != null)
                {
                    bytesDataOpen = MakeFileInputContentOpen(boundary, fileFormName, fileName, metadata);
                    bytesDataDatafile = MakeFileInputContentOpen(boundary, fileFormName, fileName, null);
                }
                else
                {
                    bytesDataOpen = MakeFileInputContentOpen(boundary, fileFormName, fileName);
                }

                byte[] bytesDataClose = MakeFileInputContentClose(boundary);

                long contentLength = bytesArguments.Length + bytesDataOpen.Length + bytesDataDatafile.Length + data.Length + bytesDataClose.Length;
                HttpWebRequest request = PrepareWebRequest(method, url, headers, cookies, contentType, contentLength);

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytesArguments, 0, bytesArguments.Length);
                    requestStream.Write(bytesDataOpen, 0, bytesDataOpen.Length);
                    requestStream.Write(bytesDataDatafile, 0, bytesDataDatafile.Length);
                    if (!TransferData(data, requestStream)) return null;
                    requestStream.Write(bytesDataClose, 0, bytesDataClose.Length);
                }

                using (WebResponse response = request.GetResponse())
                {
                    result.Response = ResponseToString(response, responseType);
                }

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                if (!StopUploadRequested)
                {
                    string response = AddWebError(e, url);

                    if (ReturnResponseOnError && e is WebException)
                    {
                        result.Response = response;
                    }
                }
            }
            finally
            {
                currentRequest = null;
                IsUploading = false;
            }

            return result;
        }

        private HttpWebResponse GetResponse(HttpMethod method, string url, Stream data = null, string contentType = null, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null)
        {
            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                url = URLHelpers.CreateQuery(url, args);

                long length = 0;

                if (data != null)
                {
                    length = data.Length;
                }

                HttpWebRequest request = PrepareWebRequest(method, url, headers, cookies, contentType, length);

                if (length > 0)
                {
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        if (!TransferData(data, requestStream))
                        {
                            return null;
                        }
                    }
                }

                return (HttpWebResponse)request.GetResponse();
            }
            catch (Exception e)
            {
                if (!StopUploadRequested)
                {
                    AddWebError(e, url);
                }
            }
            finally
            {
                currentRequest = null;
                IsUploading = false;
            }

            return null;
        }

        #region Helper methods

        private HttpWebRequest PrepareWebRequest(HttpMethod method, string url, NameValueCollection headers = null, CookieCollection cookies = null, string contentType = null, long contentLength = 0)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method.ToString();

            if (headers != null)
            {
                if (headers["Accept"] != null)
                {
                    request.Accept = headers["Accept"];
                    headers.Remove("Accept");
                }

                if (headers["Content-Length"] != null)
                {
                    request.ContentLength = Convert.ToInt32(headers["Content-Length"]);
                    headers.Remove("Content-Length");
                }

                request.Headers.Add(headers);
            }

            request.CookieContainer = new CookieContainer();
            if (cookies != null) request.CookieContainer.Add(cookies);
            IWebProxy proxy = HelpersOptions.CurrentProxy.GetWebProxy();
            if (proxy != null) request.Proxy = proxy;
            request.UserAgent = ShareXResources.UserAgent;
            request.ContentType = contentType;

            if (contentLength > 0)
            {
                request.AllowWriteStreamBuffering = HelpersOptions.CurrentProxy.IsValidProxy();
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.ContentLength = contentLength;
                request.Pipelined = false;
                request.Timeout = -1;
            }
            else
            {
                request.KeepAlive = false;
            }

            currentRequest = request;

            return request;
        }

        protected bool TransferData(Stream dataStream, Stream requestStream)
        {
            if (dataStream.CanSeek)
            {
                dataStream.Position = 0;
            }

            ProgressManager progress = new ProgressManager(dataStream.Length);
            int length = (int)Math.Min(BufferSize, dataStream.Length);
            byte[] buffer = new byte[length];
            int bytesRead;

            while (!StopUploadRequested && (bytesRead = dataStream.Read(buffer, 0, length)) > 0)
            {
                requestStream.Write(buffer, 0, bytesRead);

                if (AllowReportProgress && progress.UpdateProgress(bytesRead))
                {
                    OnProgressChanged(progress);
                }
            }

            return !StopUploadRequested;
        }

        private string CreateBoundary()
        {
            return new string('-', 20) + DateTime.Now.Ticks.ToString("x");
        }

        private byte[] MakeInputContent(string boundary, string name, string value)
        {
            string format = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, name, value);
            return Encoding.UTF8.GetBytes(format);
        }

        private byte[] MakeInputContent(string boundary, Dictionary<string, string> contents, bool isFinal = true)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (string.IsNullOrEmpty(boundary)) boundary = CreateBoundary();
                byte[] bytes;

                if (contents != null)
                {
                    foreach (KeyValuePair<string, string> content in contents)
                    {
                        if (!string.IsNullOrEmpty(content.Key) && !string.IsNullOrEmpty(content.Value))
                        {
                            bytes = MakeInputContent(boundary, content.Key, content.Value);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    if (isFinal)
                    {
                        bytes = MakeFinalBoundary(boundary);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }

                return stream.ToArray();
            }
        }

        private byte[] MakeFileInputContentOpen(string boundary, string fileFormName, string fileName)
        {
            string format = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                boundary, fileFormName, fileName, Helpers.GetMimeType(fileName));

            return Encoding.UTF8.GetBytes(format);
        }

        private byte[] MakeFileInputContentOpen(string boundary, string fileFormName, string fileName, string metadata)
        {
            string format = "";

            if (metadata != null)
            {
                format = string.Format("--{0}\r\nContent-Type: {1}; charset=UTF-8\r\n\r\n{2}\r\n\r\n", boundary, ContentTypeJSON, metadata);
            }
            else
            {
                format = string.Format("--{0}\r\nContent-Type: {1}\r\n\r\n", boundary, Helpers.GetMimeType(fileName));
            }

            return Encoding.UTF8.GetBytes(format);
        }

        private byte[] MakeFileInputContentClose(string boundary)
        {
            return Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));
        }

        private byte[] MakeFinalBoundary(string boundary)
        {
            return Encoding.UTF8.GetBytes(string.Format("--{0}--\r\n", boundary));
        }

        private string ResponseToString(WebResponse response, ResponseType responseType = ResponseType.Text)
        {
            if (response != null)
            {
                using (response)
                {
                    switch (responseType)
                    {
                        case ResponseType.Text:
                            using (Stream responseStream = response.GetResponseStream())
                            using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                            {
                                return reader.ReadToEnd();
                            }
                        case ResponseType.RedirectionURL:
                            return response.ResponseUri.OriginalString;
                        case ResponseType.Headers:
                            StringBuilder sbHeaders = new StringBuilder();
                            foreach (string key in response.Headers.AllKeys)
                            {
                                string value = response.Headers[key];
                                sbHeaders.AppendFormat("{0}: \"{1}\"{2}", key, value, Environment.NewLine);
                            }
                            return sbHeaders.ToString().Trim();
                        case ResponseType.LocationHeader:
                            return response.Headers["Location"];
                    }
                }
            }

            return null;
        }

        protected NameValueCollection CreateAuthenticationHeader(string username, string password)
        {
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            NameValueCollection headers = new NameValueCollection();
            headers["Authorization"] = "Basic " + authInfo;
            return headers;
        }

        private string AddWebError(Exception e, string url)
        {
            string response = null;

            if (Errors != null && e != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Message:");
                sb.AppendLine(e.Message);

                if (!string.IsNullOrEmpty(url))
                {
                    sb.AppendLine();
                    sb.AppendLine("Request URL:");
                    sb.AppendLine(URLHelpers.RemoveQuery(url));
                }

                if (e is WebException)
                {
                    try
                    {
                        response = ResponseToString(((WebException)e).Response);

                        if (!string.IsNullOrEmpty(response))
                        {
                            sb.AppendLine();
                            sb.AppendLine("Response:");
                            sb.AppendLine(response);
                        }
                    }
                    catch
                    {
                    }
                }

                sb.AppendLine();
                sb.AppendLine("Stack trace:");
                sb.AppendLine(e.StackTrace);

                string errorText = sb.ToString().Trim();
                Errors.Add(errorText);
                DebugHelper.WriteLine("Error:\r\n" + errorText);
            }

            return response;
        }

        #endregion Helper methods

        #region OAuth methods

        protected string GetAuthorizationURL(string requestTokenURL, string authorizeURL, OAuthInfo authInfo,
            Dictionary<string, string> customParameters = null, HttpMethod httpMethod = HttpMethod.GET)
        {
            string url = OAuthManager.GenerateQuery(requestTokenURL, customParameters, httpMethod, authInfo);

            string response = SendRequest(httpMethod, url);

            if (!string.IsNullOrEmpty(response))
            {
                return OAuthManager.GetAuthorizationURL(response, authInfo, authorizeURL);
            }

            return null;
        }

        protected bool GetAccessToken(string accessTokenURL, OAuthInfo authInfo, HttpMethod httpMethod = HttpMethod.GET)
        {
            return GetAccessTokenEx(accessTokenURL, authInfo, httpMethod) != null;
        }

        protected NameValueCollection GetAccessTokenEx(string accessTokenURL, OAuthInfo authInfo, HttpMethod httpMethod = HttpMethod.GET)
        {
            if (string.IsNullOrEmpty(authInfo.AuthToken) || string.IsNullOrEmpty(authInfo.AuthSecret))
            {
                throw new Exception("Auth infos missing. Open Authorization URL first.");
            }

            string url = OAuthManager.GenerateQuery(accessTokenURL, null, httpMethod, authInfo);

            string response = SendRequest(httpMethod, url);

            if (!string.IsNullOrEmpty(response))
            {
                return OAuthManager.ParseAccessTokenResponse(response, authInfo);
            }

            return null;
        }

        #endregion OAuth methods
    }
}