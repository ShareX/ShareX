#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public static class WebHelpers
    {
        public static async Task DownloadFileAsync(string url, string filePath)
        {
            if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(filePath))
            {
                FileHelpers.CreateDirectoryFromFilePath(filePath);

                HttpClient client = HttpClientFactory.Create();

                using (HttpResponseMessage responseMessage = await client.GetAsync(url))
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await responseStream.CopyToAsync(fileStream);
                        }
                    }
                }
            }
        }

        public static async Task<string> DownloadStringAsync(string url)
        {
            string response = null;

            if (!string.IsNullOrEmpty(url))
            {
                HttpClient client = HttpClientFactory.Create();

                using (HttpResponseMessage responseMessage = await client.GetAsync(url))
                {
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        response = await responseMessage.Content.ReadAsStringAsync();
                    }
                }
            }

            return response;
        }

        public static async Task<Bitmap> DownloadImageAsync(string url)
        {
            Bitmap bmp = null;

            if (!string.IsNullOrEmpty(url))
            {
                HttpClient client = HttpClientFactory.Create();

                using (HttpResponseMessage responseMessage = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (responseMessage.IsSuccessStatusCode && responseMessage.Content.Headers.ContentType != null)
                    {
                        string mediaType = responseMessage.Content.Headers.ContentType.MediaType;

                        string[] supportedImageTypes = new string[]
                        {
                            "image/png",
                            "image/jpeg",
                            "image/gif",
                            "image/bmp",
                            "image/tiff"
                        };

                        if (supportedImageTypes.Contains(mediaType, StringComparer.OrdinalIgnoreCase))
                        {
                            byte[] data = await responseMessage.Content.ReadAsByteArrayAsync();
                            MemoryStream memoryStream = new MemoryStream(data);

                            try
                            {
                                bmp = new Bitmap(memoryStream);
                            }
                            catch
                            {
                                memoryStream.Dispose();
                            }
                        }
                    }
                }
            }

            return bmp;
        }

        public static async Task<string> GetFileNameFromWebServerAsync(string url)
        {
            string fileName = null;

            if (!string.IsNullOrEmpty(url))
            {
                HttpClient client = HttpClientFactory.Create();

                using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Head, url))
                using (HttpResponseMessage responseMessage = await client.SendAsync(requestMessage))
                {
                    if (responseMessage.Content.Headers.ContentDisposition != null)
                    {
                        string contentDisposition = responseMessage.Content.Headers.ContentDisposition.ToString();

                        if (!string.IsNullOrEmpty(contentDisposition))
                        {
                            string fileNameMarker = "filename=\"";
                            int beginIndex = contentDisposition.IndexOf(fileNameMarker, StringComparison.OrdinalIgnoreCase);
                            contentDisposition = contentDisposition.Substring(beginIndex + fileNameMarker.Length);
                            int fileNameLength = contentDisposition.IndexOf("\"");
                            fileName = contentDisposition.Substring(0, fileNameLength);
                        }
                    }
                }
            }

            return fileName;
        }

        public static int GetRandomUnusedPort()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 0);

            try
            {
                listener.Start();
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}