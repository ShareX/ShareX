#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using Starksoft.Net.Ftp;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UploadersLib.HelperClasses;

namespace UploadersLib
{
    public sealed class FTP : IDisposable
    {
        public event Uploader.ProgressEventHandler ProgressChanged;

        public FTPAccount Account { get; set; }
        public FtpClient Client { get; set; }
        public bool AutoReconnect { get; set; }

        private ProgressManager progress;

        public FTP(FTPAccount account, int bufferSize = 8192)
        {
            Account = account;
            Client = new FtpClient(account.Host, account.Port);
            Client.TcpBufferSize = bufferSize;

            if (account.Protocol == FTPProtocol.FTP || account.FtpsSecurityProtocol == FtpSecurityProtocol.None)
            {
                Client.SecurityProtocol = (Starksoft.Net.Ftp.FtpSecurityProtocol)FtpSecurityProtocol.None;
            }
            else
            {
                Client.SecurityProtocol = (Starksoft.Net.Ftp.FtpSecurityProtocol)account.FtpsSecurityProtocol;

                if (!string.IsNullOrEmpty(account.FtpsCertLocation) && File.Exists(account.FtpsCertLocation))
                {
                    Client.SecurityCertificates.Add(X509Certificate.CreateFromSignedFile(account.FtpsCertLocation));
                }
                else
                {
                    Client.ValidateServerCertificate += (sender, e) => e.IsCertificateValid = true;
                }
            }

            Client.DataTransferMode = account.IsActive ? TransferMode.Active : TransferMode.Passive;

            if (ProxyInfo.Current != null)
            {
                IProxyClient proxy = ProxyInfo.Current.GetProxyClient();

                if (proxy != null)
                {
                    Client.Proxy = proxy;
                }
            }

            Client.TransferProgress += OnTransferProgressChanged;
            Client.ConnectionClosed += Client_ConnectionClosed;
        }

        private void OnTransferProgressChanged(object sender, TransferProgressEventArgs e)
        {
            if (ProgressChanged != null)
            {
                progress.UpdateProgress(e.BytesTransferred);
                ProgressChanged(progress);
            }
        }

        private void Client_ConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            if (AutoReconnect)
            {
                Connect();
            }
        }

        public bool Connect(string username, string password)
        {
            if (!Client.IsConnected && !string.IsNullOrEmpty(password))
            {
                Client.Open(username, password);
            }
            return Client.IsConnected;
        }

        public bool Connect()
        {
            return Connect(Account.Username, Account.Password);
        }

        public void Disconnect()
        {
            if (Client != null && Client.IsConnected)
            {
                Client.Close();
            }
        }

        public void UploadData(Stream stream, string remotePath)
        {
            if (Connect())
            {
                progress = new ProgressManager(stream.Length);

                try
                {
                    Client.PutFile(stream, remotePath, FileAction.Create);
                }
                catch (Exception e)
                {
                    if (e.InnerException.Message.Contains("No such file or directory"))
                    {
                        MakeMultiDirectory(FTPHelpers.GetDirectoryName(remotePath));
                        Client.PutFile(stream, remotePath, FileAction.Create);
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

        public void StopUpload()
        {
            if (Client != null && Client.IsConnected)
            {
                Client.Abort();
            }
        }

        public void DownloadFile(string remotePath, string localPath)
        {
            Connect();
            Client.GetFile(remotePath, localPath, FileAction.Create);
        }

        public void DownloadFile(string remotePath, Stream outStream)
        {
            Connect();
            Client.GetFile(remotePath, outStream, false);
        }

        public void DownloadFiles(IEnumerable<FtpItem> files, string localPath)
        {
            foreach (FtpItem file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name))
                {
                    if (file.ItemType == FtpItemType.Directory)
                    {
                        FtpItemCollection newFiles = GetDirList(file.FullPath);
                        string directoryPath = Path.Combine(localPath, file.Name);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        DownloadFiles(newFiles, directoryPath);
                    }
                    else if (file.ItemType == FtpItemType.File)
                    {
                        DownloadFile(file.FullPath, Path.Combine(localPath, file.Name));
                    }
                }
            }
        }

        public FtpItemCollection GetDirList(string remotePath)
        {
            Connect();
            return Client.GetDirList(remotePath);
        }

        public bool ChangeDirectory(string remotePath, bool autoCreateDirectory = false)
        {
            if (Connect())
            {
                remotePath = FTPHelpers.AddSlash(remotePath, FTPHelpers.SlashType.Prefix);

                try
                {
                    Client.ChangeDirectory(remotePath);
                    return true;
                }
                catch (Exception e)
                {
                    if (autoCreateDirectory && e.Message.StartsWith("Could not change working directory to"))
                    {
                        MakeMultiDirectory(remotePath);
                        Client.ChangeDirectory(remotePath);
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
                    Client.MakeDirectory(remotePath);
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
            Client.Rename(fromRemotePath, toRemotePath);
        }

        public void DeleteFile(string remotePath)
        {
            Connect();
            Client.DeleteFile(remotePath);
        }

        public void DeleteFiles(IEnumerable<FtpItem> files)
        {
            foreach (FtpItem file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.Name))
                {
                    if (file.ItemType == FtpItemType.Directory)
                    {
                        DeleteDirectory(file.FullPath);
                    }
                    else if (file.ItemType == FtpItemType.File)
                    {
                        DeleteFile(file.FullPath);
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

            FtpItemCollection files = GetDirList(remotePath);

            foreach (FtpItem file in files)
            {
                if (file.ItemType == FtpItemType.Directory)
                {
                    DeleteDirectory(file.FullPath);
                }
                else
                {
                    DeleteFile(file.FullPath);
                }
            }

            Client.DeleteDirectory(remotePath);
        }

        public bool SendCommand(string command)
        {
            Connect();

            try
            {
                Client.Quote(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            Disconnect();
            Client.Dispose();
        }
    }
}