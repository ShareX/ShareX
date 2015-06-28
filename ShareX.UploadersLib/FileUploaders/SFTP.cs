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

using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class SFTP : FileUploader, IDisposable
    {
        public FTPAccount Account { get; private set; }

        public bool IsValidAccount
        {
            get
            {
                return (!string.IsNullOrEmpty(Account.Keypath) && File.Exists(Account.Keypath)) || !string.IsNullOrEmpty(Account.Password);
            }
        }

        public bool IsConnected
        {
            get
            {
                return client != null && client.IsConnected;
            }
        }

        private SftpClient client;

        public SFTP(FTPAccount account)
        {
            Account = account;
        }

        #region FileUploader methods

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = new UploadResult();

            fileName = Helpers.GetValidURL(fileName);
            string subFolderPath = Account.GetSubFolderPath();
            string path = subFolderPath.CombineURL(fileName);
            bool uploadResult;

            try
            {
                IsUploading = true;
                uploadResult = UploadStream(stream, path);
            }
            finally
            {
                Dispose();
                IsUploading = false;
            }

            if (uploadResult && !StopUploadRequested && !IsError)
            {
                result.URL = Account.GetUriPath(fileName, subFolderPath);
            }

            return result;
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

        #endregion FileUploader methods

        public bool Connect()
        {
            if (client == null)
            {
                if (!string.IsNullOrEmpty(Account.Keypath) && File.Exists(Account.Keypath))
                {
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

        public void ChangeDirectory(string path)
        {
            if (Connect())
            {
                try
                {
                    client.ChangeDirectory(path);
                }
                catch (SftpPathNotFoundException)
                {
                    CreateDirectory(path);
                    ChangeDirectory(path);
                }
            }
        }

        public void CreateDirectory(string path)
        {
            if (Connect())
            {
                try
                {
                    client.CreateDirectory(path);
                }
                catch (SftpPathNotFoundException)
                {
                    CreateMultiDirectory(path);
                }
                catch (SftpPermissionDeniedException)
                {
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

        public List<string> CreateMultiDirectory(string path)
        {
            List<string> directoryList = new List<string>();

            IEnumerable<string> paths = URLHelpers.GetPaths(path).Select(x => x.TrimStart('/'));

            foreach (string directory in paths)
            {
                if (!DirectoryExists(directory))
                {
                    CreateDirectory(directory);
                    DebugHelper.WriteLine("FTP directory created: " + path);
                    directoryList.Add(directory);
                }
            }

            return directoryList;
        }

        private bool UploadStream(Stream stream, string remotePath)
        {
            if (Connect())
            {
                try
                {
                    using (SftpFileStream sftpStream = client.OpenWrite(remotePath))
                    {
                        return TransferData(stream, sftpStream);
                    }
                }
                catch (SftpPathNotFoundException)
                {
                    // Happens when directory not exist, create directory and retry uploading

                    CreateDirectory(URLHelpers.GetDirectoryPath(remotePath));

                    using (SftpFileStream sftpStream = client.OpenWrite(remotePath))
                    {
                        return TransferData(stream, sftpStream);
                    }
                }
                catch (NullReferenceException)
                {
                    // Happens when disconnected while uploading
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