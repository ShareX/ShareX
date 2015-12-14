#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class ErrorForm : BaseForm
    {
        public bool IsUnhandledException { get; private set; }
        public string LogPath { get; private set; }
        public string BugReportPath { get; private set; }

        public ErrorForm(Exception error, string logPath, string bugReportPath)
            : this(error.Message, error.ToString(), logPath, bugReportPath)
        {
        }

        public ErrorForm(string errorTitle, string errorMessage, string logPath, string bugReportPath, bool unhandledException = true)
        {
            InitializeComponent();
            IsUnhandledException = unhandledException;
            LogPath = logPath;
            BugReportPath = bugReportPath;

            if (IsUnhandledException)
            {
                DebugHelper.WriteException(errorMessage, "Unhandled exception");
            }

            lblErrorMessage.Text = errorTitle;
            txtException.Text = errorMessage;
            txtException.SelectionStart = txtException.TextLength;

            btnSendBugReport.Visible = !string.IsNullOrEmpty(BugReportPath);
            btnOpenLogFile.Visible = !string.IsNullOrEmpty(LogPath) && File.Exists(LogPath);
            btnContinue.Visible = IsUnhandledException;
            btnClose.Visible = IsUnhandledException;
            btnOK.Visible = !IsUnhandledException;
        }

        private void ErrorForm_Shown(object sender, EventArgs e)
        {
            this.ShowActivate();
        }

        private void btnSendBugReport_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(BugReportPath);
        }

        private void btnOpenLogFile_Click(object sender, EventArgs e)
        {
            Helpers.OpenFile(LogPath);
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            DebugHelper.WriteLine("ShareX continue.");
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DebugHelper.WriteLine("ShareX closing. Reason: Unhandled exception");
            Application.Exit();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}