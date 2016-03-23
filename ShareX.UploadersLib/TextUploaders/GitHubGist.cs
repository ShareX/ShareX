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

// Credits: https://github.com/gpailler

using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.HelperClasses;
using System.Collections.Generic;
using System.Net;

namespace ShareX.UploadersLib.TextUploaders
{
    public class GitHubGistTextUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.Gist;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override TextUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            OAuth2Info oauth = null;

            if (!config.GistAnonymousLogin)
            {
                oauth = config.GistOAuth2Info;
            }

            return new GitHubGist(oauth)
            {
                PublicUpload = config.GistPublishPublic,
                RawURL = config.GistRawURL
            };
        }
    }

    public sealed class GitHubGist : TextUploader, IOAuth2Basic
    {
        private const string URLAPI = "https://api.github.com/";
        private const string URLGists = URLAPI + "gists";

        public OAuth2Info AuthInfo { get; private set; }

        public bool PublicUpload { get; set; }
        public bool RawURL { get; set; }

        public GitHubGist()
        {
        }

        public GitHubGist(OAuth2Info oAuthInfos)
        {
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

            string response = SendRequest(HttpMethod.POST, "https://github.com/login/oauth/access_token", args, headers);

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
                    @public = PublicUpload,
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

                string response = SendRequestJSON(url, argsJson);

                if (response != null)
                {
                    if (RawURL)
                    {
                        ur.URL = Helpers.ParseJSON(response, "files.*.raw_url");
                    }
                    else
                    {
                        ur.URL = Helpers.ParseJSON(response, "html_url");
                    }
                }
            }

            return ur;
        }
    }
}