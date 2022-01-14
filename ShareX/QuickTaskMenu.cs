#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2022 ShareX Team

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
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public class QuickTaskMenu
    {
        public delegate void TaskInfoSelectedEventHandler(QuickTaskInfo taskInfo);
        public TaskInfoSelectedEventHandler TaskInfoSelected;
        private string FileName;
        private string CustomFileName;

        public string NewFileName
        {
            get
            {
                return this.FileName == this.CustomFileName ? null : this.CustomFileName;
            }
            set
            {             
                this.CustomFileName = value;
            }
        }

        private void Continue()
        {
            OnTaskInfoSelected(new QuickTaskInfo(this.NewFileName));
        }
        public void ShowMenu(string fileName)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            this.FileName = fileName;

            ContextMenuStrip cms = new ContextMenuStrip()
            {
                Font = new Font("Arial", 10f),
                AutoClose = false
            };

            cms.KeyUp += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    cms.Close();
                }
            };

            //cms.ImageList = new ImageList();
            //cms.ImageList.Images.Add(Resources.rename_file);

            ToolStripMenuItem tsmiFilenameCaption = new ToolStripMenuItem(Resources.QuickTaskMenu_ShowMenu_SaveAsFilename);
            tsmiFilenameCaption.Image = Resources.rename_file;
            cms.Items.Add(tsmiFilenameCaption);

            ToolStripTextBox tsmiFilename = new ToolStripTextBox();
            tsmiFilename.Text = this.FileName;
            tsmiFilename.Size = new Size(300, 22);
            //tsmiFilename.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            //tsmiFilename.ImageIndex = 0;  // does not work for some reason
            tsmiFilename.ToolTipText = Resources.QuickTaskMenu_ShowMenu_Filename_Tooltip;  //TODO move to resources
            tsmiFilename.SelectAll();
            tsmiFilename.Focus();
            tsmiFilename.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.NewFileName = tsmiFilename.Text;
                    cms.Close();
                    this.Continue();

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };
            
            cms.Items.Add(tsmiFilename);

            // Continue
            ToolStripMenuItem tsmiContinue = new ToolStripMenuItem(Resources.QuickTaskMenu_ShowMenu_Continue);
            tsmiContinue.Image = Resources.control;
            tsmiContinue.Click += (sender, e) =>
            {
                this.NewFileName = tsmiFilename.Text;
                cms.Close();
                this.Continue();
            };
            cms.Items.Add(tsmiContinue);

            cms.Items.Add(new ToolStripSeparator());

            if (Program.Settings != null && Program.Settings.QuickTaskPresets != null && Program.Settings.QuickTaskPresets.Count > 0)
            {
                foreach (QuickTaskInfo taskInfo in Program.Settings.QuickTaskPresets)
                {
                    if (taskInfo.IsValid)
                    {
                        ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = taskInfo.ToString().Replace("&", "&&"), Tag = taskInfo };
                        tsmi.Image = FindSuitableIcon(taskInfo);
                        tsmi.Click += (sender, e) =>
                        {
                            QuickTaskInfo selectedTaskInfo = ((ToolStripMenuItem)sender).Tag as QuickTaskInfo;
                            cms.Close();
                            this.NewFileName = cms.Items[1].Text;
                            selectedTaskInfo.CustomFileName = this.NewFileName;
                            OnTaskInfoSelected(selectedTaskInfo);
                        };
                        cms.Items.Add(tsmi);
                    }
                    else
                    {
                        cms.Items.Add(new ToolStripSeparator());
                    }
                }

                cms.Items.Add(new ToolStripSeparator());
            }

            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem(Resources.QuickTaskMenu_ShowMenu_Edit_this_menu___);
            tsmiEdit.Image = Resources.pencil;
            tsmiEdit.Click += (sender, e) =>
            {
                cms.Close();
                new QuickTaskMenuEditorForm().ShowDialog();
            };
            cms.Items.Add(tsmiEdit);

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiCancel = new ToolStripMenuItem(Resources.QuickTaskMenu_ShowMenu_Cancel);
            tsmiCancel.Image = Resources.cross;
            tsmiCancel.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiCancel);

            if (ShareXResources.UseCustomTheme)
            {
                ShareXResources.ApplyCustomThemeToContextMenuStrip(cms);
            }

            Point cursorPosition = CaptureHelpers.GetCursorPosition();
            cursorPosition.Offset(-10, -10);
            cms.Show(cursorPosition);
            tsmiFilename.Focus();
}

        protected void OnTaskInfoSelected(QuickTaskInfo taskInfo)
        {
            TaskInfoSelected?.Invoke(taskInfo);
        }

        public Image FindSuitableIcon(QuickTaskInfo taskInfo)
        {
            if (taskInfo.AfterCaptureTasks.HasFlag(AfterCaptureTasks.UploadImageToHost))
            {
                return Resources.upload_cloud;
            }
            else if (taskInfo.AfterCaptureTasks.HasFlag(AfterCaptureTasks.CopyImageToClipboard) || taskInfo.AfterCaptureTasks.HasFlag(AfterCaptureTasks.CopyFileToClipboard))
            {
                return Resources.clipboard;
            }
            else if (taskInfo.AfterCaptureTasks.HasFlag(AfterCaptureTasks.SaveImageToFile) || taskInfo.AfterCaptureTasks.HasFlag(AfterCaptureTasks.SaveImageToFileWithDialog))
            {
                return Resources.disk_black;
            }

            return Resources.image;
        }
    }
}