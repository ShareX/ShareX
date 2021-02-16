#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class GradientPickerForm : Form
    {
        public GradientInfo Gradient { get; private set; }

        private bool isReady;

        public GradientPickerForm(GradientInfo gradient)
        {
            Gradient = gradient;

            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            cbGradientType.Items.AddRange(Helpers.GetEnumNamesProper<LinearGradientMode>());
            cbGradientType.SelectedIndex = (int)Gradient.Type;
            UpdateGradientList(true);
            AddPresets();
        }

        private void AddPresets()
        {
            GradientInfo[] gradients = new GradientInfo[]
            {
                new GradientInfo(Color.FromArgb(184, 11, 195), Color.FromArgb(98, 54, 255)),
                new GradientInfo(Color.FromArgb(255, 3, 135), Color.FromArgb(255, 143, 3)),
                new GradientInfo(Color.FromArgb(0, 187, 138), Color.FromArgb(0, 105, 163)),

                new GradientInfo(Color.FromArgb(46, 49, 146), Color.FromArgb(27, 255, 255)),
                //new GradientInfo(Color.FromArgb(212, 20, 90), Color.FromArgb(251, 176, 59)),
                new GradientInfo(Color.FromArgb(0, 146, 69), Color.FromArgb(252, 238, 33)),
                new GradientInfo(Color.FromArgb(102, 45, 140), Color.FromArgb(237, 30, 121)),
                new GradientInfo(Color.FromArgb(238, 156, 167), Color.FromArgb(255, 221, 225)),
                new GradientInfo(Color.FromArgb(97, 67, 133), Color.FromArgb(81, 99, 149)),
                new GradientInfo(Color.FromArgb(2, 170, 189), Color.FromArgb(0, 205, 172)),
                new GradientInfo(Color.FromArgb(255, 81, 47), Color.FromArgb(221, 36, 118)),
                new GradientInfo(Color.FromArgb(255, 95, 109), Color.FromArgb(255, 195, 113)),
                new GradientInfo(Color.FromArgb(17, 153, 142), Color.FromArgb(56, 239, 125)),
                new GradientInfo(Color.FromArgb(198, 234, 141), Color.FromArgb(254, 144, 175)),
                new GradientInfo(Color.FromArgb(234, 141, 141), Color.FromArgb(168, 144, 254)),
                new GradientInfo(Color.FromArgb(216, 181, 255), Color.FromArgb(30, 174, 152)),
                new GradientInfo(Color.FromArgb(255, 97, 210), Color.FromArgb(254, 144, 144)),
                new GradientInfo(Color.FromArgb(191, 240, 152), Color.FromArgb(111, 214, 255)),
                new GradientInfo(Color.FromArgb(78, 101, 255), Color.FromArgb(146, 239, 253)),
                new GradientInfo(Color.FromArgb(169, 241, 223), Color.FromArgb(255, 187, 187)),
                new GradientInfo(Color.FromArgb(195, 55, 100), Color.FromArgb(29, 38, 113)),
                new GradientInfo(Color.FromArgb(147, 165, 207), Color.FromArgb(228, 239, 233)),
                new GradientInfo(Color.FromArgb(134, 143, 150), Color.FromArgb(89, 97, 100)),
                new GradientInfo(Color.FromArgb(9, 32, 63), Color.FromArgb(83, 120, 149)),
                new GradientInfo(Color.FromArgb(255, 236, 210), Color.FromArgb(252, 182, 159)),
                new GradientInfo(Color.FromArgb(161, 196, 253), Color.FromArgb(194, 233, 251)),
                new GradientInfo(Color.FromArgb(118, 75, 162), Color.FromArgb(102, 126, 234)),
                new GradientInfo(Color.FromArgb(253, 252, 251), Color.FromArgb(226, 209, 195)),

                new GradientInfo(Color.FromArgb(255, 0, 0), Color.FromArgb(255, 0, 255), Color.FromArgb(0, 0, 255), Color.FromArgb(0, 255, 255), Color.FromArgb(0, 255, 0), Color.FromArgb(255, 255, 0), Color.FromArgb(255, 0, 0))
            };

            lvPresets.Items.Clear();
            ilPresets.Images.Clear();

            for (int i = 0; i < gradients.Length; i++)
            {
                GradientInfo gradient = gradients[i];
                gradient.Type = Gradient.Type;
                ilPresets.Images.Add(gradient.CreateGradientPreview(100, 25, true));

                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Tag = gradient;
                lvPresets.Items.Add(lvi);
            }
        }

        private void UpdateGradientList(bool selectFirst = false)
        {
            isReady = false;
            Gradient.Sort();

            lvGradientPoints.Items.Clear();
            foreach (GradientStop gradientStop in Gradient.Colors)
            {
                AddGradientStop(gradientStop);
            }

            if (selectFirst && lvGradientPoints.Items.Count > 0)
            {
                lvGradientPoints.SelectedIndex = 0;
            }

            isReady = true;
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (isReady)
            {
                Bitmap bmp = Gradient.CreateGradientPreview(pbPreview.ClientRectangle.Width, pbPreview.ClientRectangle.Height, true);

                if (pbPreview.Image != null)
                {
                    pbPreview.Image.Dispose();
                }

                pbPreview.Image = bmp;
            }
        }

        private void AddGradientStop(GradientStop gradientStop)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = string.Format(" {0:0.##}%", gradientStop.Location);
            UpdateListViewItemColor(lvi, gradientStop.Color);
            lvi.Tag = gradientStop;
            lvGradientPoints.Items.Add(lvi);
        }

        private void UpdateListViewItemColor(ListViewItem lvi, Color color)
        {
            string argb = color.ToArgb().ToString();

            if (!ilColors.Images.ContainsKey(argb))
            {
                ilColors.Images.Add(argb, ImageHelpers.CreateColorPickerIcon(color, new Rectangle(0, 0, 16, 16)));
            }

            lvi.ImageKey = argb;
        }

        private GradientStop GetSelectedGradientStop()
        {
            return GetSelectedGradientStop(out _);
        }

        private GradientStop GetSelectedGradientStop(out ListViewItem lvi)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                lvi = lvGradientPoints.SelectedItems[0];
                return lvi.Tag as GradientStop;
            }

            lvi = null;
            return null;
        }

        private ListViewItem FindListViewItem(GradientStop gradientStop)
        {
            foreach (ListViewItem lvi in lvGradientPoints.Items)
            {
                GradientStop itemGradientstop = lvi.Tag as GradientStop;
                if (itemGradientstop == gradientStop)
                {
                    return lvi;
                }
            }

            return null;
        }

        private ListViewItem SelectGradientStop(GradientStop gradientStop)
        {
            ListViewItem lvi = FindListViewItem(gradientStop);
            if (lvi != null)
            {
                lvi.Selected = true;
            }
            return lvi;
        }

        private void cbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReady)
            {
                Gradient.Type = (LinearGradientMode)cbGradientType.SelectedIndex;
                UpdatePreview();
                AddPresets();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Color color = cbtnCurrentColor.Color;
            float offset = (float)nudLocation.Value;
            GradientStop gradientStop = new GradientStop(color, offset);
            Gradient.Colors.Add(gradientStop);
            UpdateGradientList();
            SelectGradientStop(gradientStop);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                Gradient.Colors.Remove(gradientStop);
                UpdateGradientList();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Gradient.Clear();
            UpdateGradientList();
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            if (Gradient.IsValid)
            {
                Gradient.Reverse();
                UpdateGradientList();
            }
        }

        private void cbtnCurrentColor_ColorChanged(Color color)
        {
            GradientStop gradientStop = GetSelectedGradientStop(out ListViewItem lvi);

            if (gradientStop != null)
            {
                gradientStop.Color = color;
                UpdateListViewItemColor(lvi, gradientStop.Color);
                UpdatePreview();
            }
        }

        private void nudLocation_ValueChanged(object sender, EventArgs e)
        {
            if (isReady)
            {
                GradientStop gradientStop = GetSelectedGradientStop();

                if (gradientStop != null)
                {
                    gradientStop.Location = (float)nudLocation.Value;
                    UpdateGradientList();
                    SelectGradientStop(gradientStop);
                }
            }
        }

        private void lvGradientPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            GradientStop gradientStop = GetSelectedGradientStop();

            if (gradientStop != null)
            {
                isReady = false;
                cbtnCurrentColor.Color = gradientStop.Color;
                nudLocation.SetValue((decimal)gradientStop.Location);
                isReady = true;
            }
        }

        private void lvGradientPoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvGradientPoints.SelectedItems.Count > 0)
            {
                cbtnCurrentColor.ShowColorDialog();
            }
        }

        private void lvPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isReady && lvPresets.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvPresets.SelectedItems[0];
                GradientInfo gradientInfo = lvi.Tag as GradientInfo;
                if (gradientInfo != null)
                {
                    Gradient = gradientInfo.Copy();
                    UpdateGradientList(true);
                    lvi.Selected = false;
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
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