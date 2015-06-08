#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using System.ComponentModel;

namespace ShareX.ScreenCaptureLib
{
    public enum ScreenRecordOutput
    {
        [Description("FFmpeg")]
        FFmpeg,
        [Description("Animated GIF")]
        GIF
    }

    public enum ScreenRecordGIFEncoding // Localized
    {
        [Description("FFmpeg")]
        FFmpeg,
        [Description(".NET")]
        NET,
        [Description("Octree quantizer")]
        OctreeQuantizer
    }

    public enum SurfaceResult
    {
        None,
        Close,
        Region,
        Fullscreen,
        Monitor,
        ActiveMonitor
    }

    internal enum NodePosition
    {
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

    internal enum NodeShape
    {
        Square, Circle, Diamond
    }

    public enum FFmpegVideoCodec
    {
        [Description("x264")]
        libx264,
        [Description("VP8 (WebM)")]
        libvpx,
        [Description("Animated GIF")]
        gif,
        [Description("Xvid")]
        libxvid,
        [Description("x265")]
        libx265
    }

    public enum FFmpegPreset
    {
        [Description("Ultra fast")]
        ultrafast,
        [Description("Super fast")]
        superfast,
        [Description("Very fast")]
        veryfast,
        [Description("Faster")]
        faster,
        [Description("Fast")]
        fast,
        [Description("Medium")]
        medium,
        [Description("Slow")]
        slow,
        [Description("Slower")]
        slower,
        [Description("Very slow")]
        veryslow
    }

    public enum FFmpegAudioCodec
    {
        [Description("AAC")]
        libvoaacenc,
        [Description("Vorbis")]
        libvorbis,
        [Description("MP3")]
        libmp3lame
    }

    public enum FFmpegPaletteGenStatsMode
    {
        full,
        diff
    }

    public enum FFmpegPaletteUseDither
    {
        none,
        bayer,
        heckbert,
        floyd_steinberg,
        sierra2,
        sierra2_4a
    }
}