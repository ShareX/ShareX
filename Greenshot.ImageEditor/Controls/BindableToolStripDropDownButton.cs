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
using System.ComponentModel;
using System.Windows.Forms;

namespace Greenshot.Controls
{
    /// <summary>
    /// A simple ToolStripDropDownButton implementing INotifyPropertyChanged for data binding.
    /// Also, when a DropDownItem is selected, the DropDownButton adopts its Tag and Image.
    /// The selected tag can be accessed via SelectedTag property.
    /// </summary>
    internal class BindableToolStripDropDownButton : ToolStripDropDownButton, INotifyPropertyChanged, IGreenshotLanguageBindable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [Category("Greenshot"), DefaultValue(null), Description("Specifies key of the language file to use when displaying the text.")]
        public string LanguageKey
        {
            get;
            set;
        }

        public BindableToolStripDropDownButton()
        {
        }

        public object SelectedTag
        {
            get { if (Tag == null && DropDownItems.Count > 0) Tag = DropDownItems[0].Tag; return Tag; }
            set { AdoptFromTag(value); }
        }

        protected override void OnDropDownItemClicked(ToolStripItemClickedEventArgs e)
        {
            ToolStripItem clickedItem = e.ClickedItem;
            if (Tag == null || !Tag.Equals(clickedItem.Tag))
            {
                Tag = clickedItem.Tag;
                Image = clickedItem.Image;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedTag"));
            }
            base.OnDropDownItemClicked(e);
        }

        private void AdoptFromTag(object tag)
        {
            if (Tag == null || !Tag.Equals(tag))
            {
                Tag = tag;
                foreach (ToolStripItem item in DropDownItems)
                {
                    if (item.Tag != null && item.Tag.Equals(tag))
                    {
                        Image = item.Image;
                        break;
                    }
                }
                Tag = tag;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedTag"));
            }
        }
    }
}