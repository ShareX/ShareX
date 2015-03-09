#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace ShareX.HelpersLib
{
    public class HashCheck : IDisposable
    {
        public string FilePath { get; private set; }
        public HashType HashType { get; private set; }
        public bool IsWorking { get; private set; }

        public delegate void ProgressChanged(float progress);

        public event ProgressChanged FileCheckProgressChanged;

        public delegate void Completed(string result, bool cancelled);

        public event Completed FileCheckCompleted;

        private BackgroundWorker bw;

        private bool disposed = false;

        public HashCheck()
        {
            bw = new BackgroundWorker();
            bw.DoWork += CheckThread;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (bw != null)
                    {
                        bw.DoWork -= CheckThread;
                        bw.ProgressChanged -= bw_ProgressChanged;
                        bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
                        bw.Dispose();
                    }

                    disposed = true;
                }
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (FileCheckProgressChanged != null)
            {
                FileCheckProgressChanged((float)e.UserState);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsWorking = false;

            if (FileCheckCompleted != null)
            {
                string result = string.Empty;

                if (!e.Cancelled)
                {
                    result = (string)e.Result;
                }

                FileCheckCompleted(result, e.Cancelled);
            }
        }

        public bool Start(string filePath, HashType hashType)
        {
            FilePath = filePath;
            HashType = hashType;

            if (!IsWorking && !string.IsNullOrEmpty(FilePath) && File.Exists(FilePath))
            {
                IsWorking = true;
                bw.RunWorkerAsync();
                return true;
            }

            return false;
        }

        public void Stop()
        {
            bw.CancelAsync();
        }

        private void CheckThread(object sender, DoWorkEventArgs e)
        {
            using (FileStream stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (HashAlgorithm hash = GetHashAlgorithm(HashType))
                {
                    using (CryptoStream cs = new CryptoStream(stream, hash, CryptoStreamMode.Read))
                    {
                        long bytesRead, totalRead = 0;
                        byte[] buffer = new byte[8192];
                        Stopwatch timer = Stopwatch.StartNew();

                        while ((bytesRead = cs.Read(buffer, 0, buffer.Length)) > 0 && !bw.CancellationPending)
                        {
                            totalRead += bytesRead;

                            if (timer.ElapsedMilliseconds > 200)
                            {
                                float progress = (float)totalRead / stream.Length * 100;
                                bw.ReportProgress(0, progress);
                                timer.Reset();
                                timer.Start();
                            }
                        }

                        if (bw.CancellationPending)
                        {
                            bw.ReportProgress(0, 0f);
                            e.Cancel = true;
                        }
                        else
                        {
                            bw.ReportProgress(0, 100f);
                            string[] hex = TranslatorHelper.BytesToHexadecimal(hash.Hash);
                            e.Result = string.Concat(hex);
                        }
                    }
                }
            }
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
                case HashType.RIPEMD160:
                    return new RIPEMD160Managed();
            }

            return null;
        }
    }
}