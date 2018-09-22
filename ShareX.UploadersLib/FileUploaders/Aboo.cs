﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class AbooFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Aboo;

        public override Icon ServiceIcon => Resources.Aboo;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;   // No config needed
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Aboo();
        }
    }

    public class Aboo : FileUploader
    {
        public const string AbooURL = "https://aboo.se";

        private const string AbooAPIURL = "https://api.aboo.se";
        private const string AbooAPIUploadURL = AbooAPIURL + "/upload";

        public Aboo()
        {
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = SendRequestFile(AbooAPIUploadURL, stream, fileName, "file");

            if (result.IsSuccess)
            {
                AbooUploadResponse response = JsonConvert.DeserializeObject<AbooUploadResponse>(result.Response);
                if (response.status)
                {
                    result.URL = response.data.file.url.full;
                    result.ShortenedURL = response.data.file.url.minimal;
                }
                else
                {
                    Errors.Add($"Message: {response.error.message}, type: {response.error.type}, code: {response.error.code}");
                }
            }

            return result;
        }

        //
        // Documentation for api response can be found at: https://api.aboo.se/
        //
        private class AbooUploadResponse
        {
            public bool status { get; set; }
            public AbooUploadData data { get; set; }
            public AbooUploadError error { get; set; }
        }

        private class AbooUploadError
        {
            public string message { get; set; }
            public string type { get; set; }
            public int code { get; set; }
        }

        private class AbooUploadData
        {
            public AbooUploadFile file { get; set; }
        }

        private class AbooUploadFile
        {
            public AUrl url { get; set; }
            public AMetadata metadata { get;set; }
        }

        private class AUrl
        {
            public string minimal { get; set; }
            public string full { get; set; }
        }

        private class AMetadata
        {
            public string id { get; set; }
            public string name { get; set; }
            public string mimetype { get; set; }
            public ASize size { get; set; }
        }

        private class ASize
        {
            public string bytes { get; set; }
            public string readable { get; set; }
        }
    }
}