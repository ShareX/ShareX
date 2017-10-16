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

namespace Greenshot.IniFile
{
    /// <summary>
    /// Attribute for telling that this class is linked to a section in the ini-configuration
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IniSectionAttribute : Attribute
    {
        private string name;

        public IniSectionAttribute(string name)
        {
            this.name = name;
        }

        public string Description;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    /// <summary>
    /// Attribute for telling that a field is linked to a property in the ini-configuration selection
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class IniPropertyAttribute : Attribute
    {
        private string name;

        public IniPropertyAttribute(string name)
        {
            this.name = name;
        }

        public string Description;
        public string Separator = ",";
        public string DefaultValue;
        public string LanguageKey;
        // If Encrypted is set to true, the value will be decrypted on load and encrypted on save
        public bool Encrypted = false;
        public bool FixedValue = false;
        public bool ExcludeIfNull = false;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}