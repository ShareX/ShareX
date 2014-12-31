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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class FTP : FileUploader, IDisposable
    {
        public FTPAccount Account { get; private set; }

        public bool IsConnected
        {
            get
            {
                return client != null && client.IsConnected;
            }
        }

        private FtpClient client;

        public FTP(FTPAccount account)
        {
            Account = account;

            client = new FtpClient()
            {
                Host = Account.Host,
                Port = Account.Port,
                Credentials = new NetworkCredential(Account.Username, Account.Password)
            };

            if (account.IsActive)
            {
                client.DataConnectionType = FtpDataConnectionType.AutoActive;
            }
            else
            {
                client.DataConnectionType = FtpDataConnectionType.AutoPassive;
            }

            if (account.Protocol == FTPProtocol.FTPS)
            {
                switch (Account.FTPSEncryption)
                {
                    default:
                    case FTPSEncryption.Explicit:
                        client.EncryptionMode = FtpEncryptionMode.Explicit;
                        break;
                    case FTPSEncryption.Implicit:
                        client.EncryptionMode = FtpEncryptionMode.Implicit;
                        break;
                }

                client.DataConnectionEncryption = true;

                if (!string.IsNullOrEmpty(account.FTPSCertificateLocation) && File.Exists(account.FTPSCertificateLocation))
                {
                    X509Certificate cert = X509Certificate2.CreateFromSignedFile(Account.FTPSCertificateLocation);
                    client.ClientCertificates.Add(cert);
                }
                else
                {
                    client.ValidateCertificate += (control, e) =>
                    {
                        if (e.PolicyErrors != SslPolicyErrors.None)
                        {
                            e.Accept = true;
                        }
                    };
                }
            }
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
                uploadResult = UploadData(stream, path);
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
            if (!client.IsConnected)
            {
                client.Connect();
            }

            return client.IsConnected;
        }

        public void Disconnect()
        {
            if (client != null)
            {
                client.Disconnect();
            }
        }

        public bool UploadData(Stream localStream, string remotePath)
        {
            if (Connect())
            {
                try
                {
                    using (Stream remoteStream = client.OpenWrite(remotePath))
                    {
                        return TransferData(localStream, remoteStream);
                    }
                }
                catch (FtpCommandException e)
                {
                    // Probably directory not exist, try creating it
                    if (e.CompletionCode == "550" || e.CompletionCode == "553")
                    {
                        CreateMultiDirectory(URLHelpers.GetDirectoryPath(remotePath));

                        using (Stream remoteStream = client.OpenWrite(remotePath))
                        {
                            return TransferData(localStream, remoteStream);
                        }
                    }

                    throw e;
                }
            }

            return false;
        }

        public void UploadData(byte[] data, string remotePath)
        {
            using (MemoryStream stream = new MemoryStream(data, false))
            {
                UploadData(stream, remotePath);
            }
        }

        public void UploadFile(string localPath, string remotePath)
        {
            using (FileStream stream = new FileStream(localPath, FileMode.Open))
            {
                UploadData(stream, remotePath);
            }
        }

        public void UploadImage(Image image, string remotePath)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                UploadData(stream, remotePath);
            }
        }

        public void UploadText(string text, string remotePath)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(text), false))
            {
                UploadData(stream, remotePath);
            }
        }

        public void UploadFiles(string[] localPaths, string remotePath)
        {
            foreach (string file in localPaths)
            {
                if (!string.IsNullOrEmpty(file))
                {
                    string filename = Path.GetFileName(file);

                    if (File.Exists(file))
                    {
                        UploadFile(file, URLHelpers.CombineURL(remotePath, filename));
                    }
                    else if (Directory.Exists(file))
                    {
                        List<string> filesList = new List<string>();
                        filesList.AddRange(Directory.GetFiles(file));
                        filesList.AddRange(Directory.GetDirectories(file));
                        string path = URLHelpers.CombineURL(remotePath, filename);
                        CreateDirectory(path);
                        UploadFiles(filesList.ToArray(), path);
                    }
                }
            }
        }

        public void DownloadFile(string remotePath, Stream localStream)
        {
            if (Connect())
            {
                using (Stream remoteStream = client.OpenRead(remotePath))
                {
                    TransferData(remoteStream, localStream);
                }
            }
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            using (FileStream fs = new FileStream(localPath, FileMode.Create))
            {
                DownloadFile(remotePath, fs);
            }
        }

        public void DownloadFiles(IEnumerable<FtpListItem> files, string localPath, bool recursive = true)
        {
            foreach (FtpListItem file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name))
                {
                    if (recursive && file.Type == FtpFileSystemObjectType.Directory)
                    {
                        FtpListItem[] newFiles = GetListing(file.FullName);
                        string directoryPath = Path.Combine(localPath, file.Name);

                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        DownloadFiles(newFiles, directoryPath);
                    }
                    else if (file.Type == FtpFileSystemObjectType.File)
                    {
                        string filePath = Path.Combine(localPath, file.Name);
                        DownloadFile(file.FullName, filePath);
                    }
                }
            }
        }

        public FtpListItem[] GetListing(string remotePath)
        {
            return client.GetListing(remotePath);
        }

        public bool DirectoryExists(string remotePath)
        {
            if (Connect())
            {
                return client.DirectoryExists(remotePath);
            }

            return false;
        }

        public bool CreateDirectory(string remotePath)
        {
            if (Connect())
            {
                try
                {
                    client.CreateDirectory(remotePath);
                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
                }
            }

            return false;
        }

        public List<string> CreateMultiDirectory(string remotePath)
        {
            List<string> paths = URLHelpers.GetPaths(remotePath);

            foreach (string path in paths)
            {
                if (CreateDirectory(path))
                {
                    DebugHelper.WriteLine("FTP directory created: " + path);
                }
            }

            return paths;
        }

        public void Rename(string fromRemotePath, string toRemotePath)
        {
            if (Connect())
            {
                client.Rename(fromRemotePath, toRemotePath);
            }
        }

        public void DeleteFile(string remotePath)
        {
            if (Connect())
            {
                client.DeleteFile(remotePath);
            }
        }

        public void DeleteFiles(IEnumerable<FtpListItem> files)
        {
            foreach (FtpListItem file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name))
                {
                    if (file.Type == FtpFileSystemObjectType.Directory)
                    {
                        DeleteDirectory(file.FullName);
                    }
                    else if (file.Type == FtpFileSystemObjectType.File)
                    {
                        DeleteFile(file.FullName);
                    }
                }
            }
        }

        public void DeleteDirectory(string remotePath)
        {
            if (Connect())
            {
                string filename = URLHelpers.GetFileName(remotePath);
                if (filename == "." || filename == "..")
                {
                    return;
                }

                FtpListItem[] files = GetListing(remotePath);

                DeleteFiles(files);

                client.DeleteDirectory(remotePath);
            }
        }

        public bool SendCommand(string command)
        {
            if (Connect())
            {
                try
                {
                    client.Execute(command);
                    return true;
                }
                catch (Exception e)
                {
                    DebugHelper.WriteException(e);
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