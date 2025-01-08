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

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class FileDownloader
    {
        public event Action FileSizeReceived;
        public event Action ProgressChanged;

        public string URL { get; set; }
        public string DownloadLocation { get; set; }
        public string AcceptHeader { get; set; }

        public bool IsDownloading { get; private set; }
        public bool IsCanceled { get; private set; }
        public long FileSize { get; private set; } = -1;
        public long DownloadedSize { get; private set; }
        public double DownloadSpeed { get; private set; }

        public double DownloadPercentage
        {
            get
            {
                if (FileSize > 0)
                {
                    return (double)DownloadedSize / FileSize * 100;
                }

                return 0;
            }
        }

        private const int bufferSize = 32768;

        public FileDownloader()
        {
        }

        public FileDownloader(string url, string downloadLocation)
        {
            URL = url;
            DownloadLocation = downloadLocation;
        }

        public async Task<bool> StartDownload()
        {
            if (!IsDownloading && !string.IsNullOrEmpty(URL))
            {
                IsDownloading = true;
                IsCanceled = false;
                FileSize = -1;
                DownloadedSize = 0;
                DownloadSpeed = 0;

                return await DoWork();
            }

            return false;
        }

        public void StopDownload()
        {
            IsCanceled = true;
        }

        private async Task<bool> DoWork()
        {
            try
            {
                HttpClient client = HttpClientFactory.Create();

                using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, URL))
                {
                    if (!string.IsNullOrEmpty(AcceptHeader))
                    {
                        requestMessage.Headers.Accept.ParseAdd(AcceptHeader);
                    }

                    using (HttpResponseMessage responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead))
                    {
                        responseMessage.EnsureSuccessStatusCode();

                        FileSize = responseMessage.Content.Headers.ContentLength ?? -1;

                        FileSizeReceived?.Invoke();

                        if (FileSize > 0)
                        {
                            Stopwatch timer = new Stopwatch();
                            Stopwatch progressEventTimer = new Stopwatch();
                            long speedTest = 0;

                            byte[] buffer = new byte[(int)Math.Min(bufferSize, FileSize)];
                            int bytesRead;

                            using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync())
                            using (FileStream fileStream = new FileStream(DownloadLocation, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                while (DownloadedSize < FileSize && !IsCanceled)
                                {
                                    if (!timer.IsRunning)
                                    {
                                        timer.Start();
                                    }

                                    if (!progressEventTimer.IsRunning)
                                    {
                                        progressEventTimer.Start();
                                    }

                                    bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                                    DownloadedSize += bytesRead;
                                    speedTest += bytesRead;

                                    if (timer.ElapsedMilliseconds > 500)
                                    {
                                        DownloadSpeed = (double)speedTest / timer.ElapsedMilliseconds * 1000;
                                        speedTest = 0;
                                        timer.Reset();
                                    }

                                    if (progressEventTimer.ElapsedMilliseconds > 100)
                                    {
                                        ProgressChanged?.Invoke();

                                        progressEventTimer.Reset();
                                    }
                                }

                                ProgressChanged?.Invoke();
                            }

                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (!IsCanceled)
                {
                    throw e;
                }
            }
            finally
            {
                if (IsCanceled)
                {
                    try
                    {
                        if (File.Exists(DownloadLocation))
                        {
                            File.Delete(DownloadLocation);
                        }
                    }
                    catch
                    {
                    }
                }

                IsDownloading = false;
            }

            return false;
        }
    }
}