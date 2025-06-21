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
        private UploadResult lastResult;

        public CustomUploaderSettingsForm(UploadersConfig config)
        {
            Config = config;

            InitializeComponent();

            /*
            CodeMenuItem[] inputCodeMenuItems = new CodeMenuItem[]
            {
                new CodeMenuItem("{input}", "Text/URL input"),
                new CodeMenuItem("{filename}", "File name"),
                new CodeMenuItem("{random:input1|input2}", "Random selection from list"),
                new CodeMenuItem("{select:input1|input2}", "Lets user to select one input from list"),
                new CodeMenuItem("{inputbox:title|default_value}", "Lets user to input text"),
                new CodeMenuItem("{base64:input}", "Base64 encode input")
            };
            */

            CodeMenuItem[] outputCodeMenuItems = new CodeMenuItem[]
            {
                new CodeMenuItem("{response}", "Response text"),
                new CodeMenuItem("{responseurl}", "Response/Redirection URL"),
                new CodeMenuItem("{header:header_name}", "Response header"),
                new CodeMenuItem("{json:path}", "Parse JSON response using JSONPath"),
                new CodeMenuItem("{xml:path}", "Parse XML response using XPath"),
                new CodeMenuItem("{regex:pattern|group}", "Parse response using Regex"),
                new CodeMenuItem("{filename}", "File name used when uploading"),
                new CodeMenuItem("{random:input1|input2}", "Random selection from list"),
                new CodeMenuItem("{select:input1|input2}", "Lets user to select one input from list"),
                new CodeMenuItem("{inputbox:title|default_value}", "Lets user to input text"),
                new CodeMenuItem("{outputbox:title|text}", "Lets user to output text"),
                new CodeMenuItem("{base64:input}", "Base64 encode input")
            };

            new CodeMenu(rtbResultURL, outputCodeMenuItems);
            new CodeMenu(rtbResultThumbnailURL, outputCodeMenuItems);
            new CodeMenu(rtbResultDeletionURL, outputCodeMenuItems);
            new CodeMenu(rtbResultErrorMessage, outputCodeMenuItems);

            rtbRequestURL.AddContextMenu();
            rtbData.AddContextMenu();
            rtbResultURL.AddContextMenu();
            rtbResultThumbnailURL.AddContextMenu();
            rtbResultDeletionURL.AddContextMenu();
            rtbResultErrorMessage.AddContextMenu();
            eiCustomUploaders.ObjectType = typeof(CustomUploaderItem);
            CustomUploaderAddDestinationTypes();
            cbRequestMethod.Items.AddRange(Enum.GetNames(typeof(HttpMethod)));
            cbBody.Items.AddRange(Helpers.GetEnumDescriptions<CustomUploaderBody>());

            ShareXResources.ApplyTheme(this, true);

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
            CustomUploaderAdd(CustomUploaderItem.Init());
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
            txtName.Text = uploader.Name ?? "";
            txtName.SetWatermark(URLHelpers.GetHostName(uploader.RequestURL) ?? "");
            CustomUploaderSetDestinationType(uploader.DestinationType);

            cbRequestMethod.SelectedIndex = (int)uploader.RequestMethod;
            rtbRequestURL.Text = uploader.RequestURL ?? "";
            CustomUploaderSyntaxHighlight(rtbRequestURL);

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

            cbBody.SelectedIndex = (int)uploader.Body;

            dgvArguments.Rows.Clear();
            if (uploader.Arguments != null)
            {
                foreach (KeyValuePair<string, string> arg in uploader.Arguments)
                {
                    dgvArguments.Rows.Add(new string[] { arg.Key, arg.Value });
                }
            }

            txtFileFormName.Text = uploader.FileFormName ?? "";

            rtbData.Text = uploader.Data ?? "";

            rtbResultURL.Text = uploader.URL;
            CustomUploaderSyntaxHighlight(rtbResultURL);
            rtbResultThumbnailURL.Text = uploader.ThumbnailURL;
            CustomUploaderSyntaxHighlight(rtbResultThumbnailURL);
            rtbResultDeletionURL.Text = uploader.DeletionURL;
            CustomUploaderSyntaxHighlight(rtbResultDeletionURL);
            rtbResultErrorMessage.Text = uploader.ErrorMessage;
            CustomUploaderSyntaxHighlight(rtbResultErrorMessage);

            CustomUploaderUpdateStates();
        }

        private void CustomUploaderUpdateStates()
        {
            btnRemove.Enabled = btnDuplicate.Enabled = pMain.Visible = CustomUploaderCheck(lbCustomUploaderList.SelectedIndex);

            tsmiExportAll.Enabled = tsmiClearUploaders.Enabled = cbImageUploader.Enabled =
                btnImageUploaderTest.Enabled = cbTextUploader.Enabled = btnTextUploaderTest.Enabled =
                cbFileUploader.Enabled = btnFileUploaderTest.Enabled = cbURLShortener.Enabled =
                btnURLShortenerTest.Enabled = cbURLSharingService.Enabled = btnURLSharingServiceTest.Enabled =
                lbCustomUploaderList.Items.Count > 0;

            CustomUploaderUpdateBodyState();
        }

        private void CustomUploaderUpdateBodyState()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();

            if (uploader != null)
            {
                pBodyArguments.Visible = uploader.Body == CustomUploaderBody.MultipartFormData ||
                    uploader.Body == CustomUploaderBody.FormURLEncoded;
                lblFileFormName.Visible = txtFileFormName.Visible = uploader.Body == CustomUploaderBody.MultipartFormData;
                pBodyData.Visible = uploader.Body == CustomUploaderBody.JSON || uploader.Body == CustomUploaderBody.XML;
                btnDataMinify.Visible = uploader.Body == CustomUploaderBody.JSON;
            }
        }

        private void CustomUploaderRefreshNames()
        {
            int index = lbCustomUploaderList.SelectedIndex;

            if (index >= 0)
            {
                customUploaderPauseLoad = true;

                lbCustomUploaderList.Items[index] = lbCustomUploaderList.Items[index];
                cbImageUploader.Items[index] = cbImageUploader.Items[index];
                cbTextUploader.Items[index] = cbTextUploader.Items[index];
                cbFileUploader.Items[index] = cbFileUploader.Items[index];
                cbURLShortener.Items[index] = cbURLShortener.Items[index];
                cbURLSharingService.Items[index] = cbURLSharingService.Items[index];

                customUploaderPauseLoad = false;
            }
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
            btnNew.Focus();
        }

        private void CustomUploaderClearFields()
        {
            CustomUploaderLoad(CustomUploaderItem.Init());
        }

        private void CustomUploaderSerialize(CustomUploaderItem cui, string folderPath)
        {
            try
            {
                string filePath = Path.Combine(folderPath, cui.GetFileName());
                JsonHelpers.SerializeToFile(cui, filePath, DefaultValueHandling.Ignore, NullValueHandling.Ignore);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                MessageBox.Show(Resources.ExportFailed + "\n\n" + e, "ShareX - " + "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomUploaderExportAll()
        {
            if (Config.CustomUploadersList != null && Config.CustomUploadersList.Count > 0)
            {
                using (FolderSelectDialog fsd = new FolderSelectDialog())
                {
                    if (fsd.ShowDialog())
                    {
                        foreach (CustomUploaderItem cui in Config.CustomUploadersList)
                        {
                            CustomUploaderSerialize(cui, fsd.FileName);
                        }
                    }
                }
            }
        }

        private void CustomUploaderUpdateFolder()
        {
            using (FolderSelectDialog fsd = new FolderSelectDialog())
            {
                if (fsd.ShowDialog())
                {
                    string folderPath = fsd.FileName;
                    string[] files = Directory.GetFiles(folderPath, "*.sxcu", SearchOption.TopDirectoryOnly);

                    int updated = 0;

                    if (files != null)
                    {
                        foreach (string filePath in files)
                        {
                            CustomUploaderItem cui = JsonHelpers.DeserializeFromFile<CustomUploaderItem>(filePath);

                            if (cui != null)
                            {
                                try
                                {
                                    cui.CheckBackwardCompatibility();
                                    CustomUploaderSerialize(cui, folderPath);
                                    updated++;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    MessageBox.Show($"{updated} custom uploader files updated.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            if (HelpersOptions.DevMode)
            {
                tsmiExportAll.Visible = true;
                tsmiUpdateFolder.Visible = true;
            }

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
                    ToolStripMenuItem tsmi2 = (ToolStripMenuItem)cmsDestinationType.Items[index];
                    tsmi2.Checked = !tsmi2.Checked;

                    CustomUploaderItem uploader = CustomUploaderGetSelected();
                    if (uploader != null)
                    {
                        uploader.DestinationType = CustomUploaderGetDestinationType();
                    }

                    CustomUploaderDestinationTypeUpdate();
                };

                cmsDestinationType.Items.Add(tsmi);
            }

            cmsDestinationType.Closing += (sender, e) => e.Cancel = e.CloseReason == ToolStripDropDownCloseReason.ItemClicked;
        }

        private void CustomUploaderSetDestinationType(CustomUploaderDestinationType destinationType)
        {
            for (int i = 0; i < cmsDestinationType.Items.Count; i++)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)cmsDestinationType.Items[i];
                tsmi.Checked = destinationType.HasFlag(1 << i);
            }

            CustomUploaderDestinationTypeUpdate();
        }

        private CustomUploaderDestinationType CustomUploaderGetDestinationType()
        {
            CustomUploaderDestinationType destinationType = CustomUploaderDestinationType.None;

            for (int i = 0; i < cmsDestinationType.Items.Count; i++)
            {
                ToolStripMenuItem tsmi = (ToolStripMenuItem)cmsDestinationType.Items[i];

                if (tsmi.Checked)
                {
                    destinationType |= (CustomUploaderDestinationType)(1 << i);
                }
            }

            return destinationType;
        }

        private void CheckDataGridView(DataGridView dgv, bool checkDuplicate)
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
                else if (checkDuplicate)
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
                        cell.ErrorText = Resources.DuplicateNameNotAllowed;
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
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                DataGridViewRow row = dgv.Rows[i];
                string key = row.Cells[0].Value?.ToString();

                if (!string.IsNullOrEmpty(key) && !dictionary.ContainsKey(key))
                {
                    string value = row.Cells[1].Value?.ToString();

                    dictionary.Add(key, value);
                }
            }

            return dictionary;
        }

        private void CustomUploaderDestinationTypeUpdate()
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                if (uploader.DestinationType == CustomUploaderDestinationType.None)
                {
                    mbDestinationType.Text = CustomUploaderDestinationType.None.GetLocalizedDescription();
                }
                else
                {
                    mbDestinationType.Text = string.Join(", ", uploader.DestinationType.GetFlags().Select(x => x.GetLocalizedDescription()));
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
            cbImageUploader.Items.Clear();
            cbTextUploader.Items.Clear();
            cbFileUploader.Items.Clear();
            cbURLShortener.Items.Clear();
            cbURLSharingService.Items.Clear();

            if (Config.CustomUploadersList.Count > 0)
            {
                foreach (CustomUploaderItem item in Config.CustomUploadersList)
                {
                    cbImageUploader.Items.Add(item);
                    cbTextUploader.Items.Add(item);
                    cbFileUploader.Items.Add(item);
                    cbURLShortener.Items.Add(item);
                    cbURLSharingService.Items.Add(item);
                }

                cbImageUploader.SelectedIndex = Config.CustomImageUploaderSelected.Clamp(0, Config.CustomUploadersList.Count - 1);
                cbTextUploader.SelectedIndex = Config.CustomTextUploaderSelected.Clamp(0, Config.CustomUploadersList.Count - 1);
                cbFileUploader.SelectedIndex = Config.CustomFileUploaderSelected.Clamp(0, Config.CustomUploadersList.Count - 1);
                cbURLShortener.SelectedIndex = Config.CustomURLShortenerSelected.Clamp(0, Config.CustomUploadersList.Count - 1);
                cbURLSharingService.SelectedIndex = Config.CustomURLSharingServiceSelected.Clamp(0, Config.CustomUploadersList.Count - 1);
            }
        }

        private async Task TestCustomUploader(CustomUploaderDestinationType type, int index)
        {
            if (!Config.CustomUploadersList.IsValidIndex(index))
            {
                return;
            }

            btnImageUploaderTest.Enabled = btnTextUploaderTest.Enabled = btnFileUploaderTest.Enabled =
                btnURLShortenerTest.Enabled = btnURLSharingServiceTest.Enabled = false;
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
                                result.Errors.Add(imageUploader.Errors);
                            }
                            break;
                        case CustomUploaderDestinationType.TextUploader:
                            CustomTextUploader textUploader = new CustomTextUploader(item);
                            using (TextUploadForm form = new TextUploadForm("ShareX text upload test"))
                            {
                                if (form.ShowDialog() == DialogResult.OK)
                                {
                                    string text = form.Content;

                                    if (!string.IsNullOrEmpty(text))
                                    {
                                        result = textUploader.UploadText(text, "Test.txt");
                                        result.Errors.Add(textUploader.Errors);
                                    }
                                }
                            }
                            break;
                        case CustomUploaderDestinationType.FileUploader:
                            using (Stream stream = ShareXResources.Logo.GetStream())
                            {
                                CustomFileUploader fileUploader = new CustomFileUploader(item);
                                result = fileUploader.Upload(stream, "Test.png");
                                result.Errors.Add(fileUploader.Errors);
                            }
                            break;
                        case CustomUploaderDestinationType.URLShortener:
                            CustomURLShortener urlShortener = new CustomURLShortener(item);
                            result = urlShortener.ShortenURL(Links.Website);
                            result.Errors.Add(urlShortener.Errors);
                            break;
                        case CustomUploaderDestinationType.URLSharingService:
                            CustomURLSharer urlSharer = new CustomURLSharer(item);
                            result = urlSharer.ShareURL(Links.Website);
                            result.Errors.Add(urlSharer.Errors);
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
                lastResult = result;

                if (result != null)
                {
                    ResponseForm.ShowInstance(result);
                }

                btnImageUploaderTest.Enabled = btnTextUploaderTest.Enabled = btnFileUploaderTest.Enabled =
                    btnURLShortenerTest.Enabled = btnURLSharingServiceTest.Enabled = true;
            }
        }

        private void CustomUploaderSyntaxHighlight(RichTextBox rtb)
        {
            string text = rtb.Text;

            if (!string.IsNullOrEmpty(text))
            {
                int start = rtb.SelectionStart;
                int length = rtb.SelectionLength;
                rtb.BeginUpdate();

                rtb.SelectionStart = 0;
                rtb.SelectionLength = rtb.TextLength;
                rtb.SelectionColor = rtb.ForeColor;

                ShareXCustomUploaderSyntaxParser parser = new ShareXCustomUploaderSyntaxParser();

                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];

                    if (c == parser.SyntaxStart || c == parser.SyntaxEnd || c == parser.SyntaxParameterStart ||
                        c == parser.SyntaxParameterDelimiter || c == parser.SyntaxEscape)
                    {
                        rtb.SelectionStart = i;
                        rtb.SelectionLength = 1;
                        rtb.SelectionColor = Color.Lime;
                    }
                }

                rtb.SelectionStart = start;
                rtb.SelectionLength = length;
                rtb.EndUpdate();
            }
        }

        private void CustomUploaderFormatJsonData(Formatting formatting)
        {
            string json = rtbData.Text;

            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    rtbData.Text = Helpers.JSONFormat(json, formatting);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CustomUploaderFormatXMLData()
        {
            string xml = rtbData.Text;

            if (!string.IsNullOrEmpty(xml))
            {
                try
                {
                    rtbData.Text = Helpers.XMLFormat(xml);
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
                if (e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Any(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxcu")))
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files)
            {
                foreach (string filePath in files.Where(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxcu")))
                {
                    CustomUploaderItem cui = JsonHelpers.DeserializeFromFile<CustomUploaderItem>(filePath);

                    if (cui != null)
                    {
                        try
                        {
                            cui.CheckBackwardCompatibility();
                            CustomUploaderAdd(cui);
                        }
                        catch (Exception ex)
                        {
                            ex.ShowError(false);
                        }
                    }
                }

                eiCustomUploaders_ImportCompleted();
            }
        }

        private void btnCustomUploaderNew_Click(object sender, EventArgs e)
        {
            CustomUploaderNew();
            lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
            txtName.Focus();
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
                    btnNew.Focus();
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
            CustomUploaderItem cui = obj as CustomUploaderItem;

            try
            {
                cui.CheckBackwardCompatibility();
                CustomUploaderAdd(cui);
            }
            catch (Exception e)
            {
                e.ShowError(false);
            }
        }

        private void eiCustomUploaders_ImportCompleted()
        {
            CustomUploaderUpdateList();
            CustomUploaderUpdateStates();
            lbCustomUploaderList.SelectedIndex = lbCustomUploaderList.Items.Count - 1;
        }

        private void tsmiCustomUploaderGuide_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.DocsCustomUploader);
        }

        private void tsmiClearUploaders_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.UploadersConfigForm_Remove_all_custom_uploaders_Confirmation, "ShareX",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CustomUploaderClearUploaders();
            }
        }

        private void tsmiCustomUploaderExportAll_Click(object sender, EventArgs e)
        {
            CustomUploaderExportAll();
        }

        private void tsmiUpdateFolder_Click(object sender, EventArgs e)
        {
            CustomUploaderUpdateFolder();
        }

        private void txtCustomUploaderName_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Name = txtName.Text;

            CustomUploaderRefreshNames();
        }

        private void cbCustomUploaderRequestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.RequestMethod = (HttpMethod)cbRequestMethod.SelectedIndex;
        }

        private void rtbCustomUploaderRequestURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null)
            {
                uploader.RequestURL = rtbRequestURL.Text;
                txtName.SetWatermark(URLHelpers.GetHostName(uploader.RequestURL));
            }

            CustomUploaderSyntaxHighlight(rtbRequestURL);
            CustomUploaderRefreshNames();
        }

        private void cbCustomUploaderRequestFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Body = (CustomUploaderBody)cbBody.SelectedIndex;

            CustomUploaderUpdateBodyState();
        }

        private void dgv_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (((DataGridView)sender).CurrentCell.ColumnIndex == 1)
            {
                if (e.Control is TextBox tb)
                {
                    if (tb.AutoCompleteCustomSource == null || tb.AutoCompleteCustomSource.Count == 0)
                    {
                        AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                        col.Add("{input}");
                        col.Add("{filename}");
                        col.Add("{random:");
                        col.Add("{select:");
                        col.Add("{prompt:");
                        col.Add("{base64:");

                        tb.AutoCompleteCustomSource = col;
                        tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    }

                    tb.AutoCompleteMode = AutoCompleteMode.Suggest;
                }
            }
            else
            {
                if (e.Control is TextBox tb)
                {
                    tb.AutoCompleteMode = AutoCompleteMode.None;
                }
            }
        }

        private void dgvParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvParameters, true);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Parameters = DataGridViewToDictionary(dgvParameters);
        }

        private void dgvHeaders_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvHeaders, true);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Headers = DataGridViewToDictionary(dgvHeaders);
        }

        private void dgvArguments_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CheckDataGridView(dgvArguments, true);

            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Arguments = DataGridViewToDictionary(dgvArguments);
        }

        private void txtCustomUploaderFileForm_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.FileFormName = txtFileFormName.Text;
        }

        private void rtbCustomUploaderData_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.Data = rtbData.Text;
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

        private void btnTestURLSyntax_Click(object sender, EventArgs e)
        {
            using (CustomUploaderSyntaxTestForm syntaxTestForm = new CustomUploaderSyntaxTestForm(lastResult?.ResponseInfo, rtbResultURL.Text))
            {
                syntaxTestForm.ShowDialog();
            }
        }

        private void rtbCustomUploaderURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.URL = rtbResultURL.Text;
            CustomUploaderSyntaxHighlight(rtbResultURL);
        }

        private void rtbCustomUploaderThumbnailURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.ThumbnailURL = rtbResultThumbnailURL.Text;
            CustomUploaderSyntaxHighlight(rtbResultThumbnailURL);
        }

        private void rtbCustomUploaderDeletionURL_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.DeletionURL = rtbResultDeletionURL.Text;
            CustomUploaderSyntaxHighlight(rtbResultDeletionURL);
        }

        private void rtbResultErrorMessage_TextChanged(object sender, EventArgs e)
        {
            CustomUploaderItem uploader = CustomUploaderGetSelected();
            if (uploader != null) uploader.ErrorMessage = rtbResultErrorMessage.Text;
            CustomUploaderSyntaxHighlight(rtbResultErrorMessage);
        }

        private void cbCustomUploaderImageUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomImageUploaderSelected = cbImageUploader.SelectedIndex;
        }

        private async void btnCustomUploaderImageUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.ImageUploader, Config.CustomImageUploaderSelected);
        }

        private void cbCustomUploaderTextUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomTextUploaderSelected = cbTextUploader.SelectedIndex;
        }

        private async void btnCustomUploaderTextUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.TextUploader, Config.CustomTextUploaderSelected);
        }

        private void cbCustomUploaderFileUploader_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomFileUploaderSelected = cbFileUploader.SelectedIndex;
        }

        private async void btnCustomUploaderFileUploaderTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.FileUploader, Config.CustomFileUploaderSelected);
        }

        private void cbCustomUploaderURLShortener_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomURLShortenerSelected = cbURLShortener.SelectedIndex;
        }

        private async void btnCustomUploaderURLShortenerTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.URLShortener, Config.CustomURLShortenerSelected);
        }

        private void cbCustomUploaderURLSharingService_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.CustomURLSharingServiceSelected = cbURLSharingService.SelectedIndex;
        }

        private async void btnCustomUploaderURLSharingServiceTest_Click(object sender, EventArgs e)
        {
            await TestCustomUploader(CustomUploaderDestinationType.URLSharingService, Config.CustomURLSharingServiceSelected);
        }

        #endregion Form events
    }
}