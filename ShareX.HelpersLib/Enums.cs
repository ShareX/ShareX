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

using System.ComponentModel;

namespace ShareX.HelpersLib
{
    public enum EDataType // Localized
    {
        Default,
        File,
        Image,
        Text,
        URL
    }

    public enum PNGBitDepth // Localized
    {
        Default,
        Automatic,
        Bit32,
        Bit24
    }

    public enum GIFQuality // Localized
    {
        Default,
        Bit8,
        Bit4,
        Grayscale
    }

    public enum EImageFormat
    {
        [Description("png")]
        PNG,
        [Description("jpg")]
        JPEG,
        [Description("gif")]
        GIF,
        [Description("bmp")]
        BMP,
        [Description("tif")]
        TIFF
    }

    public enum HashType
    {
        [Description("CRC-32")]
        CRC32,
        [Description("MD5")]
        MD5,
        [Description("SHA-1")]
        SHA1,
        [Description("SHA-256")]
        SHA256,
        [Description("SHA-384")]
        SHA384,
        [Description("SHA-512")]
        SHA512
    }

    public enum BorderType
    {
        Outside,
        Inside
    }

    public enum DownloaderFormStatus
    {
        Waiting,
        DownloadStarted,
        DownloadCompleted,
        InstallStarted
    }

    public enum InstallType
    {
        Default,
        Silent,
        VerySilent,
        Event
    }

    public enum ReleaseChannelType
    {
        [Description("Stable version")]
        Stable,
        [Description("Beta version")]
        Beta,
        [Description("Dev version")]
        Dev
    }

    public enum UpdateStatus
    {
        None,
        UpdateCheckFailed,
        UpdateAvailable,
        UpToDate
    }

    public enum PrintType
    {
        Image,
        Text
    }

    public enum DrawStyle
    {
        Hue,
        Saturation,
        Brightness,
        Red,
        Green,
        Blue
    }

    public enum ColorType
    {
        None, RGBA, HSB, CMYK, Hex, Decimal
    }

    public enum ColorFormat
    {
        RGB, RGBA, ARGB
    }

    public enum ProxyMethod // Localized
    {
        None,
        Manual,
        Automatic
    }

    public enum SlashType
    {
        Prefix,
        Suffix
    }

    public enum ScreenTearingTestMode
    {
        VerticalLines,
        HorizontalLines
    }

    public enum HotkeyStatus
    {
        Registered,
        Failed,
        NotConfigured
    }

    public enum ImageCombinerAlignment
    {
        LeftOrTop,
        Center,
        RightOrBottom
    }

    public enum ImageInterpolationMode
    {
        HighQualityBicubic,
        Bicubic,
        HighQualityBilinear,
        Bilinear,
        NearestNeighbor
    }

    public enum ArrowHeadDirection // Localized
    {
        End,
        Start,
        Both
    }

    public enum FFmpegArchitecture
    {
        win64,
        win32,
        macos64
    }

    public enum StepType // Localized
    {
        Numbers,
        LettersUppercase,
        LettersLowercase,
        RomanNumeralsUppercase,
        RomanNumeralsLowercase
    }

    public enum CutOutEffectType // Localized
    {
        None,
        ZigZag,
        TornEdge,
        Wave
    }
}