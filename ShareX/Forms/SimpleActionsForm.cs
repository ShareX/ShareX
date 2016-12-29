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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class SimpleActionsForm : Form
    {
        public List<HotkeyType> Actions { get; set; } = new List<HotkeyType>() { HotkeyType.RectangleRegion, HotkeyType.PrintScreen, HotkeyType.LastRegion,
            HotkeyType.None, HotkeyType.FileUpload, HotkeyType.ClipboardUploadWithContentViewer, HotkeyType.None, HotkeyType.ScreenColorPicker };

        public bool LockPosition { get; set; }

        private IContainer components;
        private ToolStripEx tsMain;
        private ToolTip ttMain;
        private ContextMenuStrip cmsTitle;

        public SimpleActionsForm()
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
            ClientSize = new Size(284, 261);
            FormBorderStyle = FormBorderStyle.None;
            Icon = ShareXResources.Icon;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ShareX - Simple actions";
            TopMost = true;

            Shown += new EventHandler(SimpleActionsForm_Shown);

            tsMain = new ToolStripEx()
            {
                AutoSize = true,
                CanOverflow = false,
                ClickThrough = true,
                Dock = DockStyle.None,
                GripStyle = ToolStripGripStyle.Hidden,
                Location = new Point(0, 0),
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

            tsMain.MouseLeave += new EventHandler(tsMain_MouseLeave);

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
            tsmiClose.Click += (sender, e) => Close();
            cmsTitle.Items.Add(tsmiClose);

            ToolStripMenuItem tsmiEdit = new ToolStripMenuItem("Edit");
            tsmiEdit.Click += (sender, e) =>
            {
                using (SimpleActionsEditForm form = new SimpleActionsEditForm(Actions))
                {
                    TopMost = false;
                    form.ShowDialog();
                    TopMost = true;

                    UpdateToolbar(Actions);
                }
            };
            cmsTitle.Items.Add(tsmiEdit);

            ToolStripMenuItem tsmiLock = new ToolStripMenuItem("Lock position");
            tsmiLock.CheckOnClick = true;
            tsmiLock.Checked = false;
            tsmiLock.Click += (sender, e) => LockPosition = tsmiLock.Checked;
            cmsTitle.Items.Add(tsmiLock);

            ToolStripMenuItem tsmiTopMost = new ToolStripMenuItem("Stay top most");
            tsmiTopMost.CheckOnClick = true;
            tsmiTopMost.Checked = true;
            tsmiTopMost.Click += (sender, e) => TopMost = tsmiTopMost.Checked;
            cmsTitle.Items.Add(tsmiTopMost);

            UpdateToolbar(Actions);

            ResumeLayout(false);
            PerformLayout();
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

            tslTitle.MouseDown += new MouseEventHandler(tslTitle_MouseDown);
            tslTitle.MouseEnter += new EventHandler(tslTitle_MouseEnter);
            tslTitle.MouseLeave += new EventHandler(tslTitle_MouseLeave);
            tslTitle.MouseUp += new MouseEventHandler(tslTitle_MouseUp);

            tsMain.Items.Add(tslTitle);

            foreach (HotkeyType action in Actions)
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
                        TopMost = false;
                        TaskHelpers.ExecuteJob(action);
                        TopMost = true;
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

        private void SimpleActionsForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
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
            if (e.Button == MouseButtons.Left && !LockPosition)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.DefWindowProc(Handle, (uint)WindowsMessages.SYSCOMMAND, (UIntPtr)NativeConstants.MOUSE_MOVE, IntPtr.Zero);
            }
        }

        private void tslTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cmsTitle.Show(Location.X, Location.Y + Size.Height);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                Close();
            }
        }
    }
}