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
using System.Windows.Forms;

namespace ShareX
{
    public class QuickTaskMenu
    {
        public delegate void TaskInfoSelectedEventHandler(QuickTaskInfo taskInfo);
        public TaskInfoSelectedEventHandler TaskInfoSelected;

        public void ShowMenu()
        {
            ContextMenuStrip cms = new ContextMenuStrip()
            {
                Font = new Font("Arial", 10f),
                AutoClose = false
            };

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
                            OnTaskInfoSelected(selectedTaskInfo);
                        };
                        cms.Items.Add(tsmi);
                    }
                    else
                    {
                        cms.Items.Add(new ToolStripSeparator());
                    }
                }

                cms.Items[0].Select();

                cms.Items.Add(new ToolStripSeparator());
            }

            // Translate
            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem("Edit this menu...");
            tsmiEdit.Image = Resources.gear;
            tsmiEdit.Click += (sender, e) =>
            {
                cms.Close();
                new QuickTaskMenuEditorForm().ShowDialog();
            };
            cms.Items.Add(tsmiEdit);

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiCancel = new ToolStripMenuItem("Cancel");
            tsmiCancel.Image = Resources.cross;
            tsmiCancel.Click += (sender, e) => cms.Close();
            cms.Items.Add(tsmiCancel);

            Point cursorPosition = CaptureHelpers.GetCursorPosition();
            cursorPosition.Offset(-10, -10);
            cms.Show(cursorPosition);
        }

        protected void OnTaskInfoSelected(QuickTaskInfo taskInfo)
        {
            if (TaskInfoSelected != null)
            {
                TaskInfoSelected(taskInfo);
            }
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