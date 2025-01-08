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
using ShareX.MediaLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.MediaLib
{
    public partial class ImageCombinerForm : Form
    {
        public event Action<Bitmap> ProcessRequested;

        public ImageCombinerOptions Options { get; private set; }

        public ImageCombinerForm(ImageCombinerOptions options)
        {
            Options = options;

            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);

            if (Options.Orientation == Orientation.Horizontal)
            {
                rbOrientationHorizontal.Checked = true;
            }
            else
            {
                rbOrientationVertical.Checked = true;
            }

            UpdateAlignmentComboBox();
            nudSpace.SetValue(Options.Space);
            nudWrapAfter.SetValue(Options.WrapAfter);
            cbAutoFillBackground.Checked = Options.AutoFillBackground;
        }

        public ImageCombinerForm(ImageCombinerOptions options, IEnumerable<string> imageFiles) : this(options)
        {
            if (imageFiles != null)
            {
                foreach (string image in imageFiles)
                {
                    lvImages.Items.Add(image);
                }

                lblImageCount.Text = lvImages.Items.Count.ToString();
            }
        }

        private void UpdateOrientation()
        {
            if (rbOrientationHorizontal.Checked)
            {
                Options.Orientation = Orientation.Horizontal;
            }
            else
            {
                Options.Orientation = Orientation.Vertical;
            }
        }

        private void UpdateAlignmentComboBox()
        {
            cbAlignment.Items.Clear();

            if (Options.Orientation == Orientation.Horizontal)
            {
                cbAlignment.Items.Add(Resources.AlignmentTop);
                cbAlignment.Items.Add(Resources.AlignmentHorizontalCenter);
                cbAlignment.Items.Add(Resources.AlignmentBottom);
            }
            else
            {
                cbAlignment.Items.Add(Resources.AlignmentLeft);
                cbAlignment.Items.Add(Resources.AlignmentVerticalCenter);
                cbAlignment.Items.Add(Resources.AlignmentRight);
            }

            cbAlignment.SelectedIndex = (int)Options.Alignment;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string[] images = ImageHelpers.OpenImageFileDialog(true);

            if (images != null)
            {
                foreach (string image in images)
                {
                    lvImages.Items.Add(image);
                }

                lblImageCount.Text = lvImages.Items.Count.ToString();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                foreach (ListViewItem lvi in lvImages.SelectedItems)
                {
                    lvImages.Items.Remove(lvi);
                }

                lblImageCount.Text = lvImages.Items.Count.ToString();
            }
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                lvImages.SelectedItems[0].MoveUp();
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (lvImages.SelectedItems.Count > 0)
            {
                lvImages.SelectedItems[0].MoveDown();
            }
        }

        private void rbOrientationHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOrientation();
            UpdateAlignmentComboBox();
        }

        private void rbOrientationVertical_CheckedChanged(object sender, EventArgs e)
        {
            UpdateOrientation();
            UpdateAlignmentComboBox();
        }

        private void cbAlignment_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Alignment = (ImageCombinerAlignment)cbAlignment.SelectedIndex;
        }

        private void nudSpace_ValueChanged(object sender, EventArgs e)
        {
            Options.Space = (int)nudSpace.Value;
        }

        private void nudWrapAfter_ValueChanged(object sender, EventArgs e)
        {
            Options.WrapAfter = (int)nudWrapAfter.Value;
        }

        private void cbAutoFillBackground_CheckedChanged(object sender, EventArgs e)
        {
            Options.AutoFillBackground = cbAutoFillBackground.Checked;
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            if (lvImages.Items.Count > 0)
            {
                try
                {
                    List<string> imageFiles = lvImages.Items.Cast<ListViewItem>().Select(x => x.Text).ToList();

                    if (imageFiles.Count > 1)
                    {
                        Bitmap output = ImageHelpers.CombineImages(imageFiles, Options.Orientation, Options.Alignment, Options.Space, Options.WrapAfter,
                            Options.AutoFillBackground);

                        if (output != null)
                        {
                            OnProcessRequested(output);
                        }
                    }
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);
                    ex.ShowError();
                }
            }
        }

        protected void OnProcessRequested(Bitmap bmp)
        {
            ProcessRequested?.Invoke(bmp);
        }

        private void ImageCombinerForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ImageCombinerForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) && e.Data.GetData(DataFormats.FileDrop, false) is string[] files)
            {
                foreach (string file in files)
                {
                    lvImages.Items.Add(file);
                }

                lblImageCount.Text = lvImages.Items.Count.ToString();
            }
        }
    }
}