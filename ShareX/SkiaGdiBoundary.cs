#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.HelpersLib;
using SkiaSharp;
using System.Drawing;

namespace ShareX
{
    /// <summary>
    /// Captures arrive as SkiaSharp bitmaps from ScreenCaptureLib. The Windows task pipeline still works on GDI+ bitmaps (see the
    /// migration debt in AGENTS.md), so it converts here, at the boundary.
    /// </summary>
    internal static class SkiaGdiBoundary
    {
        /// <summary>A GDI+ copy of <paramref name="bitmap"/>, which is disposed. Null stays null.</summary>
        public static Bitmap ToGdiBitmapAndDispose(this SKBitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }

            using (bitmap)
            {
                return GdiSkiaBitmapConverter.ToGdiBitmap(bitmap);
            }
        }
    }
}
