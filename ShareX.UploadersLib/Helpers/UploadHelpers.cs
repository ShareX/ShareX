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

using Microsoft.Win32;
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
    internal static class UploadHelpers
    {
        public const string ContentTypeMultipartFormData = "multipart/form-data";
        public const string ContentTypeJSON = "application/json";
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
            string format = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", boundary, name, value);
            return Encoding.UTF8.GetBytes(format);
        }

        public static byte[] MakeInputContent(string boundary, Dictionary<string, string> contents, bool isFinal = true)
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

        public static byte[] MakeFileInputContentOpen(string boundary, string fileFormName, string fileName)
        {
            string format = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                boundary, fileFormName, fileName, GetMimeType(fileName));

            return Encoding.UTF8.GetBytes(format);
        }

        public static byte[] MakeFileInputContentOpen(string boundary, string fileFormName, string fileName, string metadata)
        {
            string format = "";

            if (metadata != null)
            {
                format = string.Format("--{0}\r\nContent-Type: {1}; charset=UTF-8\r\n\r\n{2}\r\n\r\n", boundary, ContentTypeJSON, metadata);
            }
            else
            {
                format = string.Format("--{0}\r\nContent-Type: {1}\r\n\r\n", boundary, GetMimeType(fileName));
            }

            return Encoding.UTF8.GetBytes(format);
        }

        public static byte[] MakeFileInputContentClose(string boundary)
        {
            return Encoding.UTF8.GetBytes(string.Format("\r\n--{0}--\r\n", boundary));
        }

        public static byte[] MakeFinalBoundary(string boundary)
        {
            return Encoding.UTF8.GetBytes(string.Format("--{0}--\r\n", boundary));
        }

        public static string ResponseToString(WebResponse response, ResponseType responseType = ResponseType.Text)
        {
            if (response == null)
            {
                return null;
            }

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
                default:
                    return null;
            }
        }

        public static NameValueCollection CreateAuthenticationHeader(string username, string password)
        {
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            NameValueCollection headers = new NameValueCollection();
            headers["Authorization"] = "Basic " + authInfo;
            return headers;
        }

        public static string GetMimeType(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string ext = Path.GetExtension(fileName).ToLowerInvariant();

                if (!string.IsNullOrEmpty(ext))
                {
                    string mimeType = MimeTypes.GetMimeType(ext);

                    if (!string.IsNullOrEmpty(mimeType))
                    {
                        return mimeType;
                    }

                    mimeType = RegistryHelpers.GetRegistryValue(ext, "Content Type", RegistryHive.ClassesRoot);

                    if (!string.IsNullOrEmpty(mimeType))
                    {
                        return mimeType;
                    }
                }
            }

            return MimeTypes.DefaultMimeType;
        }

        /// <summary>
        /// Returns whether the HttpWebResponse was successful (has a 2xx status code).
        /// </summary>
        /// <param name="response">The HttpWebResponse to check.</param>
        /// <returns>true if 2xx status code, otherwise false.</returns>
        public static bool IsSuccessfulResponse(HttpWebResponse response)
        {
            return int.TryParse(response.StatusCode.ToString(), out int rc) && (rc >= 200 && rc <= 299);
        }
    }
}