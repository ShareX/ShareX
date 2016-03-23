/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using Greenshot.IniFile;
using Greenshot.Plugin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// HTTP Method to make sure we have the correct method
    /// </summary>
    public enum HTTPMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD
    };

    /// <summary>
    /// Description of NetworkHelper.
    /// </summary>
    public static class NetworkHelper
    {
        private static readonly CoreConfiguration Config = IniConfig.GetIniSection<CoreConfiguration>();

        static NetworkHelper()
        {
            // Disable certificate checking
            ServicePointManager.ServerCertificateValidationCallback +=
            delegate
            {
                return true;
            };
        }

        /// <summary>
        /// Download a uri response as string
        /// </summary>
        /// <param name="uri">An Uri to specify the download location</param>
        /// <returns>string with the file content</returns>
        public static string GetAsString(Uri uri)
        {
            return GetResponseAsString(CreateWebRequest(uri));
        }

        /// <summary>
        /// Download the FavIcon as a Bitmap
        /// </summary>
        /// <param name="baseUri"></param>
        /// <returns>Bitmap with the FavIcon</returns>
        public static Bitmap DownloadFavIcon(Uri baseUri)
        {
            Uri url = new Uri(baseUri, new Uri("favicon.ico"));
            try
            {
                HttpWebRequest request = CreateWebRequest(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (request.HaveResponse)
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                using (Image image = Image.FromStream(responseStream))
                                {
                                    return (image.Height > 16 && image.Width > 16) ? new Bitmap(image, 16, 16) : new Bitmap(image);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Problem downloading the FavIcon from: " + baseUri, e);
            }
            return null;
        }

        /// <summary>
        /// Download the uri into a memorystream, without catching exceptions
        /// </summary>
        /// <param name="url">Of an image</param>
        /// <returns>MemoryStream which is already seeked to 0</returns>
        public static MemoryStream GetAsMemoryStream(string url)
        {
            HttpWebRequest request = CreateWebRequest(url);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                MemoryStream memoryStream = new MemoryStream();
                using (Stream responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        responseStream.CopyTo(memoryStream);
                    }
                    // Make sure it can be used directly
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                return memoryStream;
            }
        }

        /// <summary>
        /// Download the uri to Bitmap
        /// </summary>
        /// <param name="url">Of an image</param>
        /// <returns>Bitmap</returns>
        public static Image DownloadImage(string url)
        {
            try
            {
                string content;
                using (MemoryStream memoryStream = GetAsMemoryStream(url))
                {
                    try
                    {
                        using (Image image = Image.FromStream(memoryStream))
                        {
                            return ImageHelper.Clone(image, PixelFormat.Format32bppArgb);
                        }
                    }
                    catch (Exception)
                    {
                        // If we arrive here, the image loading didn't work, try to see if the response has a http(s) URL to an image and just take this instead.
                        using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8, true))
                        {
                            content = streamReader.ReadLine();
                        }
                        Regex imageUrlRegex = new Regex(@"(http|https)://.*(\.png|\.gif|\.jpg|\.tiff|\.jpeg|\.bmp)");
                        Match match = imageUrlRegex.Match(content);
                        if (match.Success)
                        {
                            using (MemoryStream memoryStream2 = GetAsMemoryStream(match.Value))
                            {
                                using (Image image = Image.FromStream(memoryStream2))
                                {
                                    return ImageHelper.Clone(image, PixelFormat.Format32bppArgb);
                                }
                            }
                        }
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                LOG.Error("Problem downloading the image from: " + url, e);
            }
            return null;
        }

        /// <summary>
        /// Helper method to create a web request with a lot of default settings
        /// </summary>
        /// <param name="uri">string with uri to connect to</param>
        /// <returns>WebRequest</returns>
        public static HttpWebRequest CreateWebRequest(string uri)
        {
            return CreateWebRequest(new Uri(uri));
        }

        /// <summary>
        /// Helper method to create a web request with a lot of default settings
        /// </summary>
        /// <param name="uri">string with uri to connect to</param>
        /// /// <param name="method">Method to use</param>
        /// <returns>WebRequest</returns>
        public static HttpWebRequest CreateWebRequest(string uri, HTTPMethod method)
        {
            return CreateWebRequest(new Uri(uri), method);
        }

        /// <summary>
        /// Helper method to create a web request with a lot of default settings
        /// </summary>
        /// <param name="uri">Uri with uri to connect to</param>
        /// <param name="method">Method to use</param>
        /// <returns>WebRequest</returns>
        public static HttpWebRequest CreateWebRequest(Uri uri, HTTPMethod method)
        {
            HttpWebRequest webRequest = CreateWebRequest(uri);
            webRequest.Method = method.ToString();
            return webRequest;
        }

        /// <summary>
        /// Helper method to create a web request, eventually with proxy
        /// </summary>
        /// <param name="uri">Uri with uri to connect to</param>
        /// <returns>WebRequest</returns>
        public static HttpWebRequest CreateWebRequest(Uri uri)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            if (Config.UseProxy)
            {
                webRequest.Proxy = CreateProxy(uri);
            }
            else
            {
                // BUG-1655: Fix that Greenshot always uses the default proxy even if the "use default proxy" checkbox is unset
                webRequest.Proxy = null;
            }
            // Make sure the default credentials are available
            webRequest.Credentials = CredentialCache.DefaultCredentials;

            // Allow redirect, this is usually needed so that we don't get a problem when a service moves
            webRequest.AllowAutoRedirect = true;
            // Set default timeouts
            webRequest.Timeout = Config.WebRequestTimeout * 1000;
            webRequest.ReadWriteTimeout = Config.WebRequestReadWriteTimeout * 1000;
            return webRequest;
        }

        /// <summary>
        /// Create a IWebProxy Object which can be used to access the Internet
        /// This method will check the configuration if the proxy is allowed to be used.
        /// Usages can be found in the DownloadFavIcon or Jira and Confluence plugins
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>IWebProxy filled with all the proxy details or null if none is set/wanted</returns>
        public static IWebProxy CreateProxy(Uri uri)
        {
            IWebProxy proxyToUse = null;
            if (Config.UseProxy)
            {
                proxyToUse = WebRequest.DefaultWebProxy;
                if (proxyToUse != null)
                {
                    proxyToUse.Credentials = CredentialCache.DefaultCredentials;
                    if (LOG.IsDebugEnabled)
                    {
                        // check the proxy for the Uri
                        if (!proxyToUse.IsBypassed(uri))
                        {
                            Uri proxyUri = proxyToUse.GetProxy(uri);
                            if (proxyUri != null)
                            {
                                LOG.Debug("Using proxy: " + proxyUri + " for " + uri);
                            }
                            else
                            {
                                LOG.Debug("No proxy found!");
                            }
                        }
                        else
                        {
                            LOG.Debug("Proxy bypass for: " + uri);
                        }
                    }
                }
                else
                {
                    LOG.Debug("No proxy found!");
                }
            }
            return proxyToUse;
        }

        /// <summary>
        /// UrlEncodes a string without the requirement for System.Web
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        // [Obsolete("Use System.Uri.EscapeDataString instead")]
        public static string UrlEncode(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                // Sytem.Uri provides reliable parsing, but doesn't encode spaces.
                return Uri.EscapeDataString(text).Replace("%20", "+");
            }
            return null;
        }

        /// <summary>
        /// A wrapper around the EscapeDataString, as the limit is 32766 characters
        /// See: http://msdn.microsoft.com/en-us/library/system.uri.escapedatastring%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="text"></param>
        /// <returns>escaped data string</returns>
        public static string EscapeDataString(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                StringBuilder result = new StringBuilder();
                int currentLocation = 0;
                while (currentLocation < text.Length)
                {
                    string process = text.Substring(currentLocation, Math.Min(16384, text.Length - currentLocation));
                    result.Append(Uri.EscapeDataString(process));
                    currentLocation = currentLocation + 16384;
                }
                return result.ToString();
            }
            return null;
        }

        /// <summary>
        /// UrlDecodes a string without requiring System.Web
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <returns>decoded string</returns>
        public static string UrlDecode(string text)
        {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            text = text.Replace("+", " ");
            return Uri.UnescapeDataString(text);
        }

        /// <summary>
        /// ParseQueryString without the requirement for System.Web
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns>IDictionary string, string</returns>
        public static IDictionary<string, string> ParseQueryString(string queryString)
        {
            IDictionary<string, string> parameters = new SortedDictionary<string, string>();
            // remove anything other than query string from uri
            if (queryString.Contains("?"))
            {
                queryString = queryString.Substring(queryString.IndexOf('?') + 1);
            }
            foreach (string vp in Regex.Split(queryString, "&"))
            {
                if (string.IsNullOrEmpty(vp))
                {
                    continue;
                }
                string[] singlePair = Regex.Split(vp, "=");
                if (parameters.ContainsKey(singlePair[0]))
                {
                    parameters.Remove(singlePair[0]);
                }
                parameters.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
            }
            return parameters;
        }

        /// <summary>
        /// Generate the query paramters
        /// </summary>
        /// <param name="queryParameters">the list of query parameters</param>
        /// <returns>a string with the query parameters</returns>
        public static string GenerateQueryParameters(IDictionary<string, object> queryParameters)
        {
            if (queryParameters == null || queryParameters.Count == 0)
            {
                return string.Empty;
            }

            queryParameters = new SortedDictionary<string, object>(queryParameters);

            StringBuilder sb = new StringBuilder();
            foreach (string key in queryParameters.Keys)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}&", key, UrlEncode(string.Format("{0}", queryParameters[key])));
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// Write Multipart Form Data directly to the HttpWebRequest
        /// </summary>
        /// <param name="webRequest">HttpWebRequest to write the multipart form data to</param>
        /// <param name="postParameters">Parameters to include in the multipart form data</param>
        public static void WriteMultipartFormData(HttpWebRequest webRequest, IDictionary<string, object> postParameters)
        {
            string boundary = String.Format("----------{0:N}", Guid.NewGuid());
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            using (Stream formDataStream = webRequest.GetRequestStream())
            {
                WriteMultipartFormData(formDataStream, boundary, postParameters);
            }
        }

        /// <summary>
        /// Write Multipart Form Data to the HttpListenerResponse
        /// </summary>
        /// <param name="response">HttpListenerResponse</param>
        /// <param name="postParameters">Parameters to include in the multipart form data</param>
        public static void WriteMultipartFormData(HttpListenerResponse response, IDictionary<string, object> postParameters)
        {
            string boundary = String.Format("----------{0:N}", Guid.NewGuid());
            response.ContentType = "multipart/form-data; boundary=" + boundary;
            WriteMultipartFormData(response.OutputStream, boundary, postParameters);
        }

        /// <summary>
        /// Write Multipart Form Data to a Stream, content-type should be set before this!
        /// </summary>
        /// <param name="formDataStream">Stream to write the multipart form data to</param>
        /// <param name="boundary">String boundary for the multipart/form-data</param>
        /// <param name="postParameters">Parameters to include in the multipart form data</param>
        public static void WriteMultipartFormData(Stream formDataStream, string boundary, IDictionary<string, object> postParameters)
        {
            bool needsClrf = false;
            foreach (var param in postParameters)
            {
                // Add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsClrf)
                {
                    formDataStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, Encoding.UTF8.GetByteCount("\r\n"));
                }

                needsClrf = true;

                if (param.Value is IBinaryContainer)
                {
                    IBinaryContainer binaryParameter = (IBinaryContainer)param.Value;
                    binaryParameter.WriteFormDataToStream(boundary, param.Key, formDataStream);
                }
                else
                {
                    string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                        boundary,
                        param.Key,
                        param.Value);
                    formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                }
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(Encoding.UTF8.GetBytes(footer), 0, Encoding.UTF8.GetByteCount(footer));
        }

        /// <summary>
        /// Post the parameters "x-www-form-urlencoded"
        /// </summary>
        /// <param name="webRequest"></param>
        public static void UploadFormUrlEncoded(HttpWebRequest webRequest, IDictionary<string, object> parameters)
        {
            webRequest.ContentType = "application/x-www-form-urlencoded";
            string urlEncoded = NetworkHelper.GenerateQueryParameters(parameters);

            byte[] data = Encoding.UTF8.GetBytes(urlEncoded);
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Log the headers of the WebResponse, if IsDebugEnabled
        /// </summary>
        /// <param name="response">WebResponse</param>
        private static void DebugHeaders(WebResponse response)
        {
            if (!LOG.IsDebugEnabled)
            {
                return;
            }
            LOG.DebugFormat("Debug information on the response from {0} :", response.ResponseUri);
            foreach (string key in response.Headers.AllKeys)
            {
                LOG.DebugFormat("Reponse-header: {0}={1}", key, response.Headers[key]);
            }
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        /// This method should handle the StatusCode better!
        public static string GetResponseAsString(HttpWebRequest webRequest)
        {
            return GetResponseAsString(webRequest, false);
        }

        /// <summary>
        /// Read the response as string
        /// </summary>
        /// <param name="response"></param>
        /// <returns>string or null</returns>
        private static string GetResponseAsString(HttpWebResponse response)
        {
            string responseData = null;
            if (response == null)
            {
                return null;
            }
            using (response)
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    using (StreamReader reader = new StreamReader(responseStream, true))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }
            return responseData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public static string GetResponseAsString(HttpWebRequest webRequest, bool alsoReturnContentOnError)
        {
            string responseData = null;
            HttpWebResponse response = null;
            bool isHttpError = false;
            try
            {
                response = (HttpWebResponse)webRequest.GetResponse();
                LOG.InfoFormat("Response status: {0}", response.StatusCode);
                isHttpError = (int)response.StatusCode >= 300;
                if (isHttpError)
                {
                    LOG.ErrorFormat("HTTP error {0}", response.StatusCode);
                }
                DebugHeaders(response);
                responseData = GetResponseAsString(response);
                if (isHttpError)
                {
                    LOG.ErrorFormat("HTTP response {0}", responseData);
                }
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
                HttpStatusCode statusCode = HttpStatusCode.Unused;
                if (response != null)
                {
                    statusCode = response.StatusCode;
                    LOG.ErrorFormat("HTTP error {0}", statusCode);
                    string errorContent = GetResponseAsString(response);
                    if (alsoReturnContentOnError)
                    {
                        return errorContent;
                    }
                    LOG.ErrorFormat("Content: {0}", errorContent);
                }
                LOG.Error("WebException: ", e);
                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException(e.Message);
                }
                throw;
            }
            finally
            {
                if (response != null)
                {
                    if (isHttpError)
                    {
                        LOG.ErrorFormat("HTTP error {0} with content: {1}", response.StatusCode, responseData);
                    }
                    response.Close();
                }
            }
            return responseData;
        }

        /// <summary>
        /// Get LastModified for a URI
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns>DateTime</returns>
        public static DateTime GetLastModified(Uri uri)
        {
            try
            {
                HttpWebRequest webRequest = CreateWebRequest(uri);
                webRequest.Method = HTTPMethod.HEAD.ToString();
                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    LOG.DebugFormat("RSS feed was updated at {0}", webResponse.LastModified);
                    return webResponse.LastModified;
                }
            }
            catch (Exception wE)
            {
                LOG.WarnFormat("Problem requesting HTTP - HEAD on uri {0}", uri);
                LOG.Warn(wE.Message);
                // Pretend it is old
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>
    /// This interface can be used to pass binary information around, like byte[] or Image
    /// </summary>
    public interface IBinaryContainer
    {
        void WriteFormDataToStream(string boundary, string name, Stream formDataStream);

        void WriteToStream(Stream formDataStream);

        string ToBase64String(Base64FormattingOptions formattingOptions);

        byte[] ToByteArray();

        void Upload(HttpWebRequest webRequest);
    }

    /// <summary>
    /// A container to supply files to a Multi-part form data upload
    /// </summary>
    public class ByteContainer : IBinaryContainer
    {
        private byte[] file;
        private string fileName;
        private string contentType;
        private int fileSize;

        public ByteContainer(byte[] file) : this(file, null)
        {
        }

        public ByteContainer(byte[] file, string filename) : this(file, filename, null)
        {
        }

        public ByteContainer(byte[] file, string filename, string contenttype) : this(file, filename, contenttype, 0)
        {
        }

        public ByteContainer(byte[] file, string filename, string contenttype, int filesize)
        {
            this.file = file;
            fileName = filename;
            contentType = contenttype;
            if (filesize == 0)
            {
                fileSize = file.Length;
            }
            else
            {
                fileSize = filesize;
            }
        }

        /// <summary>
        /// Create a Base64String from the byte[]
        /// </summary>
        /// <returns>string</returns>
        public string ToBase64String(Base64FormattingOptions formattingOptions)
        {
            return Convert.ToBase64String(file, 0, fileSize, formattingOptions);
        }

        /// <summary>
        /// Returns the initial byte-array which was supplied when creating the FileParameter
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ToByteArray()
        {
            return file;
        }

        /// <summary>
        /// Write Multipart Form Data directly to the HttpWebRequest response stream
        /// </summary>
        /// <param name="boundary">Separator</param>
        /// <param name="name">name</param>
        /// <param name="formDataStream">Stream to write to</param>
        public void WriteFormDataToStream(string boundary, string name, Stream formDataStream)
        {
            // Add just the first part of this param, since we will write the file data directly to the Stream
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                boundary,
                name,
                fileName ?? name,
                contentType ?? "application/octet-stream");

            formDataStream.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));

            // Write the file data directly to the Stream, rather than serializing it to a string.
            formDataStream.Write(file, 0, fileSize);
        }

        /// <summary>
        /// A plain "write data to stream"
        /// </summary>
        /// <param name="dataStream">Stream to write to</param>
        public void WriteToStream(Stream dataStream)
        {
            // Write the file data directly to the Stream, rather than serializing it to a string.
            dataStream.Write(file, 0, fileSize);
        }

        /// <summary>
        /// Upload the file to the webrequest
        /// </summary>
        /// <param name="webRequest"></param>
        public void Upload(HttpWebRequest webRequest)
        {
            webRequest.ContentType = contentType;
            webRequest.ContentLength = fileSize;
            using (var requestStream = webRequest.GetRequestStream())
            {
                WriteToStream(requestStream);
            }
        }
    }

    /// <summary>
    /// A container to supply images to a Multi-part form data upload
    /// </summary>
    public class BitmapContainer : IBinaryContainer
    {
        private Bitmap bitmap;
        private SurfaceOutputSettings outputSettings;
        private string fileName;

        public BitmapContainer(Bitmap bitmap, SurfaceOutputSettings outputSettings, string filename)
        {
            this.bitmap = bitmap;
            this.outputSettings = outputSettings;
            fileName = filename;
        }

        /// <summary>
        /// Create a Base64String from the image by saving it to a memory stream and converting it.
        /// Should be avoided if possible, as this uses a lot of memory.
        /// </summary>
        /// <returns>string</returns>
        public string ToBase64String(Base64FormattingOptions formattingOptions)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ImageOutput.SaveToStream(bitmap, null, stream, outputSettings);
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length, formattingOptions);
            }
        }

        /// <summary>
        /// Create a byte[] from the image by saving it to a memory stream.
        /// Should be avoided if possible, as this uses a lot of memory.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ToByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ImageOutput.SaveToStream(bitmap, null, stream, outputSettings);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Write Multipart Form Data directly to the HttpWebRequest response stream
        /// </summary>
        /// <param name="boundary">Separator</param>
        /// <param name="name">Name of the thing/file</param>
        /// <param name="formDataStream">Stream to write to</param>
        public void WriteFormDataToStream(string boundary, string name, Stream formDataStream)
        {
            // Add just the first part of this param, since we will write the file data directly to the Stream
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                boundary,
                name,
                fileName ?? name,
                "image/" + outputSettings.Format.ToString());

            formDataStream.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));
            ImageOutput.SaveToStream(bitmap, null, formDataStream, outputSettings);
        }

        /// <summary>
        /// A plain "write data to stream"
        /// </summary>
        /// <param name="dataStream"></param>
        public void WriteToStream(Stream dataStream)
        {
            // Write the file data directly to the Stream, rather than serializing it to a string.
            ImageOutput.SaveToStream(bitmap, null, dataStream, outputSettings);
        }

        /// <summary>
        /// Upload the image to the webrequest
        /// </summary>
        /// <param name="webRequest"></param>
        public void Upload(HttpWebRequest webRequest)
        {
            webRequest.ContentType = "image/" + outputSettings.Format.ToString();
            using (var requestStream = webRequest.GetRequestStream())
            {
                WriteToStream(requestStream);
            }
        }
    }

    /// <summary>
    /// A container to supply surfaces to a Multi-part form data upload
    /// </summary>
    public class SurfaceContainer : IBinaryContainer
    {
        private ISurface surface;
        private SurfaceOutputSettings outputSettings;
        private string fileName;

        public SurfaceContainer(ISurface surface, SurfaceOutputSettings outputSettings, string filename)
        {
            this.surface = surface;
            this.outputSettings = outputSettings;
            fileName = filename;
        }

        /// <summary>
        /// Create a Base64String from the Surface by saving it to a memory stream and converting it.
        /// Should be avoided if possible, as this uses a lot of memory.
        /// </summary>
        /// <returns>string</returns>
        public string ToBase64String(Base64FormattingOptions formattingOptions)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ImageOutput.SaveToStream(surface, stream, outputSettings);
                return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length, formattingOptions);
            }
        }

        /// <summary>
        /// Create a byte[] from the image by saving it to a memory stream.
        /// Should be avoided if possible, as this uses a lot of memory.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] ToByteArray()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                ImageOutput.SaveToStream(surface, stream, outputSettings);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Write Multipart Form Data directly to the HttpWebRequest response stream
        /// </summary>
        /// <param name="boundary">Multipart separator</param>
        /// <param name="name">Name of the thing</param>
        /// <param name="formDataStream">Stream to write to</param>
        public void WriteFormDataToStream(string boundary, string name, Stream formDataStream)
        {
            // Add just the first part of this param, since we will write the file data directly to the Stream
            string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\";\r\nContent-Type: {3}\r\n\r\n",
                boundary,
                name,
                fileName ?? name,
                "image/" + outputSettings.Format);

            formDataStream.Write(Encoding.UTF8.GetBytes(header), 0, Encoding.UTF8.GetByteCount(header));
            ImageOutput.SaveToStream(surface, formDataStream, outputSettings);
        }

        /// <summary>
        /// A plain "write data to stream"
        /// </summary>
        /// <param name="dataStream"></param>
        public void WriteToStream(Stream dataStream)
        {
            // Write the file data directly to the Stream, rather than serializing it to a string.
            ImageOutput.SaveToStream(surface, dataStream, outputSettings);
        }

        /// <summary>
        /// Upload the Surface as image to the webrequest
        /// </summary>
        /// <param name="webRequest"></param>
        public void Upload(HttpWebRequest webRequest)
        {
            webRequest.ContentType = "image/" + outputSettings.Format.ToString();
            using (var requestStream = webRequest.GetRequestStream())
            {
                WriteToStream(requestStream);
            }
        }
    }
}