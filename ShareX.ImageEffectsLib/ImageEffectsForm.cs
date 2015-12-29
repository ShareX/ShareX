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
    public partial class ImageEffectsForm : BaseForm
    {
        public Image DefaultImage { get; private set; }

        public List<ImageEffect> Effects { get; private set; }

        public ImageEffectsForm(Image img, List<ImageEffect> effects = null)
        {
            InitializeComponent();
            DefaultImage = img;
            eiImageEffects.ObjectType = typeof(List<ImageEffect>);
            AddAllEffectsToContextMenu();

            if (effects != null)
            {
                AddEffects(effects.Copy());
            }

            UpdatePreview();
        }

        public void EditorMode()
        {
            pbResult.AllowDrop = true;
            mbLoadImage.Visible = true;
            btnSaveImage.Visible = true;
        }

        private void AddAllEffectsToContextMenu()
        {
            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Drawings,
                typeof(DrawBackground),
                typeof(DrawBorder),
                typeof(DrawCheckerboard),
                typeof(DrawImage),
                typeof(DrawText));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Manipulations,
                typeof(Canvas),
                typeof(Crop),
                typeof(Flip),
                typeof(Resize),
                typeof(Rotate),
                typeof(RoundedCorners),
                typeof(Scale),
                typeof(Skew));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Adjustments,
                typeof(Alpha),
                typeof(BlackWhite),
                typeof(Brightness),
                typeof(Colorize),
                typeof(Contrast),
                typeof(Gamma),
                typeof(Grayscale),
                typeof(Hue),
                typeof(Inverse),
                typeof(MatrixColor),
                typeof(Polaroid),
                typeof(Saturation),
                typeof(Sepia));

            AddEffectToContextMenu(Resources.ImageEffectsForm_AddAllEffectsToTreeView_Filters,
                typeof(Blur),
                typeof(EdgeDetect),
                typeof(Emboss),
                typeof(GaussianBlur),
                typeof(MatrixConvolution),
                typeof(MeanRemoval),
                typeof(Outline),
                typeof(Pixelate),
                typeof(Reflection),
                typeof(Shadow),
                typeof(Sharpen),
                typeof(Smooth),
                typeof(TornEdge));
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

        private void UpdatePreview()
        {
            if (DefaultImage != null)
            {
                Stopwatch timer = Stopwatch.StartNew();

                using (Image preview = ApplyEffects())
                {
                    pbResult.LoadImage(preview);
                    Text = string.Format("ShareX - " + Resources.ImageEffectsForm_UpdatePreview_Image_effects___Width___0___Height___1___Render_time___2__ms,
                        preview.Width, preview.Height, timer.ElapsedMilliseconds);
                }
            }
        }

        private List<ImageEffect> GetImageEffects()
        {
            return lvEffects.Items.Cast<ListViewItem>().Where(x => x != null && x.Tag is ImageEffect).Select(x => (ImageEffect)x.Tag).ToList();
        }

        private Image ApplyEffects()
        {
            List<ImageEffect> imageEffects = GetImageEffects();
            return ImageEffectManager.ApplyEffects(DefaultImage, imageEffects);
        }

        private void tsmiEffectClick(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (tsmi != null && tsmi.Tag is Type)
            {
                Type type = (Type)tsmi.Tag;
                ImageEffect imageEffect = (ImageEffect)Activator.CreateInstance(type);
                AddEffect(imageEffect);
                UpdatePreview();
            }
        }

        private void RemoveSelectedEffects()
        {
            if (lvEffects.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvEffects.SelectedItems)
                {
                    lvi.Remove();
                }

                UpdatePreview();
            }
        }

        private void ClearEffects()
        {
            foreach (ListViewItem lvi in lvEffects.Items)
            {
                lvi.Remove();
            }

            UpdatePreview();
        }

        private void AddEffect(ImageEffect imageEffect)
        {
            ListViewItem lvi = new ListViewItem(imageEffect.GetType().GetDescription());
            lvi.Checked = imageEffect.Enabled;
            lvi.Tag = imageEffect;

            if (lvEffects.SelectedIndices.Count > 0)
            {
                lvEffects.Items.Insert(lvEffects.SelectedIndices[lvEffects.SelectedIndices.Count - 1] + 1, lvi);
            }
            else
            {
                lvEffects.Items.Add(lvi);
            }

            lvEffects.Focus();
            lvi.EnsureVisible();
            lvi.Selected = true;
        }

        private void AddEffects(List<ImageEffect> imageEffects)
        {
            foreach (ImageEffect imageEffect in imageEffects)
            {
                AddEffect(imageEffect);
            }
        }

        #region Form events

        private void btnAdd_Click(object sender, EventArgs e)
        {
            cmsEffects.Show(btnAdd, 0, btnAdd.Height + 1);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            RemoveSelectedEffects();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearEffects();
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (lvEffects.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEffects.SelectedItems[0];

                if (lvi.Tag is ImageEffect)
                {
                    ImageEffect imageEffect = (ImageEffect)lvi.Tag;
                    ImageEffect imageEffectClone = imageEffect.Copy();
                    AddEffect(imageEffectClone);
                    UpdatePreview();
                }
            }
        }

        private void lvEffects_ItemMoved(object sender, int oldIndex, int newIndex)
        {
            UpdatePreview();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void lvEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgSettings.SelectedObject = null;

            if (lvEffects.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvEffects.SelectedItems[0];

                if (lvi.Tag is ImageEffect)
                {
                    pgSettings.SelectedObject = lvi.Tag;
                }
            }
        }

        private void lvEffects_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item != null && e.Item.Tag is ImageEffect)
            {
                ImageEffect imageEffect = (ImageEffect)e.Item.Tag;
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
            UpdatePreview();
        }

        private object eiImageEffects_ExportRequested()
        {
            return GetImageEffects();
        }

        private void eiImageEffects_ImportRequested(object obj)
        {
            List<ImageEffect> imageEffects = obj as List<ImageEffect>;

            if (imageEffects != null && imageEffects.Count > 0)
            {
                ClearEffects();
                AddEffects(imageEffects);
                UpdatePreview();
            }
        }

        private void tsmiLoadImageFromFile_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filePath))
            {
                if (DefaultImage != null) DefaultImage.Dispose();
                DefaultImage = ImageHelpers.LoadImage(filePath);
                UpdatePreview();
            }
        }

        private void tsmiLoadImageFromClipboard_Click(object sender, EventArgs e)
        {
            Image img = ClipboardHelpers.GetImage();

            if (img != null)
            {
                if (DefaultImage != null) DefaultImage.Dispose();
                DefaultImage = img;
                UpdatePreview();
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            if (DefaultImage != null)
            {
                using (Image img = ApplyEffects())
                {
                    ImageHelpers.SaveImageFileDialog(img);
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
                string[] files = e.Data.GetData(DataFormats.FileDrop, false) as string[];

                if (files != null && files.Length > 0)
                {
                    if (Helpers.IsImageFile(files[0]))
                    {
                        if (DefaultImage != null) DefaultImage.Dispose();
                        DefaultImage = ImageHelpers.LoadImage(files[0]);
                        UpdatePreview();
                    }
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Bitmap, false))
            {
                Image img = e.Data.GetData(DataFormats.Bitmap, false) as Image;

                if (img != null)
                {
                    if (DefaultImage != null) DefaultImage.Dispose();
                    DefaultImage = img;
                    UpdatePreview();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Effects = GetImageEffects();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Form events
    }
}