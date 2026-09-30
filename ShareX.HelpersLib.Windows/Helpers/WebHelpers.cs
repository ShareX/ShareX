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

using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace ShareX.HelpersLib
{
    // Windows-only members of WebHelpers, kept in the same namespace so existing call sites keep compiling.
    public static class WebHelpersWindows
    {
        extension(WebHelpers)
        {
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

                            if (MimeTypes.IsImageMimeType(mediaType))
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

            public static Bitmap DataURLToImage(string url)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    Match match = Regex.Match(url, @"^data:(?<mediaType>[\w\/]+);base64,(?<data>.+)$", RegexOptions.IgnoreCase);

                    if (match.Success)
                    {
                        string mediaType = match.Groups["mediaType"].Value;

                        if (MimeTypes.IsImageMimeType(mediaType))
                        {
                            string data = match.Groups["data"].Value;

                            if (!string.IsNullOrEmpty(data))
                            {
                                try
                                {
                                    byte[] dataBytes = Convert.FromBase64String(data);

                                    using (MemoryStream ms = new MemoryStream(dataBytes))
                                    {
                                        return new Bitmap(ms);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                return null;
            }
        }
    }
}
