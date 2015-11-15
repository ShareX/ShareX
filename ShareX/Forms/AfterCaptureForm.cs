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
using ShareX.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AfterCaptureForm : BaseForm
    {
        public TaskSettings TaskSettings { get; private set; }
        public string FileName { get; private set; }

        public AfterCaptureForm(Image img, TaskSettings taskSettings)
        {
            TaskSettings = taskSettings;

            InitializeComponent();

            ImageList imageList = new ImageList { ColorDepth = ColorDepth.Depth32Bit };
            imageList.Images.Add(Resources.checkbox_uncheck);
            imageList.Images.Add(Resources.checkbox_check);
            lvAfterCaptureTasks.SmallImageList = imageList;

            ucBeforeUpload.InitCapture(TaskSettings);

            AddAfterCaptureItems(TaskSettings.AfterCaptureJob);

            btnCopy.Visible = img != null;

            if (img != null)
            {
                pbImage.LoadImage(img);
            }

            FileName = TaskHelpers.GetFilename(TaskSettings, null, img);
            txtFileName.Text = FileName;
        }

        private void AddAfterCaptureItems(AfterCaptureTasks afterCaptureTasks)
        {
            AfterCaptureTasks[] enums = Helpers.GetEnums<AfterCaptureTasks>();

            for (int i = 1; i < enums.Length; i++)
            {
                ListViewItem lvi = new ListViewItem(enums[i].GetLocalizedDescription());
                CheckItem(lvi, afterCaptureTasks.HasFlag(1 << (i - 1)));
                lvi.Tag = enums[i];
                lvAfterCaptureTasks.Items.Add(lvi);
            }
        }

        private void CheckItem(ListViewItem lvi, bool check)
        {
            lvi.ImageIndex = check ? 1 : 0;
        }

        private bool IsChecked(ListViewItem lvi)
        {
            return lvi.ImageIndex == 1;
        }

        private AfterCaptureTasks GetAfterCaptureTasks()
        {
            AfterCaptureTasks afterCaptureTasks = AfterCaptureTasks.None;

            for (int i = 0; i < lvAfterCaptureTasks.Items.Count; i++)
            {
                ListViewItem lvi = lvAfterCaptureTasks.Items[i];

                if (IsChecked(lvi))
                {
                    afterCaptureTasks = afterCaptureTasks.Add((AfterCaptureTasks)(1 << i));
                }
            }

            return afterCaptureTasks;
        }

        private void lvAfterCaptureTasks_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.Selected = false;
        }

        private void lvAfterCaptureTasks_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListViewItem lvi = lvAfterCaptureTasks.GetItemAt(e.X, e.Y);

                if (lvi != null)
                {
                    CheckItem(lvi, !IsChecked(lvi));
                }
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            TaskSettings.AfterCaptureJob = GetAfterCaptureTasks();
            FileName = txtFileName.Text;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            TaskSettings.AfterCaptureJob = AfterCaptureTasks.CopyImageToClipboard;
            FileName = txtFileName.Text;
        }
    }
}