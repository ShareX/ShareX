#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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
using ShareX.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class AfterCaptureForm : Form
    {
        public AfterCaptureTasks AfterCaptureTasks { get; private set; }
        public Image Image { get; private set; }
        public AfterCaptureFormResult Result { get; private set; }

        public AfterCaptureForm(Image img, AfterCaptureTasks afterCaptureTasks)
        {
            InitializeComponent();
            Icon = Resources.ShareX_Icon;
            AfterCaptureTasks = afterCaptureTasks;
            AddAfterCaptureItems(AfterCaptureTasks);
            Image = img;
            pbImage.LoadImage(Image);
            Result = AfterCaptureFormResult.Cancel;
        }

        private void AddAfterCaptureItems(AfterCaptureTasks afterCaptureTasks)
        {
            AfterCaptureTasks[] enums = (AfterCaptureTasks[])Enum.GetValues(typeof(AfterCaptureTasks));

            for (int i = 1; i < enums.Length; i++)
            {
                ListViewItem lvi = new ListViewItem(enums[i].GetDescription());
                lvi.Checked = afterCaptureTasks.HasFlag(1 << (i - 1));
                lvi.Tag = enums[i];
                lvAfterCaptureTasks.Items.Add(lvi);
            }
        }

        private AfterCaptureTasks GetAfterCaptureTasks()
        {
            AfterCaptureTasks afterCaptureTasks = AfterCaptureTasks.None;

            for (int i = 0; i < lvAfterCaptureTasks.Items.Count; i++)
            {
                ListViewItem lvi = lvAfterCaptureTasks.Items[i];

                if (lvi.Checked)
                {
                    afterCaptureTasks = afterCaptureTasks.Add((AfterCaptureTasks)(1 << i));
                }
            }

            return afterCaptureTasks;
        }

        private void Close(AfterCaptureFormResult result)
        {
            AfterCaptureTasks = GetAfterCaptureTasks();
            Result = result;
            Close();
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            Close(AfterCaptureFormResult.Continue);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Close(AfterCaptureFormResult.Copy);
        }
    }
}