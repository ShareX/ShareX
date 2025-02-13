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

using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class JiraUpload : Form
    {
        public delegate string GetSummaryHandler(string issueId);

        private readonly string issuePrefix;
        private readonly GetSummaryHandler getSummary;

        public string IssueId
        {
            get
            {
                return txtIssueId.Text;
            }
        }

        public JiraUpload()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
        }

        public JiraUpload(string issuePrefix, GetSummaryHandler getSummary) : this()
        {
            this.issuePrefix = issuePrefix;
            this.getSummary = getSummary ?? throw new ArgumentNullException(nameof(getSummary));
        }

        private void JiraUpload_Load(object sender, EventArgs e)
        {
            UpdateSummary(null);

            txtIssueId.Text = issuePrefix;
            txtIssueId.SelectionStart = txtIssueId.Text.Length;
        }

        private void txtIssueId_TextChanged(object sender, EventArgs e)
        {
            ValidateIssueId(txtIssueId.Text);
        }

        private void ValidateIssueId(string issueId)
        {
            Task.Run(() => getSummary(issueId)).ContinueWith(UpdateSummaryAsync);
        }

        private void UpdateSummaryAsync(Task<string> task)
        {
            this.InvokeSafe(() => UpdateSummary(task.Result));
        }

        private void UpdateSummary(string summary)
        {
            btnUpload.Enabled = summary != null;

            lblSummary.Text = summary ?? Resources.JiraUpload_ValidateIssueId_Issue_not_found;
            lblSummary.Enabled = summary != null;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}