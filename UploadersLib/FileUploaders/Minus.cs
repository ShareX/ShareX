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

using HelpersLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using UploadersLib.HelperClasses;

namespace UploadersLib.FileUploaders
{
    public class Minus : FileUploader, IOAuth
    {
        private const string URL_HOST = "https://minus.com";
        private const string API_VERSION = "2";
        private const string URL_API = URL_HOST + "/api/v" + API_VERSION;

        public MinusOptions Config { get; set; }
        public OAuthInfo AuthInfo { get; set; }

        public Minus(MinusOptions config, OAuthInfo auth)
        {
            Config = config;
            AuthInfo = auth;
        }

        public string GetAuthorizationURL()
        {
            return string.Format("{0}/oauth/token?grant_type=password&client_id={1}&client_secret={2}&scope=upload_new&username={3}&password={4}",
                URL_HOST, AuthInfo.ConsumerKey, AuthInfo.ConsumerSecret, Config.Username, Config.Password);
        }

        public bool GetAccessToken(string verificationCode = null)
        {
            Config.Tokens.Clear();

            foreach (MinusScope scope in Enum.GetValues(typeof(MinusScope)))
            {
                string url = string.Format("{0}/oauth/token?grant_type=password&client_id={1}&client_secret={2}&scope={3}&username={4}&password={5}",
                    URL_HOST, AuthInfo.ConsumerKey, AuthInfo.ConsumerSecret, scope.ToString(), Config.Username, Config.Password);

                string response = SendGetRequest(url);
                MinusAuthToken mat = JsonConvert.DeserializeObject<MinusAuthToken>(response);

                if (mat != null)
                {
                    Config.Tokens.Add(mat);
                }
            }

            return true;
        }

        public void RefreshAccessTokens()
        {
            List<MinusAuthToken> newTokens = new List<MinusAuthToken>();

            foreach (MinusScope scope in Enum.GetValues(typeof(MinusScope)))
            {
                string url = string.Format("{0}/oauth/token?grant_type=refresh_token&client_id={1}&client_secret={2}&scope={3}&refresh_token={4}",
                    URL_HOST, AuthInfo.ConsumerKey, AuthInfo.ConsumerSecret, scope.ToString(), Config.GetToken(scope).refresh_token);

                string response = SendGetRequest(url);
                MinusAuthToken mat = JsonConvert.DeserializeObject<MinusAuthToken>(response);

                if (mat != null)
                {
                    newTokens.Add(mat);
                }
            }

            Config.Tokens = newTokens;
        }

        private string GetFolderLinkFromID(string id, MinusScope scope)
        {
            return URL_API + "/folders/" + id + "/files?bearer_token=" + Config.GetToken(scope).access_token;
        }

        private string GetActiveUserFolderURL(MinusScope scope)
        {
            MinusUser user = Config.MinusUser ?? (Config.MinusUser = GetActiveUser(scope));
            return URL_API + "/users/" + user.slug + "/folders?bearer_token=" + Config.GetToken(scope).access_token;
        }

        public MinusUser GetActiveUser(MinusScope scope)
        {
            string url = URL_API + "/activeuser?bearer_token=" + Config.GetToken(scope).access_token;
            string response = SendGetRequest(url);
            return JsonConvert.DeserializeObject<MinusUser>(response);
        }

        public MinusUser GetUser(string slug)
        {
            string url = URL_API + "/users/" + slug;
            string response = SendGetRequest(url);
            return JsonConvert.DeserializeObject<MinusUser>(response);
        }

        public MinusFileListResponse GetFiles(string folderId, MinusScope scope)
        {
            string url = GetFolderLinkFromID(folderId, scope);
            string response = SendGetRequest(url);
            return JsonConvert.DeserializeObject<MinusFileListResponse>(response);
        }

        private MinusFolderListResponse GetUserFolderList(MinusScope scope)
        {
            string response = SendGetRequest(GetActiveUserFolderURL(scope));
            return JsonConvert.DeserializeObject<MinusFolderListResponse>(response);
        }

        public List<MinusFolder> ReadFolderList(MinusScope scope)
        {
            MinusFolderListResponse mflr = GetUserFolderList(scope);

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
                MinusFolder mf = CreateFolder(Application.ProductName, true);
                if (mf != null)
                {
                    Config.FolderList.Add(mf);
                }
            }

            Config.FolderID = 0;

            return Config.FolderList;
        }

        /// <summary>
        /// Creates a new folder in your Minus.com account
        /// </summary>
        /// <param name="name">folder name</param>
        /// <param name="is_public">true for Public access or false for Private access</param>
        /// <returns>Returns the Minus folder object created</returns>
        public MinusFolder CreateFolder(string name, bool is_public)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("name", name);
            args.Add("is_public", is_public.ToString().ToLower());

            MinusFolder dir;

            string response = SendPostRequestURLEncoded(GetActiveUserFolderURL(MinusScope.upload_new), args);
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

        public bool DeleteFolder(int id)
        {
            if (id < Config.FolderList.Count)
            {
                MinusFolder mf = Config.FolderList[id];
                string url = GetFolderLinkFromID(mf.id, MinusScope.modify_all);
                string resp = SendDeleteRequest(url);
                Config.FolderList.RemoveAt(id);
                return !string.IsNullOrEmpty(resp);
            }
            return false;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            string url = GetFolderLinkFromID(Config.GetActiveFolder().id, MinusScope.upload_new);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("caption", fileName);
            args.Add("filename", fileName);

            UploadResult result = UploadData(stream, url, fileName, "file", args);

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

    public enum MinusScope
    {
        [Description("Read public files")]
        read_public,
        [Description("Read all folders and files")]
        read_all,
        [Description("Upload new files and folders")]
        upload_new,
        [Description("Delete/Modify all existing files and folders")]
        modify_all,
        [Description("Modify user preferences")]
        modify_user
    }

    public enum MinusLinkType
    {
        Direct,
        Page,
        Raw
    }

    public class MinusAuthToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expire_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
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
        public List<MinusAuthToken> Tokens { get; set; }
        public MinusUser MinusUser { get; set; }
        public List<MinusFolder> FolderList { get; set; }
        public int FolderID { get; set; }
        public MinusLinkType LinkType { get; set; }

        public MinusOptions()
        {
            Tokens = new List<MinusAuthToken>();
            FolderList = new List<MinusFolder>();
            LinkType = MinusLinkType.Direct;
        }

        public MinusAuthToken GetToken(MinusScope scope)
        {
            return Tokens.FirstOrDefault(mat => scope.ToString() == mat.scope);
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