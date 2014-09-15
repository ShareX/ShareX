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
                args.Add("api_paste_private", Settings.Privacy); // this makes a paste public or private, public = 0, private = 1
                args.Add("api_paste_expire_date", Settings.ExpireTime); // this sets the expiration date of your paste

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
    }

    public class PastebinSettings
    {
        [Description("This is the username of the user you want to login")]
        public string Username { get; set; }

        [PasswordPropertyText(true), Description("This is the password of the user you want to login")]
        public string Password { get; set; }

        [Description("This makes a paste public or private\r\n0 = Public, 1 = Unlisted, 2 = Private (You have to be logged into your account to access the paste)"),
        DefaultValue("1")]
        public string Privacy { get; set; }

        [Description("This sets the expiration date of your paste\r\nN = Never, 10M = 10 Minutes, 1H = 1 Hour, 1D = 1 Day, 1W = 1 Week, 2W = 2 Weeks, 1M = 1 Month"), DefaultValue("N")]
        public string ExpireTime { get; set; }

        [Description("This will be the name / title of your paste")]
        public string Title { get; set; }

        [Description("This will be the syntax highlighting value\r\nExample: c = C, java = Java, objc = Objective C, cpp = c++, csharp = C#, php = PHP, vb = VisualBasic, python = Python, perl = Perl, ruby = Ruby, javascript = JavaScript, vbnet = VB.NET")]
        public string TextFormat { get; set; }

        [Browsable(false)]
        public string UserKey { get; set; }

        public PastebinSettings()
        {
            Privacy = "1";
            ExpireTime = "N";
        }
    }
}