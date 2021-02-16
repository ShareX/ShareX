#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2020 ShareX Team

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
        AnnotateCancelTask
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
        [Description("H.264 / x264")]
        libx264,
        [Description("H.265 / x265")]
        libx265,
        [Description("VP8 (WebM)")]
        libvpx,
        [Description("VP9 (WebM)")]
        libvpx_vp9,
        [Description("MPEG-4 / Xvid")]
        libxvid,
        [Description("H.264 / NVENC")]
        h264_nvenc,
        [Description("HEVC / NVENC")]
        hevc_nvenc,
        [Description("H.264 / AMF")]
        h264_amf,
        [Description("HEVC / AMF")]
        hevc_amf,
        [Description("H.264 / Quick Sync")]
        h264_qsv,
        [Description("HEVC / Quick Sync")]
        hevc_qsv,
        [Description("GIF")]
        gif,
        [Description("WebP")]
        libwebp,
        [Description("APNG")]
        apng
    }

    public enum FFmpegAudioCodec
    {
        [Description("AAC")]
        libvoaacenc,
        [Description("Opus")]
        libopus,
        [Description("Vorbis")]
        libvorbis,
        [Description("MP3")]
        libmp3lame
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

    public enum FFmpegAMFUsage
    {
        [Description("Generic Transcoding")]
        transcoding = 0,
        [Description("Ultra Low Latency")]
        ultralowlatency = 1,
        [Description("Low Latency")]
        lowlatency = 2,
        [Description("Webcam")]
        webcam = 3
    }

    public enum FFmpegAMFQuality
    {
        [Description("Prefer Speed")]
        speed = 0,
        [Description("Balanced")]
        balanced = 1,
        [Description("Prefer Quality")]
        quality = 2
    }

    public enum FFmpegQSVPreset
    {
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
        Effect,
        Tool
    }

    public enum ShapeType // Localized
    {
        RegionRectangle,
        RegionEllipse,
        RegionFreehand,
        ToolSelect,
        DrawingRectangle,
        DrawingEllipse,
        DrawingFreehand,
        DrawingLine,
        DrawingArrow,
        DrawingTextOutline,
        DrawingTextBackground,
        DrawingSpeechBalloon,
        DrawingStep,
        DrawingMagnify,
        DrawingImage,
        DrawingImageScreen,
        DrawingSticker,
        DrawingCursor,
        DrawingSmartEraser,
        EffectBlur,
        EffectPixelate,
        EffectHighlight,
        ToolCrop
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

    public enum ImageEditorStartMode // Localized
    {
        AutoSize,
        Normal,
        Maximized,
        PreviousState,
        Fullscreen
    }

    public enum ImageInsertMethod
    {
        Center,
        CanvasExpandDown,
        CanvasExpandRight
    }

    public enum BorderStyle
    {
        Solid,
        Dash,
        Dot,
        DashDot,
        DashDotDot
    }
}