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

using GreenshotPlugin.Controls;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Greenshot.Controls
{
    /// <summary>
    /// Description of BindableToolStripButton.
    /// </summary>
    internal class BindableToolStripButton : ToolStripButton, INotifyPropertyChanged, IGreenshotLanguageBindable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Category("Greenshot"), DefaultValue(null), Description("Specifies key of the language file to use when displaying the text.")]
        public string LanguageKey
        {
            get;
            set;
        }

        public BindableToolStripButton()
        {
            CheckedChanged += BindableToolStripButton_CheckedChanged;
        }

        private void BindableToolStripButton_CheckedChanged(object sender, EventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Checked"));
        }
    }
}