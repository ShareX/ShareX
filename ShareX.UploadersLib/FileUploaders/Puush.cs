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

using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class PuushFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Puush;

        public override Icon ServiceIcon => Resources.puush;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.PuushAPIKey);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Puush(config.PuushAPIKey);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPuush;
    }

    public class Puush : FileUploader
    {
        public const string PuushURL = "https://puush.me";
        public const string PuushRegisterURL = PuushURL + "/register";
        public const string PuushResetPasswordURL = PuushURL + "/reset_password";

        private const string PuushAPIURL = PuushURL + "/api";
        private const string PuushAPILoginURL = PuushAPIURL + "/auth";
        private const string PuushAPIUploadURL = PuushAPIURL + "/up";

        public string APIKey { get; set; }

        public Puush()
        {
        }

        public Puush(string apiKey)
        {
            APIKey = apiKey;
        }

        public string Login(string email, string password)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("e", email);
            arguments.Add("p", password);
            arguments.Add("z", "ShareX");

            string response = SendRequestMultiPart(PuushAPILoginURL, arguments);

            if (!string.IsNullOrEmpty(response))
            {
                string[] values = response.Split(',');

                if (values != null && values.Length > 1)
                {
                    int status;

                    if (int.TryParse(values[0], out status) && status >= 0)
                    {
                        return values[1];
                    }
                }
            }

            return null;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("k", APIKey);
            arguments.Add("z", "ShareX");

            UploadResult result = SendRequestFile(PuushAPIUploadURL, stream, fileName, "f", arguments);

            if (result.IsSuccess)
            {
                string[] values = result.Response.Split(',');

                if (values != null && values.Length > 1)
                {
                    int status;

                    if (!int.TryParse(values[0], out status))
                    {
                        status = -2;
                    }

                    if (status < 0)
                    {
                        switch (status)
                        {
                            case -1:
                                Errors.Add("Authentication failure.");
                                break;
                            default:
                            case -2:
                                Errors.Add("Connection error.");
                                break;
                            case -3:
                                Errors.Add("Checksum error.");
                                break;
                            case -4:
                                Errors.Add("Insufficient account storage remaining.");
                                break;
                        }
                    }
                    else
                    {
                        result.URL = values[1];
                    }
                }
            }

            return result;
        }
    }
}