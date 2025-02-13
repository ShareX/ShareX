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
using ShareX.Properties;
using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ShareX
{
    public partial class InspectWindowForm : Form
    {
        public WindowInfo SelectedWindow { get; private set; }
        public bool IsWindow { get; private set; }

        private bool updating;

        public InspectWindowForm()
        {
            InitializeComponent();
            rtbInfo.AddContextMenu();
            ShareXResources.ApplyTheme(this, true);
            SelectHandle(true);
        }

        private void UpdateWindowListMenu()
        {
            cmsWindowList.Items.Clear();

            WindowsList windowsList = new WindowsList();
            List<WindowInfo> windows = windowsList.GetVisibleWindowsList();

            if (windows != null && windows.Count > 0)
            {
                List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();

                foreach (WindowInfo window in windows)
                {
                    try
                    {
                        string title = window.Text;
                        string shortTitle = title.Truncate(50, "...");
                        ToolStripMenuItem tsmi = new ToolStripMenuItem(shortTitle);
                        tsmi.Click += (sender, e) => SelectWindow(window.Handle, true);

                        using (Icon icon = window.Icon)
                        {
                            if (icon != null && icon.Width > 0 && icon.Height > 0)
                            {
                                tsmi.Image = icon.ToBitmap();
                            }
                        }

                        items.Add(tsmi);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e);
                    }
                }

                cmsWindowList.Items.AddRange(items.OrderBy(x => x.Text).ToArray());
            }
        }

        private void SelectWindow(IntPtr handle, bool isWindow)
        {
            SelectedWindow = new WindowInfo(handle);
            IsWindow = isWindow;

            UpdateWindowInfo();
        }

        private bool SelectHandle(bool isWindow)
        {
            RegionCaptureOptions options = new RegionCaptureOptions()
            {
                DetectControls = !isWindow
            };

            SelectedWindow = null;

            SimpleWindowInfo simpleWindowInfo = RegionCaptureTasks.GetWindowInfo(options);

            if (simpleWindowInfo != null)
            {
                SelectWindow(simpleWindowInfo.Handle, isWindow);

                return true;
            }

            UpdateWindowInfo();

            return false;
        }

        private void UpdateWindowInfo()
        {
            updating = true;

            btnRefresh.Enabled = SelectedWindow != null;

            if (SelectedWindow != null && IsWindow)
            {
                cbTopMost.Visible = true;
                cbTopMost.Checked = SelectedWindow.TopMost;

                nudOpacity.Visible = true;
                nudOpacity.SetValue((int)Math.Round(SelectedWindow.Opacity / 255.0 * 100));
                lblOpacity.Visible = true;
                lblOpacityTip.Visible = true;
            }
            else
            {
                cbTopMost.Visible = false;
                nudOpacity.Visible = false;
                lblOpacity.Visible = false;
                lblOpacityTip.Visible = false;
            }

            rtbInfo.ResetText();

            if (SelectedWindow != null)
            {
                try
                {
                    AddInfo(Resources.InspectWindow_WindowHandle, SelectedWindow.Handle.ToString("X8"));
                    AddInfo(Resources.InspectWindow_WindowTitle, SelectedWindow.Text);
                    AddInfo(Resources.InspectWindow_ClassName, SelectedWindow.ClassName);
                    AddInfo(Resources.InspectWindow_ProcessName, SelectedWindow.ProcessName);
                    AddInfo(Resources.InspectWindow_ProcessFileName, SelectedWindow.ProcessFileName);
                    AddInfo(Resources.InspectWindow_ProcessIdentifier, SelectedWindow.ProcessId.ToString());
                    AddInfo(Resources.InspectWindow_WindowRectangle, SelectedWindow.Rectangle.ToStringProper());
                    AddInfo(Resources.InspectWindow_ClientRectangle, SelectedWindow.ClientRectangle.ToStringProper());
                    AddInfo(Resources.InspectWindow_WindowStyles, SelectedWindow.Style.ToString().Replace(", ", "\r\n"));
                    AddInfo(Resources.InspectWindow_ExtendedWindowStyles, SelectedWindow.ExStyle.ToString().Replace(", ", "\r\n"));
                }
                catch
                {
                }
            }

            updating = false;
        }

        private void AddInfo(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (rtbInfo.TextLength > 0)
                {
                    rtbInfo.AppendLine();
                    rtbInfo.AppendLine();
                }

                rtbInfo.SetFontBold();
                rtbInfo.AppendLine(name);
                rtbInfo.SetFontRegular();
                rtbInfo.AppendText(value);
            }
        }

        private void mbWindowList_MouseDown(object sender, MouseEventArgs e)
        {
            UpdateWindowListMenu();
        }

        private void btnInspectWindow_Click(object sender, EventArgs e)
        {
            SelectHandle(true);
        }

        private void btnInspectControl_Click(object sender, EventArgs e)
        {
            SelectHandle(false);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateWindowInfo();
        }

        private void cbTopMost_CheckedChanged(object sender, EventArgs e)
        {
            if (!updating && SelectedWindow != null)
            {
                try
                {
                    WindowInfo windowInfo = new WindowInfo(SelectedWindow.Handle);
                    windowInfo.TopMost = cbTopMost.Checked;

                    UpdateWindowInfo();
                }
                catch
                {
                }
            }
        }

        private void nudOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (!updating && SelectedWindow != null)
            {
                try
                {
                    WindowInfo windowInfo = new WindowInfo(SelectedWindow.Handle);
                    windowInfo.Opacity = (byte)Math.Round(nudOpacity.Value / 100 * 255);

                    UpdateWindowInfo();
                }
                catch
                {
                }
            }
        }
    }
}