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

using System.Collections.Generic;
using System.ComponentModel;

namespace ShareX.UploadersLib.TextUploaders
{
    public sealed class Pastebin_ca : TextUploader
    {
        private const string APIURL = "http://pastebin.ca/quiet-paste.php";

        private string APIKey;

        private PastebinCaSettings settings;

        public Pastebin_ca(string apiKey)
        {
            APIKey = apiKey;
            settings = new PastebinCaSettings();
        }

        public Pastebin_ca(string apiKey, PastebinCaSettings settings)
        {
            APIKey = apiKey;
            this.settings = settings;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("api", APIKey);
                arguments.Add("content", text);
                arguments.Add("description", settings.Description);

                if (settings.Encrypt)
                {
                    arguments.Add("encrypt", "true");
                }

                arguments.Add("encryptpw", settings.EncryptPassword);
                arguments.Add("expiry", settings.ExpireTime);
                arguments.Add("name", settings.Author);
                arguments.Add("s", "Submit Post");
                arguments.Add("tags", settings.Tags);
                arguments.Add("type", settings.TextFormat);

                ur.Response = SendRequestMultiPart(APIURL, arguments);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    if (ur.Response.StartsWith("SUCCESS:"))
                    {
                        ur.URL = "http://pastebin.ca/" + ur.Response.Substring(8);
                    }
                    else if (ur.Response.StartsWith("FAIL:"))
                    {
                        Errors.Add(ur.Response.Substring(5));
                    }
                }
            }

            return ur;
        }
    }

    public class PastebinCaSettings
    {
        /// <summary>name</summary>
        [Description("Name / Title")]
        public string Author { get; set; }

        /// <summary>description</summary>
        [Description("Description / Question")]
        public string Description { get; set; }

        /// <summary>tags</summary>
        [Description("Tags (space separated, optional)")]
        public string Tags { get; set; }

        /// <summary>type</summary>
        [Description("Content Type"), DefaultValue("1")]
        public string TextFormat { get; set; }

        /// <summary>expiry</summary>
        [Description("Expire this post in ..."), DefaultValue("1 month")]
        public string ExpireTime { get; set; }

        /// <summary>encrypt</summary>
        [Description("Encrypt this paste")]
        public bool Encrypt { get; set; }

        /// <summary>encryptpw</summary>
        public string EncryptPassword { get; set; }

        public PastebinCaSettings()
        {
            Author = "";
            Description = "";
            Tags = "";
            TextFormat = "1";
            ExpireTime = "1 month";
            Encrypt = false;
            EncryptPassword = "";
        }
    }
}