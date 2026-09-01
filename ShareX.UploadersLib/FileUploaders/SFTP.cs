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

using Renci.SshNet;
using Renci.SshNet.Common;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class SFTP : FileUploader, IDisposable
    {
        public FTPAccount Account { get; private set; }

        public bool IsValidAccount => (!string.IsNullOrEmpty(Account.Keypath) && File.Exists(Account.Keypath)) || !string.IsNullOrEmpty(Account.Password);

        public bool IsConnected => client != null && client.IsConnected;

        private SftpClient client;

        public SFTP(FTPAccount account)
        {
            Account = account;
        }

        protected override Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            UploadResult result = new UploadResult();

            string subFolderPath = Account.GetSubFolderPath();
            string path = URLHelpers.CombineURL(subFolderPath, fileName);
            string url = Account.GetUriPath(fileName, subFolderPath);

            OnEarlyURLCopyRequested(url);

            try
            {
                IsUploading = true;

                bool uploadResult = UploadStream(stream, path, true);

                if (uploadResult && !StopUploadRequested && !IsError)
                {
                    result.URL = url;
                }
            }
            finally
            {
                Dispose();

                IsUploading = false;
            }

            return Task.FromResult(result);
        }

        public override void StopUpload()
        {
            if (IsUploading && !StopUploadRequested)
            {
                StopUploadRequested = true;

                try
                {
                    Disconnect();
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }

        public bool Connect()
        {
            if (client == null)
            {
                if (!string.IsNullOrEmpty(Account.Keypath))
                {
                    if (!File.Exists(Account.Keypath))
                    {
                        throw new FileNotFoundException(Localization.Strings.SFTP_Key_file_not_found, Account.Keypath);
                    }

                    PrivateKeyFile keyFile;

                    if (string.IsNullOrEmpty(Account.Passphrase))
                    {
                        keyFile = new PrivateKeyFile(Account.Keypath);
                    }
                    else
                    {
                        keyFile = new PrivateKeyFile(Account.Keypath, Account.Passphrase);
                    }

                    client = new SftpClient(Account.Host, Account.Port, Account.Username, keyFile);
                }
                else if (!string.IsNullOrEmpty(Account.Password))
                {
                    client = new SftpClient(Account.Host, Account.Port, Account.Username, Account.Password);
                }

                if (client != null)
                {
                    client.BufferSize = (uint)BufferSize;
                }
            }

            if (client != null && !client.IsConnected)
            {
                client.Connect();
            }

            return IsConnected;
        }

        public void Disconnect()
        {
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
            }
        }

        public void ChangeDirectory(string path, bool autoCreateDirectory = false)
        {
            if (Connect())
            {
                try
                {
                    client.ChangeDirectory(path);
                }
                catch (SftpPathNotFoundException) when (autoCreateDirectory)
                {
                    CreateDirectory(path, true);
                    ChangeDirectory(path);
                }
            }
        }

        public bool DirectoryExists(string path)
        {
            if (Connect())
            {
                return client.Exists(path);
            }

            return false;
        }

        public void CreateDirectory(string path, bool createMultiDirectory = false)
        {
            if (Connect())
            {
                try
                {
                    client.CreateDirectory(path);

                    DebugHelper.WriteLine($"SFTP directory created: {path}");
                }
                catch (SftpPathNotFoundException) when (createMultiDirectory)
                {
                    CreateMultiDirectory(path);
                }
                catch (SftpPermissionDeniedException)
                {
                }
            }
        }

        public List<string> CreateMultiDirectory(string path)
        {
            List<string> directoryList = new List<string>();

            List<string> paths = URLHelpers.GetPaths(path);

            foreach (string directory in paths)
            {
                if (!DirectoryExists(directory))
                {
                    CreateDirectory(directory);
                    directoryList.Add(directory);
                }
            }

            return directoryList;
        }

        private bool UploadStream(Stream stream, string remotePath, bool autoCreateDirectory = false)
        {
            if (Connect())
            {
                try
                {
                    long fileSize = stream.CanSeek ? stream.Length : -1;
                    ProgressManager progress = fileSize > 0 ? new ProgressManager(fileSize) : null;
                    ulong lastUploadedBytes = 0;
                    object progressLock = new object();

                    // We have to use a lock here because UploadFile fires progress callbacks concurrently from multiple threads.
                    client.UploadFile(stream, remotePath, canOverride: true, uploadedBytes =>
                    {
                        if (StopUploadRequested)
                        {
                            Disconnect();
                            return;
                        }

                        if (AllowReportProgress && progress != null)
                        {
                            lock (progressLock)
                            {
                                long delta = (long)(uploadedBytes - lastUploadedBytes);

                                if (delta > 0)
                                {
                                    lastUploadedBytes = uploadedBytes;

                                    if (progress.UpdateProgress(delta))
                                    {
                                        OnProgressChanged(progress);
                                    }
                                }
                            }
                        }
                    });

                    return !StopUploadRequested;
                }
                catch (SftpPathNotFoundException) when (autoCreateDirectory)
                {
                    // Happens when directory not exist, create directory and retry uploading

                    CreateDirectory(URLHelpers.GetDirectoryPath(remotePath), true);
                    return UploadStream(stream, remotePath);
                }
                catch (NullReferenceException)
                {
                    // Happens when disconnect while uploading
                }
            }

            return false;
        }

        public void Dispose()
        {
            if (client != null)
            {
                try
                {
                    client.Dispose();
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }
        }
    }
}
