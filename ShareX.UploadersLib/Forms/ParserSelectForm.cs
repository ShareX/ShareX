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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public partial class ParserSelectForm : Form
    {
        public string[] Texts { get; private set; }
        public string SelectedText { get; private set; }

        public ParserSelectForm(string[] texts)
        {
            InitializeComponent();

            Texts = texts;
            SelectedText = Texts[0];

            SuspendLayout();

            int maxButtonWidth = 0;
            int rowSize = 10;

            for (int i = 0; i < Texts.Length; i++)
            {
                string text = Texts[i];

                if (!string.IsNullOrEmpty(text))
                {
                    Button button = new Button()
                    {
                        AutoSize = true,
                        Margin = new Padding(i < rowSize ? 5 : 0, i % rowSize == 0 ? 5 : 0, 5, 5),
                        Padding = new Padding(5),
                        Font = new Font(Font.FontFamily, 12),
                        Text = text,
                        UseVisualStyleBackColor = true
                    };

                    button.Click += (sender, e) =>
                    {
                        SelectedText = text;
                        Close();
                    };

                    flpMain.Controls.Add(button);
                    if ((i + 1) % rowSize == 0) flpMain.SetFlowBreak(button, true);
                    maxButtonWidth = Math.Max(button.Width, maxButtonWidth);
                }
            }

            foreach (Control control in flpMain.Controls)
            {
                control.Width = maxButtonWidth;
            }

            ResumeLayout();

            ShareXResources.ApplyTheme(this, true);
        }

        private void ParserSelectForm_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }
    }
}