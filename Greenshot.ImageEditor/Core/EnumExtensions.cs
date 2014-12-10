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

namespace GreenshotPlugin.Core
{
    public static class EnumerationExtensions
    {
        public static bool Has<T>(this Enum type, T value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            try
            {
                if (underlyingType == typeof(int))
                {
                    return (((int)(object)type & (int)(object)value) == (int)(object)value);
                }
                else if (underlyingType == typeof(uint))
                {
                    return (((uint)(object)type & (uint)(object)value) == (uint)(object)value);
                }
            }
            catch
            {
            }
            return false;
        }

        public static bool Is<T>(this Enum type, T value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            try
            {
                if (underlyingType == typeof(int))
                {
                    return (int)(object)type == (int)(object)value;
                }
                else if (underlyingType == typeof(uint))
                {
                    return (uint)(object)type == (uint)(object)value;
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Add a flag to an enum
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Add<T>(this Enum type, T value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            try
            {
                if (underlyingType == typeof(int))
                {
                    return (T)(object)(((int)(object)type | (int)(object)value));
                }
                else if (underlyingType == typeof(uint))
                {
                    return (T)(object)(((uint)(object)type | (uint)(object)value));
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not append value '{0}' to enumerated type '{1}'.", value, typeof(T).Name), ex);
            }
            throw new ArgumentException(string.Format("Could not append value '{0}' to enumerated type '{1}'.", value, typeof(T).Name));
        }

        /// <summary>
        /// Remove a flag from an enum type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Remove<T>(this Enum type, T value)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());
            try
            {
                if (underlyingType == typeof(int))
                {
                    return (T)(object)(((int)(object)type & ~(int)(object)value));
                }
                else if (underlyingType == typeof(uint))
                {
                    return (T)(object)(((uint)(object)type & ~(uint)(object)value));
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not remove value '{0}' from enumerated type '{1}'.", value, typeof(T).Name), ex);
            }
            throw new ArgumentException(string.Format("Could not remove value '{0}' from enumerated type '{1}'.", value, typeof(T).Name));
        }
    }
}