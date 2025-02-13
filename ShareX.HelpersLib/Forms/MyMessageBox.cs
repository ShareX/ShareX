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
    public class MyMessageBox : Form
    {
        private const int LabelHorizontalPadding = 15;
        private const int LabelVerticalPadding = 20;
        private const int ButtonPadding = 10;

        private DialogResult button1Result = DialogResult.OK;
        private DialogResult button2Result = DialogResult.No;

        public bool IsChecked { get; private set; }

        public MyMessageBox(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK, string checkBoxText = null, bool isChecked = false)
        {
            Width = 180;
            Height = 100;
            Text = caption;
            BackColor = SystemColors.Window;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
            MinimizeBox = false;
            MaximizeBox = false;

            Shown += MyMessageBox_Shown;

            Label labelText = new Label();
            labelText.Margin = new Padding(0);
            labelText.Font = SystemFonts.MessageBoxFont;
            labelText.TextAlign = ContentAlignment.MiddleLeft;
            labelText.AutoSize = true;
            labelText.MinimumSize = new Size(125, 0);
            labelText.MaximumSize = new Size(400, 400);
            labelText.Location = new Point(0, 0);
            labelText.Text = text;

            Button button1 = new Button();
            button1.Margin = new Padding(0, ButtonPadding, ButtonPadding, ButtonPadding);
            button1.BackColor = Color.Transparent;
            button1.Size = new Size(80, 26);
            button1.UseVisualStyleBackColor = false;
            button1.Text = "button1";
            button1.TabIndex = 0;
            button1.Click += (sender, e) =>
            {
                DialogResult = button1Result;
                Close();
            };

            Button button2 = new Button();
            button2.Margin = new Padding(0, ButtonPadding, ButtonPadding, ButtonPadding);
            button2.BackColor = Color.Transparent;
            button2.Size = new Size(80, 26);
            button2.UseVisualStyleBackColor = false;
            button2.Text = "button2";
            button2.TabIndex = 1;
            button2.Click += (sender, e) =>
            {
                DialogResult = button2Result;
                Close();
            };

            switch (buttons)
            {
                default:
                case MessageBoxButtons.OK:
                    button1.Text = Resources.MyMessageBox_MyMessageBox_OK;
                    button1Result = DialogResult.OK;
                    button2.Visible = false;
                    break;
                case MessageBoxButtons.OKCancel:
                    button1.Text = Resources.MyMessageBox_MyMessageBox_OK;
                    button1Result = DialogResult.OK;
                    button2.Text = Resources.MyMessageBox_MyMessageBox_Cancel;
                    button2Result = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    button1.Text = Resources.MyMessageBox_MyMessageBox_Yes;
                    button1Result = DialogResult.Yes;
                    button2.Text = Resources.MyMessageBox_MyMessageBox_No;
                    button2Result = DialogResult.No;
                    break;
            }

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.BackColor = Color.FromArgb(240, 240, 240);
            panel.FlowDirection = FlowDirection.RightToLeft;

            FlowLayoutPanel labelPanel = new FlowLayoutPanel();
            labelPanel.FlowDirection = FlowDirection.TopDown;
            labelPanel.AutoSize = true;
            labelPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            labelPanel.Location = new Point(LabelHorizontalPadding, LabelVerticalPadding);

            labelPanel.Controls.Add(labelText);

            if (checkBoxText != null)
            {
                IsChecked = isChecked;

                CheckBox checkBox = new CheckBox();
                checkBox.Font = SystemFonts.MessageBoxFont;
                checkBox.Margin = new Padding(2, LabelVerticalPadding, 0, 0);
                checkBox.AutoSize = true;
                checkBox.Text = checkBoxText;
                checkBox.CheckedChanged += (sender, e) => IsChecked = checkBox.Checked;
                labelPanel.Controls.Add(checkBox);
            }

            panel.Controls.Add(button2);
            panel.Controls.Add(button1);
            Controls.Add(labelPanel);
            Controls.Add(panel);

            panel.Location = new Point(0, labelPanel.Bottom + LabelVerticalPadding);
            panel.Size = new Size(labelPanel.Width + (LabelHorizontalPadding * 2), button1.Height + (ButtonPadding * 2));
            ClientSize = new Size(panel.Width, labelPanel.Height + (LabelVerticalPadding * 2) + panel.Height);

            ShareXResources.ApplyTheme(this);

            if (ShareXResources.UseCustomTheme)
            {
                panel.BackColor = ShareXResources.Theme.BorderColor;
            }
        }

        private void MyMessageBox_Shown(object sender, EventArgs e)
        {
            this.ForceActivate();
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            using (MyMessageBox messageBox = new MyMessageBox(text, caption, buttons))
            {
                return messageBox.ShowDialog();
            }
        }
    }
}