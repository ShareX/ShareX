/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Greenshot.Controls
{
    /// <summary>
    /// ToolStripComboBox containing installed font families,
    /// implementing INotifyPropertyChanged for data binding
    /// </summary>
    internal class FontFamilyComboBox : ToolStripComboBox, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)SelectedItem;
            }
            set
            {
                if (!SelectedItem.Equals(value))
                {
                    SelectedItem = value;
                }
            }
        }

        public FontFamilyComboBox() : base()
        {
            if (ComboBox != null)
            {
                ComboBox.DataSource = FontFamily.Families;
                ComboBox.DisplayMember = "Name";
                SelectedIndexChanged += BindableToolStripComboBox_SelectedIndexChanged;
                ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
                ComboBox.DrawItem += ComboBox_DrawItem;
            }
        }

        private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // DrawBackground handles drawing the background (i.e,. hot-tracked v. not)
            // It uses the system colors (Bluish, and and white, by default)
            // same as calling e.Graphics.FillRectangle ( SystemBrushes.Highlight, e.Bounds );
            e.DrawBackground();

            if (e.Index > -1)
            {
                FontFamily fontFamily = Items[e.Index] as FontFamily;
                FontStyle fontStyle = FontStyle.Regular;
                if (fontFamily != null && !fontFamily.IsStyleAvailable(FontStyle.Regular))
                {
                    if (fontFamily.IsStyleAvailable(FontStyle.Bold))
                    {
                        fontStyle = FontStyle.Bold;
                    }
                    else if (fontFamily.IsStyleAvailable(FontStyle.Italic))
                    {
                        fontStyle = FontStyle.Italic;
                    }
                    else if (fontFamily.IsStyleAvailable(FontStyle.Strikeout))
                    {
                        fontStyle = FontStyle.Strikeout;
                    }
                    else if (fontFamily.IsStyleAvailable(FontStyle.Underline))
                    {
                        fontStyle = FontStyle.Underline;
                    }
                }
                try
                {
                    if (fontFamily != null)
                    {
                        DrawText(e.Graphics, fontFamily, fontStyle, e.Bounds, fontFamily.Name);
                    }
                }
                catch
                {
                    if (fontFamily != null)
                    {
                        // If the drawing failed, BUG-1770 seems to have a weird case that causes: Font 'Lucida Sans Typewriter' does not support style 'Regular'
                        DrawText(e.Graphics, FontFamily.GenericSansSerif, FontStyle.Regular, e.Bounds, fontFamily.Name);
                    }
                }
            }
            // Uncomment this if you actually like the way the focus rectangle looks
            //e.DrawFocusRectangle ();
        }

        /// <summary>
        /// Helper method to draw the string
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="fontFamily"></param>
        /// <param name="fontStyle"></param>
        /// <param name="bounds"></param>
        /// <param name="text"></param>
        private void DrawText(Graphics graphics, FontFamily fontFamily, FontStyle fontStyle, Rectangle bounds, string text)
        {
            using (Font font = new Font(fontFamily, Font.Size + 5, fontStyle, GraphicsUnit.Pixel))
            {
                // Make sure the text is visible by centering it in the line
                using (StringFormat stringFormat = new StringFormat())
                {
                    stringFormat.LineAlignment = StringAlignment.Center;
                    graphics.DrawString(text, font, Brushes.Black, bounds, stringFormat);
                }
            }
        }

        private void BindableToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                PropertyChanged(this, new PropertyChangedEventArgs("FontFamily"));
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedIndex"));
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }
    }
}