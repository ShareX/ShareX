#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ShareX.UploadersLib.HelperClasses
{
    public class TCPClient
    {
        private readonly TcpClient client;
        private Uri url;
        private string boundary, header, footer;
        private byte[] postMethod, headerBytes, request, requestEnd;
        private readonly ImageUploader uploader;

        public TCPClient(ImageUploader imageUploader)
        {
            uploader = imageUploader;
            client = new TcpClient();
        }

        private void PreparePackets(long length)
        {
            //string postData = "?" + string.Join("&", arguments.Select(x => x.Key + "=" + x.Value).ToArray());
            postMethod = Encoding.Default.GetBytes(string.Format("POST {0} HTTP/1.1\r\n", url.AbsolutePath));

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add(HttpRequestHeader.ContentType, "multipart/form-data; boundary=" + boundary);
            headers.Add(HttpRequestHeader.Host, url.DnsSafeHost);
            headers.Add(HttpRequestHeader.ContentLength, (request.Length + length + requestEnd.Length).ToString());
            headers.Add(HttpRequestHeader.Connection, "Keep-Alive");
            headers.Add(HttpRequestHeader.CacheControl, "no-cache");

            headerBytes = headers.ToByteArray();
        }

        private static string GetMimeType(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
            {
                if (codec.FormatID == format.Guid) return codec.MimeType;
            }
            return "image/unknown";
        }

        private void BuildRequests(string fileFormName, string fileName, string fileMimeType, Dictionary<string, string> arguments)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StringBuilder stringRequest = new StringBuilder();

                foreach (KeyValuePair<string, string> argument in arguments)
                {
                    stringRequest.Append(MakeInputContent(argument.Key, argument.Value));
                }

                byte[] bytes = Encoding.Default.GetBytes(stringRequest.ToString());

                stream.Write(bytes, 0, bytes.Length);
                bytes = MakeFileInputContent(fileFormName, fileName, fileMimeType);
                stream.Write(bytes, 0, bytes.Length);
                request = stream.ToArray();
            }
            requestEnd = Encoding.Default.GetBytes("\r\n" + footer + "\r\n");
        }

        private string MakeInputContent(string name, string value)
        {
            return string.Format("{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", header, name, value);
        }

        private byte[] MakeFileInputContent(string name, string filename, string contentType)
        {
            string format = string.Format("{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                header, name, filename, contentType);

            using (MemoryStream stream = new MemoryStream())
            {
                byte[] bytes = Encoding.Default.GetBytes(format);
                stream.Write(bytes, 0, bytes.Length);

                return stream.ToArray();
            }
        }

        public string UploadImage(Image image, string link, string fileFormName, string fileName, Dictionary<string, string> arguments)
        {
            using (image)
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                stream.Position = 0;

                url = new Uri(link);

                boundary = "----------" + FastDateTime.Now.Ticks.ToString("x");
                header = string.Format("--{0}", boundary);
                footer = string.Format("--{0}--", boundary);

                BuildRequests(fileFormName, Path.GetFileName(fileName), GetMimeType(image.RawFormat), arguments);
                PreparePackets(stream.Length);

                return Send(stream, arguments);
            }
        }

        private string Send(Stream stream, Dictionary<string, string> arguments)
        {
            client.Connect(url.DnsSafeHost, 80);

            using (NetworkStream networkStream = client.GetStream())
            {
                networkStream.Write(postMethod, 0, postMethod.Length);
                networkStream.Write(headerBytes, 0, headerBytes.Length);
                networkStream.Write(request, 0, request.Length);

                byte[] buffer = new byte[(int)Math.Min(4096, stream.Length)];

                ProgressManager progress = new ProgressManager();

                using (stream)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    while (bytesRead > 0)
                    {
                        networkStream.Write(buffer, 0, bytesRead);

                        //if (progress.ChangeProgress(stream)) uploader.OnProgressChanged(progress.Progress);

                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
                }

                networkStream.Write(requestEnd, 0, requestEnd.Length);

                using (StreamReader reader = new StreamReader(networkStream))
                    return reader.ReadToEnd();
            }
        }

        private class ProgressManager
        {
            public int Progress;

            public bool ChangeProgress(Stream stream)
            {
                return ChangeProgress(stream.Position, stream.Length);
            }

            public bool ChangeProgress(long position, long length)
            {
                int percentage = (int)((double)position / length * 100);
                if (percentage != Progress)
                {
                    Progress = percentage;
                    return true;
                }
                return false;
            }
        }
    }
}