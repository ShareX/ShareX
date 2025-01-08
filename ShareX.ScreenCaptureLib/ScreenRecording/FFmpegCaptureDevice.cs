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

namespace ShareX.ScreenCaptureLib
{
    public class FFmpegCaptureDevice
    {
        public string Value { get; set; }
        public string Title { get; set; }

        public FFmpegCaptureDevice(string value, string title)
        {
            Value = value;
            Title = title;
        }

        public static FFmpegCaptureDevice None { get; } = new FFmpegCaptureDevice("", "None");
        public static FFmpegCaptureDevice GDIGrab { get; } = new FFmpegCaptureDevice("gdigrab", "gdigrab (Graphics Device Interface)");
        public static FFmpegCaptureDevice DDAGrab { get; } = new FFmpegCaptureDevice("ddagrab", "ddagrab (Desktop Duplication API)");
        public static FFmpegCaptureDevice ScreenCaptureRecorder { get; } = new FFmpegCaptureDevice("screen-capture-recorder", "dshow (screen-capture-recorder)");
        public static FFmpegCaptureDevice VirtualAudioCapturer { get; } = new FFmpegCaptureDevice("virtual-audio-capturer", "dshow (virtual-audio-capturer)");

        public override string ToString()
        {
            return Title;
        }
    }
}