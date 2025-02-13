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

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class ClipboardViewerForm : Form
    {
        public DataObject CurrentDataObject { get; private set; }

        public ClipboardViewerForm()
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this, true);
        }

        private void ClipboardViewerForm_Load(object sender, EventArgs e)
        {
            RefreshClipboardContentList();
        }

        private void RefreshClipboardContentList()
        {
            ResetSelected();

            lvClipboardContentList.Items.Clear();

            try
            {
                CurrentDataObject = (DataObject)Clipboard.GetDataObject();
                if (CurrentDataObject != null)
                {
                    string[] formats = CurrentDataObject.GetFormats();
                    if (formats != null && formats.Length > 0)
                    {
                        foreach (string format in formats)
                        {
                            ListViewItem lvi = new ListViewItem(format);
                            lvClipboardContentList.Items.Add(lvi);
                        }

                        lvClipboardContentList.Items[0].Selected = true;
                    }
                }
            }
            catch (Exception e)
            {
                e.ShowError();
            }
        }

        private void UpdateSelectedClipboardContent()
        {
            ResetSelected();

            if (lvClipboardContentList.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvClipboardContentList.SelectedItems[0];
                string format = lvi.Text;

                if (CurrentDataObject != null)
                {
                    object data = CurrentDataObject.GetData(format);

                    if (data != null)
                    {
                        try
                        {
                            switch (data)
                            {
                                case MemoryStream ms:
                                    if (format.Equals(ClipboardHelpers.FORMAT_PNG, StringComparison.OrdinalIgnoreCase))
                                    {
                                        using (Bitmap bmp = new Bitmap(ms))
                                        {
                                            Bitmap clonedImage = ClipboardHelpersEx.CloneImage(bmp);
                                            LoadImage(clonedImage);
                                        }
                                    }
                                    else if (format.Equals(DataFormats.Dib, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Bitmap bmp = ClipboardHelpersEx.ImageFromClipboardDib(ms.ToArray());
                                        LoadImage(bmp);
                                    }
                                    else if (format.Equals(ClipboardHelpers.FORMAT_17, StringComparison.OrdinalIgnoreCase))
                                    {
                                        Bitmap bmp = ClipboardHelpersEx.DIBV5ToBitmap(ms.ToArray());
                                        LoadImage(bmp);
                                    }
                                    else
                                    {
                                        LoadText(data.ToString());
                                    }
                                    break;
                                case Bitmap bmp:
                                    LoadImage(bmp);
                                    break;
                                default:
                                    LoadText(data.ToString());
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            e.ShowError();
                        }
                    }
                }
            }
        }

        private void ResetSelected()
        {
            txtSelectedClipboardContent.Visible = false;
            txtSelectedClipboardContent.Clear();

            pbSelectedClipboardContent.Visible = false;
            pbSelectedClipboardContent.Reset();
        }

        private void LoadImage(Bitmap bmp)
        {
            pbSelectedClipboardContent.LoadImage(bmp);
            pbSelectedClipboardContent.Visible = true;
        }

        private void LoadText(string text)
        {
            txtSelectedClipboardContent.Text = text;
            txtSelectedClipboardContent.Visible = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshClipboardContentList();
        }

        private void btnClearClipboard_Click(object sender, EventArgs e)
        {
            ClipboardHelpers.Clear();
            RefreshClipboardContentList();
        }

        private void lvClipboardContentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSelectedClipboardContent();
        }
    }
}