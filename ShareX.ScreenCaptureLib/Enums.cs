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

    public enum RegionResult
    {
        None,
        Close,
        Region,
        Fullscreen,
        Monitor,
        ActiveMonitor
    }

    public enum NodeType
    {
        None,
        Rectangle,
        Line,
        Point
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
        [Description("x264 (mp4)")]
        libx264,
        [Description("VP8 (webm)")]
        libvpx,
        [Description("Xvid (avi)")]
        libxvid,
        [Description("Animated GIF (gif)")]
        gif,
        [Description("x265 (mp4)")]
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

    public enum FFmpegTune
    {
        film, animation, grain, stillimage, psnr, ssim, fastdecode, zerolatency
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
        full, diff
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

    public enum RectangleRegionMode
    {
        Default,
        Annotation,
        ScreenColorPicker,
        Ruler,
        OneClick
    }

    public enum RegionCaptureAction
    {
        [Description("Do nothing")]
        None,
        [Description("Cancel capture")]
        CancelCapture,
        [Description("Remove shape or cancel capture")]
        RemoveShapeCancelCapture,
        [Description("Remove shape")]
        RemoveShape,
        [Description("Open options menu")]
        OpenOptionsMenu,
        [Description("Swap tool type")]
        SwapToolType,
        [Description("Capture fullscreen")]
        CaptureFullscreen,
        [Description("Capture active monitor")]
        CaptureActiveMonitor
    }

    public enum ShapeType
    {
        [Description("Region: Rectangle")]
        RegionRectangle,
        [Description("Region: Rounded rectangle")]
        RegionRoundedRectangle,
        [Description("Region: Ellipse")]
        RegionEllipse,
        [Description("Drawing: Rectangle")]
        DrawingRectangle,
        [Description("Drawing: Rounded rectangle")]
        DrawingRoundedRectangle,
        [Description("Drawing: Ellipse")]
        DrawingEllipse,
        [Description("Drawing: Line")]
        DrawingLine,
        [Description("Drawing: Arrow")]
        DrawingArrow,
        [Description("Drawing: Text")]
        DrawingText,
        [Description("Drawing: Step")]
        DrawingStep,
        [Description("Effect: Blur")]
        DrawingBlur,
        [Description("Effect: Pixelate")]
        DrawingPixelate,
        [Description("Effect: Highlight")]
        DrawingHighlight
    }

    public enum RegionAnnotateMode
    {
        Capture,
        Rectangle,
        Pen
    }

    public enum ScrollingCaptureScrollMethod
    {
        [Description("Automatically try all methods until one works")]
        Automatic,
        [Description("Send scroll message to window or control")]
        SendMessageScroll,
        [Description("Simulate pressing \"Page down\" key")]
        KeyPressPageDown,
        [Description("Simulate mouse wheel scrolling")]
        MouseWheel
    }

    public enum ScrollingCaptureScrollTopMethod
    {
        [Description("First simulate pressing \"Home\" key then send scroll top message")]
        All,
        [Description("Send scroll top message")]
        SendMessageTop,
        [Description("Simulate pressing \"Home\" key")]
        KeyPressHome,
        [Description("Disable scrolling to top")]
        None
    }
}