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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class ResponseForm : Form
    {
        private static ResponseForm instance;
        private static readonly object singletonLock = new object();

        public UploadResult Result { get; private set; }

        private bool isBrowserUpdated;

        private ResponseForm(UploadResult result)
        {
            InitializeComponent();

            rtbResult.AddContextMenu();
            rtbResponseInfo.AddContextMenu();
            rtbResponseText.AddContextMenu();

            ShareXResources.ApplyTheme(this, true);

            UpdateResult(result);
        }

        public static void ShowInstance(UploadResult result)
        {
            lock (singletonLock)
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new ResponseForm(result);
                }
                else
                {
                    instance.UpdateResult(result);
                }

                instance.ForceActivate();
            }
        }

        private void AddInfo(RichTextBox rtb, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (rtb.TextLength > 0)
                {
                    rtb.AppendLine();
                    rtb.AppendLine();
                }

                rtb.SetFontBold();
                rtb.AppendLine(name + ":");
                rtb.SetFontRegular();
                rtb.AppendText(value);
            }
        }

        private void UpdateResult(UploadResult result)
        {
            Result = result;

            rtbResult.ResetText();
            rtbResponseInfo.ResetText();
            rtbResponseText.ResetText();
            isBrowserUpdated = false;
            wbResponse.DocumentText = "";

            if (result != null)
            {
                UpdateResultTab(result);

                if (result.ResponseInfo != null)
                {
                    UpdateResponseInfoTab(result.ResponseInfo, true);

                    rtbResponseText.Text = result.ResponseInfo.ResponseText;
                }
            }
        }

        private void UpdateResultTab(UploadResult result)
        {
            tsbCopyShortenedURL.Visible = !string.IsNullOrEmpty(result.ShortenedURL);
            AddInfo(rtbResult, Resources.ShortenedURL, result.ShortenedURL);
            tsbCopyURL.Visible = !string.IsNullOrEmpty(result.URL);
            AddInfo(rtbResult, Resources.URL, result.URL);
            tsbCopyThumbnailURL.Visible = !string.IsNullOrEmpty(result.ThumbnailURL);
            AddInfo(rtbResult, Resources.ThumbnailURL, result.ThumbnailURL);
            tsbCopyDeletionURL.Visible = !string.IsNullOrEmpty(result.DeletionURL);
            AddInfo(rtbResult, Resources.DeletionURL, result.DeletionURL);
            if (result.IsError) AddInfo(rtbResult, Resources.Error, result.ErrorsToString());
        }

        private void UpdateResponseInfoTab(ResponseInfo responseInfo, bool includeResponseText)
        {
            AddInfo(rtbResponseInfo, Resources.StatusCode, $"({(int)responseInfo.StatusCode}) {responseInfo.StatusDescription}");
            AddInfo(rtbResponseInfo, Resources.ResponseURL, responseInfo.ResponseURL);
            if (responseInfo.Headers != null && responseInfo.Headers.Count > 0) AddInfo(rtbResponseInfo, Resources.Headers, responseInfo.Headers.ToString().TrimEnd('\r', '\n'));
            if (includeResponseText) AddInfo(rtbResponseInfo, Resources.ResponseText, responseInfo.ResponseText);
        }

        private void tcMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tpWebBrowser && !isBrowserUpdated && Result != null && !string.IsNullOrEmpty(Result.Response))
            {
                wbResponse.DocumentText = Result.Response;
                isBrowserUpdated = true;
            }
        }

        private void tsbCopyShortenedURL_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Result.ShortenedURL);
        }

        private void tsbCopyURL_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Result.URL);
        }

        private void tsbCopyThumbnailURL_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Result.ThumbnailURL);
        }

        private void tsbCopyDeletionURL_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(Result.DeletionURL);
        }

        private void rtbResult_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        private void tsbResponseTextJSONFormat_Click(object sender, EventArgs e)
        {
            string response = rtbResponseText.Text;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    response = Helpers.JSONFormat(response, Formatting.Indented);
                    rtbResponseText.Text = response;
                }
                catch
                {
                    MessageBox.Show(Resources.FormattingFailed_JSON, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbResponseTextXMLFormat_Click(object sender, EventArgs e)
        {
            string response = rtbResponseText.Text;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    response = Helpers.XMLFormat(response);
                    rtbResponseText.Text = response;
                }
                catch
                {
                    MessageBox.Show(Resources.FormattingFailed_XML, "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbResponseTextCopy_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.CopyText(rtbResponseText.Text);
        }
    }
}