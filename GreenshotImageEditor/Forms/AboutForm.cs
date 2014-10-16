/*
* Greenshot - a free and open source screenshot tool
* Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
*
* For more information see: http://getgreenshot.org/
* The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 1 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using GreenshotPlugin.Core;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Greenshot
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Icon = GreenshotResources.getGreenshotIcon();
            lblLicense.Text = @"Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
Greenshot comes with ABSOLUTELY NO WARRANTY. This is free software, and you are welcome to redistribute it under certain conditions.
Details about the GNU General Public License:";
        }

        private void LinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel linkLabel = sender as LinkLabel;
            if (linkLabel != null)
            {
                try
                {
                    linkLabel.LinkVisited = true;
                    Process.Start(linkLabel.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show(string.Format("Could not open link '{0}'.", linkLabel.Text), "Error");
                }
            }
        }
    }
}