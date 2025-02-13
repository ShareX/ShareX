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
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib
{
    public static class UploaderFactory
    {
        public static List<IUploaderService> AllServices { get; } = new List<IUploaderService>();
        public static List<IGenericUploaderService> AllGenericUploaderServices { get; } = new List<IGenericUploaderService>();
        public static Dictionary<ImageDestination, ImageUploaderService> ImageUploaderServices { get; } = CacheServices<ImageDestination, ImageUploaderService>();
        public static Dictionary<TextDestination, TextUploaderService> TextUploaderServices { get; } = CacheServices<TextDestination, TextUploaderService>();
        public static Dictionary<FileDestination, FileUploaderService> FileUploaderServices { get; } = CacheServices<FileDestination, FileUploaderService>();
        public static Dictionary<UrlShortenerType, URLShortenerService> URLShortenerServices { get; } = CacheServices<UrlShortenerType, URLShortenerService>();
        public static Dictionary<URLSharingServices, URLSharingService> URLSharingServices { get; } = CacheServices<URLSharingServices, URLSharingService>();

        private static Dictionary<T, T2> CacheServices<T, T2>() where T2 : UploaderService<T>
        {
            IEnumerable<T2> instances = Helpers.GetInstances<T2>();

            AllServices.AddRange(instances.OfType<IUploaderService>());
            AllGenericUploaderServices.AddRange(instances.OfType<IGenericUploaderService>());

            return instances.ToDictionary(x => x.EnumValue, x => x);
        }
    }
}