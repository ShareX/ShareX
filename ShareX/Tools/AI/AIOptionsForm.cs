#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
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

            foreach (AIProvider provider in Enum.GetValues(typeof(AIProvider)))
            {
                cbProvider.Items.Add(provider.ToString());
            }

            LoadOptions();
        }

        private void LoadOptions()
        {
            cbProvider.SelectedIndex = (int)Options.Provider;

            txtOpenAIAPIKey.Text = Options.OpenAIAPIKey;
            cbOpenAIModel.Text = Options.OpenAIModel;
            txtOpenAICustomURL.Text = Options.OpenAICustomURL;

            txtGeminiAPIKey.Text = Options.GeminiAPIKey;
            cbGeminiModel.Text = Options.GeminiModel;

            txtOpenRouterAPIKey.Text = Options.OpenRouterAPIKey;
            cbOpenRouterModel.Text = Options.OpenRouterModel;

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
            Options.Provider = (AIProvider)cbProvider.SelectedIndex;

            Options.OpenAIAPIKey = txtOpenAIAPIKey.Text;
            Options.OpenAIModel = cbOpenAIModel.Text;
            Options.OpenAICustomURL = txtOpenAICustomURL.Text;

            Options.GeminiAPIKey = txtGeminiAPIKey.Text;
            Options.GeminiModel = cbGeminiModel.Text;

            Options.OpenRouterAPIKey = txtOpenRouterAPIKey.Text;
            Options.OpenRouterModel = cbOpenRouterModel.Text;

            Options.ReasoningEffort = cbReasoningEffort.Text;
            Options.Verbosity = cbVerbosity.Text;
            Options.AutoStartRegion = cbAutoStartRegion.Checked;
            Options.AutoStartAnalyze = cbAutoStartAnalyze.Checked;
            Options.AutoCopyResult = cbAutoCopyResult.Checked;
        }

        private void cbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbOpenAI.Visible = false;
            gbGemini.Visible = false;
            gbOpenRouter.Visible = false;
            lblOpenAICustomURL.Visible = false;
            txtOpenAICustomURL.Visible = false;

            switch ((AIProvider)cbProvider.SelectedIndex)
            {
                case AIProvider.OpenAI:
                    gbOpenAI.Visible = true;
                    break;
                case AIProvider.Custom:
                    gbOpenAI.Visible = true;
                    lblOpenAICustomURL.Visible = true;
                    txtOpenAICustomURL.Visible = true;
                    break;
                case AIProvider.Gemini:
                    gbGemini.Visible = true;
                    break;
                case AIProvider.OpenRouter:
                    gbOpenRouter.Visible = true;
                    break;
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                lblTestStatus.ForeColor = Color.Gold;
                lblTestStatus.Text = "Testing...";

                HttpClient client = HttpClientFactory.Create();
                HttpRequestMessage req = null;
                AIProvider provider = (AIProvider)cbProvider.SelectedIndex;

                switch (provider)
                {
                    case AIProvider.OpenAI:
                    case AIProvider.Custom:
                        string openAIKey = txtOpenAIAPIKey.Text?.Trim();
                        if (string.IsNullOrEmpty(openAIKey))
                        {
                            lblTestStatus.ForeColor = Color.IndianRed;
                            lblTestStatus.Text = "Missing OpenAI API key.";
                            return;
                        }

                        string openAIBaseURL = txtOpenAICustomURL.Text;
                        if (string.IsNullOrWhiteSpace(openAIBaseURL))
                        {
                            openAIBaseURL = "https://api.openai.com/v1";
                        }
                        openAIBaseURL = openAIBaseURL.Trim().TrimEnd('/');

                        string openAIURL = openAIBaseURL + "/models";
                        req = new HttpRequestMessage(HttpMethod.Get, openAIURL);
                        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAIKey);
                        break;
                    case AIProvider.Gemini:
                        string geminiKey = txtGeminiAPIKey.Text?.Trim();
                        if (string.IsNullOrEmpty(geminiKey))
                        {
                            lblTestStatus.ForeColor = Color.IndianRed;
                            lblTestStatus.Text = "Missing Gemini API key.";
                            return;
                        }

                        string geminiURL = "https://generativelanguage.googleapis.com/v1beta/models?key=" + Uri.EscapeDataString(geminiKey);
                        req = new HttpRequestMessage(HttpMethod.Get, geminiURL);
                        break;
                    case AIProvider.OpenRouter:
                        string openRouterKey = txtOpenRouterAPIKey.Text?.Trim();
                        if (string.IsNullOrEmpty(openRouterKey))
                        {
                            lblTestStatus.ForeColor = Color.IndianRed;
                            lblTestStatus.Text = "Missing OpenRouter API key.";
                            return;
                        }

                        string openRouterURL = "https://openrouter.ai/api/v1/models";
                        req = new HttpRequestMessage(HttpMethod.Get, openRouterURL);
                        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openRouterKey);
                        break;
                    default:
                        lblTestStatus.ForeColor = Color.IndianRed;
                        lblTestStatus.Text = "Select a provider first.";
                        return;
                }

                using (req)
                using (HttpResponseMessage resp = await client.SendAsync(req))
                {
                    bool ok = (int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300;
                    string text = await resp.Content.ReadAsStringAsync();

                    if (ok)
                    {
                        lblTestStatus.ForeColor = Color.LimeGreen;
                        lblTestStatus.Text = "Connection OK.";
                    }
                    else
                    {
                        lblTestStatus.ForeColor = Color.IndianRed;
                        string summary = resp.ReasonPhrase;
                        if (string.IsNullOrWhiteSpace(summary)) summary = resp.StatusCode.ToString();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            if (text.Length > 200) text = text.Substring(0, 200) + "...";
                            summary += ": " + text;
                        }
                        lblTestStatus.Text = summary;
                    }
                }
            }
            catch (Exception ex)
            {
                lblTestStatus.ForeColor = Color.IndianRed;
                lblTestStatus.Text = ex.Message;
            }
            finally
            {
                btnTestConnection.Enabled = true;
            }
        }

        private void btnAPIKeyHelp_Click(object sender, EventArgs e)
        {
            string url = "";
            switch ((AIProvider)cbProvider.SelectedIndex)
            {
                case AIProvider.OpenAI:
                case AIProvider.Custom:
                    url = "https://platform.openai.com/api-keys";
                    break;
                case AIProvider.Gemini:
                    url = "https://aistudio.google.com/app/apikey";
                    break;
                case AIProvider.OpenRouter:
                    url = "https://openrouter.ai/keys";
                    break;
            }
            URLHelpers.OpenURL(url);
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