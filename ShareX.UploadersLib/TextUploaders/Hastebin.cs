#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

using ShareX.HelpersLib;
using Newtonsoft.Json;

namespace ShareX.UploadersLib.TextUploaders
{
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
                        string syntaxHighlighting;

                        if (!string.IsNullOrEmpty(SyntaxHighlighting))
                        {
                            syntaxHighlighting = SyntaxHighlighting;
                        }
                        else
                        {
                            syntaxHighlighting = "hs";
                        }

                        ur.URL = URLHelpers.CombineURL(domain, response.Key + "." + syntaxHighlighting);
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