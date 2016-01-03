#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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
using System.ComponentModel;

namespace ShareX
{
    public class ScrollingCaptureOptions
    {
        [DefaultValue(ScrollingCaptureScrollMethod.Automatic)]
        public ScrollingCaptureScrollMethod ScrollMethod { get; set; } = ScrollingCaptureScrollMethod.Automatic;

        [DefaultValue(500)]
        public int StartDelay { get; set; } = 500;

        [DefaultValue(500)]
        public int ScrollDelay { get; set; } = 500;

        [DefaultValue(20)]
        public int MaximumScrollCount { get; set; } = 20;

        [DefaultValue(true)]
        public bool StartSelectionAutomatically { get; set; } = true;

        [DefaultValue(false)]
        public bool StartCaptureAutomatically { get; set; } = false;

        [DefaultValue(ScrollingCaptureScrollTopMethod.All)]
        public ScrollingCaptureScrollTopMethod ScrollTopMethodBeforeCapture { get; set; } = ScrollingCaptureScrollTopMethod.All;

        [DefaultValue(true)]
        public bool AutoDetectScrollEnd { get; set; } = true;

        [DefaultValue(true)]
        public bool RemoveDuplicates { get; set; } = true;

        [DefaultValue(true)]
        public bool AfterCaptureAutomaticallyCombine { get; set; } = true;

        [DefaultValue(false)]
        public bool AutoUpload { get; set; } = false;

        [DefaultValue(false), Description("Automatically close scrolling capture window after completing the task.")]
        public bool AutoClose { get; set; } = false;

        public int TrimLeftEdge = 0;
        public int TrimTopEdge = 0;
        public int TrimRightEdge = 0;
        public int TrimBottomEdge = 0;
        public int CombineAdjustmentVertical = 0;
        public int CombineAdjustmentLastVertical = 0;
        public int IgnoreLast = 0;
    }
}