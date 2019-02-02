#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.ImageUploaders;
using ShareX.UploadersLib.Properties;
using ShareX.UploadersLib.SharingServices;
using ShareX.UploadersLib.TextUploaders;
using ShareX.UploadersLib.URLShorteners;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class CustomUploaderSettingsForm : Form
    {
        public static bool IsInstanceActive => instance != null && !instance.IsDisposed;

        private static CustomUploaderSettingsForm instance;

        public UploadersConfig Config { get; private set; }

        private bool customUploaderPauseLoad;
        private URLType customUploaderURLType = URLType.URL;

        public CustomUploaderSettingsForm(UploadersConfig config)
        {
            Config = config;

            InitializeComponent();
            Icon = ShareXResources.Icon;

            CodeMenuItem[] inputCodeMenuItems = new CodeMenuItem[]
            {
                new CodeMenuItem("$input$", "Text/URL input"),
                new CodeMenuItem("$filename$", "File name"),
                new CodeMenuItem("$random:input1|input2$", "Random selection from list"),
                new CodeMenuItem("$select:input1|input2$", "Lets user to select one input from list"),
                new CodeMenuItem("$prompt:title|default_value$", "Lets user to input text"),
                new CodeMenuItem("$base64:input$", "Base64 encode input")
            };

            CodeMenuItem[] outputCodeMenuItems = new CodeMenuItem[]
            {
                new CodeMenuItem("$response$", "Response text"),
                new CodeMenuItem("$responseurl$", "Response/Redirection URL"),
                new CodeMenuItem("$header:header_name$", "Response header"),
                new CodeMenuItem("$json:path$", "Parse response using JSON"),
                new CodeMenuItem("$xml:path$", "Parse response using XML"),
                new CodeMenuItem("$regex:index|group$", "Parse response using Regex"),
                new CodeMenuItem("$filename$", "File name used when uploading"),
                new CodeMenuItem("$random:input1|input2$", "Random selection from list"),
                new CodeMenuItem("$select:input1|input2$", "Lets user to select one input from list"),
                new CodeMenuItem("$prompt:title|default_value$", "Lets user to input text"),
                new CodeMenuItem("$base64:input$", "Base64 encode input")
            };

            CodeMenu.Create(rtbCustomUploaderURL, outputCodeMenuItems);
            CodeMenu.Create(rtbCustomUploaderThumbnailURL, outputCodeMenuItems);
            CodeMenu.Create(rtbCustomUploaderDeletionURL, outputCodeMenuItems);

            rtbCustomUploaderRequestURL.AddContextMenu();
            rtbCustomUploaderData.AddContextMenu();
            rtbCustomUploaderURL.AddContextMenu();
            rtbCustomUploaderThumbnailURL.AddContextMenu();
            rtbCustomUploaderDeletionURL.AddContextMenu();
            rtbCustomUploaderResult.AddContextMenu();
            eiCustomUploaders.ObjectType = typeof(CustomUploaderItem);
            CustomUploaderAddDestinationTypes();
            cbCustomUploaderRequestMethod.Items.AddRange(Enum.GetNames(typeof(HttpMethod)));
            cbCustomUploaderBody.Items.AddRange(Helpers.GetEnumDescriptions<CustomUploaderBody>());

            CustomUploaderLoadTab();
        }

        public static CustomUploaderSettingsForm GetFormInstance(UploadersConfig config)
        {
            if (!IsInstanceActive)
            {
                instance = new CustomUploaderSettingsForm(config);
            }

            return instance;
        }

        private bool CustomUploaderCheck(int index)
        {
            return Config.CustomUploadersList.IsValidIndex(index);
        }

        private CustomUploaderItem CustomUploaderGetSelected()
        {
            int index = lbCustomUploaderList.SelectedIndex;

            if (CustomUploaderCheck(index))
            {
                return Config.CustomUploadersList[index];
            }

            return null;
        }

        private void CustomUploaderNew()
        {
            CustomUploaderItem uploader = new CustomUploaderItem()
            {
                Version = Application.ProductVersion
            };

            CustomUploaderAdd(uploader);
            CustomUploaderUpdateList();
        }

        private void CustomUploaderAdd(CustomUploaderItem uploader)
        {
            if (uploader != null)
            {
                Config.CustomUploadersList.Add(uploader);
                lbCustomUploaderList.Items.Add(uploader);
            }
        }

        private void CustomUploaderLoadSelected()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                CustomUploaderLoad(uploader);
            }
        }

        private void CustomUploaderLoad(CustomUploaderItem uploader)
        {
            txtCustomUploaderName.Text = uploader.Name ?? "";
            CustomUploaderSetDestinationType(uploader.DestinationType);

            cbCustomUploaderRequestMethod.SelectedIndex = (int)uploader.RequestMethod;
            rtbCustomUploaderRequestURL.Text = uploader.RequestURL ?? "";
            CustomUploaderSyntaxHighlight(rtbCustomUploaderRequestURL);

            dgvParameters.Rows.Clear();
            if (uploader.Parameters != null)
            {
                foreach (KeyValuePair<string, string> arg in uploader.Parameters)
                {
                    dgvParameters.Rows.Add(new string[] { arg.Key, arg.Value });
                }
            }

            dgvHeaders.Rows.Clear();
            if (uploader.Headers != null)
            {
                foreach (KeyValuePair<string, string> arg in uploader.Headers)
                {
                    dgvHeaders.Rows.Add(new string[] { arg.Key, arg.Value });
                }
            }

            cbCustomUploaderBody.SelectedIndex = (int)uploader.Body;

            dgvArguments.Rows.Clear();
            if (uploader.Arguments != null)
            {
                foreach (KeyValuePair<string, string> arg in uploader.Arguments)
                {
                    dgvArguments.Rows.Add(new string[] { arg.Key, arg.Value });
                }
            }

            txtCustomUploaderFileFormName.Text = uploader.FileFormName ?? "";

            rtbCustomUploaderData.Text = uploader.Data ?? "";
            CustomUploaderSyntaxHighlight(rtbCustomUploaderData);

            txtCustomUploaderJsonPath.Text = "";
            txtCustomUploaderXPath.Text = "";
            txtCustomUploaderRegexp.Text = "";
            lvCustomUploaderRegexps.Items.Clear();
            if (uploader.RegexList != null)
            {
                foreach (string regexp in uploader.RegexList)
                {
                    lvCustomUploaderRegexps.Items.Add(regexp);
                }
            }

            rtbCustomUploaderURL.Text = uploader.URL ?? "";
            CustomUploaderSyntaxHighlight(rtbCustomUploaderURL);
            rtbCustomUploaderThumbnailURL.Text = uploader.ThumbnailURL ?? "";
            CustomUploaderSyntaxHighlight(rtbCustomUploaderThumbnailURL);
            rtbCustomUploaderDeletionURL.Text = uploader.DeletionURL ?? "";
            CustomUploaderSyntaxHighlight(rtbCustomUploaderDeletionURL);

            CustomUploaderUpdateStates();
        }

        private void CustomUploaderUpdateStates()
        {
            bool isSelected = CustomUploaderCheck(lbCustomUploaderList.SelectedIndex);

            txtCustomUploaderName.Enabled = btnCustomUploaderRemove.Enabled = btnCustomUploaderDuplicate.Enabled = pCustomUploader.Enabled =
                mbCustomUploaderDestinationType.Enabled = isSelected;

            if (isSelected)
            {
                CustomUploaderUpdateRequestFormatState();
                CustomUploaderUpdateResponseState();
            }

            btnCustomUploaderClearUploaders.Enabled = tsmiCustomUploaderExportAll.Enabled = cbCustomUploaderImageUploader.Enabled =
                btnCustomUploaderImageUploaderTest.Enabled = cbCustomUploaderTextUploader.Enabled = btnCustomUploaderTextUploaderTest.Enabled =
                cbCustomUploaderFileUploader.Enabled = btnCustomUploaderFileUploaderTest.Enabled = cbCustomUploaderURLShortener.Enabled =
                btnCustomUploaderURLShortenerTest.Enabled = cbCustomUploaderURLSharingService.Enabled = btnCustomUploaderURLSharingServiceTest.Enabled =
                lbCustomUploaderList.Items.Count > 0;
        }

        private void CustomUploaderUpdateRequestFormatState()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                pCustomUploaderBodyArguments.Visible = uploader.Body == CustomUploaderBody.MultipartFormData ||
                    uploader.Body == CustomUploaderBody.FormURLEncoded;
                lblCustomUploaderFileFormName.Visible = txtCustomUploaderFileFormName.Visible = uploader.Body == CustomUploaderBody.MultipartFormData;
                pCustomUploaderBodyData.Visible = uploader.Body == CustomUploaderBody.JSON || uploader.Body == CustomUploaderBody.XML;
                btnCustomUploaderDataMinify.Visible = uploader.Body == CustomUploaderBody.JSON;
            }
        }

        private void CustomUploaderUpdateResponseState()
        {
            btnCustomUploaderJsonAddSyntax.Enabled = !string.IsNullOrEmpty(txtCustomUploaderJsonPath.Text);
            btnCustomUploaderXmlSyntaxAdd.Enabled = !string.IsNullOrEmpty(txtCustomUploaderXPath.Text);
            btnCustomUploaderRegexpAdd.Enabled = !string.IsNullOrEmpty(txtCustomUploaderRegexp.Text);
            btnCustomUploaderRegexpRemove.Enabled = btnCustomUploaderRegexpUpdate.Enabled = btnCustomUploaderRegexAddSyntax.Enabled =
                lvCustomUploaderRegexps.SelectedItems.Count > 0;
        }

        private void CustomUploaderRefreshNames()
        {
            customUploaderPauseLoad = true;
            lbCustomUploaderList.RefreshSelectedItem();
            cbCustomUploaderImageUploader.RefreshItems();
            cbCustomUploaderTextUploader.RefreshItems();
            cbCustomUploaderFileUploader.RefreshItems();
            cbCustomUploaderURLShortener.RefreshItems();
            cbCustomUploaderURLSharingService.RefreshItems();
            customUploaderPauseLoad = false;
        }

        private void CustomUploaderClearUploaders()
        {
            Config.CustomUploadersList.Clear();
            lbCustomUploaderList.Items.Clear();
            CustomUploaderClearFields();
            Config.CustomImageUploaderSelected = Config.CustomTextUploaderSelected = Config.CustomFileUploaderSelected = Config.CustomURLShortenerSelected =
                Config.CustomURLSharingServiceSelected = 0;
            CustomUploaderUpdateList();
            CustomUploaderUpdateStates();
            btnCustomUploaderNew.Focus();
        }

        private void CustomUploaderClearFields()
        {
            CustomUploaderLoad(new CustomUploaderItem());
        }

        private void CustomUploaderExportAll()
        {
            if (Config.CustomUploadersList != null && Config.CustomUploadersList.Count > 0)
            {
                using (FolderSelectDialog fsd = new FolderSelectDialog())
                {
                    if (fsd.ShowDialog())
                    {
                        foreach (CustomUploaderItem uploader in Config.CustomUploadersList)
                        {
                            string json = eiCustomUploaders.Serialize(uploader);
                            string filepath = Path.Combine(fsd.FileName, uploader.GetFileName());
                            File.WriteAllText(filepath, json, Encoding.UTF8);
                        }
                    }
                }
            }
        }

        private void CustomUploaderLoadTab(bool selectLastItem = false)
        {
            lbCustomUploaderList.Items.Clear();

            if (Config.CustomUploadersList == null)
            {
                Config.CustomUploadersList = new List<CustomUploaderItem>();
            }
            else
            {
                foreach (CustomUploaderItem customUploader in Config.CustomUploadersList)
                {
                    lbCustomUploaderList.Items.Add(customUploader);
                }

                CustomUploaderUpdateList();
            }

#if DEBUG
            tsmiCustomUploaderExportAll.Visible = true;
#endif

            CustomUploaderClearFields();

            if (lbCustomUploaderList.Items.Count > 0)
            {
                if (selectLastItem)
                {
                    lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
                }
                else if (Config.CustomUploadersList.IsValidIndex(Config.CustomImageUploaderSelected))
                {
                    lbCustomUploaderList.SelectedIndex = Config.CustomImageUploaderSelected;
                }
            }

            CustomUploaderUpdateStates();
        }

        public static void CustomUploaderUpdateTab()
        {
            if (IsInstanceActive)
            {
                CustomUploaderSettingsForm form = GetFormInstance(null);
                form.CustomUploaderLoadTab(true);
                form.ForceActivate();
            }
        }

        private void CustomUploaderAddDestinationTypes()
        {
            string[] enums = Helpers.GetLocalizedEnumDescriptions<CustomUploaderDestinationType>().Skip(1).Select(x => x.Replace("&", "&&")).ToArray();

            for (int i = 0; i < enums.Length; i++)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(enums[i]);

                int index = i;

                tsmi.Click += (sender, e) =>
                {
                    ToolStripMenuItem tsmi2 = (ToolStripMenuItem)cmsCustomUploaderDestinationType.Items[index];
                    tsmi2.Checked = !tsmi2.Checked;

                    CustomUploaderItem uploader = CustomUploaderGetSelected();
                    if (uploader != null)
                    {
                        uploader.DestinationType = CustomUploaderGetDestinationType();
                    }

                    CustomUploaderDestinationTypeUpdate();
                };

                cmsCustomUploaderDestinationType.Items.Add(tsmi);
            }

            cmsCustomUploaderDestinationType.Closing += (sender, e) => e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
        }

        private void CustomUploaderSetDestinationType(CustomUploaderDestinationType destinationType)
        {
            for (int i = 0; i < cmsCustomUploaderDestinationType.Items.Count; i++)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)cmsCustomUploaderDestinationType.Items[i];
                tsmi.Checked = destinationType.HasFlag(1 << i);
            }

            CustomUploaderDestinationTypeUpdate();
        }

        private CustomUploaderDestinationType CustomUploaderGetDestinationType()
        {
            CustomUploaderDestinationType destinationType = CustomUploaderDestinationType.None;

            for (int i = 0; i < cmsCustomUploaderDestinationType.Items.Count; i++)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)cmsCustomUploaderDestinationType.Items[i];

                if (tsmi.Checked)
                {
                    destinationType |= (CustomUploaderDestinationType)(1 << i);
                }
            }

            return destinationType;
        }

        private void CheckDataGridView(DataGridView dgv)
        {
            for (int i = dgv.Rows.Count - 1; i > -1; i--)
            {
                DataGridViewRow row = dgv.Rows[i];
                DataGridViewCell cell = row.Cells[0];

                if (cell.Value == null)
                {
                    if (!row.IsNewRow)
                    {
                        dgv.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    bool isDuplicate = false;

                    for (int i2 = 0; i2 < i; i2++)
                    {
                        if (dgv.Rows[i2].Cells[0].Value?.ToString() == cell.Value.ToString())
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                    if (isDuplicate)
                    {
                        // TODO: Translate
                        cell.ErrorText = "Duplicate name not allowed.";
                    }
                    else
                    {
                        cell.ErrorText = null;
                    }
                }
            }
        }

        private Dictionary<string, string> DataGridViewToDictionary(DataGridView dgv)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                string name = row.Cells[0].Value?.ToString();

                if (!string.IsNullOrEmpty(name) && !dic.ContainsKey(name))
                {
                    string value = row.Cells[1].Value?.ToString();

                    dic.Add(name, value);
                }
            }

            return dic;
        }

        private void CustomUploaderDestinationTypeUpdate()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                if (uploader.DestinationType == CustomUploaderDestinationType.None)
                {
                    mbCustomUploaderDestinationType.Text = CustomUploaderDestinationType.None.GetLocalizedDescription();
                }
                else
                {
                    mbCustomUploaderDestinationType.Text = string.Join(", ", uploader.DestinationType.GetFlags<CustomUploaderDestinationType>().
                        Select(x => x.GetLocalizedDescription()));
                }
            }
        }

        private void CustomUploaderFixSelectedUploader(int removedIndex)
        {
            int resetIndex = Config.CustomUploadersList.Count - 1;

            if (Config.CustomImageUploaderSelected == removedIndex)
            {
                Config.CustomImageUploaderSelected = resetIndex;
            }
            else if (Config.CustomImageUploaderSelected > removedIndex)
            {
                Config.CustomImageUploaderSelected--;
            }

            if (Config.CustomTextUploaderSelected == removedIndex)
            {
                Config.CustomTextUploaderSelected = resetIndex;
            }
            else if (Config.CustomTextUploaderSelected > removedIndex)
            {
                Config.CustomTextUploaderSelected--;
            }

            if (Config.CustomFileUploaderSelected == removedIndex)
            {
                Config.CustomFileUploaderSelected = resetIndex;
            }
            else if (Config.CustomFileUploaderSelected > removedIndex)
            {
                Config.CustomFileUploaderSelected--;
            }

            if (Config.CustomURLShortenerSelected == removedIndex)
            {
                Config.CustomURLShortenerSelected = resetIndex;
            }
            else if (Config.CustomURLShortenerSelected > removedIndex)
            {
                Config.CustomURLShortenerSelected--;
            }

            if (Config.CustomURLSharingServiceSelected == removedIndex)
            {
                Config.CustomURLSharingServiceSelected = resetIndex;
            }
            else if (Config.CustomURLSharingServiceSelected > removedIndex)
            {
                Config.CustomURLSharingServiceSelected--;
            }
        }

        private void CustomUploaderUpdateList()
        {
            cbCustomUploaderImageUploader.Items.Clear();
            cbCustomUploaderTextUploader.Items.Clear();
            cbCustomUploaderFileUploader.Items.Clear();
            cbCustomUploaderURLShortener.Items.Clear();
            cbCustomUploaderURLSharingService.Items.Clear();

            if (Config.CustomUploadersList.Count > 0)
            {
                foreach (CustomUploaderItem item in Config.CustomUploadersList)
                {
                    cbCustomUploaderImageUploader.Items.Add(item);
                    cbCustomUploaderTextUploader.Items.Add(item);
                    cbCustomUploaderFileUploader.Items.Add(item);
                    cbCustomUploaderURLShortener.Items.Add(item);
                    cbCustomUploaderURLSharingService.Items.Add(item);
                }

                cbCustomUploaderImageUploader.SelectedIndex = Config.CustomImageUploaderSelected.Between(0, Config.CustomUploadersList.Count - 1);
                cbCustomUploaderTextUploader.SelectedIndex = Config.CustomTextUploaderSelected.Between(0, Config.CustomUploadersList.Count - 1);
                cbCustomUploaderFileUploader.SelectedIndex = Config.CustomFileUploaderSelected.Between(0, Config.CustomUploadersList.Count - 1);
                cbCustomUploaderURLShortener.SelectedIndex = Config.CustomURLShortenerSelected.Between(0, Config.CustomUploadersList.Count - 1);
                cbCustomUploaderURLSharingService.SelectedIndex = Config.CustomURLSharingServiceSelected.Between(0, Config.CustomUploadersList.Count - 1);
            }
        }

        private void AddTextToActiveURLField(string text)
        {
            RichTextBox rtb;

            switch (customUploaderURLType)
            {
                default:
                case URLType.URL:
                    rtb = rtbCustomUploaderURL;
                    break;
                case URLType.ThumbnailURL:
                    rtb = rtbCustomUploaderThumbnailURL;
                    break;
                case URLType.DeletionURL:
                    rtb = rtbCustomUploaderDeletionURL;
                    break;
            }

            rtb.AppendText(text);
        }

        private async Task TestCustomUploader(CustomUploaderDestinationType type, int index)
        {
            if (!Config.CustomUploadersList.IsValidIndex(index))
            {
                return;
            }

            btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled = btnCustomUploaderFileUploaderTest.Enabled =
                btnCustomUploaderURLShortenerTest.Enabled = btnCustomUploaderURLSharingServiceTest.Enabled = false;
            rtbCustomUploaderResult.ResetText();
            txtCustomUploaderResponse.ResetText();
            lbCustomUploaderList.SelectedIndex = index;

            CustomUploaderItem item = Config.CustomUploadersList[index];
            UploadResult result = null;

            await Task.Run(() =>
            {
                try
                {
                    switch (type)
                    {
                        case CustomUploaderDestinationType.ImageUploader:
                            using (Stream stream = ShareXResources.Logo.GetStream())
                            {
                                CustomImageUploader imageUploader = new CustomImageUploader(item);
                                result = imageUploader.Upload(stream, "Test.png");
                                result.Errors = imageUploader.Errors;
                            }
                            break;
                        case CustomUploaderDestinationType.TextUploader:
                            CustomTextUploader textUploader = new CustomTextUploader(item);
                            result = textUploader.UploadText("ShareX text upload test", "Test.txt");
                            result.Errors = textUploader.Errors;
                            break;
                        case CustomUploaderDestinationType.FileUploader:
                            using (Stream stream = ShareXResources.Logo.GetStream())
                            {
                                CustomFileUploader fileUploader = new CustomFileUploader(item);
                                result = fileUploader.Upload(stream, "Test.png");
                                result.Errors = fileUploader.Errors;
                            }
                            break;
                        case CustomUploaderDestinationType.URLShortener:
                            CustomURLShortener urlShortener = new CustomURLShortener(item);
                            result = urlShortener.ShortenURL(Links.URL_WEBSITE);
                            result.Errors = urlShortener.Errors;
                            break;
                        case CustomUploaderDestinationType.URLSharingService:
                            CustomURLSharer urlSharer = new CustomURLSharer(item);
                            result = urlSharer.ShareURL(Links.URL_WEBSITE);
                            result.Errors = urlSharer.Errors;
                            break;
                    }
                }
                catch (Exception e)
                {
                    result = new UploadResult();
                    result.Errors.Add(e.Message);
                }
            });

            if (!IsDisposed)
            {
                if (result != null)
                {
                    StringBuilder sbResult = new StringBuilder();

                    if (((type == CustomUploaderDestinationType.ImageUploader || type == CustomUploaderDestinationType.TextUploader ||
                        type == CustomUploaderDestinationType.FileUploader) && !string.IsNullOrEmpty(result.URL)) ||
                        (type == CustomUploaderDestinationType.URLShortener && !string.IsNullOrEmpty(result.ShortenedURL)) ||
                        (type == CustomUploaderDestinationType.URLSharingService && !result.IsError && !string.IsNullOrEmpty(result.URL)))
                    {
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
                    }
                    else if (result.IsError)
                    {
                        sbResult.AppendLine(result.ErrorsToString());
                    }
                    else
                    {
                        sbResult.AppendLine(Resources.UploadersConfigForm_TestCustomUploader_Error__Result_is_empty_);
                    }

                    rtbCustomUploaderResult.Text = sbResult.ToString();
                    txtCustomUploaderResponse.Text = result.ResponseInfo?.ResponseText;

                    tcCustomUploader.SelectedTab = tpCustomUploaderTest;
                }

                btnCustomUploaderImageUploaderTest.Enabled = btnCustomUploaderTextUploaderTest.Enabled = btnCustomUploaderFileUploaderTest.Enabled =
                    btnCustomUploaderURLShortenerTest.Enabled = btnCustomUploaderURLSharingServiceTest.Enabled = true;
            }
        }

        private void CustomUploaderSyntaxHighlight(RichTextBox rtb)
        {
            if (!string.IsNullOrEmpty(rtb.Text))
            {
                CustomUploaderParser parser = new CustomUploaderParser();
                parser.SkipSyntaxParse = true;
                parser.Parse(rtb.Text);

                if (parser.SyntaxInfoList != null)
                {
                    int start = rtb.SelectionStart;
                    int length = rtb.SelectionLength;
                    rtb.BeginUpdate();

                    rtb.SelectionStart = 0;
                    rtb.SelectionLength = rtb.TextLength;
                    rtb.SelectionColor = rtb.ForeColor;

                    foreach (CustomUploaderSyntaxInfo syntaxInfo in parser.SyntaxInfoList)
                    {
                        rtb.SelectionStart = syntaxInfo.StartPosition;
                        rtb.SelectionLength = syntaxInfo.Length;
                        rtb.SelectionColor = Color.Green;
                    }

                    rtb.SelectionStart = start;
                    rtb.SelectionLength = length;
                    rtb.EndUpdate();
                }
            }
        }

        private void CustomUploaderFormatJsonData(Formatting formatting)
        {
            string json = rtbCustomUploaderData.Text;

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    rtbCustomUploaderData.Text = Helpers.JSONFormat(json, formatting);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CustomUploaderFormatXMLData()
        {
            string xml = rtbCustomUploaderData.Text;

            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    rtbCustomUploaderData.Text = Helpers.XMLFormat(xml);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        #region Form events

        private void CustomUploaderSettingsForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];

                if (files != null && files.Any(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxcu")))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void CustomUploaderSettingsForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];

                if (files != null)
                {
                    foreach (string filePath in files.Where(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxcu")))
                    {
                        CustomUploaderItem cui = JsonHelpers.DeserializeFromFilePath<CustomUploaderItem>(filePath);

                        if (cui != null)
                        {
                            cui.CheckBackwardCompatibility();
                            CustomUploaderAdd(cui);
                        }
                    }

                    eiCustomUploaders_ImportCompleted();
                }
            }
        }

        private void btnCustomUploaderNew_Click(object sender, EventArgs e)
        {
            CustomUploaderNew();
            lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
            txtCustomUploaderName.Focus();
        }

        private void btnCustomUploaderRemove_Click(object sender, EventArgs e)
        {
            int selected = lbCustomUploaderList.SelectedIndex;

            if (selected > -1)
            {
                lbCustomUploaderList.Items.RemoveAt(selected);
                Config.CustomUploadersList.RemoveAt(selected);

                if (lbCustomUploaderList.Items.Count > 0)
                {
                    lbCustomUploaderList.SelectedIndex = selected == lbCustomUploaderList.Items.Count ? lbCustomUploaderList.Items.Count - 1 : selected;
                }
                else
                {
                    CustomUploaderClearFields();
                    btnCustomUploaderNew.Focus();
                }

                CustomUploaderFixSelectedUploader(selected);
                CustomUploaderUpdateList();
            }
        }

        private void btnCustomUploaderDuplicate_Click(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                CustomUploaderItem clone = uploader.Copy();
                CustomUploaderAdd(clone);
                CustomUploaderUpdateList();
                lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
            }
        }

        private void lbCustomUploaderList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!customUploaderPauseLoad)
            {
                CustomUploaderLoadSelected();
            }
        }

        private void btnCustomUploaderClearUploaders_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.UploadersConfigForm_Remove_all_custom_uploaders_Confirmation, "ShareX",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CustomUploaderClearUploaders();
            }
        }

        private object eiCustomUploaders_ExportRequested()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();

            if (uploader != null)
            {
                if (string.IsNullOrEmpty(uploader.RequestURL))
                {
                    MessageBox.Show(Resources.UploadersConfigForm_eiCustomUploaders_ExportRequested_RequestURLMustBeConfigured, "ShareX - " + Resources.UploadersConfigForm_Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                if (uploader.DestinationType == CustomUploaderDestinationType.None)
                {
                    MessageBox.Show(Resources.UploadersConfigForm_eiCustomUploaders_ExportRequested_DestinationTypeMustBeConfigured, "ShareX - " + Resources.UploadersConfigForm_Error,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                eiCustomUploaders.DefaultFileName = uploader.GetFileName();
            }

            return uploader;
        }

        private void eiCustomUploaders_ImportRequested(object obj)
        {
            CustomUploaderItem uploader = obj as CustomUploaderItem;
            uploader.CheckBackwardCompatibility();
            CustomUploaderAdd(uploader);
        }

        private void eiCustomUploaders_ImportCompleted()
        {
            CustomUploaderUpdateList();
            CustomUploaderUpdateStates();
            lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
        }

        private void tsmiCustomUploaderGuide_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_CUSTOM_UPLOADER);
        }

        private void tsmiCustomUploaderExamples_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.URL_CUSTOM_UPLOADERS);
        }

        private void tsmiCustomUploaderExportAll_Click(object sender, EventArgs e)
        {
            CustomUploaderExportAll();
        }

        private void txtCustomUploaderName_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Name = txtCustomUploaderName.Text;

            CustomUploaderRefreshNames();
        }

        private void cbCustomUploaderRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.RequestMethod = (HttpMethod)cbCustomUploaderRequestMethod.SelectedIndex;
        }

        private void rtbCustomUploaderRequestURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.RequestURL = rtbCustomUploaderRequestURL.Text;

            CustomUploaderSyntaxHighlight(rtbCustomUploaderRequestURL);
            CustomUploaderRefreshNames();
        }

        private void cbCustomUploaderRequestFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Body = (CustomUploaderBody)cbCustomUploaderBody.SelectedIndex;

            CustomUploaderUpdateRequestFormatState();
        }

        private void dgvParameters_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvParameters);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Parameters = DataGridViewToDictionary(dgvParameters);
        }

        private void dgvHeaders_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvHeaders);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Headers = DataGridViewToDictionary(dgvHeaders);
        }

        private void dgvArguments_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvArguments);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Arguments = DataGridViewToDictionary(dgvArguments);
        }

        private void txtCustomUploaderFileForm_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.FileFormName = txtCustomUploaderFileFormName.Text;
        }

        private void rtbCustomUploaderData_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Data = rtbCustomUploaderData.Text;

            CustomUploaderSyntaxHighlight(rtbCustomUploaderData);
        }

        private void btnCustomUploaderDataBeautify_Click(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                if (uploader.Body == CustomUploaderBody.JSON)
                {
                    CustomUploaderFormatJsonData(Formatting.Indented);
                }
                else if (uploader.Body == CustomUploaderBody.XML)
                {
                    CustomUploaderFormatXMLData();
                }
            }
        }

        private void btnCustomUploaderDataMinify_Click(object sender, EventArgs e)
        {
            CustomUploaderFormatJsonData(Formatting.None);
        }

        private void txtCustomUploaderJsonPath_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderUpdateResponseState();
        }

        private void btnCustomUploadJsonPathHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://goessner.net/articles/JsonPath/");
        }

        private void btnCustomUploaderJsonAddSyntax_Click(object sender, EventArgs e)
        {
            string syntax = txtCustomUploaderJsonPath.Text;

            if (!string.IsNullOrEmpty(syntax))
            {
                if (syntax.StartsWith("$."))
                {
                    syntax = syntax.Substring(2);
                }

                AddTextToActiveURLField($"$json:{syntax}$");
            }
        }

        private void txtCustomUploaderXPath_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderUpdateResponseState();
        }

        private void btnCustomUploaderXPathHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("https://www.w3schools.com/xml/xpath_syntax.asp");
        }

        private void btnCustomUploaderXmlSyntaxAdd_Click(object sender, EventArgs e)
        {
            string syntax = txtCustomUploaderXPath.Text;

            if (!string.IsNullOrEmpty(syntax))
            {
                AddTextToActiveURLField($"$xml:{syntax}$");
            }
        }

        private void txtCustomUploaderRegexp_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderUpdateResponseState();
        }

        private void btnCustomUploaderRegexpAdd_Click(object sender, EventArgs e)
        {
            string regexp = txtCustomUploaderRegexp.Text;

            if (!string.IsNullOrEmpty(regexp))
            {
                lvCustomUploaderRegexps.Items.Add(regexp);
                txtCustomUploaderRegexp.Text = "";
                txtCustomUploaderRegexp.Focus();

                CustomUploaderItem uploader = CustomUploaderGetSelected();
                if (uploader != null)
                {
                    if (uploader.RegexList == null) uploader.RegexList = new List<string>();
                    uploader.RegexList.Add(regexp);
                }
            }
        }

        private void btnCustomUploaderRegexpRemove_Click(object sender, EventArgs e)
        {
            int index = lvCustomUploaderRegexps.SelectedIndex;
            if (index > -1)
            {
                lvCustomUploaderRegexps.Items.RemoveAt(index);

                CustomUploaderItem uploader = CustomUploaderGetSelected();
                if (uploader != null) uploader.RegexList.RemoveAt(index);
            }
        }

        private void btnCustomUploaderRegexpEdit_Click(object sender, EventArgs e)
        {
            string regexp = txtCustomUploaderRegexp.Text;
            if (!string.IsNullOrEmpty(regexp))
            {
                int index = lvCustomUploaderRegexps.SelectedIndex;
                if (index > -1)
                {
                    lvCustomUploaderRegexps.Items[index].Text = regexp;

                    CustomUploaderItem uploader = CustomUploaderGetSelected();
                    if (uploader != null) uploader.RegexList[index] = regexp;
                }
            }
        }

        private void btnCustomUploaderRegexHelp_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL("http://regexone.com");
        }

        private void lvCustomUploaderRegexps_SelectedIndexChanged(object sender, EventArgs e)
        {
            string regex = "";

            if (lvCustomUploaderRegexps.SelectedItems.Count > 0)
            {
                regex = lvCustomUploaderRegexps.SelectedItems[0].Text;
            }

            txtCustomUploaderRegexp.Text = regex;

            CustomUploaderUpdateResponseState();
        }

        private void btnCustomUploaderRegexAddSyntax_Click(object sender, EventArgs e)
        {
            if (lvCustomUploaderRegexps.SelectedIndices.Count > 0)
            {
                int selectedIndex = lvCustomUploaderRegexps.SelectedIndices[0];
                string regex = lvCustomUploaderRegexps.Items[selectedIndex].Text;

                if (!string.IsNullOrEmpty(regex))
                {
                    string syntax;
                    Match match = Regex.Match(regex, @"\((?:\?<(.+?)>)?.+?\)");

                    if (match.Success)
                    {
                        if (match.Groups.Count > 1 && !string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            syntax = string.Format("$regex:{0}|{1}$", selectedIndex + 1, match.Groups[1].Value);
                        }
                        else
                        {
                            syntax = string.Format("$regex:{0}|1$", selectedIndex + 1);
                        }
                    }
                    else
                    {
                        syntax = string.Format("$regex:{0}$", selectedIndex + 1);
                    }

                    AddTextToActiveURLField(syntax);
                }
            }
        }

        private void rtbCustomUploaderURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.URL;
        }

        private void rtbCustomUploaderURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.URL = rtbCustomUploaderURL.Text;
            CustomUploaderSyntaxHighlight(rtbCustomUploaderURL);
        }

        private void rtbCustomUploaderThumbnailURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.ThumbnailURL;
        }

        private void rtbCustomUploaderThumbnailURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.ThumbnailURL = rtbCustomUploaderThumbnailURL.Text;
            CustomUploaderSyntaxHighlight(rtbCustomUploaderThumbnailURL);
        }

        private void rtbCustomUploaderDeletionURL_Enter(object sender, EventArgs e)
        {
            customUploaderURLType = URLType.DeletionURL;
        }

        private void rtbCustomUploaderDeletionURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.DeletionURL = rtbCustomUploaderDeletionURL.Text;
            CustomUploaderSyntaxHighlight(rtbCustomUploaderDeletionURL);
        }

        private void txtCustomUploaderLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            URLHelpers.OpenURL(e.LinkText);
        }

        private void tsbCustomUploaderJSONFormat_Click(object sender, EventArgs e)
        {
            string response = txtCustomUploaderResponse.Text;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    response = Helpers.JSONFormat(response, Formatting.Indented);
                    txtCustomUploaderResponse.Text = response;
                }
                catch
                {
                    MessageBox.Show("Formatting failed.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbCustomUploaderXMLFormat_Click(object sender, EventArgs e)
        {
            string response = txtCustomUploaderResponse.Text;
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    response = Helpers.XMLFormat(response);
                    txtCustomUploaderResponse.Text = response;
                }
                catch
                {
                    MessageBox.Show("Formatting failed.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbCustomUploaderCopyResponseText_Click(object sender, EventArgs e)
        {
            string response = txtCustomUploaderResponse.Text;
            if (!string.IsNullOrEmpty(response))
            {
                ClipboardHelpers.CopyText(response);
            }
        }

        private void cbCustomUploaderImageUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomImageUploaderSelected = cbCustomUploaderImageUploader.SelectedIndex;
        }

        private async void btnCustomUploaderImageUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.ImageUploader, Config.CustomImageUploaderSelected);
        }

        private void cbCustomUploaderTextUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomTextUploaderSelected = cbCustomUploaderTextUploader.SelectedIndex;
        }

        private async void btnCustomUploaderTextUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.TextUploader, Config.CustomTextUploaderSelected);
        }

        private void cbCustomUploaderFileUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomFileUploaderSelected = cbCustomUploaderFileUploader.SelectedIndex;
        }

        private async void btnCustomUploaderFileUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.FileUploader, Config.CustomFileUploaderSelected);
        }

        private void cbCustomUploaderURLShortener_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomURLShortenerSelected = cbCustomUploaderURLShortener.SelectedIndex;
        }

        private async void btnCustomUploaderURLShortenerTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.URLShortener, Config.CustomURLShortenerSelected);
        }

        private void cbCustomUploaderURLSharingService_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomURLSharingServiceSelected = cbCustomUploaderURLSharingService.SelectedIndex;
        }

        private async void btnCustomUploaderURLSharingServiceTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.URLSharingService, Config.CustomURLSharingServiceSelected);
        }

        #endregion Form events
    }
}