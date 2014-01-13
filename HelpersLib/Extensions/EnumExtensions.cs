#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace HelpersLib
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
            }

            return value.ToString();
        }

        public static int GetIndex(this Enum value)
        {
            Array values = Enum.GetValues(value.GetType());
            return Array.IndexOf(values, value);
        }

        public static IEnumerable<T> GetFlags<T>(this Enum value)
        {
            return Enum.GetValues(value.GetType()).OfType<T>().Where(x => Convert.ToUInt64(x) != 0 && value.HasFlag(x));
        }

        public static bool HasFlag<T>(this Enum value, params T[] flags)
        {
            ulong keysVal = Convert.ToUInt64(value);
            ulong flagVal = flags.Select(x => Convert.ToUInt64(x)).Aggregate((x, next) => x | next);
            return (keysVal & flagVal) == flagVal;
        }

        public static bool HasFlagAny<T>(this Enum value, params T[] flags)
        {
            return flags.Any(x => value.HasFlag(x));
        }

        public static T Add<T>(this Enum value, params T[] flags)
        {
            ulong keysVal = Convert.ToUInt64(value);
            ulong flagVal = flags.Select(x => Convert.ToUInt64(x)).Aggregate(keysVal, (x, next) => x | next);
            return (T)Enum.ToObject(typeof(T), flagVal);
        }

        public static T Remove<T>(this Enum value, params T[] flags)
        {
            ulong keysVal = Convert.ToUInt64(value);
            ulong flagVal = flags.Select(x => Convert.ToUInt64(x)).Aggregate((x, next) => x | next);
            return (T)Enum.ToObject(typeof(T), keysVal & ~flagVal);
        }

        public static T Swap<T>(this Enum value, params T[] flags)
        {
            ulong keysVal = Convert.ToUInt64(value);
            ulong flagVal = flags.Select(x => Convert.ToUInt64(x)).Aggregate((x, next) => x | next);
            return (T)Enum.ToObject(typeof(T), keysVal ^ flagVal);
        }
    }
}