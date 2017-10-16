#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2017 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class MinusFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Minus;

        public override Icon ServiceIcon => Resources.Minus;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.MinusConfig != null && config.MinusConfig.MinusUser != null;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Minus(config.MinusConfig, config.MinusOAuth2Info);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpMinus;
    }

    public class Minus : FileUploader
    {
        private const string URL_HOST = "https://minus.com";
        private const string URL_OAUTH_TOKEN = URL_HOST + "/oauth/token";
        private const string URL_API = URL_HOST + "/api/v2";

        public MinusOptions Config { get; set; }
        public OAuth2Info AuthInfo { get; set; }

        public Minus(MinusOptions config, OAuth2Info auth)
        {
            Config = config;
            AuthInfo = auth;
        }

        public bool GetAccessToken()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("grant_type", "password");
            args.Add("client_id", AuthInfo.Client_ID);
            args.Add("client_secret", AuthInfo.Client_Secret);
            args.Add("scope", "read_public read_all upload_new modify_all");
            args.Add("username", Config.Username);
            args.Add("password", Config.Password);

            string response = SendRequestMultiPart(URL_OAUTH_TOKEN, args);

            if (!string.IsNullOrEmpty(response))
            {
                OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                if (token != null && !string.IsNullOrEmpty(token.access_token))
                {
                    token.UpdateExpireDate();
                    AuthInfo.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool RefreshAccessToken()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo) && !string.IsNullOrEmpty(AuthInfo.Token.refresh_token))
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("grant_type", "refresh_token");
                args.Add("client_id", AuthInfo.Client_ID);
                args.Add("client_secret", AuthInfo.Client_Secret);
                args.Add("scope", AuthInfo.Token.scope);
                args.Add("refresh_token", AuthInfo.Token.refresh_token);

                string response = SendRequestMultiPart(URL_OAUTH_TOKEN, args);

                if (!string.IsNullOrEmpty(response))
                {
                    OAuth2Token token = JsonConvert.DeserializeObject<OAuth2Token>(response);

                    if (token != null && !string.IsNullOrEmpty(token.access_token))
                    {
                        token.UpdateExpireDate();
                        AuthInfo.Token = token;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool CheckAuthorization()
        {
            if (OAuth2Info.CheckOAuth(AuthInfo))
            {
                if (AuthInfo.Token.IsExpired && !RefreshAccessToken())
                {
                    Errors.Add("Refresh access token failed.");
                    return false;
                }
            }
            else
            {
                Errors.Add("Login is required.");
                return false;
            }

            return true;
        }

        private string GetActiveUserFolderURL()
        {
            MinusUser user = Config.MinusUser ?? (Config.MinusUser = GetActiveUser());
            return URL_API + "/users/" + user.slug + "/folders?bearer_token=" + AuthInfo.Token.access_token;
        }

        public MinusUser GetActiveUser()
        {
            string url = URL_API + "/activeuser?bearer_token=" + AuthInfo.Token.access_token;
            string response = SendRequest(HttpMethod.GET, url);
            return JsonConvert.DeserializeObject<MinusUser>(response);
        }

        private MinusFolderListResponse GetUserFolderList()
        {
            string response = SendRequest(HttpMethod.GET, GetActiveUserFolderURL());
            return JsonConvert.DeserializeObject<MinusFolderListResponse>(response);
        }

        public List<MinusFolder> ReadFolderList()
        {
            MinusFolderListResponse mflr = GetUserFolderList();

            if (mflr.results != null && mflr.results.Length > 0)
            {
                Config.FolderList.Clear();
                foreach (MinusFolder minusFolder in mflr.results)
                {
                    Config.FolderList.Add(minusFolder);
                }
            }
            else
            {
                MinusFolder mf = CreateFolder("ShareX", true);
                if (mf != null)
                {
                    Config.FolderList.Add(mf);
                }
            }

            Config.FolderID = 0;

            return Config.FolderList;
        }

        public MinusFolder CreateFolder(string name, bool is_public)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("name", name);
            args.Add("is_public", is_public.ToString().ToLower());

            MinusFolder dir;

            string response = SendRequestMultiPart(GetActiveUserFolderURL(), args);
            if (!string.IsNullOrEmpty(response))
            {
                dir = JsonConvert.DeserializeObject<MinusFolder>(response);
                if (dir != null && !string.IsNullOrEmpty(dir.id))
                {
                    Config.FolderList.Add(dir);
                    return dir;
                }
            }

            return null;
        }

        public bool DeleteFolder(int index)
        {
            if (index >= 0 && index < Config.FolderList.Count)
            {
                MinusFolder folder = Config.FolderList[index];
                string url = string.Format("{0}/folders/{1}?bearer_token={2}", URL_API, folder.id, AuthInfo.Token.access_token);

                try
                {
                    string response = SendRequest(HttpMethod.DELETE, url);
                }
                catch
                {
                    return false;
                }

                Config.FolderList.RemoveAt(index);
                return true;
            }

            return false;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            if (!CheckAuthorization())
            {
                return null;
            }

            string url = string.Format("{0}/folders/{1}/files?bearer_token={2}", URL_API, Config.GetActiveFolder().id, AuthInfo.Token.access_token);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("caption", fileName);
            args.Add("filename", fileName);

            UploadResult result = SendRequestFile(url, stream, fileName, "file", args);

            if (result.IsSuccess)
            {
                MinusFile minusFile = JsonConvert.DeserializeObject<MinusFile>(result.Response);

                if (minusFile != null)
                {
                    string ext = Path.GetExtension(minusFile.name);

                    switch (Config.LinkType)
                    {
                        case MinusLinkType.Direct:
                            result.URL = string.Format("http://i.minus.com/i{0}{1}", minusFile.id, ext);
                            break;
                        case MinusLinkType.Page:
                            result.URL = string.Format("http://minus.com/l{0}", minusFile.id);
                            break;
                        case MinusLinkType.Raw:
                            result.URL = minusFile.url_rawfile;
                            break;
                    }

                    result.ShortenedURL = string.Format("http://i.min.us/i{0}{1}", minusFile.id, ext);
                    result.ThumbnailURL = string.Format("http://i.minus.com/j{0}_xs{1}", minusFile.id, ext);
                }
            }

            return result;
        }
    }

    public enum MinusLinkType // Localized
    {
        Direct,
        Page,
        Raw
    }

    public abstract class MinusListResponse
    {
        public int page { get; set; }
        public string next { get; set; }
        public int per_page { get; set; }
        public int total { get; set; }
        public int pages { get; set; }
        public string previous { get; set; }
    }

    public class MinusFolderListResponse : MinusListResponse
    {
        public MinusFolder[] results { get; set; }
    }

    public class MinusFileListResponse : MinusListResponse
    {
        public MinusFile[] results { get; set; }
    }

    public class MinusOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public MinusUser MinusUser { get; set; }
        public List<MinusFolder> FolderList { get; set; }
        public int FolderID { get; set; }
        public MinusLinkType LinkType { get; set; }

        public MinusOptions()
        {
            FolderList = new List<MinusFolder>();
            LinkType = MinusLinkType.Direct;
        }

        public MinusFolder GetActiveFolder()
        {
            return FolderList.ReturnIfValidIndex(FolderID);
        }
    }

    public class MinusUser
    {
        public string username { get; set; }
        public string display_name { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string slug { get; set; }
        public string fb_profile_link { get; set; }
        public string fb_username { get; set; }
        public string twitter_screen_name { get; set; }
        public int visits { get; set; }
        public int karma { get; set; }
        public int shared { get; set; }
        public string folders { get; set; }
        public string url { get; set; }
        public string avatar { get; set; }
        public long storage_used { get; set; }
        public long storage_quota { get; set; }

        public override string ToString()
        {
            return username;
        }
    }

    public class MinusFolder
    {
        public string id { get; set; }
        public string thumbnail_url { get; set; }
        public string name { get; set; }
        public bool is_public { get; set; }
        public int view_count { get; set; }
        public string creator { get; set; }
        public int file_count { get; set; }
        public DateTime date_last_updated { get; set; }
        public string files { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class MinusFile
    {
        public string id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string caption { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int filesize { get; set; }
        public string mimetype { get; set; }
        public string folder { get; set; }
        public string url { get; set; }
        public DateTime uploaded { get; set; }
        public string url_rawfile { get; set; }
        public string url_thumbnail { get; set; }

        public override string ToString()
        {
            return url_rawfile;
        }
    }
}