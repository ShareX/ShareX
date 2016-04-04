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

// Credits: https://github.com/DanielMcAssey

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public class SomeImageImageUploaderService : ImageUploaderService
    {
        public override ImageDestination EnumValue { get; } = ImageDestination.SomeImage;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            string someImageAPIKey = config.SomeImageAPIKey;

            if (string.IsNullOrEmpty(someImageAPIKey))
            {
                someImageAPIKey = APIKeys.SomeImageKey;
            }

            return new SomeImage(someImageAPIKey)
            {
                DirectURL = config.SomeImageDirectURL
            };
        }
    }

    public sealed class SomeImage : ImageUploader
    {
        private const string API_ENDPOINT = "https://someimage.com/api/2/image/upload";

        public string API_KEY { get; set; }
        public bool DirectURL { get; set; }

        public SomeImage(string apiKey)
        {
            API_KEY = apiKey;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("apikey", API_KEY);
            arguments.Add("familysafe", "0"); // Set to 0 as images could possibly not be family safe.

            UploadResult result = UploadData(stream, API_ENDPOINT, fileName, "file", arguments);

            if (result.IsSuccess)
            {
                if (!string.IsNullOrEmpty(result.Response))
                {
                    SomeImageResponse jsonResponse = JsonConvert.DeserializeObject<SomeImageResponse>(result.Response);

                    if (jsonResponse != null)
                    {
                        if (DirectURL)
                        {
                            if (!string.IsNullOrEmpty(jsonResponse.imagelink))
                            {
                                Uri responseUri = new Uri(jsonResponse.imagelink); // http://someimage.com/asdf
                                string host = responseUri.Host; // someimage.com
                                string filename = Path.GetFileName(responseUri.AbsolutePath); // /asdf
                                if (filename.StartsWith("/"))
                                {
                                    filename = filename.Remove(0, 1); // asdf
                                }
                                if (host.StartsWith("www."))
                                {
                                    host = host.Remove(0, 4);
                                }
                                string extension = Path.GetExtension(fileName);
                                result.URL = $"https://i1.{host}/{filename}{extension}";
                            }
                        }
                        else
                        {
                            result.URL = jsonResponse.imagelink;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class SomeImageResponse
    {
        public string success { get; set; }
        public string imageid { get; set; }
        public string imagelink { get; set; }
        public string thumblink { get; set; }
        public string embedhtml { get; set; }
        public string embedbb { get; set; }
    }
}