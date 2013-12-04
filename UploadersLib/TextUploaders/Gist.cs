#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2013 ShareX Developers

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

namespace UploadersLib.TextUploaders
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net;

    using Newtonsoft.Json;

    using UploadersLib.HelperClasses;

    public sealed class Gist : TextUploader
    {
        private readonly Uri GistUri = new Uri("https://api.github.com/gists");
        private readonly Uri GistAuthorizeUri = new Uri("https://github.com/login/oauth/authorize");
        private readonly Uri GistCompleteUri = new Uri("https://github.com/login/oauth/access_token");
        private readonly Uri GistRedirectUri = new Uri("http://getsharex.com/github/");

        private readonly OAuth2Info oAuthInfos;
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
            this.oAuthInfos = oAuthInfos;
        }

        public string GetAuthorizationURL()
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            queryString["client_id"] = this.oAuthInfos.Client_ID;
            queryString["redirect_uri"] = this.GistRedirectUri.ToString();
            queryString["scope"] = "gist";

            UriBuilder uri = new UriBuilder(this.GistAuthorizeUri);
            uri.Query = queryString.ToString();

            return uri.ToString();
        }

        public bool GetAccessToken(string code)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("client_id", this.oAuthInfos.Client_ID);
            args.Add("client_secret", this.oAuthInfos.Client_Secret);
            args.Add("code", code);

            WebHeaderCollection headers = new WebHeaderCollection();
            headers.Add("Accept", "application/json");
            
            string response = this.SendPostRequest(this.GistCompleteUri.ToString(), args, headers: headers);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    this.oAuthInfos.Token = token;
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

                string Uri = GistUri.ToString();
                if (this.oAuthInfos != null)
                {
                    Uri += "?access_token=" + this.oAuthInfos.Token.access_token;
                }

                string response = SendPostRequestJSON(Uri, argsJson);
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