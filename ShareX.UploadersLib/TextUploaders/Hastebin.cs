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

using Newtonsoft.Json;
using ShareX.HelpersLib;

namespace ShareX.UploadersLib.TextUploaders
{
    public class HastebinTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Hastebin;

        public override bool CheckConfig(UploadersConfig uploadersConfig) => true;

        public override TextUploader CreateUploader(UploadersConfig uploadersConfig, TaskReferenceHelper taskInfo)
        {
            return new Hastebin()
            {
                CustomDomain = uploadersConfig.HastebinCustomDomain,
                SyntaxHighlighting = uploadersConfig.HastebinSyntaxHighlighting
            };
        }
    }

    public sealed class Hastebin : TextUploader
    {
        public string CustomDomain { get; set; }
        public string SyntaxHighlighting { get; set; }

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
                    domain = "http://hastebin.com";
                }

                ur.Response = SendRequest(HttpMethod.POST, URLHelpers.CombineURL(domain, "documents"), text);

                if (!string.IsNullOrEmpty(ur.Response))
                {
                    HastebinResponse response = JsonConvert.DeserializeObject<HastebinResponse>(ur.Response);

                    if (response != null && !string.IsNullOrEmpty(response.Key))
                    {
                        string url = URLHelpers.CombineURL(domain, response.Key);

                        if (!string.IsNullOrEmpty(SyntaxHighlighting))
                        {
                            url += "." + SyntaxHighlighting;
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