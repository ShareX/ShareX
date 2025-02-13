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

namespace ShareX.MediaLib
{
    public enum ThumbnailLocationType
    {
        [Description("Default folder")]
        DefaultFolder,
        [Description("Parent folder of the media file")]
        ParentFolder,
        [Description("Custom folder")]
        CustomFolder
    }

    public enum ConverterVideoCodecs
    {
        [Description("H.264 / x264")]
        x264,
        [Description("H.265 / x265")]
        x265,
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
        [Description("VP8")]
        vp8,
        [Description("VP9")]
        vp9,
        [Description("AV1")]
        av1,
        [Description("MPEG-4 / Xvid")]
        xvid,
        [Description("GIF")]
        gif,
        [Description("WebP")]
        webp,
        [Description("APNG")]
        apng
    }

    public enum ImageBeautifierBackgroundType
    {
        Gradient,
        Color,
        Image,
        Desktop,
        Transparent
    }
}