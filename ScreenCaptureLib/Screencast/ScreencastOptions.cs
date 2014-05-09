#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ScreenCaptureLib
{
    public class ScreencastOptions
    {
        public string OutputPath;
        public int FPS;
        public Size Size;
        public float Duration;

        public IntPtr ParentWindow;
        public bool ShowAVIOptionsDialog;
        public AVIOptions AVI = new AVIOptions();

        public FFmpegOptions FFmpegCLI = new FFmpegOptions();
        public FFmpegNetOptions FFmpegNet = new FFmpegNetOptions();
    }

    public class AVIOptions
    {
        public AVICOMPRESSOPTIONS CompressOptions;
    }

    public class FFmpegOptions
    {
        public string CLIPath { get; set; }
        public FFmpegVideoCodec VideoCodec { get; set; }
        public string Extension { get; set; }

        // H264
        public FFmpegPreset Preset { get; set; }
        public int CRF { get; set; }

        // H263
        public int qscale { get; set; }

        public FFmpegOptions()
        {
            VideoCodec = FFmpegVideoCodec.libx264;
            Preset = FFmpegPreset.medium;
            CRF = 23;
            Extension = "mp4";
        }
    }

    public class FFmpegNetOptions
    {
        public FFmpegNetVideoCodec VideoCodec { get; set; }
        public int BitRate { get; set; }

        public FFmpegNetOptions()
        {
            VideoCodec = FFmpegNetVideoCodec.MPEG4;
        }
    }
}