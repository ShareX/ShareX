#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class ActionsToolbarForm : Form
    {
        private static ActionsToolbarForm instance;

        public static ActionsToolbarForm Instance
        {
            get
            {
                if (!IsInstanceActive)
                {
                    instance = new ActionsToolbarForm();
                }

                return instance;
            }
        }

        public static bool IsInstanceActive => instance != null && !instance.IsDisposed;

        private IContainer components;
        private ToolStripEx tsMain;
        private ToolTip ttMain;
        private ContextMenuStrip cmsTitle;

        private ActionsToolbarForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            AllowDrop = true;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = SystemColors.ActiveBorder;
            ClientSize = new Size(284, 261);
            FormBorderStyle = FormBorderStyle.None;
            Icon = ShareXResources.Icon;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Text = "ShareX - Actions toolbar";
            TopMost = Program.Settings.ActionsToolbarStayTopMost;

            Shown += ActionsToolbarForm_Shown;
            LocationChanged += ActionsToolbarForm_LocationChanged;
            DragEnter += ActionsToolbarForm_DragEnter;
            DragDrop += ActionsToolbarForm_DragDrop;

            tsMain = new ToolStripEx()
            {
                AutoSize = true,
                CanOverflow = false,
                ClickThrough = true,
                Dock = DockStyle.None,
                GripStyle = ToolStripGripStyle.Hidden,
                Location = new Point(1, 1),
                Margin = new Padding(1),
                MinimumSize = new Size(10, 30),
                Padding = new Padding(0, 1, 0, 0),
                Renderer = new ToolStripRoundedEdgeRenderer(),
                TabIndex = 0,
                ShowItemToolTips = false
            };

            tsMain.MouseLeave += tsMain_MouseLeave;

            Controls.Add(tsMain);

            components = new Container();

            ttMain = new ToolTip(components)
            {
                AutoPopDelay = 15000,
                InitialDelay = 300,
                ReshowDelay = 100,
                ShowAlways = true
            };

            cmsTitle = new ContextMenuStrip(components);

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem(Resources.ActionsToolbar_Close);
            tsmiClose.Click += TsmiClose_Click;
            cmsTitle.Items.Add(tsmiClose);

            cmsTitle.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiLock = new ToolStripMenuItem(Resources.ActionsToolbar__LockPosition);
            tsmiLock.CheckOnClick = true;
            tsmiLock.Checked = Program.Settings.ActionsToolbarLockPosition;
            tsmiLock.Click += TsmiLock_Click;
            cmsTitle.Items.Add(tsmiLock);

            ToolStripMenuItem tsmiTopMost = new ToolStripMenuItem(Resources.ActionsToolbar_StayTopMost);
            tsmiTopMost.CheckOnClick = true;
            tsmiTopMost.Checked = Program.Settings.ActionsToolbarStayTopMost;
            tsmiTopMost.Click += TsmiTopMost_Click;
            cmsTitle.Items.Add(tsmiTopMost);

            ToolStripMenuItem tsmiRunAtStartup = new ToolStripMenuItem(Resources.ActionsToolbar_OpenAtShareXStartup);
            tsmiRunAtStartup.CheckOnClick = true;
            tsmiRunAtStartup.Checked = Program.Settings.ActionsToolbarRunAtStartup;
            tsmiRunAtStartup.Click += TsmiRunAtStartup_Click;
            cmsTitle.Items.Add(tsmiRunAtStartup);

            cmsTitle.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem(Resources.ActionsToolbar_Edit);
            tsmiEdit.Click += TsmiEdit_Click;
            cmsTitle.Items.Add(tsmiEdit);

            UpdateToolbar(Program.Settings.ActionsToolbarList);

            ResumeLayout(false);
            PerformLayout();

            UpdatePosition();
        }

        private void ActionsToolbarForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void ActionsToolbarForm_LocationChanged(object sender, EventArgs e)
        {
            CheckToolbarPosition();
        }

        private void ActionsToolbarForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(DataFormats.Bitmap, false) ||
                e.Data.GetDataPresent(DataFormats.Text, false))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void ActionsToolbarForm_DragDrop(object sender, DragEventArgs e)
        {
            UploadManager.DragDropUpload(e.Data);
        }

        private void CheckToolbarPosition()
        {
            Rectangle rectToolbar = Bounds;
            Rectangle rectScreen = CaptureHelpers.GetScreenWorkingArea();
            Point pos = rectToolbar.Location;

            if (rectToolbar.Width < rectScreen.Width)
            {
                if (rectToolbar.X < rectScreen.X)
                {
                    pos.X = rectScreen.X;
                }
                else if (rectToolbar.Right > rectScreen.Right)
                {
                    pos.X = rectScreen.Right - rectToolbar.Width;
                }
            }

            if (rectToolbar.Height < rectScreen.Height)
            {
                if (rectToolbar.Y < rectScreen.Y)
                {
                    pos.Y = rectScreen.Y;
                }
                else if (rectToolbar.Bottom > rectScreen.Bottom)
                {
                    pos.Y = rectScreen.Bottom - rectToolbar.Height;
                }
            }

            if (pos != rectToolbar.Location)
            {
                Location = pos;
            }

            Program.Settings.ActionsToolbarPosition = pos;
        }

        private void UpdatePosition()
        {
            Rectangle rectScreen = CaptureHelpers.GetScreenWorkingArea();

            if (!Program.Settings.ActionsToolbarPosition.IsEmpty && rectScreen.Contains(Program.Settings.ActionsToolbarPosition))
            {
                Location = Program.Settings.ActionsToolbarPosition;
            }
            else
            {
                Rectangle rectActiveScreen = CaptureHelpers.GetActiveScreenWorkingArea();

                if (Width < rectActiveScreen.Width)
                {
                    Location = new Point(rectActiveScreen.X + rectActiveScreen.Width - Width, rectActiveScreen.Y + rectActiveScreen.Height - Height);
                }
                else
                {
                    Location = rectActiveScreen.Location;
                }
            }
        }

        private void TsmiClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TsmiLock_Click(object sender, EventArgs e)
        {
            Program.Settings.ActionsToolbarLockPosition = ((ToolStripMenuItem)sender).Checked;
        }

        private void TsmiTopMost_Click(object sender, EventArgs e)
        {
            Program.Settings.ActionsToolbarStayTopMost = ((ToolStripMenuItem)sender).Checked;
            TopMost = Program.Settings.ActionsToolbarStayTopMost;
        }

        private void TsmiRunAtStartup_Click(object sender, EventArgs e)
        {
            Program.Settings.ActionsToolbarRunAtStartup = ((ToolStripMenuItem)sender).Checked;
        }

        private void TsmiEdit_Click(object sender, EventArgs e)
        {
            using (ActionsToolbarEditForm form = new ActionsToolbarEditForm(Program.Settings.ActionsToolbarList))
            {
                if (Program.Settings.ActionsToolbarStayTopMost)
                {
                    TopMost = false;
                }

                form.ShowDialog();

                if (Program.Settings.ActionsToolbarStayTopMost)
                {
                    TopMost = true;
                }

                UpdateToolbar(Program.Settings.ActionsToolbarList);
                CheckToolbarPosition();
            }
        }

        private void UpdateToolbar(List<HotkeyType> actions)
        {
            tsMain.SuspendLayout();

            tsMain.Items.Clear();

            ToolStripLabel tslTitle = new ToolStripLabel()
            {
                Margin = new Padding(4, 0, 3, 0),
                Text = "ShareX",
                ToolTipText = Resources.ActionsToolbar_Tip
            };

            tslTitle.MouseDown += tslTitle_MouseDown;
            tslTitle.MouseEnter += tslTitle_MouseEnter;
            tslTitle.MouseLeave += tslTitle_MouseLeave;
            tslTitle.MouseUp += tslTitle_MouseUp;

            tsMain.Items.Add(tslTitle);

            foreach (HotkeyType action in Program.Settings.ActionsToolbarList)
            {
                if (action == HotkeyType.None)
                {
                    ToolStripSeparator tss = new ToolStripSeparator()
                    {
                        Margin = new Padding(0)
                    };

                    tsMain.Items.Add(tss);
                }
                else
                {
                    ToolStripButton tsb = new ToolStripButton()
                    {
                        Text = action.GetLocalizedDescription(),
                        DisplayStyle = ToolStripItemDisplayStyle.Image,
                        Image = TaskHelpers.GetHotkeyTypeIcon(action)
                    };

                    tsb.Click += (sender, e) =>
                    {
                        if (Program.Settings.ActionsToolbarStayTopMost)
                        {
                            TopMost = false;
                        }

                        TaskHelpers.ExecuteJob(action);

                        if (Program.Settings.ActionsToolbarStayTopMost)
                        {
                            TopMost = true;
                        }
                    };

                    tsMain.Items.Add(tsb);
                }
            }

            foreach (ToolStripItem tsi in tsMain.Items)
            {
                tsi.MouseEnter += (sender, e) =>
                {
                    string text;

                    if (!string.IsNullOrEmpty(tsi.ToolTipText))
                    {
                        text = tsi.ToolTipText;
                    }
                    else
                    {
                        text = tsi.Text;
                    }

                    ttMain.SetToolTip(tsMain, text);
                };

                tsi.MouseLeave += tsMain_MouseLeave;
            }

            tsMain.ResumeLayout(false);
            tsMain.PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void tsMain_MouseLeave(object sender, EventArgs e)
        {
            ttMain.RemoveAll();
        }

        private void tslTitle_MouseEnter(object sender, EventArgs e)
        {
            if (!Program.Settings.ActionsToolbarLockPosition)
            {
                Cursor = Cursors.SizeAll;
            }
        }

        private void tslTitle_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void tslTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !Program.Settings.ActionsToolbarLockPosition)
            {
                NativeMethods.ReleaseCapture();
                Message message = Message.Create(Handle, (int)WindowsMessages.SYSCOMMAND, new IntPtr(NativeConstants.MOUSE_MOVE), IntPtr.Zero);
                DefWndProc(ref message);
            }
        }

        private void tslTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cmsTitle.Show(Location.X, Location.Y + Size.Height - 1);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                Close();
            }
        }
    }
}