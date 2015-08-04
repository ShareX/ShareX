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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace ShareX.MediaLib
{
    public class VideoThumbnailOptions
    {
        [Category("Screenshots"), DefaultValue(ThumbnailLocationType.ParentFolder), Description("Create screenshots in the same folders as the media file, default torrent folder or in a custom folder.")]
        public ThumbnailLocationType OutputLocation { get; set; }

        [Category("Screenshots"), DefaultValue(""), Description("Output folder where screenshots will get saved.")]
        public string OutputDirectory { get; set; }

        [Category("Screenshots"), DefaultValue(EImageFormat.PNG), Description("FFmpeg thumbnail extension e.g. png or jpg.")]
        public EImageFormat FFmpegThumbnailExtension { get; set; }

        [Category("Screenshots"), DefaultValue(3), Description("Total number of screenshots to take.")]
        public int ScreenshotCount { get; set; }

        [Category("Screenshots"), DefaultValue(false), Description("Choose random frame each time a media file is processed.")]
        public bool RandomFrame { get; set; }

        [Category("Screenshots"), DefaultValue(true), Description("Upload screenshots.")]
        public bool UploadScreenshots { get; set; }

        [Category("Screenshots"), DefaultValue(true), Description("Keep or delete screenshots after processing files.")]
        public bool KeepScreenshots { get; set; }

        [Category("Screenshots"), DefaultValue(true), Description("After all screenshots taken open output directory automatically.")]
        public bool OpenDirectory { get; set; }

        [Category("Screenshots"), DefaultValue(0), Description("Maximum thumbnail width size, 0 means don't resize.")]
        public int MaxThumbnailWidth { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(false), Description("Combine all screenshots to one large screenshot.")]
        public bool CombineScreenshots { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(20), Description("Space between border and content as pixel.")]
        public int Padding { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(10), Description("Space between screenshots as pixel.")]
        public int Spacing { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(1), Description("Number of screenshots per row.")]
        public int ColumnCount { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(true), Description("Add movie information to the combined screenshot.")]
        public bool AddMovieInfo { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(true), Description("Add timestamp of screenshot at corner of image.")]
        public bool AddTimestamp { get; set; }

        [Category("Screenshots / Combined"), DefaultValue(true), Description("Draw rectangle shadow behind thumbnails.")]
        public bool DrawShadow { get; set; }

        public VideoThumbnailOptions()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}