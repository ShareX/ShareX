#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

namespace HelpersLib
{
    // http://en.wikipedia.org/wiki/List_of_file_formats

    public enum ImageFileExtensions
    {
        [Description("Joint Photographic Experts Group")]
        jpg,
        jpeg,
        [Description("Portable Network Graphic")]
        png,
        [Description("CompuServe's Graphics Interchange Format")]
        gif,
        [Description("Microsoft Windows Bitmap formatted image")]
        bmp,
        [Description("File format used for icons in Microsoft Windows")]
        ico,
        [Description("Tagged Image File Format")]
        tif,
        tiff
    }

    public enum TextFileExtensions
    {
        [Description("ASCII or Unicode plaintext")]
        txt,
        log,
        [Description("ASCII or extended ASCII text file")]
        nfo,
        [Description("C source")]
        c,
        [Description("C++ source")]
        cpp,
        cc,
        cxx,
        [Description("C/C++ header file")]
        h,
        [Description("C++ header file")]
        hpp,
        hxx,
        [Description("C# source")]
        cs,
        [Description("Visual Basic.NET source")]
        vb,
        [Description("HyperText Markup Language")]
        html,
        htm,
        [Description("eXtensible HyperText Markup Language")]
        xhtml,
        xht,
        [Description("eXtensible Markup Language")]
        xml,
        [Description("Cascading Style Sheets")]
        css,
        [Description("JavaScript and JScript")]
        js,
        [Description("Hypertext Preprocessor")]
        php,
        [Description("Batch file")]
        bat,
        [Description("Java source")]
        java,
        [Description("Lua")]
        lua,
        [Description("Python source")]
        py,
        [Description("Perl")]
        pl,
        [Description("Visual Studio solution")]
        sln
    }

    public enum VideoFileExtensions
    {
        [Description("MPEG-4 Video File")]
        mp4,
        m4v
    }

    public enum EncryptionStrength
    {
        Low = 128,
        Medium = 192,
        High = 256
    }

    public enum EDataType
    {
        Default,
        File,
        Image,
        Text,
        URL
    }

    public enum EInputType
    {
        None,
        Clipboard,
        FileSystem,
        Screenshot
    }

    public enum GIFQuality
    {
        Default,
        Bit8,
        Bit4,
        Grayscale
    }

    public enum EImageFormat
    {
        PNG,
        JPEG,
        GIF,
        BMP,
        TIFF
    }

    public enum AnimatedImageFormat
    {
        PNG,
        GIF
    }

    public enum TaskStatus
    {
        InQueue,
        Preparing,
        Working,
        Stopping,
        Completed
    }

    public enum TaskProgress
    {
        ReportStarted,
        ReportProgress
    }

    public enum WindowButtonAction
    {
        [Description("Minimize to Tray")]
        MinimizeToTray,
        [Description("Minimize to Taskbar")]
        MinimizeToTaskbar,
        [Description("Exit Application")]
        ExitApplication,
        [Description("Do Nothing")]
        Nothing
    }

    public enum TriangleAngle
    {
        Top,
        Right,
        Bottom,
        Left
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
        SHA512,
        [Description("RIPEMD-160")]
        RIPEMD160
    }

    public enum TokenType
    {
        Unknown,
        Whitespace,
        Symbol,
        Literal,
        Identifier,
        Numeric,
        Keyword
    }

    public enum BorderType
    {
        Outside,
        Inside
    }

    public enum ScreenRecordOutput
    {
        [Description("Animated GIF")]
        GIF,
        [Description("AVI")]
        AVI,
        [Description("AVI CLI encoder")]
        AVICommandLine
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
        VerySilent
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
        UpdateRequired,
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

    public enum PositionType
    {
        [Description("Top - Left")]
        Top_Left,
        [Description("Top - Center")]
        Top,
        [Description("Top - Right")]
        Top_Right,
        [Description("Center - Left")]
        Left,
        [Description("Center")]
        Center,
        [Description("Center - Right")]
        Right,
        [Description("Bottom - Left")]
        Bottom_Left,
        [Description("Bottom - Center")]
        Bottom,
        [Description("Bottom - Right")]
        Bottom_Right
    }
}