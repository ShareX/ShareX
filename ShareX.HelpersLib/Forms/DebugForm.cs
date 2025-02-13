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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class DebugForm : Form
    {
        private static DebugForm instance;

        public delegate void EventHandler(string log);
        public event EventHandler UploadRequested;

        public Logger Logger { get; private set; }

        public bool HasUploadRequested => UploadRequested != null;

        private DebugForm(Logger logger)
        {
            Logger = logger;

            InitializeComponent();

            rtbDebug.Text = Logger.ToString();
            rtbDebug.SelectionStart = rtbDebug.TextLength;
            rtbDebug.ScrollToCaret();
            rtbDebug.AddContextMenu();

            btnOpenLogFile.Enabled = !string.IsNullOrEmpty(Logger.LogFilePath);

            ShareXResources.ApplyTheme(this, true);

            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            llRunningFrom.Text = startupPath;
            llRunningFrom.LinkClicked += (sender, e) => FileHelpers.OpenFolder(startupPath);

            Logger.MessageAdded += logger_MessageAdded;
            Activated += (sender, e) => btnUploadLog.Visible = HasUploadRequested;
            FormClosing += (sender, e) => Logger.MessageAdded -= logger_MessageAdded;
        }

        public static DebugForm GetFormInstance(Logger logger)
        {
            if (instance == null || instance.IsDisposed)
            {
                instance = new DebugForm(logger);
            }

            return instance;
        }

        private void logger_MessageAdded(string message)
        {
            this.InvokeSafe(() => AppendMessage(message));
        }

        private void AppendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                int start = rtbDebug.SelectionStart;
                int len = rtbDebug.SelectionLength;
                rtbDebug.AppendText(message);
                if (len > 0)
                {
                    rtbDebug.Select(start, len);
                }
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            string text = rtbDebug.Text.Trim();
            ClipboardHelpers.CopyText(text);
        }

        private void btnOpenLogFile_Click(object sender, EventArgs e)
        {
            FileHelpers.OpenFile(Logger.LogFilePath);
        }

        private void btnLoadedAssemblies_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && assembly.Location.StartsWith(directoryPath, StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(assembly.ManifestModule.Name);
                }
            }
            string assemblies = sb.ToString().Trim();

            DebugHelper.WriteLine($"Loaded assemblies:\r\n{assemblies}");
        }

        private void btnUploadLog_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(rtbDebug.Text))
            {
                this.InvokeSafe(() =>
                {
                    UploadRequested?.Invoke(rtbDebug.Text);
                });
            }
        }

        private void rtbDebug_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }
    }
}