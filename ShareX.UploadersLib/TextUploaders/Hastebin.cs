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
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    public class HastebinTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Hastebin;

        public override Image ServiceImage => Resources.Hastebin;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Hastebin()
            {
                CustomDomain = config.HastebinCustomDomain,
                SyntaxHighlighting = config.HastebinSyntaxHighlighting,
                UseFileExtension = config.HastebinUseFileExtension
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpHastebin;
    }

    public sealed class Hastebin : TextUploader
    {
        public string CustomDomain { get; set; }
        public string SyntaxHighlighting { get; set; }
        public bool UseFileExtension { get; set; }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                string domain;

                if (!string.IsNullOrEmpty(CustomDomain))
                {
                    domain = CustomDomain;
                }
                else
                {
                    domain = "https://hastebin.com";
                }

                ur.Response = SendRequest(HttpMethod.POST, URLHelpers.CombineURL(domain, "documents"), text);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    HastebinResponse response = JsonConvert.DeserializeObject<HastebinResponse>(ur.Response);

                    if (response != null && !string.IsNullOrEmpty(response.Key))
                    {
                        string url = URLHelpers.CombineURL(domain, response.Key);

                        string syntaxHighlighting = SyntaxHighlighting;

                        if (UseFileExtension)
                        {
                            string ext = FileHelpers.GetFileNameExtension(fileName);

                            if (!string.IsNullOrEmpty(ext) && !ext.Equals("txt", StringComparison.OrdinalIgnoreCase))
                            {
                                syntaxHighlighting = ext.ToLowerInvariant();
                            }
                        }

                        if (!string.IsNullOrEmpty(syntaxHighlighting))
                        {
                            url += "." + syntaxHighlighting;
                        }

                        ur.URL = url;
                    }
                }
            }

            return ur;
        }

        private class HastebinResponse
        {
            public string Key { get; set; }
        }
    }
}