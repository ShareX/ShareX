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

using ShareX.HelpersLib;
using System.ComponentModel;
using System.Drawing.Design;

namespace ShareX.MediaLib
{
    public class VideoThumbnailOptions
    {
        [Category("Thumbnails"), DefaultValue(ThumbnailLocationType.DefaultFolder), Description("Create thumbnails in default screenshot folder, same folder as the media file or in a custom folder.")]
        public ThumbnailLocationType OutputLocation { get; set; }

        [Category("Thumbnails"), DefaultValue(""), Description("Output folder where thumbnails will get saved."), Editor(typeof(DirectoryNameEditor), typeof(UITypeEditor))]
        public string CustomOutputDirectory { get; set; }

        [Category("Thumbnails"), DefaultValue(EImageFormat.PNG), Description("Thumbnail image format to save.")]
        public EImageFormat ImageFormat { get; set; }

        [Category("Thumbnails"), DefaultValue(9), Description("Total number of thumbnails to take.")]
        public int ThumbnailCount { get; set; }

        [Category("Thumbnails"), DefaultValue("_Thumbnail"), Description("Suffix to append to the thumbnail filename.")]
        public string FilenameSuffix { get; set; }

        [Category("Thumbnails"), DefaultValue(false), Description("Choose random frame each time a media file is processed.")]
        public bool RandomFrame { get; set; }

        [Category("Thumbnails"), DefaultValue(true), Description("Upload thumbnails.")]
        public bool UploadThumbnails { get; set; }

        [Category("Thumbnails"), DefaultValue(false), Description("After combine thumbnails keep single image files.")]
        public bool KeepScreenshots { get; set; }

        [Category("Thumbnails"), DefaultValue(false), Description("After all thumbnails taken open output directory automatically.")]
        public bool OpenDirectory { get; set; }

        [Category("Thumbnails"), DefaultValue(512), Description("Maximum thumbnail width size, 0 means don't resize.")]
        public int MaxThumbnailWidth { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(true), Description("Combine all thumbnails to one large thumbnail.")]
        public bool CombineScreenshots { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(10), Description("Space between border and content as pixel.")]
        public int Padding { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(10), Description("Space between thumbnails as pixel.")]
        public int Spacing { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(3), Description("Number of thumbnails per row.")]
        public int ColumnCount { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(true), Description("Add video information to the combined thumbnail.")]
        public bool AddVideoInfo { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(true), Description("Add timestamp of thumbnail at corner of image.")]
        public bool AddTimestamp { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(true), Description("Draw rectangle shadow behind thumbnails.")]
        public bool DrawShadow { get; set; }

        [Category("Thumbnails / Combined"), DefaultValue(true), Description("Draw border around thumbnails.")]
        public bool DrawBorder { get; set; }

        public string DefaultOutputDirectory, LastVideoPath;

        public VideoThumbnailOptions()
        {
            this.ApplyDefaultPropertyValues();
        }
    }
}