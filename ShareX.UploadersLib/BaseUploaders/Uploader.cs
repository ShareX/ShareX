#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.Text;

namespace ShareX.UploadersLib
{
    public class Uploader
    {
        public delegate void ProgressEventHandler(ProgressManager progress);
        public event ProgressEventHandler ProgressChanged;

        public event Action<string> EarlyURLCopyRequested;

        public bool IsUploading { get; protected set; }
        public List<string> Errors { get; private set; } = new List<string>();
        public bool IsError => !StopUploadRequested && Errors != null && Errors.Count > 0;
        public int BufferSize { get; set; } = 8192;
        public bool VerboseLogs { get; set; }
        public string VerboseLogsPath { get; set; }

        protected bool StopUploadRequested { get; set; }
        protected bool AllowReportProgress { get; set; } = true;
        protected bool ReturnResponseOnError { get; set; }

        private HttpWebRequest currentRequest;
        private Logger verboseLogger;

        public static void UpdateServicePointManager()
        {
            ServicePointManager.DefaultConnectionLimit = 25;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
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
            using (HttpWebResponse webResponse = GetResponse(method, url, data, contentType, args, headers, cookies))
            {
                string response = UploadHelpers.ResponseToString(webResponse, responseType);

                if (VerboseLogs && !string.IsNullOrEmpty(VerboseLogsPath))
                {
                    WriteVerboseLog(url, args, headers, response);
                }

                return response;
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

        public string SendRequestURLEncoded(HttpMethod method, string url, Dictionary<string, string> args, NameValueCollection headers = null, CookieCollection cookies = null,
            ResponseType responseType = ResponseType.Text)
        {
            string query = URLHelpers.CreateQuery(args);

            return SendRequest(method, url, query, UploadHelpers.ContentTypeURLEncoded, args, headers, cookies, responseType);
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
            string boundary = UploadHelpers.CreateBoundary();
            string contentType = UploadHelpers.ContentTypeMultipartFormData + "; boundary=" + boundary;
            byte[] data = UploadHelpers.MakeInputContent(boundary, args);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);

                using (HttpWebResponse webResponse = GetResponse(HttpMethod.POST, url, stream, contentType, null, headers, cookies))
                {
                    string response = UploadHelpers.ResponseToString(webResponse, responseType);

                    if (VerboseLogs && !string.IsNullOrEmpty(VerboseLogsPath))
                    {
                        WriteVerboseLog(url, args, headers, response);
                    }

                    return response;
                }
            }
        }

        protected UploadResult SendRequestFile(string url, Stream data, string fileName, string fileFormName = "file", Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, ResponseType responseType = ResponseType.Text, HttpMethod method = HttpMethod.POST,
            string contentType = UploadHelpers.ContentTypeMultipartFormData, string metadata = null)
        {
            UploadResult result = new UploadResult();

            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                string boundary = UploadHelpers.CreateBoundary();
                contentType += "; boundary=" + boundary;

                byte[] bytesArguments = UploadHelpers.MakeInputContent(boundary, args, false);
                byte[] bytesDataOpen;
                byte[] bytesDataDatafile = { };

                if (metadata != null)
                {
                    bytesDataOpen = UploadHelpers.MakeFileInputContentOpen(boundary, fileFormName, fileName, metadata);
                    bytesDataDatafile = UploadHelpers.MakeFileInputContentOpen(boundary, fileFormName, fileName, null);
                }
                else
                {
                    bytesDataOpen = UploadHelpers.MakeFileInputContentOpen(boundary, fileFormName, fileName);
                }

                byte[] bytesDataClose = UploadHelpers.MakeFileInputContentClose(boundary);

                long contentLength = bytesArguments.Length + bytesDataOpen.Length + bytesDataDatafile.Length + data.Length + bytesDataClose.Length;

                HttpWebRequest request = UploadHelpers.CreateWebRequest(method, url, headers, cookies, contentType, contentLength);
                currentRequest = request;

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
                    result.Response = UploadHelpers.ResponseToString(response, responseType);
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

                if (VerboseLogs && !string.IsNullOrEmpty(VerboseLogsPath))
                {
                    WriteVerboseLog(url, args, headers, result.Response);
                }
            }

            return result;
        }

        protected UploadResult SendRequestFileRange(string url, Stream data, string fileName, long contentPosition = 0, long contentLength = -1,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null, ResponseType responseType = ResponseType.Text,
            HttpMethod method = HttpMethod.PUT)
        {
            UploadResult result = new UploadResult();

            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                url = URLHelpers.CreateQuery(url, args);

                if (contentLength == -1)
                {
                    contentLength = data.Length;
                }
                contentLength = Math.Min(contentLength, data.Length - contentPosition);

                string contentType = UploadHelpers.GetMimeType(fileName);

                if (headers == null)
                {
                    headers = new NameValueCollection();
                }
                long startByte = contentPosition;
                long endByte = startByte + contentLength - 1;
                long dataLength = data.Length;
                headers.Add("Content-Range", $"bytes {startByte}-{endByte}/{dataLength}");

                HttpWebRequest request = UploadHelpers.CreateWebRequest(method, url, headers, cookies, contentType, contentLength);
                currentRequest = request;

                using (Stream requestStream = request.GetRequestStream())
                {
                    if (!TransferData(data, requestStream, contentPosition, contentLength))
                    {
                        return null;
                    }
                }

                using (WebResponse response = request.GetResponse())
                {
                    result.Response = UploadHelpers.ResponseToString(response, responseType);
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

                if (VerboseLogs && !string.IsNullOrEmpty(VerboseLogsPath))
                {
                    WriteVerboseLog(url, args, headers, result.Response);
                }
            }

            return result;
        }

        protected HttpWebResponse GetResponse(HttpMethod method, string url, Stream data = null, string contentType = null, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, bool allowNon2xxResponses = false)
        {
            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                url = URLHelpers.CreateQuery(url, args);

                long contentLength = 0;

                if (data != null)
                {
                    contentLength = data.Length;
                }

                HttpWebRequest request = UploadHelpers.CreateWebRequest(method, url, headers, cookies, contentType, contentLength);
                currentRequest = request;

                if (contentLength > 0)
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
            catch (WebException we) when (we.Response != null && allowNon2xxResponses)
            {
                // if we.Response != null, then the request was successful, but
                // returned a non-200 status code
                return we.Response as HttpWebResponse;
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

        protected bool TransferData(Stream dataStream, Stream requestStream, long dataPosition = 0, long dataLength = -1)
        {
            if (dataPosition >= dataStream.Length)
            {
                return true;
            }

            if (dataStream.CanSeek)
            {
                dataStream.Position = dataPosition;
            }

            if (dataLength == -1)
            {
                dataLength = dataStream.Length;
            }
            dataLength = Math.Min(dataLength, dataStream.Length - dataPosition);

            ProgressManager progress = new ProgressManager(dataStream.Length, dataPosition);
            int length = (int)Math.Min(BufferSize, dataLength);
            byte[] buffer = new byte[length];
            int bytesRead;

            long bytesRemaining = dataLength;
            while (!StopUploadRequested && (bytesRead = dataStream.Read(buffer, 0, length)) > 0)
            {
                requestStream.Write(buffer, 0, bytesRead);
                bytesRemaining -= bytesRead;
                length = (int)Math.Min(buffer.Length, bytesRemaining);

                if (AllowReportProgress && progress.UpdateProgress(bytesRead))
                {
                    OnProgressChanged(progress);
                }
            }

            return !StopUploadRequested;
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

                if (e is WebException webException)
                {
                    try
                    {
                        WebResponse res = webException.Response;
                        using (res)
                        {
                            response = UploadHelpers.ResponseToString(res);

                            if (!string.IsNullOrEmpty(response))
                            {
                                sb.AppendLine();
                                sb.AppendLine("Response:");
                                sb.AppendLine(response);
                            }
                        }
                    }
                    catch (Exception nested)
                    {
                        DebugHelper.WriteException(nested, "AddWebError() WebException handler");
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

        private void WriteVerboseLog(string url, Dictionary<string, string> args, NameValueCollection headers, string response)
        {
            if (verboseLogger == null)
            {
                verboseLogger = new Logger(VerboseLogsPath)
                {
                    MessageFormat = "Date: {0:yyyy-MM-dd HH:mm:ss.fff}\r\n{1}",
                    StringWrite = false
                };
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("URL: " + (url ?? ""));

            if (args != null && args.Count > 0)
            {
                sb.AppendLine("Arguments:");

                foreach (KeyValuePair<string, string> arg in args)
                {
                    sb.AppendLine($"    Name: {arg.Key}, Value: {arg.Value}");
                }
            }

            if (headers != null && headers.Count > 0)
            {
                sb.AppendLine("Headers:");

                foreach (string key in headers)
                {
                    string value = headers[key];
                    sb.AppendLine($"    Name: {key}, Value: {value}");
                }
            }

            sb.AppendLine("Response:");

            if (!string.IsNullOrEmpty(response))
            {
                sb.AppendLine(response);
            }

            sb.Append(new string('-', 30));

            verboseLogger.WriteLine(sb.ToString());
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