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

using HelpersLib;
using System.Collections.Generic;
using System.ComponentModel;

namespace UploadersLib.TextUploaders
{
    public sealed class Pastebin : TextUploader
    {
        private string APIKey;

        public PastebinSettings Settings { get; private set; }

        public Pastebin(string apiKey)
        {
            APIKey = apiKey;
            Settings = new PastebinSettings();
        }

        public Pastebin(string apiKey, PastebinSettings settings)
        {
            APIKey = apiKey;
            Settings = settings;
        }

        public bool Login()
        {
            if (!string.IsNullOrEmpty(Settings.Username) && !string.IsNullOrEmpty(Settings.Password))
            {
                Dictionary<string, string> loginArgs = new Dictionary<string, string>();

                loginArgs.Add("api_dev_key", APIKey);
                loginArgs.Add("api_user_name", Settings.Username);
                loginArgs.Add("api_user_password", Settings.Password);

                string loginResponse = SendRequest(HttpMethod.POST, "http://pastebin.com/api/api_login.php", loginArgs);

                if (!string.IsNullOrEmpty(loginResponse) && !loginResponse.StartsWith("Bad API request"))
                {
                    Settings.UserKey = loginResponse;
                    return true;
                }
            }

            Settings.UserKey = null;
            Errors.Add("Pastebin login failed.");
            return false;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text) && Settings != null)
            {
                Dictionary<string, string> args = new Dictionary<string, string>();

                args.Add("api_dev_key", APIKey); // which is your unique API Developers Key
                args.Add("api_option", "paste"); // set as 'paste', this will indicate you want to create a new paste
                args.Add("api_paste_code", text); // this is the text that will be written inside your paste

                // Optional args
                args.Add("api_paste_name", Settings.Title); // this will be the name / title of your paste
                args.Add("api_paste_format", Settings.TextFormat); // this will be the syntax highlighting value
                args.Add("api_paste_private", GetPrivacy(Settings.Exposure)); // this makes a paste public or private, public = 0, private = 1
                args.Add("api_paste_expire_date", GetExpiration(Settings.Expiration)); // this sets the expiration date of your paste

                if (!string.IsNullOrEmpty(Settings.UserKey))
                {
                    args.Add("api_user_key", Settings.UserKey); // this paramater is part of the login system
                }

                ur.Response = SendRequest(HttpMethod.POST, "http://pastebin.com/api/api_post.php", args);

                if (!string.IsNullOrEmpty(ur.Response) && !ur.Response.StartsWith("Bad API request") && ur.Response.IsValidUrl())
                {
                    ur.URL = ur.Response;
                }
                else
                {
                    Errors.Add(ur.Response);
                }
            }

            return ur;
        }

        private string GetPrivacy(PastebinPrivacy privacy)
        {
            switch (privacy)
            {
                case PastebinPrivacy.Public:
                    return "0";
                default:
                case PastebinPrivacy.Unlisted:
                    return "1";
                case PastebinPrivacy.Private:
                    return "2";
            }
        }

        private string GetExpiration(PastebinExpiration expiration)
        {
            switch (expiration)
            {
                default:
                case PastebinExpiration.N:
                    return "N";
                case PastebinExpiration.M10:
                    return "10M";
                case PastebinExpiration.H1:
                    return "1H";
                case PastebinExpiration.D1:
                    return "1D";
                case PastebinExpiration.W1:
                    return "1W";
                case PastebinExpiration.W2:
                    return "2W";
                case PastebinExpiration.M1:
                    return "1M";
            }
        }
    }

    public enum PastebinPrivacy
    {
        [Description("Public")]
        Public,
        [Description("Unlisted")]
        Unlisted,
        [Description("Private (Only you can see)")]
        Private
    }

    public enum PastebinExpiration
    {
        [Description("Never")]
        N,
        [Description("10 minutes")]
        M10,
        [Description("1 hour")]
        H1,
        [Description("1 day")]
        D1,
        [Description("1 week")]
        W1,
        [Description("2 weeks")]
        W2,
        [Description("1 month")]
        M1
    }

    public class PastebinSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public PastebinPrivacy Exposure { get; set; }
        public PastebinExpiration Expiration { get; set; }
        public string Title { get; set; }
        public string TextFormat { get; set; }
        public string UserKey { get; set; }

        public PastebinSettings()
        {
            Exposure = PastebinPrivacy.Unlisted;
            Expiration = PastebinExpiration.N;
            TextFormat = "text";
        }
    }
}