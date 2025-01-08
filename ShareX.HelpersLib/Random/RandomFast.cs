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

using System;
using System.Collections.Generic;

namespace ShareX.HelpersLib
{
    public static class RandomFast
    {
        private static readonly object randomLock = new object();
        private static readonly Random random = new Random();

        /// <summary>Returns a non-negative random integer.</summary>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <c>System.Int32.MaxValue.</c></returns>
        public static int Next()
        {
            lock (randomLock)
            {
                return random.Next();
            }
        }

        /// <summary>Returns a non-negative random integer that is less than or equal to <paramref name="maxValue"/>.</summary>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than or equal to <paramref name="maxValue"/>.</returns>
        public static int Next(int maxValue)
        {
            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            lock (randomLock)
            {
                return random.Next(maxValue + 1);
            }
        }

        /// <summary>Returns a random integer that is within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The inclusive upper bound of the random number returned.</param>
        /// <returns>A 32-bit signed integer that is greater than or equal to <paramref name="minValue"/> and less than or equal to <paramref name="maxValue"/>.</returns>
        public static int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue));
            }

            if (minValue == maxValue)
            {
                return minValue;
            }

            lock (randomLock)
            {
                return random.Next(minValue, maxValue + 1);
            }
        }

        /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
        /// <returns>A double-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        public static double NextDouble()
        {
            lock (randomLock)
            {
                return random.NextDouble();
            }
        }

        /// <summary>Fills the elements of a specified array of bytes with random numbers.</summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public static void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            lock (randomLock)
            {
                random.NextBytes(buffer);
            }
        }

        public static T Pick<T>(params T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Length == 0)
            {
                throw new ArgumentException(nameof(array));
            }

            return array[Next(array.Length - 1)];
        }

        public static T Pick<T>(List<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (list.Count == 0)
            {
                throw new ArgumentException(nameof(list));
            }

            return list[Next(list.Count - 1)];
        }

        public static void Run(params Action[] actions)
        {
            Pick(actions)();
        }
    }
}