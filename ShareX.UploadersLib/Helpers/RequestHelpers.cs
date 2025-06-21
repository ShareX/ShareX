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
using System.Net.Cache;
using System.Text;

namespace ShareX.UploadersLib
{
    internal static class RequestHelpers
    {
        public const string ContentTypeMultipartFormData = "multipart/form-data";
        public const string ContentTypeJSON = "application/json";
        public const string ContentTypeXML = "application/xml";
        public const string ContentTypeURLEncoded = "application/x-www-form-urlencoded";
        public const string ContentTypeOctetStream = "application/octet-stream";

        public static HttpWebRequest CreateWebRequest(HttpMethod method, string url, NameValueCollection headers = null, CookieCollection cookies = null,
            string contentType = null, long contentLength = 0)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            string accept = null;
            string referer = null;
            string userAgent = ShareXResources.UserAgent;

            if (headers != null)
            {
                if (headers["Accept"] != null)
                {
                    accept = headers["Accept"];
                    headers.Remove("Accept");
                }

                if (headers["Content-Length"] != null)
                {
                    if (long.TryParse(headers["Content-Length"], out contentLength))
                    {
                        request.ContentLength = contentLength;
                    }

                    headers.Remove("Content-Length");
                }

                if (headers["Content-Type"] != null)
                {
                    contentType = headers["Content-Type"];
                    headers.Remove("Content-Type");
                }

                if (headers["Cookie"] != null)
                {
                    string cookieHeader = headers["Cookie"];

                    if (cookies == null)
                    {
                        cookies = new CookieCollection();
                    }

                    foreach (string cookie in cookieHeader.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] cookieValues = cookie.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                        if (cookieValues.Length == 2)
                        {
                            cookies.Add(new Cookie(cookieValues[0], cookieValues[1], "/", request.Host.Split(':')[0]));
                        }
                    }

                    headers.Remove("Cookie");
                }

                if (headers["Referer"] != null)
                {
                    referer = headers["Referer"];
                    headers.Remove("Referer");
                }

                if (headers["User-Agent"] != null)
                {
                    userAgent = headers["User-Agent"];
                    headers.Remove("User-Agent");
                }

                request.Headers.Add(headers);
            }

            request.Accept = accept;
            request.ContentType = contentType;
            request.CookieContainer = new CookieContainer();
            if (cookies != null) request.CookieContainer.Add(cookies);
            request.Method = method.ToString();
            IWebProxy proxy = HelpersOptions.CurrentProxy.GetWebProxy();
            if (proxy != null) request.Proxy = proxy;
            request.Referer = referer;
            request.UserAgent = userAgent;

            if (contentLength > 0)
            {
                request.AllowWriteStreamBuffering = HelpersOptions.CurrentProxy.IsValidProxy();

                if (method == HttpMethod.GET)
                {
                    request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                }

                request.ContentLength = contentLength;
                request.Pipelined = false;
                request.Timeout = -1;
            }
            else
            {
                request.KeepAlive = false;
            }

            return request;
        }

        public static string CreateBoundary()
        {
            return new string('-', 20) + DateTime.Now.Ticks.ToString("x");
        }

        public static byte[] MakeInputContent(string boundary, string name, string value)
        {
            string content = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{name}\"\r\n\r\n{value}\r\n";
            return Encoding.UTF8.GetBytes(content);
        }

        public static byte[] MakeInputContent(string boundary, Dictionary<string, string> contents, bool isFinal = true)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (string.IsNullOrEmpty(boundary)) boundary = CreateBoundary();

                if (contents != null)
                {
                    byte[] bytes;

                    foreach (KeyValuePair<string, string> content in contents)
                    {
                        if (!string.IsNullOrEmpty(content.Key))
                        {
                            bytes = MakeInputContent(boundary, content.Key, content.Value);
                            stream.Write(bytes, 0, bytes.Length);
                        }
                    }

                    if (isFinal)
                    {
                        bytes = Encoding.UTF8.GetBytes($"--{boundary}--\r\n");
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }

                return stream.ToArray();
            }
        }

        public static byte[] MakeFileInputContentOpen(string boundary, string fileFormName, string fileName)
        {
            string mimeType = MimeTypes.GetMimeTypeFromFileName(fileName);
            string content = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{fileFormName}\"; filename=\"{fileName}\"\r\nContent-Type: {mimeType}\r\n\r\n";
            return Encoding.UTF8.GetBytes(content);
        }

        public static byte[] MakeRelatedFileInputContentOpen(string boundary, string contentType, string relatedData, string fileName)
        {
            string mimeType = MimeTypes.GetMimeTypeFromFileName(fileName);
            string content = $"--{boundary}\r\nContent-Type: {contentType}\r\n\r\n{relatedData}\r\n\r\n";
            content += $"--{boundary}\r\nContent-Type: {mimeType}\r\n\r\n";
            return Encoding.UTF8.GetBytes(content);
        }

        public static byte[] MakeFileInputContentClose(string boundary)
        {
            return Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
        }

        public static string ResponseToString(WebResponse response)
        {
            if (response != null)
            {
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }

            return null;
        }

        public static NameValueCollection CreateAuthenticationHeader(string username, string password)
        {
            string authorization = TranslatorHelper.TextToBase64(username + ":" + password);
            NameValueCollection headers = new NameValueCollection();
            headers["Authorization"] = "Basic " + authorization;
            return headers;
        }
    }
}