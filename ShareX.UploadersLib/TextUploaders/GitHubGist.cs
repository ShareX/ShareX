#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2024 ShareX Team

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
using ShareX.HelpersLib.Helpers;
using ShareX.UploadersLib.BaseServices;
using ShareX.UploadersLib.BaseUploaders;
using ShareX.UploadersLib.Helpers;
using ShareX.UploadersLib.OAuth;
using ShareX.UploadersLib.Properties;

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace ShareX.UploadersLib.TextUploaders;

public class GitHubGistTextUploaderService : TextUploaderService
{
    public override TextDestination EnumValue { get; } = TextDestination.Gist;

    public override Icon ServiceIcon => Resources.GitHub;

    public override bool CheckConfig(UploadersConfig config)
    {
        return OAuth2Info.CheckOAuth(config.GistOAuth2Info);
    }

    public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
    {
        return new GitHubGist(config.GistOAuth2Info)
        {
            PublicUpload = config.GistPublishPublic,
            RawURL = config.GistRawURL,
            CustomURLAPI = config.GistCustomURL
        };
    }

    public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpGist;
}

public sealed class GitHubGist : TextUploader, IOAuth2Basic
{
    private const string URLAPI = "https://api.github.com";

    public OAuth2Info AuthInfo { get; private set; }

    public bool PublicUpload { get; set; }
    public bool RawURL { get; set; }
    public string CustomURLAPI { get; set; }

    public GitHubGist(OAuth2Info oAuthInfos)
    {
        AuthInfo = oAuthInfos;
    }

    public string GetAuthorizationURL()
    {
        Dictionary<string, string> args = new();
        args.Add("client_id", AuthInfo.Client_ID);
        args.Add("redirect_uri", Links.Callback);
        args.Add("scope", "gist");

        return URLHelpers.CreateQueryString("https://github.com/login/oauth/authorize", args);
    }

    public bool GetAccessToken(string code)
    {
        Dictionary<string, string> args = new();
        args.Add("client_id", AuthInfo.Client_ID);
        args.Add("client_secret", AuthInfo.Client_Secret);
        args.Add("code", code);

        WebHeaderCollection headers = new();
        headers.Add("Accept", RequestHelpers.ContentTypeJSON);

        string response = SendRequestMultiPart("https://github.com/login/oauth/access_token", args, headers);

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
        UploadResult ur = new();

        if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(fileName))
        {
            string url = !string.IsNullOrEmpty(CustomURLAPI) ? CustomURLAPI : URLAPI;
            url = URLHelpers.CombineURL(url, "gists");

            GistUpload gistUpload = new()
            {
                description = "",
                @public = PublicUpload,
                files = new Dictionary<string, GistUploadFileInfo>()
                {
                    { fileName, new GistUploadFileInfo() { content = text } }
                }
            };

            string json = JsonConvert.SerializeObject(gistUpload);

            NameValueCollection headers = new();
            headers.Add("Authorization", "token " + AuthInfo.Token.access_token);

            string response = SendRequest(HttpMethod.POST, url, json, RequestHelpers.ContentTypeJSON, null, headers);

            GistResponse gistResponse = JsonConvert.DeserializeObject<GistResponse>(response);

            if (response != null)
            {
                ur.URL = RawURL ? gistResponse.files.First().Value.raw_url : gistResponse.html_url;
            }
        }

        return ur;
    }

    private class GistUpload
    {
        public string description { get; set; }
        public bool @public { get; set; }
        public Dictionary<string, GistUploadFileInfo> files { get; set; }
    }

    private class GistUploadFileInfo
    {
        public string content { get; set; }
    }

    private class GistResponse
    {
        public string html_url { get; set; }
        public Dictionary<string, GistResponseFileInfo> files { get; set; }
    }

    private class GistResponseFileInfo
    {
        public string filename { get; set; }
        public string raw_url { get; set; }
    }
}