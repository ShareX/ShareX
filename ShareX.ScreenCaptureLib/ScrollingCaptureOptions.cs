#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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

using ShareX.ScreenCaptureLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX
{
    public class ScrollingCaptureOptions
    {
        public ScrollingCaptureScrollMethod ScrollMethod { get; set; } = ScrollingCaptureScrollMethod.SendMessageScroll;
        public int ScrollDelay { get; set; } = 250;
        public int MaximumScrollCount { get; set; } = 20;
        public bool AutoDetectScrollEnd { get; set; } = true;
        public bool RemoveDuplicates { get; set; } = true;
        public int TrimLeftEdge { get; set; } = 0;
        public int TrimTopEdge { get; set; } = 0;
        public int TrimRightEdge { get; set; } = 0;
        public int TrimBottomEdge { get; set; } = 0;
        public int CombineAdjustmentVertical { get; set; } = 0;
        public int CombineAdjustmentLastVertical { get; set; } = 0;
    }
}