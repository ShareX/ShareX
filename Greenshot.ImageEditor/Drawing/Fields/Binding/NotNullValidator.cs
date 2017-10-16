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

namespace Greenshot.Drawing.Fields.Binding
{
    /// <summary>
    /// Validates a value not to be null.
    /// </summary>
    public class NotNullValidator : IBindingValidator
    {
        private static NotNullValidator uniqueInstance;

        private NotNullValidator()
        {
        }

        public bool validate(object o)
        {
            return o != null;
        }

        public static NotNullValidator GetInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new NotNullValidator();
            return uniqueInstance;
        }
    }
}