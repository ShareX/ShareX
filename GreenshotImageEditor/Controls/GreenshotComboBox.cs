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

using GreenshotPlugin.Core;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GreenshotPlugin.Controls
{
    public class GreenshotComboBox : ComboBox, IGreenshotConfigBindable
    {
        private Type enumType = null;
        private Enum selectedEnum = null;
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

        public GreenshotComboBox()
        {
            SelectedIndexChanged += delegate
            {
                StoreSelectedEnum();
            };
        }

        public void SetValue(Enum currentValue)
        {
            if (currentValue != null)
            {
                selectedEnum = currentValue;
                SelectedItem = Language.Translate(currentValue);
            }
        }

        /// <summary>
        /// This is a method to popululate the ComboBox
        /// with the items from the enumeration
        /// </summary>
        /// <param name="enumType">TEnum to populate with</param>
        public void Populate(Type enumType)
        {
            // Store the enum-type, so we can work with it
            this.enumType = enumType;

            var availableValues = Enum.GetValues(enumType);
            Items.Clear();
            string enumTypeName = enumType.Name;
            foreach (var enumValue in availableValues)
            {
                Items.Add(Language.Translate((Enum)enumValue));
            }
        }

        /// <summary>
        /// Store the selected value internally
        /// </summary>
        private void StoreSelectedEnum()
        {
            string enumTypeName = enumType.Name;
            string selectedValue = SelectedItem as string;
            var availableValues = Enum.GetValues(enumType);
            object returnValue = null;

            try
            {
                returnValue = Enum.Parse(enumType, selectedValue);
            }
            catch (Exception)
            {
            }

            foreach (Enum enumValue in availableValues)
            {
                string enumKey = enumTypeName + "." + enumValue.ToString();
                if (Language.hasKey(enumKey))
                {
                    string translation = Language.GetString(enumTypeName + "." + enumValue.ToString());
                    if (translation.Equals(selectedValue))
                    {
                        returnValue = enumValue;
                    }
                }
            }
            selectedEnum = (Enum)returnValue;
        }

        /// <summary>
        /// Get the selected enum value from the combobox, uses generics
        /// </summary>
        /// <returns>The enum value of the combobox</returns>
        public Enum GetSelectedEnum()
        {
            return selectedEnum;
        }
    }
}