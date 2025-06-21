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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.UploadersLib.URLShorteners
{
    public class FirebaseDynamicLinksURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.FirebaseDynamicLinks;

        public override Icon ServiceIcon => Resources.Firebase;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrEmpty(config.FirebaseWebAPIKey) && !string.IsNullOrEmpty(config.FirebaseDynamicLinkDomain);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new FirebaseDynamicLinksURLShortener
            {
                WebAPIKey = config.FirebaseWebAPIKey,
                DynamicLinkDomain = config.FirebaseDynamicLinkDomain,
                IsShort = config.FirebaseIsShort
            };
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpFirebaseDynamicLinks;
    }

    public class FirebaseRequest
    {
        public DynamicLinkInfo dynamicLinkInfo { get; set; }
        public FirebaseSuffix suffix { get; set; }
    }

    public class DynamicLinkInfo
    {
        public string dynamicLinkDomain { get; set; }
        public string link { get; set; }
    }

    public class FirebaseSuffix
    {
        public string option { get; set; }
    }

    public class FirebaseResponse
    {
        public string shortLink { get; set; }
        public string previewLink { get; set; }
    }

    public sealed class FirebaseDynamicLinksURLShortener : URLShortener
    {
        public string WebAPIKey { get; set; }
        public string DynamicLinkDomain { get; set; }
        public bool IsShort { get; set; }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };

            FirebaseRequest requestOptions = new FirebaseRequest
            {
                dynamicLinkInfo = new DynamicLinkInfo
                {
                    dynamicLinkDomain = URLHelpers.RemovePrefixes(DynamicLinkDomain),
                    link = url
                }
            };

            if (IsShort)
            {
                requestOptions.suffix = new FirebaseSuffix
                {
                    option = "SHORT"
                };
            }

            Dictionary<string, string> args = new Dictionary<string, string>
            {
                { "key", WebAPIKey },
                { "fields", "shortLink" }
            };

            string serializedRequestOptions = JsonConvert.SerializeObject(requestOptions);
            result.Response = SendRequest(HttpMethod.POST, "https://firebasedynamiclinks.googleapis.com/v1/shortLinks", serializedRequestOptions, RequestHelpers.ContentTypeJSON, args);

            FirebaseResponse firebaseResponse = JsonConvert.DeserializeObject<FirebaseResponse>(result.Response);

            if (firebaseResponse != null)
            {
                result.ShortenedURL = firebaseResponse.shortLink;
            }

            return result;
        }
    }
}