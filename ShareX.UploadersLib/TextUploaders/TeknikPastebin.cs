#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2023 ShareX Team

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
using ShareX.UploadersLib.FileUploaders;
using ShareX.UploadersLib.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders
{
    public class TeknikTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.TeknikPasteAPIUrl) && !string.IsNullOrEmpty(config.TeknikAuthUrl);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikPaster(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikPasteAPIUrl,
                ExpirationUnit = config.TeknikExpirationUnit,
                ExpirationLength = config.TeknikExpirationLength
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpTeknik;
    }

    public sealed class TeknikPaster : TextUploader, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string APIUrl { get; set; }
        public TeknikExpirationUnit ExpirationUnit { get; set; }
        public int ExpirationLength { get; set; }

        private Teknik teknik;

        public TeknikPaster(OAuth2Info oauth, string authUrl)
        {
            teknik = new Teknik(oauth, authUrl);
            AuthInfo = teknik.AuthInfo;
        }

        public bool GetAccessToken(string code)
        {
            return teknik.GetAccessToken(code);
        }

        public string GetAuthorizationURL()
        {
            return teknik.GetAuthorizationURL();
        }

        public bool RefreshAccessToken()
        {
            return teknik.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return teknik.CheckAuthorization();
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("code", text);
            args.Add("expirationUnit", ExpirationUnit.ToString());
            args.Add("expirationLength", ExpirationLength.ToString());

            string response = SendRequestMultiPart(APIUrl, args, teknik.GetAuthHeaders());
            TeknikPasteResponseWrapper apiResponse = JsonConvert.DeserializeObject<TeknikPasteResponseWrapper>(response);

            UploadResult ur = new UploadResult();
            if (apiResponse.Result != null && apiResponse.Error == null)
            {
                ur.URL = apiResponse.Result.Url;
            }

            return ur;
        }
    }

    public class TeknikPasteResponseWrapper
    {
        public TeknikPasteResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikPasteResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Syntax { get; set; }
        public string Password { get; set; }
    }
}