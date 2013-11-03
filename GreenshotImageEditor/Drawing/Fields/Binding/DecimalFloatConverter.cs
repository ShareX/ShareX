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

namespace Greenshot.Drawing.Fields.Binding
{
    /// <summary>
    /// Converts decimal to float and vice versa.
    /// </summary>
    public class DecimalFloatConverter : AbstractBindingConverter<float, decimal>
    {
        private static DecimalFloatConverter uniqueInstance;

        private DecimalFloatConverter()
        {
        }

        protected override decimal convert(float o)
        {
            return Convert.ToDecimal(o);
        }

        protected override float convert(decimal o)
        {
            return Convert.ToInt16(o);
        }

        public static DecimalFloatConverter GetInstance()
        {
            if (uniqueInstance == null) uniqueInstance = new DecimalFloatConverter();
            return uniqueInstance;
        }
    }
}