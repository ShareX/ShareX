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
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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

        private enum OpenAIReasoningEffort
        {
            minimal,
            low,
            medium,
            high
        }

        private enum OpenAIVerbosity
        {
            low,
            medium,
            high
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

            OpenAIReasoningEffort reasoningEffort;
            if (Enum.TryParse(Options.OpenAIReasoningEffort, out reasoningEffort))
            {
                cbOpenAIReasoningEffort.SelectedIndex = (int)reasoningEffort;
            }
            else
            {
                cbOpenAIReasoningEffort.SelectedIndex = 2;
            }
            OpenAIVerbosity verbosity;
            if (Enum.TryParse(Options.OpenAIVerbosity, out verbosity))
            {
                cbOpenAIVerbosity.SelectedIndex = (int)verbosity;
            }
            else
            {
                cbOpenAIVerbosity.SelectedIndex = 2;
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

            Options.OpenAIReasoningEffort = ((OpenAIReasoningEffort)cbOpenAIReasoningEffort.SelectedIndex).ToString();
            Options.OpenAIVerbosity = ((OpenAIVerbosity)cbOpenAIVerbosity.SelectedIndex).ToString();
            Options.AutoStartRegion = cbAutoStartRegion.Checked;
            Options.AutoStartAnalyze = cbAutoStartAnalyze.Checked;
            Options.AutoCopyResult = cbAutoCopyResult.Checked;
        }

        private void cbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbOpenAI.Visible = false;
            gbGemini.Visible = false;
            gbOpenRouter.Visible = false;

            switch ((AIProvider)cbProvider.SelectedIndex)
            {
                case AIProvider.OpenAI:
                    gbOpenAI.Visible = true;
                    break;
                case AIProvider.Gemini:
                    gbGemini.Visible = true;
                    break;
                case AIProvider.OpenRouter:
                    gbOpenRouter.Visible = true;
                    break;
            }
        }

        private void btnAPIKeyHelp_Click(object sender, EventArgs e)
        {
            string url = "";

            switch ((AIProvider)cbProvider.SelectedIndex)
            {
                case AIProvider.OpenAI:
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

        private async void btnOpenAILoadModels_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                btnOpenAILoadModels.Enabled = false;
                lblTestStatus.ForeColor = Color.Gold;
                lblTestStatus.Text = "Loading Models...";

                HttpClient client = HttpClientFactory.Create();
                HttpRequestMessage req = null;
                AIProvider provider = (AIProvider)cbProvider.SelectedIndex;

                string openAIKey = txtOpenAIAPIKey.Text?.Trim();
                if (string.IsNullOrEmpty(openAIKey))
                {
                    openAIKey = "";
                }

                string openAIBaseURL = txtOpenAICustomURL.Text;
                if (string.IsNullOrWhiteSpace(openAIBaseURL))
                {
                    openAIBaseURL = "https://api.openai.com";
                }
                string openAIURL = URLHelpers.CombineURL(openAIBaseURL, "v1/models");
                req = new HttpRequestMessage(HttpMethod.Get, openAIURL);
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", openAIKey);

                using (req)
                using (HttpResponseMessage resp = await client.SendAsync(req))
                {
                    bool ok = (int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300;
                    string text = await resp.Content.ReadAsStringAsync();

                    if (ok)
                    {
                        var json = JsonSerializer.Deserialize<JsonElement>(text);
                        var modelIds = new List<string>();
                        int modelCount = 0;
                        cbOpenAIModel.Items.Clear();
                        foreach (var item in json.GetProperty("data").EnumerateArray())
                        {
                            cbOpenAIModel.Items.Add(item.GetProperty("id").GetString());
                            modelCount++;
                        }
                        if (modelCount == 0)
                        {
                            lblTestStatus.ForeColor = Color.IndianRed;
                            lblTestStatus.Text = "No models found.";
                            return;
                        }
                        lblTestStatus.ForeColor = Color.LimeGreen;
                        lblTestStatus.Text = modelCount + " models loaded.";
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
                btnOpenAILoadModels.Enabled = true;
            }
        }

        private async void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                btnTestConnection.Enabled = false;
                btnOpenAILoadModels.Enabled = false;
                lblTestStatus.ForeColor = Color.Gold;
                lblTestStatus.Text = "Testing...";

                HttpClient client = HttpClientFactory.Create();
                HttpRequestMessage req = null;
                AIProvider provider = (AIProvider)cbProvider.SelectedIndex;

                switch (provider)
                {
                    case AIProvider.OpenAI:
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
                            openAIBaseURL = "https://api.openai.com";
                        }
                        string openAIURL = URLHelpers.CombineURL(openAIBaseURL, "v1/models");
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
                btnOpenAILoadModels.Enabled = true;
            }
        }

        private void lblTestStatus_MouseHover(object sender, EventArgs e)
        {
            tipStatus.SetToolTip(this, lblTestStatus.Text);
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