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

using ShareX.HelpersLib.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class DownloaderForm : Form
    {
        public delegate void DownloaderInstallEventHandler(string filePath);
        public event DownloaderInstallEventHandler InstallRequested;

        public string URL { get; set; }
        public string Filename { get; set; }
        public string SavePath { get; private set; }
        public IWebProxy Proxy { get; set; }
        public string Changelog { get; set; }
        public string AcceptHeader { get; set; }
        public bool AutoStartDownload { get; set; }
        public InstallType InstallType { get; set; }
        public bool AutoStartInstall { get; set; }
        public DownloaderFormStatus Status { get; private set; }
        public bool RunInstallerInBackground { get; set; }

        private FileDownloader fileDownloader;
        private FileStream fileStream;
        private Rectangle fillRect;

        private DownloaderForm()
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            fillRect = new Rectangle(0, 0, ClientSize.Width, ClientSize.Height);

            UpdateFormSize();
            ChangeStatus(Resources.DownloaderForm_DownloaderForm_Waiting_);

            Status = DownloaderFormStatus.Waiting;

            AutoStartDownload = true;
            InstallType = InstallType.Silent;
            AutoStartInstall = true;
            RunInstallerInBackground = true;
        }

        public DownloaderForm(UpdateChecker updateChecker)
            : this(updateChecker.DownloadURL, updateChecker.Filename)
        {
            Proxy = updateChecker.Proxy;

            if (updateChecker is GitHubUpdateChecker)
            {
                AcceptHeader = "application/octet-stream";
            }
        }

        public DownloaderForm(string url, string filename)
            : this()
        {
            URL = url;
            Filename = filename;
            lblFilename.Text = string.Format(Resources.DownloaderForm_DownloaderForm_Filename___0_, Filename);
        }

        private void UpdaterForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            using (LinearGradientBrush brush = new LinearGradientBrush(fillRect, Color.FromArgb(80, 80, 80), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, fillRect);
            }
        }

        private void DownloaderForm_Shown(object sender, EventArgs e)
        {
            if (AutoStartDownload)
            {
                StartDownload();
            }
        }

        private void btnAction_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Status == DownloaderFormStatus.Waiting)
                {
                    StartDownload();
                }
                else if (Status == DownloaderFormStatus.DownloadCompleted)
                {
                    DialogResult = DialogResult.OK;
                    Install();
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
            }
        }

        public void Install()
        {
            if (Status == DownloaderFormStatus.DownloadCompleted)
            {
                Status = DownloaderFormStatus.InstallStarted;
                btnAction.Enabled = false;
                RunInstallerWithDelay();
                Close();
            }
        }

        // This function will give time for ShareX to close so installer won't tell ShareX is already running
        private void RunInstallerWithDelay(int delay = 1000)
        {
            if (RunInstallerInBackground)
            {
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(delay);
                    RunInstaller();
                });
                thread.Start();
            }
            else
            {
                Hide();
                RunInstaller();
            }
        }

        private void RunInstaller()
        {
            if (InstallType == InstallType.Event)
            {
                OnInstallRequested();
            }
            else
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(SavePath);

                    if (InstallType == InstallType.Silent)
                    {
                        psi.Arguments = "/SILENT";
                    }
                    else if (InstallType == InstallType.VerySilent)
                    {
                        psi.Arguments = "/VERYSILENT";
                    }

                    if (Helpers.IsDefaultInstallDir())
                    {
                        psi.Verb = "runas";
                    }

                    psi.UseShellExecute = true;
                    Process.Start(psi);
                }
                catch { }
            }
        }

        protected void OnInstallRequested()
        {
            if (InstallRequested != null)
            {
                DialogResult = DialogResult.OK;
                InstallRequested(SavePath);
            }
        }

        private void ChangeStatus(string status)
        {
            lblStatus.Text = string.Format(Resources.DownloaderForm_ChangeStatus_Status___0_, status);
        }

        private void ChangeProgress()
        {
            if (fileDownloader != null)
            {
                pbProgress.Value = (int)Math.Round(fileDownloader.DownloadPercentage);
                lblProgress.Text = String.Format(CultureInfo.CurrentCulture, Resources.DownloaderForm_ChangeProgress_Progress,
                    fileDownloader.DownloadPercentage, fileDownloader.DownloadSpeed / 1024, fileDownloader.DownloadedSize / 1024, fileDownloader.FileSize / 1024);
            }
        }

        private void StartDownload()
        {
            if (!string.IsNullOrEmpty(URL) && Status == DownloaderFormStatus.Waiting)
            {
                Status = DownloaderFormStatus.DownloadStarted;
                btnAction.Text = Resources.DownloaderForm_StartDownload_Cancel;

                SavePath = Path.Combine(Path.GetTempPath(), Filename);
                fileStream = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.Read);
                fileDownloader = new FileDownloader(URL, fileStream, Proxy, AcceptHeader);
                fileDownloader.FileSizeReceived += (v1, v2) => ChangeProgress();
                fileDownloader.DownloadStarted += (v1, v2) => ChangeStatus(Resources.DownloaderForm_StartDownload_Downloading_);
                fileDownloader.ProgressChanged += (v1, v2) => ChangeProgress();
                fileDownloader.DownloadCompleted += fileDownloader_DownloadCompleted;
                fileDownloader.ExceptionThrowed += (v1, v2) => ChangeStatus(fileDownloader.LastException.Message);
                fileDownloader.StartDownload();

                ChangeStatus(Resources.DownloaderForm_StartDownload_Getting_file_size_);
            }
        }

        private void UpdateFormSize()
        {
            if (cbShowChangelog.Checked)
            {
                ClientSize = new Size(fillRect.Width, fillRect.Height);
            }
            else
            {
                ClientSize = new Size(fillRect.Width, txtChangelog.Location.Y - 3);
            }
        }

        private void fileDownloader_DownloadCompleted(object sender, EventArgs e)
        {
            ChangeStatus(Resources.DownloaderForm_fileDownloader_DownloadCompleted_Download_completed_);
            Status = DownloaderFormStatus.DownloadCompleted;
            btnAction.Text = Resources.DownloaderForm_fileDownloader_DownloadCompleted_Install;

            if (AutoStartInstall)
            {
                Install();
            }
        }

        private void cbShowChangelog_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFormSize();
        }

        private void UpdaterForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Status == DownloaderFormStatus.DownloadStarted && fileDownloader != null)
            {
                fileDownloader.StopDownload();
            }
        }
    }
}