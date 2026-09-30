#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

using ShareX.Destinations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib
{
    /// <summary>The image, text and file destinations ShareX used before routes. Kept in task settings so older versions still read them.</summary>
    public sealed class LegacyDestinations
    {
        public ImageDestination ImageDestination { get; set; } = ImageDestination.Imgur;
        public FileDestination ImageFileDestination { get; set; } = FileDestination.Dropbox;
        public TextDestination TextDestination { get; set; } = TextDestination.Pastebin;
        public FileDestination TextFileDestination { get; set; } = FileDestination.Dropbox;
        public FileDestination FileDestination { get; set; } = FileDestination.Dropbox;
    }

    /// <summary>Creates instances and routes from existing settings so upgraded configurations behave exactly as before.</summary>
    public static class DestinationMigration
    {
        /// <summary>
        /// Adds a default instance for every uploader and an instance for every FTP account and custom uploader.
        /// Safe to call on every start: it only adds what is missing and keeps names of account instances in sync.
        /// </summary>
        public static void EnsureInstances(UploadersConfig config)
        {
            DestinationRoutingConfig routing = config.DestinationRouting ??= new DestinationRoutingConfig();
            routing.Instances ??= new List<DestinationInstance>();
            routing.CustomFileTypes ??= new List<FileTypeDefinition>();

            foreach ((UploaderCategory category, string uploader) in DestinationCatalog.GetUploaders())
            {
                if (routing.FindDefaultInstance(category, uploader) == null)
                {
                    routing.Instances.Add(new DestinationInstance
                    {
                        Name = DestinationCatalog.GetUploaderName(category, uploader),
                        Category = category,
                        Uploader = uploader,
                        IsDefault = true
                    });
                }
            }

            SyncFTPAccounts(config, routing);
            SyncCustomUploaders(config, routing);
        }

        private static void SyncFTPAccounts(UploadersConfig config, DestinationRoutingConfig routing)
        {
            int count = config.FTPAccountList?.Count ?? 0;

            for (int i = 0; i < count; i++)
            {
                string name = "FTP: " + (string.IsNullOrEmpty(config.FTPAccountList[i].Name) ? (i + 1).ToString() : config.FTPAccountList[i].Name);
                SyncAccountInstance(routing, UploaderCategory.File, nameof(FileDestination.FTP), i, name);
            }

            RemoveAccountInstances(routing, instance => DestinationCatalog.IsFTP(instance) && instance.AccountIndex >= count);
        }

        private static void SyncCustomUploaders(UploadersConfig config, DestinationRoutingConfig routing)
        {
            int count = config.CustomUploadersList?.Count ?? 0;

            for (int i = 0; i < count; i++)
            {
                CustomUploaderItem item = config.CustomUploadersList[i];
                string name = string.IsNullOrEmpty(item.Name) ? $"Custom uploader {i + 1}" : item.Name;
                List<UploaderCategory> categories = new List<UploaderCategory>();

                if (item.DestinationType.HasFlag(CustomUploaderDestinationType.ImageUploader)) categories.Add(UploaderCategory.Image);
                if (item.DestinationType.HasFlag(CustomUploaderDestinationType.TextUploader)) categories.Add(UploaderCategory.Text);
                if (item.DestinationType.HasFlag(CustomUploaderDestinationType.FileUploader)) categories.Add(UploaderCategory.File);

                foreach (UploaderCategory category in categories)
                {
                    // A custom uploader that serves several categories gets one instance per category, because each category sends a different request.
                    string instanceName = categories.Count > 1 ? $"{name} ({GetCategoryName(category)})" : name;
                    SyncAccountInstance(routing, category, GetCustomUploader(category), i, instanceName);
                }

                RemoveAccountInstances(routing, instance => DestinationCatalog.IsCustomUploader(instance) && instance.AccountIndex == i &&
                    !categories.Contains(instance.Category));
            }

            RemoveAccountInstances(routing, instance => DestinationCatalog.IsCustomUploader(instance) && instance.AccountIndex >= count);
        }

        private static void SyncAccountInstance(DestinationRoutingConfig routing, UploaderCategory category, string uploader, int index, string name)
        {
            DestinationInstance instance = routing.Instances.FirstOrDefault(x => x.IsDefault && x.Category == category && x.Uploader == uploader && x.AccountIndex == index);

            if (instance == null)
            {
                routing.Instances.Add(new DestinationInstance { Name = name, Category = category, Uploader = uploader, AccountIndex = index, IsDefault = true });
            }
            else
            {
                // The account list is edited elsewhere, so its name wins.
                instance.Name = name;
            }
        }

        private static void RemoveAccountInstances(DestinationRoutingConfig routing, Func<DestinationInstance, bool> predicate)
        {
            // Only the instances this class created. Duplicates the user made stay, and fail the config check with a clear message.
            routing.Instances.RemoveAll(instance => instance.IsDefault && instance.AccountIndex != null && predicate(instance));
        }

        private static string GetCustomUploader(UploaderCategory category) => category switch
        {
            UploaderCategory.Image => nameof(ImageDestination.CustomImageUploader),
            UploaderCategory.Text => nameof(TextDestination.CustomTextUploader),
            _ => nameof(FileDestination.CustomFileUploader)
        };

        private static string GetCategoryName(UploaderCategory category) => category switch
        {
            UploaderCategory.Image => "Images",
            UploaderCategory.Text => "Text",
            _ => "Files"
        };

        /// <summary>
        /// The routes that reproduce the legacy destinations: Images, Text and Other files. The default instances keep using
        /// FTPSelectedImage/Text/File and Custom*UploaderSelected, so FTP and custom uploader choices carry over too.
        /// </summary>
        public static List<DestinationRoute> CreateRoutes(UploadersConfig config, LegacyDestinations legacy)
        {
            EnsureInstances(config);
            DestinationRoutingConfig routing = config.DestinationRouting;

            DestinationInstance images = legacy.ImageDestination == ImageDestination.FileUploader
                ? routing.FindDefaultInstance(UploaderCategory.File, legacy.ImageFileDestination.ToString())
                : routing.FindDefaultInstance(UploaderCategory.Image, legacy.ImageDestination.ToString());

            DestinationInstance text = legacy.TextDestination == TextDestination.FileUploader
                ? routing.FindDefaultInstance(UploaderCategory.File, legacy.TextFileDestination.ToString())
                : routing.FindDefaultInstance(UploaderCategory.Text, legacy.TextDestination.ToString());

            DestinationInstance files = routing.FindDefaultInstance(UploaderCategory.File, legacy.FileDestination.ToString());

            List<DestinationRoute> routes = new List<DestinationRoute>();
            if (images != null) routes.Add(new DestinationRoute(PremadeFileTypes.Images, images.Id));
            if (text != null) routes.Add(new DestinationRoute(PremadeFileTypes.Text, text.Id));
            if (files != null) routes.Add(new DestinationRoute(PremadeFileTypes.OtherFiles, files.Id));
            return routes;
        }

        /// <summary>
        /// Writes the Images, Text and Other files routes back to the legacy fields where they can be expressed,
        /// so code and older versions that still read them see the same destinations.
        /// </summary>
        public static void UpdateLegacy(UploadersConfig config, IEnumerable<DestinationRoute> routes, LegacyDestinations legacy)
        {
            DestinationRoutingConfig routing = config.DestinationRouting;

            if (routing == null)
            {
                return;
            }

            DestinationInstance Find(string fileTypeId)
            {
                DestinationRoute route = DestinationRoutes.Find(routes, fileTypeId);
                return route != null ? routing.FindInstance(route.InstanceId) : null;
            }

            DestinationInstance images = Find(PremadeFileTypes.Images);

            if (images != null)
            {
                if (images.Category == UploaderCategory.Image && Enum.TryParse(images.Uploader, out ImageDestination image))
                {
                    legacy.ImageDestination = image;
                }
                else if (images.Category == UploaderCategory.File && Enum.TryParse(images.Uploader, out FileDestination imageFile))
                {
                    legacy.ImageDestination = ImageDestination.FileUploader;
                    legacy.ImageFileDestination = imageFile;
                }
            }

            DestinationInstance text = Find(PremadeFileTypes.Text);

            if (text != null)
            {
                if (text.Category == UploaderCategory.Text && Enum.TryParse(text.Uploader, out TextDestination textDestination))
                {
                    legacy.TextDestination = textDestination;
                }
                else if (text.Category == UploaderCategory.File && Enum.TryParse(text.Uploader, out FileDestination textFile))
                {
                    legacy.TextDestination = TextDestination.FileUploader;
                    legacy.TextFileDestination = textFile;
                }
            }

            DestinationInstance files = Find(PremadeFileTypes.OtherFiles);

            if (files != null && files.Category == UploaderCategory.File && Enum.TryParse(files.Uploader, out FileDestination file))
            {
                legacy.FileDestination = file;
            }
        }

        /// <summary>Duplicates an instance. A copy of an instance that uses the shared settings starts with a snapshot of them.</summary>
        public static DestinationInstance Duplicate(UploadersConfig config, DestinationInstance instance, string name = null)
        {
            DestinationInstance copy = config.DestinationRouting.Duplicate(instance, name);

            if (copy.Settings == null && DestinationCatalog.HasSettings(copy.Uploader))
            {
                DestinationCatalog.CaptureSettings(config, copy);
            }

            return copy;
        }

        /// <summary>Adds a new instance of an uploader, starting from the shared settings.</summary>
        public static DestinationInstance AddInstance(UploadersConfig config, UploaderCategory category, string uploader, string name = null)
        {
            DestinationInstance instance = new DestinationInstance
            {
                Name = name ?? config.DestinationRouting.GetUniqueName(DestinationCatalog.GetUploaderName(category, uploader)),
                Category = category,
                Uploader = uploader
            };

            if (DestinationCatalog.HasSettings(uploader))
            {
                DestinationCatalog.CaptureSettings(config, instance);
            }

            return config.DestinationRouting.Add(instance);
        }
    }
}
