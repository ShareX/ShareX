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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    internal partial class TextDrawingInputBox : Form
    {
        public string InputText { get; private set; }
        public Color TextColor { get; private set; }
        public int TextSize { get; private set; }

        public TextDrawingInputBox(string inputText, Color textColor, int textSize)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            if (inputText != null)
            {
                txtInput.Text = inputText;
            }

            txtInput.Font = new Font(txtInput.Font.FontFamily, textSize);
            btnTextColor.Color = textColor;
            nudTextSize.Value = textSize;
        }

        private void nudTextSize_ValueChanged(object sender, EventArgs e)
        {
            txtInput.Font = new Font(txtInput.Font.FontFamily, (float)nudTextSize.Value);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInput.Text;
            TextColor = btnTextColor.Color;
            TextSize = (int)nudTextSize.Value;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}