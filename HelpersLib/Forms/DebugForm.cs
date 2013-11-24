#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

using HelpersLib.Properties;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HelpersLib
{
    public partial class DebugForm : Form
    {
        private Logger logger;

        public DebugForm(Logger logger)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            this.logger = logger;

            rtbDebug.Text = logger.ToString();
            rtbDebug.SelectionStart = rtbDebug.TextLength;
            rtbDebug.ScrollToCaret();
            rtbDebug.AddContextMenu();

            logger.MessageAdded += logger_MessageAdded;
        }

        private void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logger.MessageAdded -= logger_MessageAdded;
        }

        private void logger_MessageAdded(string message)
        {
            this.InvokeSafe(() => AppendMessage(message));
        }

        private void AppendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                int start = rtbDebug.SelectionStart;
                int len = rtbDebug.SelectionLength;
                rtbDebug.AppendText(message + Environment.NewLine);
                if (len > 0)
                {
                    rtbDebug.Select(start, len);
                }
            }
        }

        private void btnCopyAll_Click(object sender, EventArgs e)
        {
            string text = rtbDebug.Text.Trim();
            ClipboardHelpers.CopyText(text);
        }

        private void btnLoadedAssemblies_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            string directoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.Location.StartsWith(directoryPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    sb.AppendLine(assembly.ManifestModule.Name);
                }
            }
            string assemblies = sb.ToString().Trim();

            MessageBox.Show(assemblies, "ShareX - Loaded assemblies", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void rtbDebug_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Helpers.LoadBrowserAsync(e.LinkText);
        }
    }
}