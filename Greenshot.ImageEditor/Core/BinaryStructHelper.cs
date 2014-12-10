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
using System.Runtime.InteropServices;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// A helper class which does the mashalling for structs
    /// </summary>
    public static class BinaryStructHelper
    {
        /// <summary>
        /// Get a struct from a byte array
        /// </summary>
        /// <typeparam name="T">typeof struct</typeparam>
        /// <param name="bytes">byte[]</param>
        /// <returns>struct</returns>
        public static T FromByteArray<T>(byte[] bytes) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, 0, ptr, size);
                return FromIntPtr<T>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        /// <summary>
        /// Get a struct from a byte array
        /// </summary>
        /// <typeparam name="T">typeof struct</typeparam>
        /// <param name="bytes">byte[]</param>
        /// <returns>struct</returns>
        public static T FromIntPtr<T>(IntPtr intPtr) where T : struct
        {
            object obj = Marshal.PtrToStructure(intPtr, typeof(T));
            return (T)obj;
        }

        /// <summary>
        /// copy a struct to a byte array
        /// </summary>
        /// <typeparam name="T">typeof struct</typeparam>
        /// <param name="obj">struct</param>
        /// <returns>byte[]</returns>
        public static byte[] ToByteArray<T>(T obj) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, true);
                return FromPtrToByteArray<T>(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }

        /// <summary>
        /// copy a struct from a pointer to a byte array
        /// </summary>
        /// <typeparam name="T">typeof struct</typeparam>
        /// <param name="ptr">IntPtr to struct</param>
        /// <returns>byte[]</returns>
        public static byte[] FromPtrToByteArray<T>(IntPtr ptr) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            return bytes;
        }
    }
}