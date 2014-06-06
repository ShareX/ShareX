#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ImageEffectsLib
{
    public partial class ImageEffectsForm : Form
    {
        public Image DefaultImage { get; private set; }

        public List<ImageEffect> Effects { get; private set; }

        public ImageEffectsForm(Image img, List<ImageEffect> effects = null)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            DefaultImage = img;
            eiImageEffects.ObjectType = typeof(List<ImageEffect>);
            AddAllEffectsToTreeView();

            if (effects != null)
            {
                AddEffects(effects.Copy());
            }

            UpdatePreview();
        }

        public void EditorMode()
        {
            pbResult.AllowDrop = true;
            btnLoadImage.Visible = true;
            btnSaveImage.Visible = true;
        }

        private void AddAllEffectsToTreeView()
        {
            AddEffectToTreeView("Drawings",
                typeof(DrawBackground),
                typeof(DrawBorder),
                typeof(DrawCheckerboard),
                typeof(DrawImage),
                typeof(DrawText));

            AddEffectToTreeView("Manipulations",
                typeof(Canvas),
                typeof(Crop),
                typeof(Flip),
                typeof(Resize),
                typeof(Rotate),
                typeof(Scale),
                typeof(Skew));

            AddEffectToTreeView("Adjustments",
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

            AddEffectToTreeView("Filters",
                typeof(Blur),
                typeof(EdgeDetect),
                typeof(Emboss),
                typeof(GaussianBlur),
                typeof(MatrixConvolution),
                typeof(MeanRemoval),
                typeof(Pixelate),
                typeof(Reflection),
                typeof(Shadow),
                typeof(Sharpen),
                typeof(Smooth),
                typeof(TornEdge));

            tvEffects.ExpandAll();
        }

        private void AddEffectToTreeView(string groupName, params Type[] imageEffects)
        {
            TreeNode parentNode = tvEffects.Nodes.Add(groupName, groupName);

            foreach (Type imageEffect in imageEffects)
            {
                TreeNode childNode = parentNode.Nodes.Add(imageEffect.GetDescription());
                childNode.Tag = imageEffect;
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
                    Text = string.Format("ShareX - Image effects - Width: {0}, Height: {1}, Render time: {2} ms", preview.Width, preview.Height, timer.ElapsedMilliseconds);
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

        private void AddSelectedEffect()
        {
            TreeNode node = tvEffects.SelectedNode;

            if (node != null && node.Tag is Type)
            {
                Type type = (Type)node.Tag;
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

        private void tvEffects_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Cancel = true;
        }

        private void tvEffects_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                AddSelectedEffect();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddSelectedEffect();
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

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lvEffects.SelectedItems.Count > 0)
            {
                lvEffects.SelectedItems[0].MoveUp();
                UpdatePreview();
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lvEffects.SelectedItems.Count > 0)
            {
                lvEffects.SelectedItems[0].MoveDown();
                UpdatePreview();
            }
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

        private void btnLoadImage_Click(object sender, EventArgs e)
        {
            string filePath = ImageHelpers.OpenImageFileDialog();

            if (!string.IsNullOrEmpty(filePath))
            {
                if (DefaultImage != null) DefaultImage.Dispose();
                DefaultImage = ImageHelpers.LoadImage(filePath);
                UpdatePreview();
            }
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            using (Image img = ApplyEffects())
            {
                ImageHelpers.SaveImageFileDialog(img);
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