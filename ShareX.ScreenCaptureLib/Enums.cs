#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
        FFmpeg,
        NET,
        OctreeQuantizer
    }

    public enum RegionResult
    {
        Close,
        Region,
        LastRegion,
        Fullscreen,
        Monitor,
        ActiveMonitor,
        AnnotateRunAfterCaptureTasks,
        AnnotateContinueTask,
        AnnotateCancelTask,
        AnnotateSaveImage,
        AnnotateSaveImageAs,
        AnnotateCopyImage,
        AnnotateUploadImage,
        AnnotatePrintImage
    }

    public enum NodeType
    {
        None,
        Rectangle,
        Line,
        Point,
        Freehand
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
        Left,
        Extra
    }

    internal enum NodeShape
    {
        Square, Circle, Diamond, CustomNode
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
        libx265,
        [Description("H.264 NVENC (mp4)")]
        h264_nvenc,
        [Description("HEVC NVENC (mp4)")]
        hevc_nvenc
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

    public enum FFmpegNVENCPreset
    {
        [Description("Default")]
        @default,
        [Description("High quality 2 passes")]
        slow,
        [Description("High quality 1 pass")]
        medium,
        [Description("High performance 1 pass")]
        fast,
        [Description("High performance")]
        hp,
        [Description("High quality")]
        hq,
        [Description("Bluray disk")]
        bd,
        [Description("Low latency")]
        ll,
        [Description("Low latency high quality")]
        llhq,
        [Description("Low latency high performance")]
        llhp,
        [Description("Lossless")]
        lossless,
        [Description("Lossless high performance")]
        losslesshp
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

    public enum RegionCaptureMode
    {
        Default,
        Annotation,
        ScreenColorPicker,
        Ruler,
        OneClick,
        Editor,
        TaskEditor
    }

    public enum RegionCaptureAction // Localized
    {
        None,
        CancelCapture,
        RemoveShapeCancelCapture,
        RemoveShape,
        SwapToolType,
        CaptureFullscreen,
        CaptureActiveMonitor
    }

    public enum ShapeCategory
    {
        Region,
        Drawing,
        Effect
    }

    public enum ShapeType // Localized
    {
        RegionRectangle,
        RegionEllipse,
        RegionFreehand,
        DrawingRectangle,
        DrawingEllipse,
        DrawingFreehand,
        DrawingLine,
        DrawingArrow,
        DrawingTextOutline,
        DrawingTextBackground,
        DrawingSpeechBalloon,
        DrawingStep,
        DrawingImage,
        DrawingImageScreen,
        DrawingCursor,
        EffectBlur,
        EffectPixelate,
        EffectHighlight,
        DrawingCrop
    }

    public enum RegionAnnotateMode
    {
        Capture,
        Rectangle,
        Pen
    }

    public enum ScrollingCaptureScrollMethod // Localized
    {
        Automatic,
        SendMessageScroll,
        KeyPressPageDown,
        MouseWheel
    }

    public enum ScrollingCaptureScrollTopMethod // Localized
    {
        All,
        SendMessageTop,
        KeyPressHome,
        None
    }
}