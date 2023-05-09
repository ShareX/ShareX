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

namespace ShareX.UploadersLib.URLShorteners
{
    public class TeknikUrlShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.Teknik;

        public override Icon ServiceIcon => Resources.Teknik;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.TeknikUrlShortenerAPIUrl) && !string.IsNullOrEmpty(config.TeknikAuthUrl);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new TeknikUrlShortener(config.TeknikOAuth2Info, config.TeknikAuthUrl)
            {
                APIUrl = config.TeknikUrlShortenerAPIUrl
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpTeknik;
    }

    public sealed class TeknikUrlShortener : URLShortener, IOAuth2
    {
        public OAuth2Info AuthInfo { get; set; }
        public string APIUrl { get; set; }

        private Teknik teknik;

        public TeknikUrlShortener(OAuth2Info oauth, string authUrl)
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

        public override UploadResult ShortenURL(string url)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("url", url);

            string response = SendRequestMultiPart(APIUrl, args, teknik.GetAuthHeaders());
            TeknikUrlShortenerResponseWrapper apiResponse = JsonConvert.DeserializeObject<TeknikUrlShortenerResponseWrapper>(response);

            UploadResult ur = new UploadResult();
            if (apiResponse.Result != null && apiResponse.Error == null)
            {
                ur.ShortenedURL = apiResponse.Result.shortUrl;
            }

            return ur;
        }

        public bool RefreshAccessToken()
        {
            return teknik.RefreshAccessToken();
        }

        public bool CheckAuthorization()
        {
            return teknik.CheckAuthorization();
        }
    }

    public class TeknikUrlShortenerResponseWrapper
    {
        public TeknikUrlShortenerResponse Result { get; set; }
        public TeknikErrorResponse Error { get; set; }
    }

    public class TeknikUrlShortenerResponse
    {
        public string shortUrl { get; set; }
        public string originalUrl { get; set; }
    }
}