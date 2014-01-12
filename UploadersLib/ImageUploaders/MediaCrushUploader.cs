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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading;

namespace UploadersLib.ImageUploaders
{
    public class MediaCrushUploader : ImageUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            string hash = CreateHash(stream);

            UploadResult result = CheckExists(hash);

            if (result != null)
            {
                return result;
            }

            try
            {
                result = UploadData(stream, "https://mediacru.sh/api/upload/file", fileName, suppressWebExceptions: false);
            }
            catch (WebException e)
            {
                HttpWebResponse response = e.Response as HttpWebResponse;

                if (response == null)
                {
                    throw;
                }

                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    return HandleDuplicate(response);
                }

                throw;
            }

            hash = JToken.Parse(result.Response)["hash"].Value<string>();

            while (true)
            {
                result.Response = SendGetRequest("https://mediacru.sh/api/" + hash + "/status");
                JToken jsonResponse = JToken.Parse(result.Response);
                string status = jsonResponse["status"].Value<string>();

                switch (status)
                {
                    case "processing":
                    case "pending":
                        Thread.Sleep(1000);
                        break;
                    case "done":
                    case "ready":
                        MediaCrushBlob blob = jsonResponse[hash].ToObject<MediaCrushBlob>();
                        result.URL = blob.DirectURL;
                        result.DeletionURL = blob.DeletionURL;
                        return result;
                    case "unrecognized":
                        // Note: MediaCrush accepts just about _every_ kind of media file,
                        // so the file itself is probably corrupted or just not actually a media file
                        throw new Exception("This file is not an acceptable file type.");
                    case "timeout":
                        throw new Exception("This file took too long to process.");
                    default:
                        throw new Exception("This file failed to process.");
                }
            }
        }

        private string CreateHash(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Seek(0, SeekOrigin.Begin);
                string hash = Convert.ToBase64String(md5.ComputeHash(buffer));
                return hash.Replace('+', '-').Replace('/', '_').Remove(12);
            }
        }

        private UploadResult HandleDuplicate(HttpWebResponse httpResponse)
        {
            JToken response;
            using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
                response = JToken.Parse(streamReader.ReadToEnd());
            string hash = response["hash"].Value<string>();
            MediaCrushBlob blob = response[hash].ToObject<MediaCrushBlob>();

            return new UploadResult
            {
                URL = blob.DirectURL,
                DeletionURL = blob.DeletionURL
            };
        }

        private UploadResult CheckExists(string hash)
        {
            try
            {
                string response = SendGetRequest("https://mediacru.sh/api/" + hash);

                if (!string.IsNullOrEmpty(response))
                {
                    MediaCrushBlob blob = JsonConvert.DeserializeObject<MediaCrushBlob>(response);

                    return new UploadResult(response)
                    {
                        URL = blob.DirectURL,
                        DeletionURL = blob.DeletionURL
                    };
                }
            }
            catch
            {
            }

            return null;
        }
    }

    internal class MediaCrushBlob
    {
        public class MediaCrushFile
        {
            [JsonProperty("file")]
            public string Path { get; set; }
            [JsonProperty("type")]
            public string Mimetype { get; set; }
        }

        [JsonProperty("blob_type")]
        public string BlobType { get; set; }
        [JsonProperty("compression")]
        public double Compression { get; set; }
        [JsonProperty("files")]
        public MediaCrushFile[] Files { get; set; }
        [JsonProperty("extras")]
        public MediaCrushFile[] Extras { get; set; }
        [JsonProperty("original")]
        public string Original { get; set; }
        [JsonProperty("type")]
        public string UserMimetype { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonIgnore]
        public string URL
        {
            get
            {
                return "https://mediacru.sh/" + Hash;
            }
        }

        [JsonIgnore]
        public string DirectURL
        {
            get
            {
                if (Files != null && Files.Length > 0 && IsDirectURLPossible(Files[0]))
                {
                    return "https://mediacru.sh" + Files[0].Path;
                }

                return URL;
            }
        }

        [JsonIgnore]
        public string DeletionURL
        {
            get
            {
                return "https://mediacru.sh/" + Hash + "/delete";
            }
        }

        private bool IsDirectURLPossible(MediaCrushFile file)
        {
            switch (file.Mimetype)
            {
                case "image/png":
                case "image/jpeg":
                case "image/bmp":
                    return true;
            }

            return false;
        }
    }
}