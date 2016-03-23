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

using ShareX.HelpersLib;
using System.Linq;

namespace ShareX.UploadersLib
{
    public static class UploaderFactory
    {
        private static readonly ImageUploaderService[] imageUploaderServices = Helpers.GetInstances<ImageUploaderService>();
        private static readonly TextUploaderService[] textUploaderServices = Helpers.GetInstances<TextUploaderService>();
        private static readonly FileUploaderService[] fileUploaderServices = Helpers.GetInstances<FileUploaderService>();
        private static readonly URLShortenerService[] urlShortenerServices = Helpers.GetInstances<URLShortenerService>();
        private static readonly URLSharingService[] sharingServices = Helpers.GetInstances<URLSharingService>();

        public static ImageUploaderService GetImageUploaderServiceByEnum(ImageDestination enumValue)
        {
            return imageUploaderServices.First(x => x.EnumValue == enumValue);
        }

        public static TextUploaderService GetTextUploaderServiceByEnum(TextDestination enumValue)
        {
            return textUploaderServices.First(x => x.EnumValue == enumValue);
        }

        public static FileUploaderService GetFileUploaderServiceByEnum(FileDestination enumValue)
        {
            return fileUploaderServices.First(x => x.EnumValue == enumValue);
        }

        public static URLShortenerService GetURLShortenerServiceByEnum(UrlShortenerType enumValue)
        {
            return urlShortenerServices.First(x => x.EnumValue == enumValue);
        }

        public static URLSharingService GetSharingServiceByEnum(URLSharingServices enumValue)
        {
            return sharingServices.First(x => x.EnumValue == enumValue);
        }
    }
}