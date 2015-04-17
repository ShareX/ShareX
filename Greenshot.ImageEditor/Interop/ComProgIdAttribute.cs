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

namespace Greenshot.Interop
{
    /// <summary>
    /// An attribute to specifiy the ProgID of the COM class to create. (As suggested by Kristen Wegner)
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ComProgIdAttribute : Attribute
    {
        private string _value;

        /// <summary>
        /// Extracts the attribute from the specified type.
        /// </summary>
        /// <param name="interfaceType">
        /// The interface type.
        /// </param>
        /// <returns>
        /// The <see cref="ComProgIdAttribute"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="interfaceType"/> is <see langword="null"/>.
        /// </exception>
        public static ComProgIdAttribute GetAttribute(Type interfaceType)
        {
            if (null == interfaceType)
            {
                throw new ArgumentNullException("interfaceType");
            }

            Type attributeType = typeof(ComProgIdAttribute);
            object[] attributes = interfaceType.GetCustomAttributes(attributeType, false);

            if (null == attributes || 0 == attributes.Length)
            {
                Type[] interfaces = interfaceType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    interfaceType = interfaces[i];
                    attributes = interfaceType.GetCustomAttributes(attributeType, false);
                    if (null != attributes && 0 != attributes.Length)
                    {
                        break;
                    }
                }
            }

            if (null == attributes || 0 == attributes.Length)
            {
                return null;
            }
            return (ComProgIdAttribute)attributes[0];
        }

        /// <summary>Constructor</summary>
        /// <param name="value">The COM ProgID.</param>
        public ComProgIdAttribute(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the COM ProgID
        /// </summary>
        public string Value
        {
            get { return _value; }
        }
    }
}