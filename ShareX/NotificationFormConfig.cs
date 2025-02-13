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

namespace ShareX
{
    public class NotificationFormConfig : IDisposable
    {
        public int Duration { get; set; }
        public int FadeDuration { get; set; }
        public ContentAlignment Placement { get; set; }
        public int Offset { get; set; } = 5;
        public Size Size { get; set; }
        public bool IsValid => (Duration > 0 || FadeDuration > 0) && Size.Width > 0 && Size.Height > 0;
        public Color BackgroundColor { get; set; } = Color.FromArgb(50, 50, 50);
        public Color BorderColor { get; set; } = Color.FromArgb(40, 40, 40);
        public int TextPadding { get; set; } = 10;
        public Font TextFont { get; set; } = new Font("Arial", 11);
        public Color TextColor { get; set; } = Color.FromArgb(210, 210, 210);
        public Font TitleFont { get; set; } = new Font("Arial", 11, FontStyle.Bold);
        public Color TitleColor { get; set; } = Color.FromArgb(240, 240, 240);

        public Bitmap Image { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string FilePath { get; set; }
        public string URL { get; set; }
        public ToastClickAction LeftClickAction { get; set; }
        public ToastClickAction RightClickAction { get; set; }
        public ToastClickAction MiddleClickAction { get; set; }

        public void Dispose()
        {
            TextFont?.Dispose();
            TitleFont?.Dispose();
            Image?.Dispose();
        }
    }
}