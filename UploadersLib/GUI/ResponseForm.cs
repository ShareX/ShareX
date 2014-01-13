#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Windows.Forms;
using UploadersLib.Properties;

namespace UploadersLib
{
    public partial class ResponseForm : Form
    {
        public string Response { get; private set; }

        private bool isOpened;

        public ResponseForm(string response)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            Response = response;
            txtSource.Text = Response;
        }

        private void tcResponse_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1 && !isOpened)
            {
                wbResponse.DocumentText = Response;
                isOpened = true;
            }
        }

        private void ResponseForm_Resize(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}