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
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    public class HashChecker
    {
        public bool IsWorking { get; private set; }

        public delegate void ProgressChanged(float progress);
        public event ProgressChanged FileCheckProgressChanged;

        private CancellationTokenSource cts;

        private void OnProgressChanged(float percentage)
        {
            FileCheckProgressChanged?.Invoke(percentage);
        }

        public async Task<string> Start(string filePath, HashType hashType)
        {
            string result = null;

            if (!IsWorking && !string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                IsWorking = true;

                Progress<float> progress = new Progress<float>(OnProgressChanged);

                using (cts = new CancellationTokenSource())
                {
                    result = await Task.Run(() =>
                    {
                        try
                        {
                            return HashCheckThread(filePath, hashType, progress, cts.Token);
                        }
                        catch (OperationCanceledException)
                        {
                        }

                        return null;
                    }, cts.Token);
                }

                IsWorking = false;
            }

            return result;
        }

        public void Stop()
        {
            cts?.Cancel();
        }

        private string HashCheckThread(string filePath, HashType hashType, IProgress<float> progress, CancellationToken ct)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (HashAlgorithm hash = GetHashAlgorithm(hashType))
            using (CryptoStream cs = new CryptoStream(stream, hash, CryptoStreamMode.Read))
            {
                long bytesRead, totalRead = 0;
                byte[] buffer = new byte[8192];
                Stopwatch timer = Stopwatch.StartNew();

                while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0 && !ct.IsCancellationRequested)
                {
                    totalRead += bytesRead;

                    if (timer.ElapsedMilliseconds > 200)
                    {
                        float percentage = (float)totalRead / stream.Length * 100;
                        progress.Report(percentage);

                        timer.Reset();
                        timer.Start();
                    }
                }

                if (ct.IsCancellationRequested)
                {
                    progress.Report(0);

                    ct.ThrowIfCancellationRequested();
                }
                else
                {
                    progress.Report(100);

                    string[] hex = TranslatorHelper.BytesToHexadecimal(hash.Hash);
                    return string.Concat(hex);
                }
            }

            return null;
        }

        public static HashAlgorithm GetHashAlgorithm(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.CRC32:
                    return new Crc32();
                case HashType.MD5:
                    return new MD5CryptoServiceProvider();
                case HashType.SHA1:
                    return new SHA1CryptoServiceProvider();
                case HashType.SHA256:
                    return new SHA256CryptoServiceProvider();
                case HashType.SHA384:
                    return new SHA384CryptoServiceProvider();
                case HashType.SHA512:
                    return new SHA512CryptoServiceProvider();
            }

            return null;
        }
    }
}