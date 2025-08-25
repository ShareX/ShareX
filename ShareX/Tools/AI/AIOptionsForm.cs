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
using System;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AIOptionsForm : Form
    {
        public AIOptions Options { get; private set; }

        public AIOptionsForm(AIOptions options)
        {
            Options = options;
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            LoadOptions();
        }

        private void LoadOptions()
        {
            cbModel.Text = Options.Model;
            txtAPIKey.Text = Options.ChatGPTAPIKey;
            int index = cbReasoningEffort.FindStringExact(Options.ReasoningEffort);
            if (index >= 0)
            {
                cbReasoningEffort.SelectedIndex = index;
            }
            else
            {
                cbReasoningEffort.SelectedIndex = 2;
            }
            index = cbVerbosity.FindStringExact(Options.Verbosity);
            if (index >= 0)
            {
                cbVerbosity.SelectedIndex = index;
            }
            else
            {
                cbVerbosity.SelectedIndex = 2;
            }
            cbAutoStartRegion.Checked = Options.AutoStartRegion;
            cbAutoStartAnalyze.Checked = Options.AutoStartAnalyze;
            cbAutoCopyResult.Checked = Options.AutoCopyResult;
        }

        private void SaveOptions()
        {
            Options.Model = cbModel.Text;
            Options.ChatGPTAPIKey = txtAPIKey.Text;
            Options.ReasoningEffort = cbReasoningEffort.Text;
            Options.Verbosity = cbVerbosity.Text;
            Options.AutoStartRegion = cbAutoStartRegion.Checked;
            Options.AutoStartAnalyze = cbAutoStartAnalyze.Checked;
            Options.AutoCopyResult = cbAutoCopyResult.Checked;
        }

        private void btnAPIKeyHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://platform.openai.com/api-keys");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveOptions();

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