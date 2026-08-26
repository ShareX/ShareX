#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetHttpMethod = System.Net.Http.HttpMethod;

namespace ShareX.UploadersLib
{
    public class Uploader
    {
        public delegate void ProgressEventHandler(ProgressManager progress);
        public event ProgressEventHandler ProgressChanged;
        public event Action<string> EarlyURLCopyRequested;

        private readonly object operationLock = new object();
        private CancellationTokenSource operationCancellation;

        public bool IsUploading { get; protected set; }
        public UploaderErrorManager Errors { get; private set; } = new UploaderErrorManager();
        public bool IsError => !StopUploadRequested && Errors != null && Errors.Count > 0;
        public int BufferSize { get; set; } = 8192;
        public TimeSpan RequestTimeout { get; set; } = Timeout.InfiniteTimeSpan;
        public bool AllowAutoRedirect { get; set; } = true;

        protected bool StopUploadRequested { get; set; }
        protected bool AllowReportProgress { get; set; } = true;
        protected bool ReturnResponseOnError { get; set; }
        protected ResponseInfo LastResponseInfo { get; set; }
        protected CancellationToken CurrentCancellationToken => operationCancellation?.Token ?? CancellationToken.None;

        protected void OnProgressChanged(ProgressManager progress)
        {
            ProgressChanged?.Invoke(progress);
        }

        protected void OnEarlyURLCopyRequested(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                EarlyURLCopyRequested?.Invoke(url);
            }
        }

        public string ToErrorString()
        {
            return IsError ? string.Join(Environment.NewLine, Errors) : "";
        }

        public virtual void StopUpload()
        {
            CancellationTokenSource cancellation = null;

            lock (operationLock)
            {
                if (IsUploading)
                {
                    StopUploadRequested = true;
                    cancellation = operationCancellation;
                }
            }

            cancellation?.Cancel();
        }

        protected async Task<T> RunOperationAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            CancellationTokenSource cancellation;

            lock (operationLock)
            {
                if (operationCancellation != null)
                {
                    throw new InvalidOperationException("This uploader is already processing an operation.");
                }

                StopUploadRequested = false;
                IsUploading = true;
                cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                operationCancellation = cancellation;
            }

            try
            {
                return await operation(cancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (StopUploadRequested && !cancellationToken.IsCancellationRequested)
            {
                return default;
            }
            finally
            {
                lock (operationLock)
                {
                    operationCancellation = null;
                    IsUploading = false;
                }

                cancellation.Dispose();
            }
        }

        internal Task<string> SendRequestAsync(HttpMethod method, string url, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null, CancellationToken cancellationToken = default)
        {
            return SendRequestAsync(method, url, (HttpContent)null, args, headers, cookies, false, cancellationToken);
        }

        protected Task<string> SendRequestAsync(HttpMethod method, string url, Stream data, string contentType = null, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, CancellationToken cancellationToken = default)
        {
            HttpContent content = data == null ? null : CreateStreamContent(data, 0, GetStreamLength(data), contentType);
            return SendRequestAsync(method, url, content, args, headers, cookies, true, cancellationToken);
        }

        protected Task<string> SendRequestAsync(HttpMethod method, string url, Stream data, long position, long length, string contentType = null,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null,
            CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            long streamLength = GetStreamLength(data);

            if (position < 0 || position > streamLength)
            {
                throw new ArgumentOutOfRangeException(nameof(position), "The requested upload position must be within the stream.");
            }

            if (length < 0 || length > streamLength - position)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "The requested upload range must be within the stream.");
            }

            HttpContent content = CreateStreamContent(data, position, length, contentType);
            return SendRequestAsync(method, url, content, args, headers, cookies, true, cancellationToken);
        }

        protected Task<string> SendRequestAsync(HttpMethod method, string url, string content, string contentType = null, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, CancellationToken cancellationToken = default)
        {
            ByteArrayContent requestContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content ?? ""));
            SetContentType(requestContent, contentType);
            return SendRequestAsync(method, url, requestContent, args, headers, cookies, true, cancellationToken);
        }

        internal Task<string> SendRequestURLEncodedAsync(HttpMethod method, string url, Dictionary<string, string> args, NameValueCollection headers = null,
            CookieCollection cookies = null, CancellationToken cancellationToken = default)
        {
            string query = URLHelpers.CreateQueryString(args);
            return SendRequestAsync(method, url, query, RequestHelpers.ContentTypeURLEncoded, null, headers, cookies, cancellationToken);
        }

        protected async Task<bool> SendRequestDownloadAsync(HttpMethod method, string url, Stream downloadStream, Dictionary<string, string> args = null,
            NameValueCollection headers = null, CookieCollection cookies = null, string contentType = null, CancellationToken cancellationToken = default)
        {
            if (downloadStream == null)
            {
                throw new ArgumentNullException(nameof(downloadStream));
            }

            url = URLHelpers.CreateQueryString(url, args);
            using HttpRequestMessage request = CreateRequest(method, url, null, contentType, headers, cookies);
            CancellationToken effectiveToken = GetEffectiveCancellationToken(cancellationToken);
            bool ownsUploadingState = BeginRequestScope();

            try
            {
                using CancellationTokenSource timeoutCancellation = CreateTimeoutCancellation(effectiveToken);
                CancellationToken requestToken = timeoutCancellation?.Token ?? effectiveToken;
                HttpClient client = HttpClientFactory.Create(AllowAutoRedirect, infiniteTimeout: true);

                using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, requestToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    ResponseInfo errorInfo = await CreateResponseInfoAsync(response, true, requestToken).ConfigureAwait(false);
                    LastResponseInfo = errorInfo;
                    ProcessError(CreateStatusCodeException(response), url, errorInfo);
                    return false;
                }

                await using Stream responseStream = await response.Content.ReadAsStreamAsync(requestToken).ConfigureAwait(false);
                await responseStream.CopyToAsync(downloadStream, BufferSize, requestToken).ConfigureAwait(false);
                LastResponseInfo = await CreateResponseInfoAsync(response, false, requestToken).ConfigureAwait(false);
                return true;
            }
            catch (OperationCanceledException e) when (!effectiveToken.IsCancellationRequested)
            {
                ProcessError(CreateTimeoutException(e), url);
                return false;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(e, url);
                return false;
            }
            finally
            {
                EndRequestScope(ownsUploadingState);
            }
        }

        protected Task<string> SendRequestMultiPartAsync(string url, Dictionary<string, string> args, NameValueCollection headers = null,
            CookieCollection cookies = null, HttpMethod method = HttpMethod.POST, CancellationToken cancellationToken = default)
        {
            MultipartFormDataContent content = CreateMultipartFormDataContent(args);
            return SendRequestAsync(method, url, content, null, headers, cookies, true, cancellationToken);
        }

        protected async Task<UploadResult> SendRequestFileAsync(string url, Stream data, string fileName, string fileFormName,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null,
            HttpMethod method = HttpMethod.POST, string contentType = RequestHelpers.ContentTypeMultipartFormData, string relatedData = null,
            CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            UploadResult result = new UploadResult();
            using HttpContent content = relatedData == null
                ? CreateFileMultipartContent(data, fileName, fileFormName, args)
                : CreateRelatedMultipartContent(data, fileName, relatedData);

            content.Headers.ContentType.MediaType = contentType;
            ResponseInfo responseInfo = await GetResponseAsync(method, url, content, headers: headers, cookies: cookies,
                cancellationToken: cancellationToken).ConfigureAwait(false);

            result.ResponseInfo = responseInfo ?? LastResponseInfo;
            result.Response = responseInfo?.ResponseText;

            if (responseInfo != null)
            {
                result.IsSuccess = true;
            }
            else if (ReturnResponseOnError)
            {
                result.Response = LastResponseInfo?.ResponseText;
            }

            return result;
        }

        protected async Task<UploadResult> SendRequestFileRangeAsync(string url, Stream data, string fileName, long contentPosition = 0,
            long contentLength = -1, Dictionary<string, string> args = null, NameValueCollection headers = null,
            CookieCollection cookies = null, HttpMethod method = HttpMethod.PUT, CancellationToken cancellationToken = default)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            long dataLength = GetStreamLength(data);

            if (contentPosition < 0 || contentPosition > dataLength)
            {
                throw new ArgumentOutOfRangeException(nameof(contentPosition));
            }

            if (contentLength < 0)
            {
                contentLength = dataLength - contentPosition;
            }

            contentLength = Math.Min(contentLength, dataLength - contentPosition);
            using HttpContent content = CreateStreamContent(data, contentPosition, contentLength, MimeTypes.GetMimeTypeFromFileName(fileName));

            if (contentLength > 0)
            {
                content.Headers.ContentRange = new ContentRangeHeaderValue(contentPosition, contentPosition + contentLength - 1, dataLength);
            }

            ResponseInfo responseInfo = await GetResponseAsync(method, url, content, args, headers, cookies,
                cancellationToken: cancellationToken).ConfigureAwait(false);
            UploadResult result = new UploadResult()
            {
                IsSuccess = responseInfo != null,
                ResponseInfo = responseInfo ?? LastResponseInfo,
                Response = responseInfo?.ResponseText
            };

            if (responseInfo == null && ReturnResponseOnError)
            {
                result.Response = LastResponseInfo?.ResponseText;
            }

            return result;
        }

        protected Task<ResponseInfo> GetResponseAsync(HttpMethod method, string url, Stream data = null, string contentType = null,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null,
            bool allowNon2xxResponses = false, CancellationToken cancellationToken = default)
        {
            HttpContent content = data == null ? null : CreateStreamContent(data, 0, GetStreamLength(data), contentType);
            return GetResponseAsync(method, url, content, args, headers, cookies, allowNon2xxResponses, true, cancellationToken);
        }

        protected bool TransferData(Stream dataStream, Stream destinationStream, long dataPosition = 0, long dataLength = -1)
        {
            if (dataStream == null)
            {
                throw new ArgumentNullException(nameof(dataStream));
            }

            if (destinationStream == null)
            {
                throw new ArgumentNullException(nameof(destinationStream));
            }

            long sourceLength = GetStreamLength(dataStream);

            if (dataPosition >= sourceLength)
            {
                return true;
            }

            if (dataStream.CanSeek)
            {
                dataStream.Position = dataPosition;
            }

            if (dataLength < 0)
            {
                dataLength = sourceLength - dataPosition;
            }

            dataLength = Math.Min(dataLength, sourceLength - dataPosition);
            ProgressManager progress = new ProgressManager(sourceLength, dataPosition);
            byte[] buffer = new byte[Math.Max(1, (int)Math.Min(BufferSize, dataLength))];
            long remaining = dataLength;

            while (!StopUploadRequested && remaining > 0)
            {
                int bytesRead = dataStream.Read(buffer, 0, (int)Math.Min(buffer.Length, remaining));

                if (bytesRead == 0)
                {
                    break;
                }

                destinationStream.Write(buffer, 0, bytesRead);
                remaining -= bytesRead;

                if (AllowReportProgress && progress.UpdateProgress(bytesRead))
                {
                    OnProgressChanged(progress);
                }
            }

            return !StopUploadRequested && remaining == 0;
        }

        protected async Task<string> GetAuthorizationURLAsync(string requestTokenURL, string authorizeURL, OAuthInfo authInfo,
            Dictionary<string, string> customParameters = null, HttpMethod httpMethod = HttpMethod.GET,
            CancellationToken cancellationToken = default)
        {
            string url = OAuthManager.GenerateQuery(requestTokenURL, customParameters, httpMethod, authInfo);
            string response = await SendRequestAsync(httpMethod, url, cancellationToken: cancellationToken).ConfigureAwait(false);
            return string.IsNullOrEmpty(response) ? null : OAuthManager.GetAuthorizationURL(response, authInfo, authorizeURL);
        }

        protected async Task<bool> GetAccessTokenAsync(string accessTokenURL, OAuthInfo authInfo, HttpMethod httpMethod = HttpMethod.GET,
            CancellationToken cancellationToken = default)
        {
            return await GetAccessTokenExAsync(accessTokenURL, authInfo, httpMethod, cancellationToken).ConfigureAwait(false) != null;
        }

        protected async Task<NameValueCollection> GetAccessTokenExAsync(string accessTokenURL, OAuthInfo authInfo,
            HttpMethod httpMethod = HttpMethod.GET, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(authInfo.AuthToken) || string.IsNullOrEmpty(authInfo.AuthSecret))
            {
                throw new Exception(Localization.Strings.Uploader_Authorization_information_missing);
            }

            string url = OAuthManager.GenerateQuery(accessTokenURL, null, httpMethod, authInfo);
            string response = await SendRequestAsync(httpMethod, url, cancellationToken: cancellationToken).ConfigureAwait(false);
            return string.IsNullOrEmpty(response) ? null : OAuthManager.ParseAccessTokenResponse(response, authInfo);
        }

        private async Task<string> SendRequestAsync(HttpMethod method, string url, HttpContent content, Dictionary<string, string> args,
            NameValueCollection headers, CookieCollection cookies, bool disposeContent, CancellationToken cancellationToken)
        {
            ResponseInfo responseInfo = await GetResponseAsync(method, url, content, args, headers, cookies, false, disposeContent,
                cancellationToken).ConfigureAwait(false);
            return responseInfo?.ResponseText;
        }

        private async Task<ResponseInfo> GetResponseAsync(HttpMethod method, string url, HttpContent content,
            Dictionary<string, string> args = null, NameValueCollection headers = null, CookieCollection cookies = null,
            bool allowNon2xxResponses = false, bool disposeContent = false, CancellationToken cancellationToken = default)
        {
            url = URLHelpers.CreateQueryString(url, args);
            using HttpRequestMessage request = CreateRequest(method, url, content, null, headers, cookies);
            CancellationToken effectiveToken = GetEffectiveCancellationToken(cancellationToken);
            bool ownsUploadingState = BeginRequestScope();

            try
            {
                using CancellationTokenSource timeoutCancellation = CreateTimeoutCancellation(effectiveToken);
                CancellationToken requestToken = timeoutCancellation?.Token ?? effectiveToken;
                HttpClient client = HttpClientFactory.Create(AllowAutoRedirect, infiniteTimeout: true);

                using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, requestToken).ConfigureAwait(false);
                ResponseInfo responseInfo = await CreateResponseInfoAsync(response, true, requestToken).ConfigureAwait(false);
                LastResponseInfo = responseInfo;

                if (!response.IsSuccessStatusCode && !allowNon2xxResponses)
                {
                    ProcessError(CreateStatusCodeException(response), url, responseInfo);
                    return null;
                }

                return responseInfo;
            }
            catch (OperationCanceledException e) when (!effectiveToken.IsCancellationRequested)
            {
                ProcessError(CreateTimeoutException(e), url);
                return null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                ProcessError(e, url);
                return null;
            }
            finally
            {
                if (disposeContent)
                {
                    content?.Dispose();
                }

                EndRequestScope(ownsUploadingState);
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent content, string contentType,
            NameValueCollection headers, CookieCollection cookies)
        {
            LastResponseInfo = null;

            if (content == null && (!string.IsNullOrEmpty(contentType) || HasContentHeaders(headers)))
            {
                content = new ByteArrayContent(Array.Empty<byte>());
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                SetContentType(content, contentType);
            }

            HttpRequestMessage request = new HttpRequestMessage(new NetHttpMethod(method.ToString()), url)
            {
                Content = content
            };

            AddHeaders(request, headers);
            AddCookies(request, headers?["Cookie"], cookies);
            return request;
        }

        private HttpContent CreateStreamContent(Stream data, long position, long length, string contentType)
        {
            ProgressManager progress = new ProgressManager(GetStreamLength(data), position);
            ProgressStreamContent content = new ProgressStreamContent(data, position, length, BufferSize, bytesRead =>
            {
                if (AllowReportProgress && progress.UpdateProgress(bytesRead))
                {
                    OnProgressChanged(progress);
                }
            });
            SetContentType(content, contentType);
            return content;
        }

        private MultipartFormDataContent CreateFileMultipartContent(Stream data, string fileName, string fileFormName,
            Dictionary<string, string> args)
        {
            MultipartFormDataContent content = CreateMultipartFormDataContent(args);
            HttpContent fileContent = CreateStreamContent(data, 0, GetStreamLength(data), MimeTypes.GetMimeTypeFromFileName(fileName));
            fileContent.Headers.ContentDisposition = CreateFormDataContentDisposition(fileFormName, fileName);
            content.Add(fileContent);
            return content;
        }

        private MultipartContent CreateRelatedMultipartContent(Stream data, string fileName, string relatedData)
        {
            string boundary = RequestHelpers.CreateBoundary();
            MultipartContent content = new MultipartContent("related", boundary);
            ByteArrayContent metadataContent = new ByteArrayContent(Encoding.UTF8.GetBytes(relatedData));
            metadataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json; charset=UTF-8");
            content.Add(metadataContent);
            content.Add(CreateStreamContent(data, 0, GetStreamLength(data), MimeTypes.GetMimeTypeFromFileName(fileName)));
            return content;
        }

        private static MultipartFormDataContent CreateMultipartFormDataContent(Dictionary<string, string> args)
        {
            string boundary = RequestHelpers.CreateBoundary();
            MultipartFormDataContent content = new MultipartFormDataContent(boundary);

            if (args != null)
            {
                foreach (KeyValuePair<string, string> argument in args)
                {
                    if (!string.IsNullOrEmpty(argument.Key))
                    {
                        StringContent valueContent = new StringContent(argument.Value ?? "", Encoding.UTF8);
                        valueContent.Headers.ContentType = null;
                        valueContent.Headers.ContentDisposition = CreateFormDataContentDisposition(argument.Key);
                        content.Add(valueContent);
                    }
                }
            }

            return content;
        }

        private static ContentDispositionHeaderValue CreateFormDataContentDisposition(string name, string fileName = null)
        {
            ContentDispositionHeaderValue contentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = QuoteHeaderValue(name)
            };

            if (fileName != null)
            {
                contentDisposition.FileName = QuoteHeaderValue(fileName);
            }

            return contentDisposition;
        }

        private static string QuoteHeaderValue(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return $"\"{value.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
        }

        private static void AddHeaders(HttpRequestMessage request, NameValueCollection headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (string name in headers.AllKeys)
            {
                if (string.IsNullOrEmpty(name) || name.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string[] values = headers.GetValues(name);

                if (!request.Headers.TryAddWithoutValidation(name, values))
                {
                    request.Content?.Headers.TryAddWithoutValidation(name, values);
                }
            }
        }

        private static void AddCookies(HttpRequestMessage request, string cookieHeader, CookieCollection cookies)
        {
            List<string> values = new List<string>();

            if (!string.IsNullOrWhiteSpace(cookieHeader))
            {
                values.Add(cookieHeader);
            }

            if (cookies != null)
            {
                values.AddRange(cookies.Cast<Cookie>().Select(cookie => $"{cookie.Name}={cookie.Value}"));
            }

            if (values.Count > 0)
            {
                request.Headers.TryAddWithoutValidation("Cookie", string.Join("; ", values));
            }
        }

        private static bool HasContentHeaders(NameValueCollection headers)
        {
            return headers != null && headers.AllKeys.Any(name => name != null && name.StartsWith("Content-", StringComparison.OrdinalIgnoreCase));
        }

        private static void SetContentType(HttpContent content, string contentType)
        {
            if (content != null && !string.IsNullOrWhiteSpace(contentType))
            {
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            }
        }

        private static async Task<ResponseInfo> CreateResponseInfoAsync(HttpResponseMessage response, bool readResponseText,
            CancellationToken cancellationToken)
        {
            WebHeaderCollection headers = new WebHeaderCollection();

            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Headers.Concat(response.Content.Headers))
            {
                headers[header.Key] = string.Join(", ", header.Value);
            }

            return new ResponseInfo()
            {
                StatusCode = response.StatusCode,
                StatusDescription = response.ReasonPhrase,
                ResponseURL = response.RequestMessage?.RequestUri?.OriginalString,
                Headers = headers,
                ResponseText = readResponseText ? await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false) : null
            };
        }

        private string ProcessError(Exception exception, string requestURL, ResponseInfo responseInfo = null)
        {
            if (exception == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Localization.Strings.Uploader_Error_message);
            sb.AppendLine(exception.Message);

            if (!string.IsNullOrEmpty(requestURL))
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Strings.Uploader_Request_URL);
                sb.AppendLine(requestURL);
            }

            if (responseInfo != null)
            {
                sb.AppendLine();
                sb.AppendLine(Localization.Strings.ResponseWindow_Status_code + ":");
                sb.AppendLine($"({(int)responseInfo.StatusCode}) {responseInfo.StatusDescription}");

                if (!string.IsNullOrEmpty(responseInfo.ResponseURL) && !string.Equals(requestURL, responseInfo.ResponseURL, StringComparison.Ordinal))
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Strings.ResponseWindow_Response_URL + ":");
                    sb.AppendLine(responseInfo.ResponseURL);
                }

                if (responseInfo.Headers != null)
                {
                    sb.AppendLine();
                    sb.AppendLine(Localization.Strings.ResponseWindow_Headers + ":");
                    sb.AppendLine(responseInfo.Headers.ToString().TrimEnd());
                }

                sb.AppendLine();
                sb.AppendLine(Localization.Strings.ResponseWindow_Response_text + ":");
                sb.AppendLine(responseInfo.ResponseText);
            }

            sb.AppendLine();
            sb.AppendLine(Localization.Strings.Uploader_Stack_trace);
            sb.Append(exception.StackTrace);

            string errorText = sb.ToString();
            Errors ??= new UploaderErrorManager();
            Errors.Add(errorText);
            DebugHelper.WriteLine("Error:\r\n" + errorText);
            return responseInfo?.ResponseText;
        }

        private static HttpRequestException CreateStatusCodeException(HttpResponseMessage response)
        {
            return new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.ReasonPhrase}).",
                null, response.StatusCode);
        }

        private TimeoutException CreateTimeoutException(OperationCanceledException exception)
        {
            string message = RequestTimeout == Timeout.InfiniteTimeSpan
                ? "The HTTP request timed out."
                : $"The request timed out after {RequestTimeout}.";
            return new TimeoutException(message, exception);
        }

        private CancellationToken GetEffectiveCancellationToken(CancellationToken cancellationToken)
        {
            return cancellationToken.CanBeCanceled ? cancellationToken : CurrentCancellationToken;
        }

        private CancellationTokenSource CreateTimeoutCancellation(CancellationToken cancellationToken)
        {
            if (RequestTimeout == Timeout.InfiniteTimeSpan)
            {
                return null;
            }

            if (RequestTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(RequestTimeout), "The request timeout must be positive or infinite.");
            }

            CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellation.CancelAfter(RequestTimeout);
            return cancellation;
        }

        private bool BeginRequestScope()
        {
            if (IsUploading)
            {
                return false;
            }

            StopUploadRequested = false;
            IsUploading = true;
            return true;
        }

        private void EndRequestScope(bool ownsUploadingState)
        {
            if (ownsUploadingState)
            {
                IsUploading = false;
            }
        }

        private static long GetStreamLength(Stream stream)
        {
            if (!stream.CanSeek)
            {
                throw new NotSupportedException("Uploader request streams must be seekable so their content length can be sent without buffering.");
            }

            return stream.Length;
        }
    }
}
