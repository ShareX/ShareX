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

using Newtonsoft.Json.Serialization;
using ShareX.HelpersLib;
using ShareX.ImageEffectsLib.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ImageEffectsLib
{
    public partial class ImageEffectsForm : Form
    {
        public static bool IsInstanceActive => instance != null && !instance.IsDisposed;

        private static ImageEffectsForm instance;

        public event Action<Bitmap> ImageProcessRequested;

        public bool AutoGeneratePreviewImage { get; set; }
        public Bitmap PreviewImage { get; private set; }
        public List<ImageEffectPreset> Presets { get; private set; }
        public int SelectedPresetIndex { get; private set; }
        public string FilePath { get; private set; }

        private bool pauseUpdate;
        private ISerializationBinder serializationBinder = new ImageEffectsSerializationBinder();

        public ImageEffectsForm(Bitmap bmp, List<ImageEffectPreset> presets, int selectedPresetIndex)
        {
            pauseUpdate = true;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            PreviewImage = bmp;
            if (PreviewImage == null)
            {
                AutoGeneratePreviewImage = true;
            }

            Presets = presets;
            if (Presets.Count == 0)
            {
                Presets.Add(new ImageEffectPreset());
            }

            SelectedPresetIndex = selectedPresetIndex;

            AddAllEffectsToContextMenu();
            LoadSettings();
        }

        public static ImageEffectsForm GetFormInstance(List<ImageEffectPreset> presets, int selectedPresetIndex)
        {
            if (!IsInstanceActive)
            {
                instance = new ImageEffectsForm(null, presets, selectedPresetIndex);
            }

            return instance;
        }

        public void EnableToolMode(Action<Bitmap> imageProcessRequested, string filePath = null)
        {
            FilePath = filePath;
            ImageProcessRequested += imageProcessRequested;
            pbResult.AllowDrop = true;
            mbLoadImage.Visible = true;
            btnSaveImage.Visible = true;
            btnUploadImage.Visible = true;
        }

        public void EditorMode()
        {
            btnOK.Visible = true;
            btnClose.Text = Resources.ImageEffectsForm_EditorMode_Cancel;
        }

        public void ImportImageEffect(string json)
        {
            ImageEffectPreset preset = null;

            try
            {
                preset = JsonHelpers.DeserializeFromString<ImageEffectPreset>(json, serializationBinder);
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
                e.ShowError();
            }

            if (preset != null && preset.Effects.Count > 0)
            {
                AddPreset(preset);
            }
        }

        public void ImportImageEffectFile(string filePath)
        {
            try
            {
                string configJson = ImageEffectPackager.ExtractPackage(filePath, HelpersOptions.ShareXSpecialFolders["ShareXImageEffects"]);

                if (!string.IsNullOrEmpty(configJson))
                {
                    ImportImageEffect(configJson);
                }
            }
            catch (Exception ex)
            {
                ex.ShowError(false);
            }
        }

        protected void OnImageProcessRequested(Bitmap bmp)
        {
            ImageProcessRequested?.Invoke(bmp);
        }

        private void AddAllEffectsToContextMenu()
        {
            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Drawings,
                typeof(DrawBackground),
                typeof(DrawBackgroundImage),
                typeof(DrawBorder),
                typeof(DrawCheckerboard),
                typeof(DrawImage),
                typeof(DrawParticles),
                typeof(DrawTextEx),
                typeof(DrawText));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Manipulations,
                typeof(AutoCrop),
                typeof(Canvas),
                typeof(Crop),
                typeof(Flip),
                typeof(ForceProportions),
                typeof(Resize),
                typeof(Rotate),
                typeof(RoundedCorners),
                typeof(Scale),
                typeof(Skew));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Adjustments,
                typeof(Alpha),
                typeof(BlackWhite),
                typeof(Brightness),
                typeof(MatrixColor), // "Color matrix"
                typeof(Colorize),
                typeof(Contrast),
                typeof(Gamma),
                typeof(Grayscale),
                typeof(Hue),
                typeof(Inverse),
                typeof(Polaroid),
                typeof(ReplaceColor),
                typeof(Saturation),
                typeof(SelectiveColor),
                typeof(Sepia));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Filters,
                typeof(Blur),
                typeof(ColorDepth),
                typeof(MatrixConvolution), // "Convolution matrix"
                typeof(EdgeDetect),
                typeof(Emboss),
                typeof(GaussianBlur),
                typeof(Glow),
                typeof(MeanRemoval),
                typeof(Outline),
                typeof(Pixelate),
                typeof(Reflection),
                typeof(RGBSplit),
                typeof(Shadow),
                typeof(Sharpen),
                typeof(Slice),
                typeof(Smooth),
                typeof(TornEdge),
                typeof(WaveEdge));
        }

        private void AddEffectToContextMenu(string groupName, params Type[] imageEffects)
        {
            ToolStripMenuItem tsmiParent = new ToolStripMenuItem(groupName);
            tsmiParent.HideImageMargin();

            cmsEffects.Items.Add(tsmiParent);

            foreach (Type imageEffect in imageEffects)
            {
                ToolStripItem tsmiChild = tsmiParent.DropDownItems.Add(imageEffect.GetDescription().Replace("&", "&&"));
                tsmiChild.Tag = imageEffect;
                tsmiChild.Click += tsmiEffectClick;
            }
        }

        private void LoadSettings()
        {
            if (Presets != null && Presets.Count > 0)
            {
                foreach (ImageEffectPreset preset in Presets)
                {
                    ListViewItem lvi = new ListViewItem(preset.ToString());
                    lvPresets.Items.Add(lvi);
                }

                lvPresets.SelectedIndex = SelectedPresetIndex.Clamp(0, Presets.Count - 1);
            }

            UpdateControlStates();
        }

        private ImageEffectPreset GetSelectedPreset()
        {
            return GetSelectedPreset(out _);
        }

        private ImageEffectPreset GetSelectedPreset(out ListViewItem lvi)
        {
            int index = lvPresets.SelectedIndex;

            if (Presets.IsValidIndex(index))
            {
                lvi = lvPresets.Items[index];
                return Presets[index];
            }

            lvi = null;
            return null;
        }

        private void AddPreset()
        {
            AddPreset(new ImageEffectPreset());
        }

        private void AddPreset(ImageEffectPreset preset)
        {
            if (preset != null)
            {
                Presets.Add(preset);
                ListViewItem lvi = new ListViewItem(preset.ToString());
                lvPresets.Items.Add(lvi);
                lvPresets.SelectLast();
                txtPresetName.Focus();
            }
        }

        private void UpdatePreview()
        {
            if (!pauseUpdate)
            {
                ImageEffectPreset preset = GetSelectedPreset();

                if (preset != null)
                {
                    Cursor = Cursors.WaitCursor;

                    try
                    {
                        if (AutoGeneratePreviewImage)
                        {
                            GeneratePreviewImage(24);
                        }

                        if (PreviewImage != null)
                        {
                            //Debug.WriteLine("Updating preview...");

                            Stopwatch timer = Stopwatch.StartNew();

                            using (Image preview = ApplyEffects())
                            {
                                if (preview != null)
                                {
                                    pbResult.LoadImage(preview);
                                    Text = string.Format("ShareX - " + Resources.ImageEffectsForm_UpdatePreview_Image_effects___Width___0___Height___1___Render_time___2__ms,
                                        preview.Width, preview.Height, timer.ElapsedMilliseconds);
                                }
                                else
                                {
                                    pbResult.Reset();
                                    Text = string.Format("ShareX - " + Resources.ImageEffectsForm_UpdatePreview_Image_effects___Width___0___Height___1___Render_time___2__ms,
                                        0, 0, timer.ElapsedMilliseconds);
                                }
                            }
                        }
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    pbResult.Reset();
                }

                UpdateControlStates();
            }
        }

        private void UpdateControlStates()
        {
            btnPresetRemove.Enabled = btnPresetDuplicate.Enabled = txtPresetName.Enabled = btnEffectAdd.Enabled = lvPresets.SelectedItems.Count > 0;
            btnEffectClear.Enabled = btnEffectRefresh.Enabled = btnPackager.Enabled = lvEffects.Items.Count > 0;
            btnEffectRemove.Enabled = btnEffectDuplicate.Enabled = lvEffects.SelectedItems.Count > 0;
        }

        private void UpdateEffectName()
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                if (lvEffects.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = lvEffects.SelectedItems[0];

                    if (lvi.Tag is ImageEffect imageEffect)
                    {
                        string text = imageEffect.ToString();

                        if (lvi.Text != text)
                        {
                            lvi.Text = text;
                            txtEffectName.SetWatermark(imageEffect.ToString());
                        }
                    }
                }
            }
        }

        private void GeneratePreviewImage(int padding)
        {
            if (pbResult.ClientSize.Width > 0 && pbResult.ClientSize.Height > 0)
            {
                int size = Math.Min(pbResult.ClientSize.Width, pbResult.ClientSize.Height);
                int minSizePadding = 300;

                if (size < minSizePadding + (padding * 2))
                {
                    padding = 0;
                }

                size -= padding * 2;

                if (PreviewImage != null) PreviewImage.Dispose();
                PreviewImage = new Bitmap(size, size);

                Color backgroundColor;

                if (ShareXResources.UseCustomTheme)
                {
                    backgroundColor = ShareXResources.Theme.BackgroundColor;
                }
                else
                {
                    backgroundColor = Color.DarkGray;
                }

                using (Graphics g = Graphics.FromImage(PreviewImage))
                {
                    g.Clear(backgroundColor);

                    if (PreviewImage.Width > 260 && PreviewImage.Height > 260)
                    {
                        using (Bitmap logo = ShareXResources.Logo)
                        {
                            g.DrawImage(logo, (PreviewImage.Width / 2) - (logo.Width / 2), (PreviewImage.Height / 2) - (logo.Height / 2));
                        }
                    }
                }
            }
        }

        private Bitmap ApplyEffects()
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                return preset.ApplyEffects(PreviewImage);
            }

            return null;
        }

        private void tsmiEffectClick(object sender, EventArgs e)
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                if (sender is ToolStripMenuItem tsmi && tsmi.Tag is Type type)
                {
                    ImageEffect imageEffect = (ImageEffect)Activator.CreateInstance(type);
                    AddEffect(imageEffect, preset);
                    UpdatePreview();
                }
            }
        }

        private void RemoveSelectedEffects()
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                int index = lvEffects.SelectedIndex;

                if (index > -1)
                {
                    preset.Effects.RemoveAt(index);
                    lvEffects.Items.RemoveAt(index);

                    if (lvEffects.Items.Count > 0)
                    {
                        lvEffects.SelectedIndex = index == lvEffects.Items.Count ? lvEffects.Items.Count - 1 : index;
                    }

                    UpdatePreview();
                }
            }
        }

        private void ClearFields()
        {
            txtPresetName.Text = "";
            lvEffects.Items.Clear();
            ClearSelectedEffect();
            UpdatePreview();
        }

        private void ClearSelectedEffect()
        {
            txtEffectName.Text = "";
            txtEffectName.SetWatermark("");
            pgSettings.SelectedObject = null;
        }

        private void AddEffect(ImageEffect imageEffect, ImageEffectPreset preset = null)
        {
            ListViewItem lvi = new ListViewItem(imageEffect.ToString());
            lvi.Checked = imageEffect.Enabled;
            lvi.Tag = imageEffect;

            if (lvEffects.SelectedIndices.Count > 0)
            {
                int index = lvEffects.SelectedIndices[lvEffects.SelectedIndices.Count - 1] + 1;
                lvEffects.Items.Insert(index, lvi);

                if (preset != null)
                {
                    preset.Effects.Insert(index, imageEffect);
                }
            }
            else
            {
                lvEffects.Items.Add(lvi);

                if (preset != null)
                {
                    preset.Effects.Add(imageEffect);
                }
            }

            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        private void LoadPreset(ImageEffectPreset preset)
        {
            txtPresetName.Text = preset.Name;
            lvEffects.Items.Clear();
            ClearSelectedEffect();

            foreach (ImageEffect imageEffect in preset.Effects)
            {
                AddEffect(imageEffect);
            }

            UpdatePreview();
        }

        #region Form events

        private void ImageEffectsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();

            pauseUpdate = false;
            lvPresets.EnsureSelectedVisible();
            UpdatePreview();
        }

        private void ImageEffectsForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                if (e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Any(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxie")))
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

        private void ImageEffectsForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files)
            {
                foreach (string filePath in files.Where(x => !string.IsNullOrEmpty(x) && x.EndsWith(".sxie")))
                {
                    ImportImageEffectFile(filePath);
                }
            }
        }

        private void btnPresetNew_Click(object sender, EventArgs e)
        {
            AddPreset();
        }

        private void btnPresetRemove_Click(object sender, EventArgs e)
        {
            int selected = lvPresets.SelectedIndex;

            if (selected > -1)
            {
                lvPresets.Items.RemoveAt(selected);
                Presets.RemoveAt(selected);

                if (lvPresets.Items.Count > 0)
                {
                    lvPresets.SelectedIndex = selected == lvPresets.Items.Count ? lvPresets.Items.Count - 1 : selected;
                }
                else
                {
                    ClearFields();
                    btnPresetNew.Focus();
                }
            }
        }

        private void btnPresetDuplicate_Click(object sender, EventArgs e)
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                ImageEffectPreset presetClone = preset.Copy();
                AddPreset(presetClone);
            }
        }

        private void lvPresets_ItemMoving(object sender, int oldIndex, int newIndex)
        {
            Presets.Move(oldIndex, newIndex);
        }

        private void lvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedPresetIndex = lvPresets.SelectedIndex;

            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                LoadPreset(preset);
            }
        }

        private void txtPresetName_TextChanged(object sender, EventArgs e)
        {
            ImageEffectPreset preset = GetSelectedPreset(out ListViewItem lvi);

            if (preset != null)
            {
                preset.Name = txtPresetName.Text;
                lvi.Text = preset.ToString();
            }
        }

        private void btnEffectAdd_Click(object sender, EventArgs e)
        {
            cmsEffects.Show(btnEffectAdd, 0, btnEffectAdd.Height + 1);
        }

        private void btnEffectRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedEffects();
        }

        private void btnEffectDuplicate_Click(object sender, EventArgs e)
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                if (lvEffects.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = lvEffects.SelectedItems[0];

                    if (lvi.Tag is ImageEffect imageEffect)
                    {
                        ImageEffect imageEffectClone = imageEffect.Copy();
                        AddEffect(imageEffectClone, preset);
                        UpdatePreview();
                    }
                }
            }
        }

        private void btnEffectClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.WouldYouLikeToClearEffects, "ShareX - " + Resources.Confirmation, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                ImageEffectPreset preset = GetSelectedPreset();

                if (preset != null)
                {
                    lvEffects.Items.Clear();
                    preset.Effects.Clear();
                    ClearSelectedEffect();
                    UpdatePreview();
                }
            }
        }

        private void btnEffectRefresh_Click(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void txtEffectName_TextChanged(object sender, EventArgs e)
        {
            if (lvEffects.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEffects.SelectedItems[0];

                if (lvi.Tag is ImageEffect imageEffect)
                {
                    imageEffect.Name = txtEffectName.Text;
                    lvi.Text = imageEffect.ToString();
                }
            }
        }

        private void lvEffects_ItemMoved(object sender, int oldIndex, int newIndex)
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                preset.Effects.Move(oldIndex, newIndex);
                UpdatePreview();
            }
        }

        private void lvEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearSelectedEffect();

            if (lvEffects.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEffects.SelectedItems[0];

                if (lvi.Tag is ImageEffect imageEffect)
                {
                    txtEffectName.Text = imageEffect.Name;
                    txtEffectName.SetWatermark(imageEffect.ToString());
                    pgSettings.SelectedObject = imageEffect;
                }
            }

            UpdateControlStates();
        }

        private void lvEffects_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item != null && e.Item.Focused && e.Item.Tag is ImageEffect imageEffect)
            {
                imageEffect.Enabled = e.Item.Checked;
                UpdatePreview();
            }
        }

        private void lvEffects_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Delete:
                    RemoveSelectedEffects();
                    e.SuppressKeyPress = true;
                    break;
                case Keys.F5:
                    UpdatePreview();
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void pgSettings_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateEffectName();
            UpdatePreview();
        }

        private void btnPackager_Click(object sender, EventArgs e)
        {
            ImageEffectPreset preset = GetSelectedPreset();

            if (preset != null)
            {
                if (string.IsNullOrEmpty(preset.Name))
                {
                    MessageBox.Show(Resources.PresetNameCannotBeEmpty, "ShareX - " + Resources.MissingPresetName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    string json = null;

                    try
                    {
                        json = JsonHelpers.SerializeToString(preset, serializationBinder: serializationBinder);
                    }
                    catch (Exception ex)
                    {
                        DebugHelper.WriteException(ex);
                        ex.ShowError();
                    }

                    if (!string.IsNullOrEmpty(json))
                    {
                        using (ImageEffectPackagerForm packagerForm = new ImageEffectPackagerForm(json, preset.Name,
                            HelpersOptions.ShareXSpecialFolders["ShareXImageEffects"]))
                        {
                            packagerForm.ShowDialog();
                        }
                    }
                }
            }
        }

        private void btnImageEffects_Click(object sender, EventArgs e)
        {
            URLHelpers.OpenURL(Links.ImageEffects);
        }

        private void tsmiLoadImageFromFile_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filePath))
            {
                if (PreviewImage != null) PreviewImage.Dispose();
                PreviewImage = ImageHelpers.LoadImage(filePath);
                FilePath = filePath;
                UpdatePreview();
            }
        }

        private void tsmiLoadImageFromClipboard_Click(object sender, EventArgs e)
        {
            Bitmap bmp = ClipboardHelpers.GetImage();

            if (bmp != null)
            {
                if (PreviewImage != null) PreviewImage.Dispose();
                PreviewImage = bmp;
                FilePath = null;
                UpdatePreview();
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                using (Image img = ApplyEffects())
                {
                    if (img != null)
                    {
                        string filePath = ImageHelpers.SaveImageFileDialog(img, FilePath);

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            FilePath = filePath;
                        }
                    }
                }
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            if (PreviewImage != null)
            {
                Bitmap bmp = ApplyEffects();

                if (bmp != null)
                {
                    OnImageProcessRequested(bmp);
                }
            }
        }

        private void pbResult_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.Bitmap, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pbResult_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                if (e.Data.GetData(DataFormats.FileDrop, false) is string[] files && files.Length > 0 && FileHelpers.IsImageFile(files[0]))
                {
                    if (PreviewImage != null) PreviewImage.Dispose();
                    PreviewImage = ImageHelpers.LoadImage(files[0]);
                    UpdatePreview();
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap, false))
            {
                if (e.Data.GetData(DataFormats.Bitmap, false) is Bitmap bmp)
                {
                    if (PreviewImage != null) PreviewImage.Dispose();
                    PreviewImage = bmp;
                    UpdatePreview();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Form events
    }
}