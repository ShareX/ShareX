#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.FtpClient;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UploadersLib.FileUploaders
{
    public sealed class FTP : FileUploader, IDisposable
    {
        public FTPAccount Account { get; private set; }
        //public bool AutoReconnect { get; set; }

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

            if (account.Protocol == FTPProtocol.FTPS && account.FtpsSecurityProtocol != FtpSecurityProtocol.None)
            {
                client.EncryptionMode = FtpEncryptionMode.Explicit;
                client.DataConnectionEncryption = true;

                if (!string.IsNullOrEmpty(account.FtpsCertLocation) && File.Exists(account.FtpsCertLocation))
                {
                    X509Certificate cert = X509Certificate2.CreateFromSignedFile(Account.FtpsCertLocation);
                    client.ClientCertificates.Add(cert);
                }
                else
                {
                    client.ValidateCertificate += (FtpClient control, FtpSslValidationEventArgs e) =>
                    {
                        if (e.PolicyErrors != SslPolicyErrors.None)
                        {
                            e.Accept = true;
                        }
                    };
                }
            }
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = new UploadResult();

            fileName = Helpers.GetValidURL(fileName);
            string path = Account.GetSubFolderPath(fileName);

            try
            {
                IsUploading = true;
                UploadData(stream, path);
            }
            finally
            {
                IsUploading = false;
            }

            if (!stopUpload && Errors.Count == 0)
            {
                result.URL = Account.GetUriPath(fileName);
            }

            return result;
        }

        public override void StopUpload()
        {
            if (IsUploading && !stopUpload)
            {
                stopUpload = true;
                Disconnect();
            }
        }

        private string GetRemotePath(string filename)
        {
            filename = Helpers.GetValidURL(filename);
            return Account.GetSubFolderPath(filename);
        }

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
            if (client != null && client.IsConnected)
            {
                client.Disconnect();
            }
        }

        public void UploadData(Stream localStream, string remotePath)
        {
            if (Connect())
            {
                try
                {
                    using (Stream remoteStream = client.OpenWrite(remotePath))
                    {
                        TransferData(localStream, remoteStream);
                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException.Message.Contains("No such file or directory"))
                    {
                        MakeMultiDirectory(FTPHelpers.GetDirectoryName(remotePath));

                        using (Stream remoteStream = client.OpenWrite(remotePath))
                        {
                            TransferData(localStream, remoteStream);
                        }
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
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
                        UploadFile(file, Helpers.CombineURL(remotePath, filename));
                    }
                    else if (Directory.Exists(file))
                    {
                        List<string> filesList = new List<string>();
                        filesList.AddRange(Directory.GetFiles(file));
                        filesList.AddRange(Directory.GetDirectories(file));
                        string path = Helpers.CombineURL(remotePath, filename);
                        MakeDirectory(path);
                        UploadFiles(filesList.ToArray(), path);
                    }
                }
            }
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            Connect();

            using (FileStream fs = new FileStream(localPath, FileMode.Create))
            {
                DownloadFile(remotePath, fs);
            }
        }

        public void DownloadFile(string remotePath, Stream localStream)
        {
            Connect();

            using (Stream remoteStream = client.OpenRead(remotePath))
            {
                TransferData(remoteStream, localStream);
            }
        }

        public void DownloadFiles(IEnumerable<FtpListItem> files, string localPath, bool recursive = true)
        {
            Connect();

            foreach (FtpListItem file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name))
                {
                    if (recursive && file.Type == FtpFileSystemObjectType.Directory)
                    {
                        FtpListItem[] newFiles = client.GetListing(file.FullName);
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

        public bool ChangeDirectory(string remotePath, bool autoCreateDirectory = false)
        {
            if (Connect())
            {
                remotePath = FTPHelpers.AddSlash(remotePath, FTPHelpers.SlashType.Prefix);

                try
                {
                    client.SetWorkingDirectory(remotePath);
                    return true;
                }
                catch (Exception e)
                {
                    if (autoCreateDirectory && e.Message.StartsWith("Could not change working directory to"))
                    {
                        MakeMultiDirectory(remotePath);
                        client.SetWorkingDirectory(remotePath);
                        return true;
                    }

                    throw e;
                }
            }

            return false;
        }

        public bool MakeDirectory(string remotePath)
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

        public void MakeMultiDirectory(string remotePath)
        {
            List<string> paths = FTPHelpers.GetPaths(remotePath);

            foreach (string path in paths)
            {
                if (MakeDirectory(path))
                {
                    DebugHelper.WriteLine("FTP MakeDirectory: " + path);
                }
            }
        }

        public void Rename(string fromRemotePath, string toRemotePath)
        {
            Connect();
            client.Rename(fromRemotePath, toRemotePath);
        }

        public void DeleteFile(string remotePath)
        {
            Connect();
            client.DeleteFile(remotePath);
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
            Connect();

            string filename = FTPHelpers.GetFileName(remotePath);
            if (filename == "." || filename == "..")
            {
                return;
            }

            FtpListItem[] files = client.GetListing(remotePath);

            DeleteFiles(files);

            client.DeleteDirectory(remotePath);
        }

        public bool SendCommand(string command)
        {
            Connect();

            try
            {
                client.Execute(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (client != null)
            {
                Disconnect();
                client.Dispose();
            }
        }
    }
}