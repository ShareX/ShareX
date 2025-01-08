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
using System.Drawing;

namespace ShareX.HelpersLib
{
    public struct Vector2
    {
        private float x, y;

        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public static readonly Vector2 Empty = new Vector2();

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2 v)
            {
                return v.x == x && v.y == y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"X={x}, Y={y}";
        }

        public static bool operator ==(Vector2 u, Vector2 v)
        {
            return u.x == v.x && u.y == v.y;
        }

        public static bool operator !=(Vector2 u, Vector2 v)
        {
            return !(u == v);
        }

        public static Vector2 operator +(Vector2 u, Vector2 v)
        {
            return new Vector2(u.x + v.x, u.y + v.y);
        }

        public static Vector2 operator -(Vector2 u, Vector2 v)
        {
            return new Vector2(u.x - v.x, u.y - v.y);
        }

        public static Vector2 operator *(Vector2 u, float a)
        {
            return new Vector2(a * u.x, a * u.y);
        }

        public static Vector2 operator /(Vector2 u, float a)
        {
            return new Vector2(u.x / a, u.y / a);
        }

        public static Vector2 operator -(Vector2 u)
        {
            return new Vector2(-u.x, -u.y);
        }

        public static explicit operator Point(Vector2 u)
        {
            return new Point((int)Math.Round(u.x), (int)Math.Round(u.y));
        }

        public static explicit operator PointF(Vector2 u)
        {
            return new PointF(u.x, u.y);
        }

        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }
}