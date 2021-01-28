#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using System;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class ResponseForm : Form
    {
        public UploadResult Result { get; private set; }

        private bool isBrowserOpened;

        public ResponseForm(UploadResult result)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            rtbResult.AddContextMenu();
            rtbResponseInfo.AddContextMenu();
            rtbResponseText.AddContextMenu();

            Result = result;
            UpdateResult(Result);
        }

        private void UpdateResult(UploadResult result)
        {
            isBrowserOpened = false;

            if (result != null)
            {
                StringBuilder sbResult = new StringBuilder();

                if (!string.IsNullOrEmpty(result.ShortenedURL))
                {
                    sbResult.AppendLine("Shortened URL: " + result.ShortenedURL);
                }

                if (!string.IsNullOrEmpty(result.URL))
                {
                    sbResult.AppendLine("URL: " + result.URL);
                }

                if (!string.IsNullOrEmpty(result.ThumbnailURL))
                {
                    sbResult.AppendLine("Thumbnail URL: " + result.ThumbnailURL);
                }

                if (!string.IsNullOrEmpty(result.DeletionURL))
                {
                    sbResult.AppendLine("Deletion URL: " + result.DeletionURL);
                }

                if (result.IsError)
                {
                    sbResult.AppendLine(result.ErrorsToString());
                }

                rtbResult.Text = sbResult.ToString();

                if (result.ResponseInfo != null)
                {
                    rtbResponseText.ResetText();
                    rtbResponseText.Text = result.ResponseInfo.ResponseText;

                    UpdateResponseInfoTextBox(result.ResponseInfo, true);
                }
            }
        }

        private void UpdateResponseInfoTextBox(ResponseInfo responseInfo, bool includeResponseText)
        {
            rtbResponseInfo.ResetText();

            rtbResponseInfo.SetFontBold();
            rtbResponseInfo.AppendText("Status code:\r\n");
            rtbResponseInfo.SetFontRegular();
            rtbResponseInfo.AppendText($"({(int)responseInfo.StatusCode}) {responseInfo.StatusDescription}");

            if (!string.IsNullOrEmpty(responseInfo.ResponseURL))
            {
                rtbResponseInfo.SetFontBold();
                rtbResponseInfo.AppendText("\r\n\r\nResponse URL:\r\n");
                rtbResponseInfo.SetFontRegular();
                rtbResponseInfo.AppendText(responseInfo.ResponseURL);
            }

            if (responseInfo.Headers != null && responseInfo.Headers.Count > 0)
            {
                rtbResponseInfo.SetFontBold();
                rtbResponseInfo.AppendText("\r\n\r\nHeaders:\r\n");
                rtbResponseInfo.SetFontRegular();
                rtbResponseInfo.AppendText(responseInfo.Headers.ToString().TrimEnd('\r', '\n'));
            }

            if (includeResponseText && !string.IsNullOrEmpty(responseInfo.ResponseText))
            {
                rtbResponseInfo.SetFontBold();
                rtbResponseInfo.AppendText("\r\n\r\nResponse text:\r\n");
                rtbResponseInfo.SetFontRegular();
                rtbResponseInfo.AppendText(responseInfo.ResponseText);
            }
        }

        private void ResponseForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void tcMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == tpWebBrowser && !isBrowserOpened && Result != null && !string.IsNullOrEmpty(Result.Response))
            {
                wbResponse.DocumentText = Result.Response;
                isBrowserOpened = true;
            }
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
                    MessageBox.Show("Formatting failed.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Formatting failed.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbResponseTextCopy_Click(object sender, EventArgs e)
        {
            string response = rtbResponseText.Text;
            if (!string.IsNullOrEmpty(response))
            {
                ClipboardHelpers.CopyText(response);
            }
        }
    }
}