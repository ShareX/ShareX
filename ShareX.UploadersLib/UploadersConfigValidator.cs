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

using System;

namespace ShareX.UploadersLib
{
    public static class UploadersConfigValidator
    {
        public static bool Validate<T>(int index, UploadersConfig config)
        {
            Enum destination = (Enum)Enum.ToObject(typeof(T), index);

            switch (destination)
            {
                case ImageDestination imageDestination:
                    return Validate(imageDestination, config);
                case TextDestination textDestination:
                    return Validate(textDestination, config);
                case FileDestination fileDestination:
                    return Validate(fileDestination, config);
                case UrlShortenerType urlShortenerType:
                    return Validate(urlShortenerType, config);
                case URLSharingServices urlSharingServices:
                    return Validate(urlSharingServices, config);
            }

            return true;
        }

        public static bool Validate(ImageDestination destination, UploadersConfig config)
        {
            if (destination == ImageDestination.FileUploader) return true;
            return UploaderFactory.ImageUploaderServices[destination].CheckConfig(config);
        }

        public static bool Validate(TextDestination destination, UploadersConfig config)
        {
            if (destination == TextDestination.FileUploader) return true;
            return UploaderFactory.TextUploaderServices[destination].CheckConfig(config);
        }

        public static bool Validate(FileDestination destination, UploadersConfig config)
        {
            return UploaderFactory.FileUploaderServices[destination].CheckConfig(config);
        }

        public static bool Validate(UrlShortenerType destination, UploadersConfig config)
        {
            return UploaderFactory.URLShortenerServices[destination].CheckConfig(config);
        }

        public static bool Validate(URLSharingServices destination, UploadersConfig config)
        {
            return UploaderFactory.URLSharingServices[destination].CheckConfig(config);
        }
    }
}