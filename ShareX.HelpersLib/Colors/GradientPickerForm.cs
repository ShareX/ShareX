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
            Icon = ShareXResources.Icon;
            cbGradientType.Items.AddRange(Helpers.GetEnumNamesProper<LinearGradientMode>());
            cbGradientType.SelectedIndex = (int)Gradient.Type;
            foreach (GradientStop gradientStop in Gradient.Colors)
            {
                AddGradientStop(gradientStop);
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

        private void cbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Gradient.Type = (LinearGradientMode)cbGradientType.SelectedIndex;
            UpdatePreview();
        }

        private void AddGradientStop(GradientStop gradientStop)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = string.Format(" {0:0.##}%", gradientStop.Location);
            lvi.BackColor = gradientStop.Color;
            lvi.ForeColor = ColorHelpers.VisibleColor(gradientStop.Color);
            lvi.Tag = gradientStop;
            lvGradientPoints.Items.Add(lvi);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Color color = cbtnCurrentColor.Color;
            float offset = (float)(nudLocation.Value / 100);
            GradientStop gradientStop = new GradientStop(color, offset);
            Gradient.Colors.Add(gradientStop);
            AddGradientStop(gradientStop);
            UpdatePreview();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvGradientPoints.SelectedItems[0];
                GradientStop gradientStop = (GradientStop)lvi.Tag;
                Gradient.Colors.Remove(gradientStop);
                lvGradientPoints.Items.Remove(lvi);
                UpdatePreview();
            }
        }

        private void cbtnCurrentColor_ColorChanged(Color color)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvGradientPoints.SelectedItems[0];
                GradientStop gradientStop = (GradientStop)lvi.Tag;
                gradientStop.Color = color;
                lvi.BackColor = gradientStop.Color;
                lvi.ForeColor = ColorHelpers.VisibleColor(gradientStop.Color);
                UpdatePreview();
            }
        }

        private void nudLocation_ValueChanged(object sender, EventArgs e)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvGradientPoints.SelectedItems[0];
                GradientStop gradientStop = (GradientStop)lvi.Tag;
                gradientStop.Location = (float)nudLocation.Value;
                lvi.Text = string.Format(" {0:0.##}%", gradientStop.Location);
                UpdatePreview();
            }
        }

        private void lvGradientPoints_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvGradientPoints.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvGradientPoints.SelectedItems[0];
                GradientStop gradientStop = (GradientStop)lvi.Tag;
                cbtnCurrentColor.Color = gradientStop.Color;
                nudLocation.SetValue((decimal)gradientStop.Location);
            }
        }

        private void lvGradientPoints_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && lvGradientPoints.SelectedItems.Count > 0)
            {
                cbtnCurrentColor.ShowColorDialog();
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