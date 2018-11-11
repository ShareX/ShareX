#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using System.Security.Cryptography;

namespace ShareX.HelpersLib
{
    public static class MathHelpers
    {
        public const float RadianPI = 57.29578f; // 180.0 / Math.PI
        public const float DegreePI = 0.01745329f; // Math.PI / 180.0
        public const float TwoPI = 6.28319f; // Math.PI * 2

        private static readonly object randomLock = new object();
        private static readonly Random random = new Random();

        private static readonly object cryptoRandomLock = new object();
        private static readonly RNGCryptoServiceProvider cryptoRandom = new RNGCryptoServiceProvider();
        private static byte[] rngBuf = new byte[4];

        /// <summary>
        /// Returns a random number between 0 and <c>max</c> (inclusive).
        /// </summary>
        /// <remarks>
        /// This uses <c>System.Random()</c>, which does not provide safe random numbers. This function
        /// should not be used to generate things that should be unique, like random file names.
        /// </remarks>
        /// <param name="max">The upper limit of the number (inclusive).</param>
        /// <returns>A random number.</returns>
        public static int Random(int max)
        {
            lock (randomLock)
            {
                return random.Next(max + 1);
            }
        }

        public static int Random(int min, int max)
        {
            lock (randomLock)
            {
                return random.Next(min, max + 1);
            }
        }

        /// <summary>
        /// Returns a random number between 0 and <c>max</c> (inclusive) generated with a cryptographic PRNG.
        /// </summary>
        /// <param name="max">The upper limit of the number (inclusive).</param>
        /// <returns>A cryptographically random number.</returns>
        public static int CryptoRandom(int max)
        {
            return CryptoRandom(0, max);
        }

        /// <summary>
        /// Returns a random number between <c>min</c> and <c>max</c> (inclusive) generated with a cryptographic PRNG.
        /// </summary>
        /// <param name="min">The lower limit of the number.</param>
        /// <param name="max">The upper limit of the number (inclusive).</param>
        /// <returns>A cryptographically random number.</returns>
        public static int CryptoRandom(int min, int max)
        {
            // this code avoids bias in random number generation, which is important when generating random filenames, etc.
            // adapted from https://web.archive.org/web/20150114085328/http://msdn.microsoft.com:80/en-us/magazine/cc163367.aspx
            if (min > max)
            {
                throw new ArgumentOutOfRangeException("min");
            }

            if (min == max)
            {
                return min;
            }

            lock (cryptoRandomLock)
            {
                long diff = (long)max - min;
                long ceiling = 1 + (long)uint.MaxValue;
                long remainder = ceiling % diff;
                // this should only iterate once unless we generate really large numbers
                uint r;

                do
                {
                    cryptoRandom.GetBytes(rngBuf);
                    r = BitConverter.ToUInt32(rngBuf, 0);
                } while (r >= ceiling - remainder);

                return (int)(min + (r % diff));
            }
        }

        public static float RadianToDegree(float radian)
        {
            return radian * RadianPI;
        }

        public static float DegreeToRadian(float degree)
        {
            return degree * DegreePI;
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2((float)Math.Cos(radian), (float)Math.Sin(radian));
        }

        public static Vector2 RadianToVector2(float radian, float length)
        {
            return RadianToVector2(radian) * length;
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(DegreeToRadian(degree));
        }

        public static Vector2 DegreeToVector2(float degree, float length)
        {
            return RadianToVector2(DegreeToRadian(degree), length);
        }

        public static float Vector2ToRadian(Vector2 direction)
        {
            return (float)Math.Atan2(direction.Y, direction.X);
        }

        public static float Vector2ToDegree(Vector2 direction)
        {
            return RadianToDegree(Vector2ToRadian(direction));
        }

        public static float LookAtRadian(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
        }

        public static Vector2 LookAtVector2(Vector2 pos1, Vector2 pos2)
        {
            return RadianToVector2(LookAtRadian(pos1, pos2));
        }

        public static float LookAtDegree(Vector2 pos1, Vector2 pos2)
        {
            return RadianToDegree(LookAtRadian(pos1, pos2));
        }

        public static float Distance(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) + Math.Pow(pos2.Y - pos1.Y, 2));
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + ((value2 - value1) * amount);
        }

        public static Vector2 Lerp(Vector2 pos1, Vector2 pos2, float amount)
        {
            float x = Lerp(pos1.X, pos2.X, amount);
            float y = Lerp(pos1.Y, pos2.Y, amount);
            return new Vector2(x, y);
        }
    }
}