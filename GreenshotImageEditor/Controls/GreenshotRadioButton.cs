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

using System.ComponentModel;
using System.Windows.Forms;

namespace GreenshotPlugin.Controls
{
    /// <summary>
    /// Description of GreenshotCheckbox.
    /// </summary>
    public class GreenshotRadioButton : RadioButton, IGreenshotLanguageBindable, IGreenshotConfigBindable
    {
        [Category("Greenshot"), DefaultValue(null), Description("Specifies key of the language file to use when displaying the text.")]
        public string LanguageKey
        {
            get;
            set;
        }

        private string sectionName = "Core";
        [Category("Greenshot"), DefaultValue("Core"), Description("Specifies the Ini-Section to map this control with.")]
        public string SectionName
        {
            get
            {
                return sectionName;
            }
            set
            {
                sectionName = value;
            }
        }

        [Category("Greenshot"), DefaultValue(null), Description("Specifies the property name to map the configuration.")]
        public string PropertyName
        {
            get;
            set;
        }
    }
}