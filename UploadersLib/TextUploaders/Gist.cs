#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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

using HelpersLib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using UploadersLib.HelperClasses;

namespace UploadersLib.TextUploaders
{
    public sealed class Gist : TextUploader, IOAuth2Simple
    {
        private const string URLAPI = "https://api.github.com/";
        private const string URLGists = URLAPI + "gists";

        public OAuth2Info AuthInfo { get; private set; }

        private readonly bool publishPublic;

        public Gist(OAuth2Info oAuthInfos)
            : this(false, oAuthInfos)
        {
        }

        public Gist(bool publishPublic)
            : this(publishPublic, null)
        {
        }

        public Gist(bool publishPublic, OAuth2Info oAuthInfos)
        {
            this.publishPublic = publishPublic;
            AuthInfo = oAuthInfos;
        }

        public string GetAuthorizationURL()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("redirect_uri", Links.URL_CALLBACK);
            args.Add("scope", "gist");

            return CreateQuery("https://github.com/login/oauth/authorize", args);
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("code", code);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Accept", "application/json");

            string response = SendRequest(HttpMethod.POST, "https://github.com/login/oauth/access_token", args, headers: headers);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(fileName))
            {
                var gistUploadObject = new
                {
                    @public = this.publishPublic,
                    files = new Dictionary<string, object>
                    {
                        { fileName, new { content = text } }
                    }
                };

                string argsJson = JsonConvert.SerializeObject(gistUploadObject);

                string url = URLGists;

                if (AuthInfo != null)
                {
                    url += "?access_token=" + AuthInfo.Token.access_token;
                }

                string response = SendPostRequestJSON(url, argsJson);

                if (response != null)
                {
                    var gistReturnType = new { html_url = string.Empty };
                    var gistReturnObject = JsonConvert.DeserializeAnonymousType(response, gistReturnType);
                    ur.URL = gistReturnObject.html_url;
                }
            }

            return ur;
        }
    }
}