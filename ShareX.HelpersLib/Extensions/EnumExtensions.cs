#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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

using ShareX.HelpersLib.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace ShareX.HelpersLib
{
    public static class EnumExtensions
    {
        public const string HotkeyType_Category_Upload = "Upload";
        public const string HotkeyType_Category_ScreenCapture = "ScreenCapture";
        public const string HotkeyType_Category_ScreenRecord = "ScreenRecord";
        public const string HotkeyType_Category_Tools = "Tools";
        public const string HotkeyType_Category_Other = "Other";

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }

        public static string GetLocalizedDescription(this Enum value)
        {
            return value.GetLocalizedDescription(Resources.ResourceManager);
        }

        public static string GetLocalizedDescription(this Enum value, ResourceManager resourceManager)
        {
            string resourceName = value.GetType().Name + "_" + value;
            string description = resourceManager.GetString(resourceName);

            if (string.IsNullOrEmpty(description))
            {
                description = value.GetDescription();
            }

            return description;
        }

        public static string GetLocalizedCategory(this Enum value)
        {
            return value.GetLocalizedCategory(Resources.ResourceManager);
        }

        public static string GetLocalizedCategory(this Enum value, ResourceManager resourceManager)
        {
            string category = null;

            FieldInfo fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                CategoryAttribute[] attributes = (CategoryAttribute[])fi.GetCustomAttributes(typeof(CategoryAttribute), false);

                if (attributes.Length > 0)
                {
                    string resourceName = $"{value.GetType().Name}_Category_{attributes[0].Category}";
                    category = resourceManager.GetString(resourceName);
                }
            }

            return category;
        }

        public static int GetIndex(this Enum value)
        {
            Array values = Enum.GetValues(value.GetType());
            return Array.IndexOf(values, value);
        }

        public static IEnumerable<T> GetFlags<T>(this T value) where T : Enum
        {
            return Helpers.GetEnums<T>().Where(x => Convert.ToUInt64(x) != 0 && value.HasFlag(x));
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

        public static T Next<T>(this Enum value)
        {
            Array values = Enum.GetValues(value.GetType());
            int i = Array.IndexOf(values, value) + 1;
            return i == values.Length ? (T)values.GetValue(0) : (T)values.GetValue(i);
        }

        public static T Previous<T>(this Enum value)
        {
            Array values = Enum.GetValues(value.GetType());
            int i = Array.IndexOf(values, value) - 1;
            return i == -1 ? (T)values.GetValue(values.Length - 1) : (T)values.GetValue(i);
        }
    }
}