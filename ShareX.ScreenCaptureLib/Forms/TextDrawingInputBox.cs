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
        public string TextFont { get; private set; }
        public Color TextColor { get; private set; }
        public int TextSize { get; private set; }
        public bool TextBold { get; private set; }
        public bool TextItalic { get; private set; }
        public bool TextUnderline { get; private set; }

        public TextDrawingInputBox(string inputText, string textFont, Color textColor, int textSize)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;

            InputText = inputText;
            TextFont = textFont;
            TextColor = textColor;
            TextSize = textSize;

            if (InputText != null)
            {
                txtInput.Text = InputText;
            }

            UpdateInputBox();

            btnTextColor.Color = TextColor;
            nudTextSize.Value = TextSize;
        }

        private void nudTextSize_ValueChanged(object sender, EventArgs e)
        {
            UpdateInputBox();
        }

        private void cbBold_CheckedChanged(object sender, EventArgs e)
        {
            UpdateInputBox();
        }

        private void cbItalic_CheckedChanged(object sender, EventArgs e)
        {
            UpdateInputBox();
        }

        private void cbUnderline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateInputBox();
        }

        private void btnTextColor_ColorChanged(Color color)
        {
            UpdateInputBox();
        }

        private void UpdateInputBox()
        {
            FontStyle fontStyle = FontStyle.Regular;

            if (cbBold.Checked)
            {
                fontStyle |= FontStyle.Bold;
            }

            if (cbItalic.Checked)
            {
                fontStyle |= FontStyle.Italic;
            }

            if (cbUnderline.Checked)
            {
                fontStyle |= FontStyle.Underline;
            }

            txtInput.Font = new Font(TextFont, (float)nudTextSize.Value, fontStyle);

            txtInput.ForeColor = btnTextColor.Color;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            InputText = txtInput.Text;
            TextColor = btnTextColor.Color;
            TextSize = (int)nudTextSize.Value;
            TextBold = cbBold.Checked;
            TextItalic = cbItalic.Checked;
            TextUnderline = cbUnderline.Checked;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}