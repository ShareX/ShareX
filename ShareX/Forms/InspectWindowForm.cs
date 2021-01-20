#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
using ShareX.ScreenCaptureLib;
using System;
using System.Windows.Forms;

namespace ShareX
{
    public partial class InspectWindowForm : Form
    {
        public WindowInfo SelectedWindow { get; private set; }

        public InspectWindowForm()
        {
            InitializeComponent();
            rtbInfo.AddContextMenu();
            ShareXResources.ApplyTheme(this);
            SelectHandle();
        }

        private bool SelectHandle()
        {
            return SelectHandle(new RegionCaptureOptions());
        }

        private bool SelectHandle(RegionCaptureOptions options)
        {
            SelectedWindow = null;

            SimpleWindowInfo simpleWindowInfo = RegionCaptureTasks.GetWindowInfo(options);

            if (simpleWindowInfo != null)
            {
                SelectedWindow = new WindowInfo(simpleWindowInfo.Handle);
                UpdateWindowInfo();
                return true;
            }

            return false;
        }

        private void UpdateWindowInfo()
        {
            rtbInfo.ResetText();

            if (SelectedWindow != null)
            {
                try
                {
                    AddInfo("Window handle", SelectedWindow.Handle.ToString("X8"));
                    AddInfo("Window title", SelectedWindow.Text);
                    AddInfo("Class name", SelectedWindow.ClassName);
                    AddInfo("Process name", SelectedWindow.ProcessName);
                    AddInfo("Process file name", SelectedWindow.ProcessFileName);
                    AddInfo("Process identifier", SelectedWindow.ProcessId.ToString());
                    AddInfo("Window rectangle", SelectedWindow.Rectangle.ToStringProper());
                    AddInfo("Client rectangle", SelectedWindow.ClientRectangle.ToStringProper());
                    AddInfo("Window styles", SelectedWindow.Styles.ToString());
                }
                catch
                {
                }
            }
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

        private void btnInspectWindow_Click(object sender, EventArgs e)
        {
            RegionCaptureOptions options = new RegionCaptureOptions()
            {
                DetectControls = false
            };

            SelectHandle(options);
        }

        private void btnInspectControl_Click(object sender, EventArgs e)
        {
            SelectHandle();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateWindowInfo();
        }
    }
}