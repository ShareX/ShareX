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

using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class SulFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Sul;

        public override Image ServiceImage => Resources.Sul;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.SulAPIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new SulUploader(config.SulAPIKey);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpSul;
    }

    public sealed class SulUploader : FileUploader
    {
        private string APIKey { get; set; }

        public SulUploader(string apiKey)
        {
            APIKey = apiKey;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("wizard", "true");
            args.Add("key", APIKey);
            args.Add("client", "sharex-native");

            string url = "https://s-ul.eu";
            string upload_url = URLHelpers.CombineURL(url, "api/v1/upload");

            UploadResult result = SendRequestFile(upload_url, stream, fileName, "file", args);

            if (result.IsSuccess)
            {
                JToken jsonResponse = JToken.Parse(result.Response);

                string protocol = "";
                string domain = "";
                string file = "";
                string extension = "";
                string error = "";

                if (jsonResponse != null)
                {
                    protocol = (string)jsonResponse.SelectToken("protocol");
                    domain = (string)jsonResponse.SelectToken("domain");
                    file = (string)jsonResponse.SelectToken("filename");
                    extension = (string)jsonResponse.SelectToken("extension");
                    error = (string)jsonResponse.SelectToken("error");
                }

                if (!string.IsNullOrEmpty(error) || string.IsNullOrEmpty(protocol))
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        Errors.Add("Generic error occurred, please contact support@s-ul.eu");
                    }
                    else
                    {
                        Errors.Add(error);
                    }
                }
                else
                {
                    result.URL = protocol + domain + "/" + file + extension;
                    result.DeletionURL = URLHelpers.CombineURL(url, "delete.php?key=" + APIKey + "&file=" + file);
                }
            }

            return result;
        }
    }
}