#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using ShareX.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ImageCombinerForm : Form
    {
        public event Action<Image> ProcessRequested;

        public ImageCombinerOptions Options { get; private set; }

        public ImageCombinerForm(ImageCombinerOptions options)
        {
            Options = options;
            InitializeComponent();
            Icon = ShareXResources.Icon;
            cbOrientation.Items.AddRange(Enum.GetNames(typeof(Orientation)));
            cbOrientation.SelectedIndex = (int)Options.Orientation;
            nudSpace.SetValue(Options.Space);
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

        private void cbOrientation_SelectedIndexChanged(object sender, EventArgs e)
        {
            Options.Orientation = (Orientation)cbOrientation.SelectedIndex;
        }

        private void nudSpace_ValueChanged(object sender, EventArgs e)
        {
            Options.Space = (int)nudSpace.Value;
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            if (lvImages.Items.Count > 0)
            {
                IEnumerable<Image> images = null;

                try
                {
                    images = lvImages.Items.Cast<ListViewItem>().Select(x => ImageHelpers.LoadImage(x.Text));
                    Image output = ImageHelpers.CombineImages(images, Options.Orientation, Options.Space);
                    OnProcessRequested(output);
                }
                catch (Exception ex)
                {
                    DebugHelper.WriteException(ex);

                    MessageBox.Show(ex.ToString(), $"ShareX - {Resources.Program_Run_Error}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (images != null)
                    {
                        foreach (Image image in images)
                        {
                            if (image != null)
                            {
                                image.Dispose();
                            }
                        }
                    }
                }
            }
        }

        protected void OnProcessRequested(Image image)
        {
            if (ProcessRequested != null)
            {
                ProcessRequested(image);
            }
        }
    }
}