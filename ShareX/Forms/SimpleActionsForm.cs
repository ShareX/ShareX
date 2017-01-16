#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class SimpleActionsForm : Form
    {
        private static SimpleActionsForm instance;

        public static SimpleActionsForm Instance
        {
            get
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new SimpleActionsForm();
                }

                return instance;
            }
        }

        private IContainer components;
        private ToolStripEx tsMain;
        private ToolTip ttMain;
        private ContextMenuStrip cmsTitle;

        private SimpleActionsForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

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
            Text = "ShareX - Simple actions";
            TopMost = Program.Settings.SimpleActionsFormStayTopMost;

            LocationChanged += SimpleActionsForm_LocationChanged;
            Shown += SimpleActionsForm_Shown;

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
                Padding = new Padding(0),
                Renderer = new CustomToolStripProfessionalRenderer(),
                TabIndex = 0,
                ShowItemToolTips = false
            };

            // https://www.medo64.com/2014/01/scaling-toolstrip-with-dpi/
            using (Graphics g = CreateGraphics())
            {
                double scale = Math.Max(g.DpiX, g.DpiY) / 96.0;
                double newScale = ((int)Math.Floor(scale * 100) / 25 * 25) / 100.0;
                if (newScale > 1)
                {
                    int newWidth = (int)(tsMain.ImageScalingSize.Width * newScale);
                    int newHeight = (int)(tsMain.ImageScalingSize.Height * newScale);
                    tsMain.ImageScalingSize = new Size(newWidth, newHeight);
                }
            }

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

            ToolStripMenuItem tsmiClose = new ToolStripMenuItem("Close");
            tsmiClose.Click += TsmiClose_Click;
            cmsTitle.Items.Add(tsmiClose);

            cmsTitle.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiLock = new ToolStripMenuItem("Lock position");
            tsmiLock.CheckOnClick = true;
            tsmiLock.Checked = Program.Settings.SimpleActionsFormLockPosition;
            tsmiLock.Click += TsmiLock_Click;
            cmsTitle.Items.Add(tsmiLock);

            ToolStripMenuItem tsmiTopMost = new ToolStripMenuItem("Stay top most");
            tsmiTopMost.CheckOnClick = true;
            tsmiTopMost.Checked = Program.Settings.SimpleActionsFormStayTopMost;
            tsmiTopMost.Click += TsmiTopMost_Click;
            cmsTitle.Items.Add(tsmiTopMost);

            ToolStripMenuItem tsmiRunAtStartup = new ToolStripMenuItem("Open at ShareX startup");
            tsmiRunAtStartup.CheckOnClick = true;
            tsmiRunAtStartup.Checked = Program.Settings.SimpleActionsFormRunAtStartup;
            tsmiRunAtStartup.Click += TsmiRunAtStartup_Click;
            cmsTitle.Items.Add(tsmiRunAtStartup);

            cmsTitle.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem("Edit...");
            tsmiEdit.Click += TsmiEdit_Click;
            cmsTitle.Items.Add(tsmiEdit);

            UpdateToolbar(Program.Settings.SimpleActionsList);

            ResumeLayout(false);
            PerformLayout();

            UpdatePosition();
        }

        private void SimpleActionsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        private void SimpleActionsForm_LocationChanged(object sender, EventArgs e)
        {
            CheckToolbarPosition();
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

            Program.Settings.SimpleActionsFormPosition = pos;
        }

        private void UpdatePosition()
        {
            Rectangle rectScreen = CaptureHelpers.GetScreenWorkingArea();

            if (!Program.Settings.SimpleActionsFormPosition.IsEmpty && rectScreen.Contains(Program.Settings.SimpleActionsFormPosition))
            {
                Location = Program.Settings.SimpleActionsFormPosition;
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
            Program.Settings.SimpleActionsFormLockPosition = ((ToolStripMenuItem)sender).Checked;
        }

        private void TsmiTopMost_Click(object sender, EventArgs e)
        {
            Program.Settings.SimpleActionsFormStayTopMost = ((ToolStripMenuItem)sender).Checked;
            TopMost = Program.Settings.SimpleActionsFormStayTopMost;
        }

        private void TsmiRunAtStartup_Click(object sender, EventArgs e)
        {
            Program.Settings.SimpleActionsFormRunAtStartup = ((ToolStripMenuItem)sender).Checked;
        }

        private void TsmiEdit_Click(object sender, EventArgs e)
        {
            using (SimpleActionsEditForm form = new SimpleActionsEditForm(Program.Settings.SimpleActionsList))
            {
                if (Program.Settings.SimpleActionsFormStayTopMost)
                {
                    TopMost = false;
                }

                form.ShowDialog();

                if (Program.Settings.SimpleActionsFormStayTopMost)
                {
                    TopMost = true;
                }

                UpdateToolbar(Program.Settings.SimpleActionsList);
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
                ToolTipText = "Hold left down to drag\r\nRight click to open menu\r\nMiddle click to close"
            };

            tslTitle.MouseDown += tslTitle_MouseDown;
            tslTitle.MouseEnter += tslTitle_MouseEnter;
            tslTitle.MouseLeave += tslTitle_MouseLeave;
            tslTitle.MouseUp += tslTitle_MouseUp;

            tsMain.Items.Add(tslTitle);

            foreach (HotkeyType action in Program.Settings.SimpleActionsList)
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
                        if (Program.Settings.SimpleActionsFormStayTopMost)
                        {
                            TopMost = false;
                        }

                        TaskHelpers.ExecuteJob(action);

                        if (Program.Settings.SimpleActionsFormStayTopMost)
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
            Cursor = Cursors.SizeAll;
        }

        private void tslTitle_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void tslTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !Program.Settings.SimpleActionsFormLockPosition)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
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