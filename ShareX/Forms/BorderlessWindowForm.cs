#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2021 ShareX Team

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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX
{
    public partial class BorderlessWindowForm : Form
    {
        public BorderlessWindowSettings Settings { get; private set; }

        public BorderlessWindowForm(BorderlessWindowSettings settings)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            Settings = settings;
        }

        private bool MakeWindowBorderless(string windowTitle)
        {
            if (!string.IsNullOrEmpty(windowTitle))
            {
                IntPtr hWnd = SearchWindow(windowTitle);

                if (hWnd == IntPtr.Zero)
                {
                    // TODO: Translate
                    MessageBox.Show("Unable to find a window with specified window title.", "ShareX", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MakeWindowBorderless(hWnd);
                    return true;
                }
            }

            return false;
        }

        private void MakeWindowBorderless(IntPtr hWnd)
        {
            WindowInfo windowInfo = new WindowInfo(hWnd);

            if (windowInfo.IsMinimized)
            {
                windowInfo.Restore();
            }

            WindowStyles windowStyle = windowInfo.Style;
            windowStyle &= ~(WindowStyles.WS_CAPTION | WindowStyles.WS_THICKFRAME | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_MAXIMIZEBOX | WindowStyles.WS_SYSMENU);
            windowInfo.Style = windowStyle;

            WindowStyles windowExStyle = windowInfo.ExStyle;
            windowExStyle &= ~(WindowStyles.WS_EX_DLGMODALFRAME | WindowStyles.WS_EX_CLIENTEDGE | WindowStyles.WS_EX_STATICEDGE);
            windowInfo.ExStyle = windowExStyle;

            Rectangle rect = Screen.FromHandle(hWnd).Bounds;

            SetWindowPosFlags setWindowPosFlag = SetWindowPosFlags.SWP_FRAMECHANGED | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOOWNERZORDER;

            if (rect.IsEmpty)
            {
                setWindowPosFlag |= SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOSIZE;
            }

            windowInfo.SetWindowPos(rect, setWindowPosFlag);
        }

        private IntPtr SearchWindow(string windowTitle)
        {
            IntPtr hWnd = NativeMethods.FindWindow(null, windowTitle);

            if (hWnd == IntPtr.Zero)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if (process.MainWindowTitle.Contains(windowTitle, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return process.MainWindowHandle;
                    }
                }
            }

            return hWnd;
        }

        #region Form events

        private void BorderlessWindowForm_Shown(object sender, EventArgs e)
        {
            if (Settings.RememberWindowTitle && !string.IsNullOrEmpty(Settings.WindowTitle))
            {
                txtWindowTitle.Text = Settings.WindowTitle;
                btnMakeWindowBorderless.Focus();
            }
        }

        private void txtWindowTitle_TextChanged(object sender, EventArgs e)
        {
            btnMakeWindowBorderless.Enabled = !string.IsNullOrEmpty(txtWindowTitle.Text);
        }

        private void btnMakeWindowBorderless_Click(object sender, EventArgs e)
        {
            try
            {
                string windowTitle = txtWindowTitle.Text;

                if (Settings.RememberWindowTitle)
                {
                    Settings.WindowTitle = windowTitle;
                }
                else
                {
                    Settings.WindowTitle = "";
                }

                bool result = MakeWindowBorderless(windowTitle);

                if (result && Settings.AutoCloseWindow)
                {
                    Close();
                }
            }
            catch (Exception ex)
            {
                ex.ShowError();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (BorderlessWindowSettingsForm form = new BorderlessWindowSettingsForm(Settings))
            {
                form.ShowDialog();
            }
        }

        #endregion
    }
}