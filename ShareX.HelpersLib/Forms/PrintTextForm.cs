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

using ShareX.HelpersLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.HelpersLib
{
    public partial class PrintTextForm : Form
    {
        private PrintHelper printHelper;
        private PrintSettings printSettings;

        public PrintTextForm(string text, PrintSettings settings)
        {
            InitializeComponent();
            ShareXResources.ApplyTheme(this);

            printHelper = new PrintHelper(text);
            printHelper.Settings = printSettings = settings;
            LoadSettings();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (printHelper != null)
                {
                    printHelper.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void LoadSettings()
        {
            Font font = printSettings.TextFont;
            lblFont.Text = string.Format(Resources.PrintTextForm_LoadSettings_Name___0___Size___1_, font.Name, font.Size);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            printHelper.Print();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnShowPreview_Click(object sender, EventArgs e)
        {
            printHelper.ShowPreview();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnChangeFont_Click(object sender, EventArgs e)
        {
            using (FontDialog fd = new FontDialog())
            {
                fd.Font = printSettings.TextFont;

                if (fd.ShowDialog() == DialogResult.OK)
                {
                    printSettings.TextFont = fd.Font;
                    LoadSettings();
                }
            }
        }
    }
}