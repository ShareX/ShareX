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

        private ContextMenuStrip cms;
        private List<QuickTaskInfo> quickTaskInfoList;

        public void ShowMenu()
        {
            quickTaskInfoList = new List<QuickTaskInfo>();

            cms = new ContextMenuStrip()
            {
                Font = new Font("Arial", 10f)
            };

            cms.KeyUp += Cms_KeyUp;
            cms.Closed += (sender, e) => cms = null;

            if (Program.Settings != null && Program.Settings.QuickTaskPresets != null && Program.Settings.QuickTaskPresets.Count > 0)
            {
                int index = 0;

                foreach (QuickTaskInfo taskInfo in Program.Settings.QuickTaskPresets)
                {
                    if (taskInfo.IsValid)
                    {
                        ToolStripMenuItem tsmi = new ToolStripMenuItem { Text = taskInfo.ToString().Replace("&", "&&"), Tag = taskInfo };
                        tsmi.Image = GetNumberImage(index);
                        tsmi.Click += (sender, e) =>
                        {
                            QuickTaskInfo selectedTaskInfo = ((ToolStripMenuItem)sender).Tag as QuickTaskInfo;
                            OnTaskInfoSelected(selectedTaskInfo);
                        };
                        cms.Items.Add(tsmi);
                        quickTaskInfoList.Add(taskInfo);
                        index++;
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
                new QuickTaskMenuEditorForm().ShowDialog();
            };
            cms.Items.Add(tsmiEdit);

            cms.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiCancel = new ToolStripMenuItem("Cancel");
            tsmiCancel.Image = Resources.cross;
            cms.Items.Add(tsmiCancel);

            Point cursorPosition = CaptureHelpers.GetCursorPosition();
            cursorPosition.Offset(-10, -10);
            cms.Show(cursorPosition);
        }

        private void Cms_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.D1: case Keys.NumPad1: ExecuteTaskAt(0); break;
                case Keys.D2: case Keys.NumPad2: ExecuteTaskAt(1); break;
                case Keys.D3: case Keys.NumPad3: ExecuteTaskAt(2); break;
                case Keys.D4: case Keys.NumPad4: ExecuteTaskAt(3); break;
                case Keys.D5: case Keys.NumPad5: ExecuteTaskAt(4); break;
                case Keys.D6: case Keys.NumPad6: ExecuteTaskAt(5); break;
                case Keys.D7: case Keys.NumPad7: ExecuteTaskAt(6); break;
                case Keys.D8: case Keys.NumPad8: ExecuteTaskAt(7); break;
                case Keys.D9: case Keys.NumPad9: ExecuteTaskAt(8); break;
                case Keys.D0: case Keys.NumPad0: ExecuteTaskAt(9); break;
            }
        }

        private Image GetNumberImage(int index)
        {
            switch (index)
            {
                case 0: return Resources.notification_counter;
                case 1: return Resources.notification_counter_02;
                case 2: return Resources.notification_counter_03;
                case 3: return Resources.notification_counter_04;
                case 4: return Resources.notification_counter_05;
                case 5: return Resources.notification_counter_06;
                case 6: return Resources.notification_counter_07;
                case 7: return Resources.notification_counter_08;
                case 8: return Resources.notification_counter_09;
                case 9: return Resources.notification_counter_00;
            }

            return null;
        }

        private void ExecuteTaskAt(int index)
        {
            if (quickTaskInfoList != null && quickTaskInfoList.Count > 0 && quickTaskInfoList.Count > index)
            {
                OnTaskInfoSelected(quickTaskInfoList[index]);
            }
        }

        protected void OnTaskInfoSelected(QuickTaskInfo taskInfo)
        {
            if (TaskInfoSelected != null)
            {
                if (cms != null && !cms.IsDisposed)
                {
                    cms.Close();
                }

                TaskInfoSelected(taskInfo);
            }
        }
    }
}