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
    public class FontFamilyComboBox : ToolStripComboBox, INotifyPropertyChanged
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

        public FontFamilyComboBox()
            : base()
        {
            ComboBox.DataSource = FontFamily.Families;
            ComboBox.DisplayMember = "Name";
            SelectedIndexChanged += BindableToolStripComboBox_SelectedIndexChanged;
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