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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class TaskThumbnailView : UserControl
    {
        public List<TaskThumbnailPanel> Panels { get; private set; }
        public List<TaskThumbnailPanel> SelectedPanels { get; private set; }

        public TaskThumbnailPanel SelectedPanel
        {
            get
            {
                if (SelectedPanels.Count > 0)
                {
                    return SelectedPanels[SelectedPanels.Count - 1];
                }

                return null;
            }
        }

        private bool titleVisible = true;

        public bool TitleVisible
        {
            get
            {
                return titleVisible;
            }
            set
            {
                if (titleVisible != value)
                {
                    titleVisible = value;

                    foreach (TaskThumbnailPanel panel in Panels)
                    {
                        panel.TitleVisible = titleVisible;
                    }
                }
            }
        }

        private ThumbnailTitleLocation titleLocation;

        public ThumbnailTitleLocation TitleLocation
        {
            get
            {
                return titleLocation;
            }
            set
            {
                if (titleLocation != value)
                {
                    titleLocation = value;

                    foreach (TaskThumbnailPanel panel in Panels)
                    {
                        panel.TitleLocation = titleLocation;
                    }
                }
            }
        }

        private Size thumbnailSize = new Size(200, 150);

        public Size ThumbnailSize
        {
            get
            {
                return thumbnailSize;
            }
            set
            {
                if (thumbnailSize != value)
                {
                    thumbnailSize = value;

                    foreach (TaskThumbnailPanel panel in Panels)
                    {
                        panel.ThumbnailSize = thumbnailSize;
                    }

                    UpdateAllThumbnails(true);
                }
            }
        }

        private ThumbnailViewClickAction clickAction = ThumbnailViewClickAction.Default;

        public ThumbnailViewClickAction ClickAction
        {
            get
            {
                return clickAction;
            }
            set
            {
                if (clickAction != value)
                {
                    clickAction = value;

                    foreach (TaskThumbnailPanel panel in Panels)
                    {
                        panel.ClickAction = clickAction;
                    }
                }
            }
        }

        public delegate void TaskViewMouseEventHandler(object sender, MouseEventArgs e);
        public event TaskViewMouseEventHandler ContextMenuRequested;

        public event EventHandler SelectedPanelChanged;

        public TaskThumbnailView()
        {
            Panels = new List<TaskThumbnailPanel>();
            SelectedPanels = new List<TaskThumbnailPanel>();

            InitializeComponent();
            UpdateTheme();
        }

        protected override Point ScrollToControl(Control activeControl)
        {
            return AutoScrollPosition;
        }

        public void UpdateTheme()
        {
            if (ShareXResources.UseCustomTheme)
            {
                BackColor = ShareXResources.Theme.BackgroundColor;
            }
            else
            {
                BackColor = SystemColors.Window;
            }

            foreach (TaskThumbnailPanel panel in Panels)
            {
                panel.UpdateTheme();
            }
        }

        private TaskThumbnailPanel CreatePanel(WorkerTask task)
        {
            TaskThumbnailPanel panel = new TaskThumbnailPanel(task);
            panel.ThumbnailSize = ThumbnailSize;
            panel.ClickAction = ClickAction;
            panel.TitleVisible = TitleVisible;
            panel.TitleLocation = TitleLocation;
            panel.MouseEnter += Panel_MouseEnter;
            panel.MouseDown += (object sender, MouseEventArgs e) => Panel_MouseDown(e, panel);
            panel.MouseUp += Panel_MouseUp;
            panel.ImagePreviewRequested += Panel_ImagePreviewRequested;
            return panel;
        }

        public TaskThumbnailPanel AddPanel(WorkerTask task)
        {
            TaskThumbnailPanel panel = CreatePanel(task);
            Panels.Add(panel);
            flpMain.Controls.Add(panel);
            flpMain.Controls.SetChildIndex(panel, 0);
            return panel;
        }

        public void RemovePanel(WorkerTask task)
        {
            TaskThumbnailPanel panel = FindPanel(task);

            if (panel != null)
            {
                Panels.Remove(panel);
                SelectedPanels.Remove(panel);
                flpMain.Controls.Remove(panel);
                panel.Dispose();
            }
        }

        public TaskThumbnailPanel FindPanel(WorkerTask task)
        {
            return Panels.FirstOrDefault(x => x.Task == task);
        }

        public void UpdateAllThumbnails(bool forceUpdate = false)
        {
            foreach (TaskThumbnailPanel panel in Panels)
            {
                if (forceUpdate || !panel.ThumbnailExists)
                {
                    panel.UpdateThumbnail();
                }
            }
        }

        public void UnselectAllPanels(TaskThumbnailPanel ignorePanel = null)
        {
            SelectedPanels.Clear();

            foreach (TaskThumbnailPanel panel in Panels)
            {
                if (panel != ignorePanel)
                {
                    panel.Selected = false;
                }
            }

            OnSelectedPanelChanged();
        }

        protected void OnContextMenuRequested(object sender, MouseEventArgs e)
        {
            ContextMenuRequested?.Invoke(sender, e);
        }

        protected void OnSelectedPanelChanged()
        {
            SelectedPanelChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Panel_MouseEnter(object sender, EventArgs e)
        {
            // Workaround to handle mouse wheel scrolling in Windows 7
            if (NativeMethods.GetForegroundWindow() == ParentForm.Handle && !flpMain.Focused)
            {
                flpMain.Focus();
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            Panel_MouseDown(e, null);
        }

        private void Panel_MouseDown(MouseEventArgs e, TaskThumbnailPanel panel)
        {
            if (panel == null)
            {
                UnselectAllPanels();
            }
            else
            {
                if (ModifierKeys == Keys.Control)
                {
                    if (panel.Selected)
                    {
                        panel.Selected = false;
                        SelectedPanels.Remove(panel);
                    }
                    else
                    {
                        panel.Selected = true;
                        SelectedPanels.Add(panel);
                    }
                }
                else if (ModifierKeys == Keys.Shift)
                {
                    if (SelectedPanels.Count > 0)
                    {
                        TaskThumbnailPanel firstPanel = SelectedPanels[0];

                        UnselectAllPanels();

                        foreach (TaskThumbnailPanel p in Panels.Range(firstPanel, panel))
                        {
                            p.Selected = true;
                            SelectedPanels.Add(p);
                        }
                    }
                    else
                    {
                        panel.Selected = true;
                        SelectedPanels.Add(panel);
                    }
                }
                else
                {
                    if (!panel.Selected || e.Button == MouseButtons.Left)
                    {
                        UnselectAllPanels(panel);
                        panel.Selected = true;
                        SelectedPanels.Add(panel);
                    }
                }
            }

            OnSelectedPanelChanged();
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OnContextMenuRequested(sender, e);
            }
        }

        private void Panel_ImagePreviewRequested(TaskThumbnailPanel panel)
        {
            string[] images = Panels.Select(x => x.Task.Info.FilePath).Reverse().ToArray();
            int currentImageIndex = Panels.Count - Panels.IndexOf(panel) - 1;
            ImageViewer.ShowImage(images, currentImageIndex);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            OnKeyDown(new KeyEventArgs(keyData));

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}