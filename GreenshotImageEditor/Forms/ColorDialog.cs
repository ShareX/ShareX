/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Configuration;
using Greenshot.Controls;
using Greenshot.IniFile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Greenshot
{
    /// <summary>
    /// Description of ColorDialog.
    /// </summary>
    public partial class ColorDialog : BaseForm
    {
        private static ColorDialog uniqueInstance;
        private static EditorConfiguration editorConfiguration = IniConfig.GetIniSection<EditorConfiguration>();

        private ColorDialog()
        {
            SuspendLayout();
            InitializeComponent();
            SuspendLayout();
            createColorPalette(5, 5, 15, 15);
            createLastUsedColorButtonRow(5, 190, 15, 15);
            ResumeLayout();
            updateRecentColorsButtonRow();
        }

        public static ColorDialog GetInstance()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new ColorDialog();
            }
            return uniqueInstance;
        }

        private List<Button> colorButtons = new List<Button>();
        private List<Button> recentColorButtons = new List<Button>();
        private ToolTip toolTip = new ToolTip();
        private bool updateInProgress = false;

        public Color Color
        {
            get { return colorPanel.BackColor; }
            set { previewColor(value, this); }
        }

        #region user interface generation

        private void createColorPalette(int x, int y, int w, int h)
        {
            createColorButtonColumn(255, 0, 0, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255, 255 / 2, 0, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255, 255, 0, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255 / 2, 255, 0, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(0, 255, 0, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(0, 255, 255 / 2, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(0, 255, 255, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(0, 255 / 2, 255, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(0, 0, 255, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255 / 2, 0, 255, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255, 0, 255, x, y, w, h, 11);
            x += w;
            createColorButtonColumn(255, 0, 255 / 2, x, y, w, h, 11);
            x += w + 5;
            createColorButtonColumn(255 / 2, 255 / 2, 255 / 2, x, y, w, h, 11);

            Controls.AddRange(colorButtons.ToArray());
        }

        private void createColorButtonColumn(int red, int green, int blue, int x, int y, int w, int h, int shades)
        {
            int shadedColorsNum = (shades - 1) / 2;
            for (int i = 0; i <= shadedColorsNum; i++)
            {
                colorButtons.Add(createColorButton(red * i / shadedColorsNum, green * i / shadedColorsNum, blue * i / shadedColorsNum, x, y + i * h, w, h));
                if (i > 0) colorButtons.Add(createColorButton(red + (255 - red) * i / shadedColorsNum, green + (255 - green) * i / shadedColorsNum, blue + (255 - blue) * i / shadedColorsNum, x, y + (i + shadedColorsNum) * h, w, h));
            }
        }

        private Button createColorButton(int red, int green, int blue, int x, int y, int w, int h)
        {
            return createColorButton(Color.FromArgb(255, red, green, blue), x, y, w, h);
        }

        private Button createColorButton(Color color, int x, int y, int w, int h)
        {
            Button b = new Button();
            b.BackColor = color;
            b.FlatAppearance.BorderSize = 0;
            b.FlatStyle = FlatStyle.Flat;
            b.Location = new Point(x, y);
            b.Size = new Size(w, h);
            b.TabStop = false;
            b.Click += colorButtonClick;
            toolTip.SetToolTip(b, ColorTranslator.ToHtml(color) + " | R:" + color.R + ", G:" + color.G + ", B:" + color.B);
            return b;
        }

        private void createLastUsedColorButtonRow(int x, int y, int w, int h)
        {
            for (int i = 0; i < 12; i++)
            {
                Button b = createColorButton(Color.Transparent, x, y, w, h);
                b.Enabled = false;
                recentColorButtons.Add(b);
                x += w;
            }
            Controls.AddRange(recentColorButtons.ToArray());
        }

        #endregion user interface generation

        #region update user interface

        private void updateRecentColorsButtonRow()
        {
            for (int i = 0; i < editorConfiguration.RecentColors.Count && i < 12; i++)
            {
                recentColorButtons[i].BackColor = editorConfiguration.RecentColors[i];
                recentColorButtons[i].Enabled = true;
            }
        }

        private void previewColor(Color c, Control trigger)
        {
            updateInProgress = true;
            colorPanel.BackColor = c;
            if (trigger != textBoxHtmlColor)
            {
                textBoxHtmlColor.Text = ColorTranslator.ToHtml(c);
            }
            else
            {
                if (!textBoxHtmlColor.Text.StartsWith("#"))
                {
                    int selStart = textBoxHtmlColor.SelectionStart;
                    int selLength = textBoxHtmlColor.SelectionLength;
                    textBoxHtmlColor.Text = "#" + textBoxHtmlColor.Text;
                    textBoxHtmlColor.Select(selStart + 1, selLength + 1);
                }
            }
            if (trigger != textBoxRed && trigger != textBoxGreen && trigger != textBoxBlue && trigger != textBoxAlpha)
            {
                textBoxRed.Text = c.R.ToString();
                textBoxGreen.Text = c.G.ToString();
                textBoxBlue.Text = c.B.ToString();
                textBoxAlpha.Text = c.A.ToString();
            }
            updateInProgress = false;
        }

        private void addToRecentColors(Color c)
        {
            editorConfiguration.RecentColors.Remove(c);
            editorConfiguration.RecentColors.Insert(0, c);
            if (editorConfiguration.RecentColors.Count > 12) editorConfiguration.RecentColors.RemoveRange(12, editorConfiguration.RecentColors.Count - 12);
            updateRecentColorsButtonRow();
        }

        #endregion update user interface

        #region textbox event handlers

        private void TextBoxHexadecimalTextChanged(object sender, EventArgs e)
        {
            if (updateInProgress) return;
            TextBox tb = (TextBox)sender;
            string t = tb.Text.Replace("#", "");
            int i = 0;
            Int32.TryParse(t, NumberStyles.AllowHexSpecifier, Thread.CurrentThread.CurrentCulture, out i);
            Color c = Color.FromArgb(i);
            Color opaqueColor = Color.FromArgb(255, c.R, c.G, c.B);
            previewColor(opaqueColor, tb);
        }

        private void TextBoxRGBTextChanged(object sender, EventArgs e)
        {
            if (updateInProgress) return;
            TextBox tb = (TextBox)sender;
            previewColor(Color.FromArgb(getColorPartIntFromString(textBoxAlpha.Text), getColorPartIntFromString(textBoxRed.Text), getColorPartIntFromString(textBoxGreen.Text), getColorPartIntFromString(textBoxBlue.Text)), tb);
        }

        private void TextBoxGotFocus(object sender, EventArgs e)
        {
            textBoxHtmlColor.SelectAll();
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter)
            {
                addToRecentColors(colorPanel.BackColor);
            }
        }

        #endregion textbox event handlers

        #region button event handlers

        private void colorButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            previewColor(b.BackColor, b);
        }

        private void btnTransparentClick(object sender, EventArgs e)
        {
            colorButtonClick(sender, e);
        }

        private void BtnApplyClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
            addToRecentColors(colorPanel.BackColor);
        }

        #endregion button event handlers

        #region helper functions

        private int getColorPartIntFromString(string s)
        {
            int ret = 0;
            Int32.TryParse(s, out ret);
            if (ret < 0) ret = 0;
            else if (ret > 255) ret = 255;
            return ret;
        }

        #endregion helper functions

        private void pipetteUsed(object sender, PipetteUsedArgs e)
        {
            Color = e.color;
        }
    }
}