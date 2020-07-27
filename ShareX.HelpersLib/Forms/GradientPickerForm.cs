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
        public GradientInfo Gradient { get; set; }

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
                new GradientInfo(new GradientStop(Color.FromArgb(184, 11, 195), 0f), new GradientStop(Color.FromArgb(98, 54, 255), 100f)),
                new GradientInfo(new GradientStop(Color.FromArgb(255, 3, 135), 0f), new GradientStop(Color.FromArgb(255, 143, 3), 100f)),
                new GradientInfo(new GradientStop(Color.FromArgb(0, 187, 138), 0f), new GradientStop(Color.FromArgb(0, 105, 163), 100f))
            };

            for (int i = 0; i < gradients.Length; i++)
            {
                GradientInfo gradient = gradients[i];
                gradient.Type = Gradient.Type;
                ilPresets.Images.Add(gradient.CreateGradientPreview(100, 25));

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
                Bitmap bmp = new Bitmap(pbPreview.ClientRectangle.Width, pbPreview.ClientRectangle.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    Gradient.Draw(g, new Rectangle(0, 0, bmp.Width, bmp.Height));
                }

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
            Gradient.Type = (LinearGradientMode)cbGradientType.SelectedIndex;
            UpdatePreview();
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
                    Gradient = gradientInfo;
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