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

using Newtonsoft.Json;
using System.IO;

namespace ShareX.UploadersLib.ImageUploaders
{
    public sealed class ImmioUploader : ImageUploader
    {
        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = SendRequestFile("http://imm.io/store/", stream, fileName, "image");
            if (result.IsSuccess)
            {
                ImmioResponse response = JsonConvert.DeserializeObject<ImmioResponse>(result.Response);
                if (response != null) result.URL = response.Payload.Uri;
            }
            return result;
        }

        private class ImmioResponse
        {
            public bool Success { get; set; }
            public ImmioPayload Payload { get; set; }
        }

        private class ImmioPayload
        {
            public string Uid { get; set; }
            public string Uri { get; set; }
            public string Link { get; set; }
            public string Name { get; set; }
            public string Format { get; set; }
            public string Ext { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public string Size { get; set; }
        }
    }
}