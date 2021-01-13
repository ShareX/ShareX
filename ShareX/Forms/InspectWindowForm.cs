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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareX
{
    public partial class InspectWindowForm : Form
    {
        public WindowInfo SelectedWindow { get; private set; }

        public InspectWindowForm()
        {
            InitializeComponent();
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
                StringBuilder sbWindowInfo = new StringBuilder();
                sbWindowInfo.Append("Handle: " + SelectedWindow.Handle.ToString("X8"));
                rtbInfo.Text = sbWindowInfo.ToString();
            }
        }
    }
}