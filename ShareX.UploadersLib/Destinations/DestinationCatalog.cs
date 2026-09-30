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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareX.Destinations;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.UploadersLib
{
    /// <summary>Connects destination instances to the uploader services and to UploadersConfig.</summary>
    public static class DestinationCatalog
    {
        // UploadersConfig properties that belong to each uploader. A property matches when its name starts with one of the prefixes.
        // FTP and custom uploaders are absent on purpose: their instances select an account from the shared list instead.
        private static readonly Dictionary<string, string[]> settingPrefixes = new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            [nameof(ImageDestination.Imgur)] = ["Imgur"],
            [nameof(ImageDestination.ImageShack)] = ["ImageShack"],
            [nameof(ImageDestination.Flickr)] = ["Flickr"],
            [nameof(ImageDestination.Photobucket)] = ["Photobucket"],
            [nameof(ImageDestination.Chevereto)] = ["Chevereto"],
            [nameof(ImageDestination.Vgyme)] = ["Vgyme"],
            [nameof(TextDestination.Pastebin)] = ["Pastebin"],
            [nameof(TextDestination.Paste_ee)] = ["Paste_ee"],
            [nameof(TextDestination.Gist)] = ["Gist"],
            [nameof(TextDestination.Upaste)] = ["Upaste"],
            [nameof(TextDestination.Hastebin)] = ["Hastebin"],
            [nameof(TextDestination.OneTimeSecret)] = ["OneTimeSecret"],
            [nameof(TextDestination.Pastie)] = ["Pastie"],
            [nameof(TextDestination.PrivateBin)] = ["PrivateBin"],
            [nameof(FileDestination.Dropbox)] = ["Dropbox"],
            [nameof(FileDestination.OneDrive)] = ["OneDrive"],
            [nameof(FileDestination.GoogleDrive)] = ["GoogleDrive"],
            [nameof(FileDestination.Puush)] = ["Puush"],
            [nameof(FileDestination.Box)] = ["Box"],
            [nameof(FileDestination.Mega)] = ["Mega"],
            [nameof(FileDestination.AmazonS3)] = ["AmazonS3"],
            [nameof(FileDestination.GoogleCloudStorage)] = ["GoogleCloudStorage"],
            [nameof(FileDestination.AzureStorage)] = ["AzureStorage"],
            [nameof(FileDestination.BackblazeB2)] = ["B2"],
            [nameof(FileDestination.OwnCloud)] = ["OwnCloud"],
            [nameof(FileDestination.Immich)] = ["Immich"],
            [nameof(FileDestination.MediaFire)] = ["MediaFire"],
            [nameof(FileDestination.Pushbullet)] = ["Pushbullet"],
            [nameof(FileDestination.SendSpace)] = ["SendSpace"],
            [nameof(FileDestination.Localhostr)] = ["Localhostr"],
            [nameof(FileDestination.Lambda)] = ["Lambda"],
            [nameof(FileDestination.ImgFish)] = ["ImgFish"],
            [nameof(FileDestination.Pomf)] = ["Pomf"],
            [nameof(FileDestination.Seafile)] = ["Seafile"],
            [nameof(FileDestination.Streamable)] = ["Streamable"],
            [nameof(FileDestination.Sul)] = ["Sul"],
            [nameof(FileDestination.Lithiio)] = ["Lithiio"],
            [nameof(FileDestination.Plik)] = ["Plik"],
            [nameof(FileDestination.YouTube)] = ["YouTube"],
            [nameof(FileDestination.SharedFolder)] = ["LocalhostAccountList", "LocalhostSelected"],
            [nameof(FileDestination.Email)] = ["Email"]
        };

        /// <summary>Every uploader that can back an instance. The "File uploader" entries of the image and text lists are routes now, not uploaders.</summary>
        public static IEnumerable<(UploaderCategory Category, string Uploader)> GetUploaders()
        {
            foreach (ImageDestination destination in Helpers.GetEnums<ImageDestination>().Where(x => x != ImageDestination.FileUploader))
            {
                yield return (UploaderCategory.Image, destination.ToString());
            }

            foreach (TextDestination destination in Helpers.GetEnums<TextDestination>().Where(x => x != TextDestination.FileUploader))
            {
                yield return (UploaderCategory.Text, destination.ToString());
            }

            foreach (FileDestination destination in Helpers.GetEnums<FileDestination>())
            {
                yield return (UploaderCategory.File, destination.ToString());
            }
        }

        public static IGenericUploaderService GetService(UploaderCategory category, string uploader)
        {
            switch (category)
            {
                case UploaderCategory.Image when Enum.TryParse(uploader, out ImageDestination image) && UploaderFactory.ImageUploaderServices.TryGetValue(image, out ImageUploaderService imageService):
                    return imageService;
                case UploaderCategory.Text when Enum.TryParse(uploader, out TextDestination text) && UploaderFactory.TextUploaderServices.TryGetValue(text, out TextUploaderService textService):
                    return textService;
                case UploaderCategory.File when Enum.TryParse(uploader, out FileDestination file) && UploaderFactory.FileUploaderServices.TryGetValue(file, out FileUploaderService fileService):
                    return fileService;
                default:
                    return null;
            }
        }

        public static IGenericUploaderService GetService(DestinationInstance instance) => GetService(instance.Category, instance.Uploader);

        public static string GetUploaderName(UploaderCategory category, string uploader)
        {
            switch (category)
            {
                case UploaderCategory.Image when Enum.TryParse(uploader, out ImageDestination image):
                    return image.GetLocalizedDescription(Localization.Strings.ResourceManager);
                case UploaderCategory.Text when Enum.TryParse(uploader, out TextDestination text):
                    return text.GetLocalizedDescription(Localization.Strings.ResourceManager);
                case UploaderCategory.File when Enum.TryParse(uploader, out FileDestination file):
                    return file.GetLocalizedDescription(Localization.Strings.ResourceManager);
                default:
                    return uploader;
            }
        }

        public static bool IsFTP(DestinationInstance instance) => instance.Category == UploaderCategory.File && instance.Uploader == nameof(FileDestination.FTP);

        public static bool IsCustomUploader(DestinationInstance instance) =>
            instance.Uploader is nameof(ImageDestination.CustomImageUploader) or nameof(TextDestination.CustomTextUploader) or nameof(FileDestination.CustomFileUploader);

        /// <summary>Whether a duplicate of this uploader can hold its own settings. FTP and custom uploaders pick an account instead.</summary>
        public static bool HasSettings(string uploader) => settingPrefixes.ContainsKey(uploader);

        public static IEnumerable<string> GetSettingNames(string uploader)
        {
            if (!settingPrefixes.TryGetValue(uploader, out string[] prefixes))
            {
                return Enumerable.Empty<string>();
            }

            return typeof(UploadersConfig).GetProperties()
                .Where(property => property.CanRead && property.CanWrite && prefixes.Any(prefix => property.Name.StartsWith(prefix, StringComparison.Ordinal)))
                .Select(property => property.Name);
        }

        private static JsonSerializer CreateSerializer() => new JsonSerializer { ContractResolver = new DPAPIEncryptedStringPropertyResolver() };

        /// <summary>
        /// The settings an upload through this instance uses. Default instances return <paramref name="config"/> itself.
        /// Other instances get a copy with their own settings applied, so the shared settings are never touched.
        /// </summary>
        public static UploadersConfig CreateConfig(UploadersConfig config, DestinationInstance instance)
        {
            if (instance.Settings == null || instance.Settings.Count == 0)
            {
                return config;
            }

            JsonSerializer serializer = CreateSerializer();
            JObject values = JObject.FromObject(config, serializer);
            values.Remove(nameof(UploadersConfig.DestinationRouting));

            foreach (JProperty property in instance.Settings.Properties())
            {
                values[property.Name] = property.Value.DeepClone();
            }

            return values.ToObject<UploadersConfig>(serializer);
        }

        /// <summary>Stores the uploader's current values from <paramref name="config"/> in the instance. Secrets stay encrypted like in the settings file.</summary>
        public static void CaptureSettings(UploadersConfig config, DestinationInstance instance)
        {
            JObject values = JObject.FromObject(config, CreateSerializer());
            JObject settings = new JObject();

            foreach (string name in GetSettingNames(instance.Uploader))
            {
                if (values.TryGetValue(name, out JToken value))
                {
                    settings[name] = value.DeepClone();
                }
            }

            instance.Settings = settings;
        }

        public static TaskReferenceHelper ApplyAccount(TaskReferenceHelper helper, DestinationInstance instance)
        {
            if (instance.AccountIndex is int index)
            {
                if (IsFTP(instance))
                {
                    helper.OverrideFTP = true;
                    helper.FTPIndex = index;
                }
                else if (IsCustomUploader(instance))
                {
                    helper.OverrideCustomUploader = true;
                    helper.CustomUploaderIndex = index;
                }
            }

            return helper;
        }

        public static bool CheckConfig(DestinationInstance instance, UploadersConfig config)
        {
            IGenericUploaderService service = GetService(instance);

            if (service == null)
            {
                return false;
            }

            if (instance.AccountIndex is int index)
            {
                if (IsFTP(instance))
                {
                    return config.FTPAccountList != null && index >= 0 && index < config.FTPAccountList.Count;
                }

                if (IsCustomUploader(instance))
                {
                    return config.CustomUploadersList != null && index >= 0 && index < config.CustomUploadersList.Count;
                }
            }

            return service.CheckConfig(CreateConfig(config, instance));
        }
    }
}
