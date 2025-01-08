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
        public UploaderErrorManager Errors { get; private set; } = new UploaderErrorManager();
        public bool IsError => !StopUploadRequested && Errors != null && Errors.Count > 0;
        public int BufferSize { get; set; } = 8192;

        protected bool StopUploadRequested { get; set; }
        protected bool AllowReportProgress { get; set; } = true;
        protected bool ReturnResponseOnError { get; set; }

        protected ResponseInfo LastResponseInfo { get; set; }

        private HttpWebRequest currentWebRequest;

        public static void UpdateServicePointManager()
        {
            ServicePointManager.DefaultConnectionLimit = 25;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;

            if (Helpers.IsWindows7())
            {
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                }
                catch (NotSupportedException)
                {
                    DebugHelper.WriteLine("Unable to configure TLS 1.2 as the default security protocol.");
                }
            }
        }

        protected void OnProgressChanged(ProgressManager progress)
        {
            ProgressChanged?.Invoke(progress);
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

                if (currentWebRequest != null)
                {
                    try
                    {
                        currentWebRequest.Abort();
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                    }
                }
            }
        }

        internal string SendRequest(HttpMethod method, string url, Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null)
        {
            return SendRequest(method, url, (Stream)null, null, args, headers, cookies);
        }

        protected string SendRequest(HttpMethod method, string url, Stream data, string contentType = null, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null)
        {
            using (HttpWebResponse webResponse = GetResponse(method, url, data, contentType, args, headers, cookies))
            {
                return ProcessWebResponseText(webResponse);
            }
        }

        protected string SendRequest(HttpMethod method, string url, string content, string contentType = null, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null)
        {
            byte[] data = Encoding.UTF8.GetBytes(content);

            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(data, 0, data.Length);

                return SendRequest(method, url, ms, contentType, args, headers, cookies);
            }
        }

        internal string SendRequestURLEncoded(HttpMethod method, string url, Dictionary<string, string> args, NameValueCollection headers = null, CookieCollection cookies = null)
        {
            string query = URLHelpers.CreateQueryString(args);

            return SendRequest(method, url, query, RequestHelpers.ContentTypeURLEncoded, null, headers, cookies);
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
            HttpMethod method = HttpMethod.POST)
        {
            string boundary = RequestHelpers.CreateBoundary();
            string contentType = RequestHelpers.ContentTypeMultipartFormData + "; boundary=" + boundary;
            byte[] data = RequestHelpers.MakeInputContent(boundary, args);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);

                using (HttpWebResponse webResponse = GetResponse(method, url, stream, contentType, null, headers, cookies))
                {
                    return ProcessWebResponseText(webResponse);
                }
            }
        }

        protected UploadResult SendRequestFile(string url, Stream data, string fileName, string fileFormName, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, HttpMethod method = HttpMethod.POST, string contentType = RequestHelpers.ContentTypeMultipartFormData,
            string relatedData = null)
        {
            UploadResult result = new UploadResult();

            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                string boundary = RequestHelpers.CreateBoundary();
                contentType += "; boundary=" + boundary;

                byte[] bytesArguments = RequestHelpers.MakeInputContent(boundary, args, false);
                byte[] bytesDataOpen;

                if (relatedData != null)
                {
                    bytesDataOpen = RequestHelpers.MakeRelatedFileInputContentOpen(boundary, "application/json; charset=UTF-8", relatedData, fileName);
                }
                else
                {
                    bytesDataOpen = RequestHelpers.MakeFileInputContentOpen(boundary, fileFormName, fileName);
                }

                byte[] bytesDataClose = RequestHelpers.MakeFileInputContentClose(boundary);

                long contentLength = bytesArguments.Length + bytesDataOpen.Length + data.Length + bytesDataClose.Length;

                HttpWebRequest request = CreateWebRequest(method, url, headers, cookies, contentType, contentLength);

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytesArguments, 0, bytesArguments.Length);
                    requestStream.Write(bytesDataOpen, 0, bytesDataOpen.Length);
                    if (!TransferData(data, requestStream)) return null;
                    requestStream.Write(bytesDataClose, 0, bytesDataClose.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    result.ResponseInfo = ProcessWebResponse(response);
                    result.Response = result.ResponseInfo?.ResponseText;
                }

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                if (!StopUploadRequested)
                {
                    string response = ProcessError(e, url);

                    if (ReturnResponseOnError && e is WebException)
                    {
                        result.Response = response;
                    }
                }
            }
            finally
            {
                currentWebRequest = null;
                IsUploading = false;
            }

            return result;
        }

        protected UploadResult SendRequestFileRange(string url, Stream data, string fileName, long contentPosition = 0, long contentLength = -1,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null, HttpMethod method = HttpMethod.PUT)
        {
            UploadResult result = new UploadResult();

            IsUploading = true;
            StopUploadRequested = false;

            try
            {
                url = URLHelpers.CreateQueryString(url, args);

                if (contentLength == -1)
                {
                    contentLength = data.Length;
                }
                contentLength = Math.Min(contentLength, data.Length - contentPosition);

                string contentType = MimeTypes.GetMimeTypeFromFileName(fileName);

                if (headers == null)
                {
                    headers = new NameValueCollection();
                }
                long startByte = contentPosition;
                long endByte = startByte + contentLength - 1;
                long dataLength = data.Length;
                headers.Add("Content-Range", $"bytes {startByte}-{endByte}/{dataLength}");

                HttpWebRequest request = CreateWebRequest(method, url, headers, cookies, contentType, contentLength);

                using (Stream requestStream = request.GetRequestStream())
                {
                    if (!TransferData(data, requestStream, contentPosition, contentLength))
                    {
                        return null;
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    result.ResponseInfo = ProcessWebResponse(response);
                    result.Response = result.ResponseInfo?.ResponseText;
                }

                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                if (!StopUploadRequested)
                {
                    string response = ProcessError(e, url);

                    if (ReturnResponseOnError && e is WebException)
                    {
                        result.Response = response;
                    }
                }
            }
            finally
            {
                currentWebRequest = null;
                IsUploading = false;
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
                url = URLHelpers.CreateQueryString(url, args);

                long contentLength = 0;

                if (data != null)
                {
                    contentLength = data.Length;
                }

                HttpWebRequest request = CreateWebRequest(method, url, headers, cookies, contentType, contentLength);

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
                    ProcessError(e, url);
                }
            }
            finally
            {
                currentWebRequest = null;
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

        private string ProcessError(Exception e, string requestURL)
        {
            string responseText = null;

            if (e != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Error message:");
                sb.AppendLine(e.Message);

                if (!string.IsNullOrEmpty(requestURL))
                {
                    sb.AppendLine();
                    sb.AppendLine("Request URL:");
                    sb.AppendLine(requestURL);
                }

                if (e is WebException webException)
                {
                    try
                    {
                        using (HttpWebResponse webResponse = (HttpWebResponse)webException.Response)
                        {
                            ResponseInfo responseInfo = ProcessWebResponse(webResponse);

                            if (responseInfo != null)
                            {
                                responseText = responseInfo.ResponseText;

                                sb.AppendLine();
                                sb.AppendLine("Status code:");
                                sb.AppendLine($"({(int)responseInfo.StatusCode}) {responseInfo.StatusDescription}");

                                if (!string.IsNullOrEmpty(requestURL) && !requestURL.Equals(responseInfo.ResponseURL))
                                {
                                    sb.AppendLine();
                                    sb.AppendLine("Response URL:");
                                    sb.AppendLine(responseInfo.ResponseURL);
                                }

                                if (responseInfo.Headers != null)
                                {
                                    sb.AppendLine();
                                    sb.AppendLine("Headers:");
                                    sb.AppendLine(responseInfo.Headers.ToString().TrimEnd());
                                }

                                sb.AppendLine();
                                sb.AppendLine("Response text:");
                                sb.AppendLine(responseInfo.ResponseText);
                            }
                        }
                    }
                    catch (Exception nested)
                    {
                        DebugHelper.WriteException(nested, "ProcessError() WebException handler");
                    }
                }

                sb.AppendLine();
                sb.AppendLine("Stack trace:");
                sb.Append(e.StackTrace);

                string errorText = sb.ToString();

                if (Errors == null) Errors = new UploaderErrorManager();
                Errors.Add(errorText);

                DebugHelper.WriteLine("Error:\r\n" + errorText);
            }

            return responseText;
        }

        private HttpWebRequest CreateWebRequest(HttpMethod method, string url, NameValueCollection headers = null, CookieCollection cookies = null,
            string contentType = null, long contentLength = 0)
        {
            LastResponseInfo = null;

            HttpWebRequest request = RequestHelpers.CreateWebRequest(method, url, headers, cookies, contentType, contentLength);
            currentWebRequest = request;
            return request;
        }

        private ResponseInfo ProcessWebResponse(HttpWebResponse response)
        {
            if (response != null)
            {
                ResponseInfo responseInfo = new ResponseInfo()
                {
                    StatusCode = response.StatusCode,
                    StatusDescription = response.StatusDescription,
                    ResponseURL = response.ResponseUri.OriginalString,
                    Headers = response.Headers
                };

                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    responseInfo.ResponseText = reader.ReadToEnd();
                }

                LastResponseInfo = responseInfo;

                return responseInfo;
            }

            return null;
        }

        private string ProcessWebResponseText(HttpWebResponse response)
        {
            ResponseInfo responseInfo = ProcessWebResponse(response);

            if (responseInfo != null)
            {
                return responseInfo.ResponseText;
            }

            return null;
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