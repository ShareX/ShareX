#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class DropfileFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Dropfile;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Dropfile();
        }
    }

    public sealed class Dropfile : FileUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = SendRequestFile("https://d1.dropfile.to/upload", stream, fileName);

            if (result.IsSuccess)
            {
                DropfileResponse response = JsonConvert.DeserializeObject<DropfileResponse>(result.Response);

                if (response != null && response.Status == 0)
                {
                    result.URL = response.URL;
                    result.DeletionURL = response.URL.Replace("dropfile.to/", "dropfile.to/api/") + "?delete=" + response.Access_key;
                }
            }

            return result;
        }

        private class DropfileResponse
        {
            public int Status { get; set; }
            public string URL { get; set; }
            public string Access_key { get; set; }
        }
    }
}