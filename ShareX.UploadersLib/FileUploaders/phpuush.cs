#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright © 2007-2015 ShareX Developers

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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public sealed class phpuush : FileUploader
    {
        public phpuushSettings settings;

        public phpuush(phpuushSettings settings)
        {
            this.settings = settings;
        }

        public string GetAPIKey()
        {
            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("e", this.settings.email);
            args.Add("p", this.settings.password);
            args.Add("z", "poop"); // don't even ask

            string url = this.ConstructURL("api/auth");
            string response = SendRequest(HttpMethod.POST, url, args);
            
            if(!string.IsNullOrEmpty(response) && !(response == "-1"))
            {
                string[] parameters = response.Split(new string[] { "," }, StringSplitOptions.None);

                if(parameters.Length == 4)
                {
                    return parameters[1];
                }
            }
            
            return string.Empty;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);

            Dictionary<string, string> args = new Dictionary<string, string>();
            args.Add("c", this.GetHash(buffer));
            args.Add("k", this.settings.APIKey);
            args.Add("z", "poop"); // don't even ask

            UploadResult result = UploadData(stream, this.ConstructURL("api/up"), fileName, "f", args);
            
            if(result.IsSuccess)
            {
                string[] parameters = result.Response.Split(new string[] { "," }, StringSplitOptions.None);

                if(parameters.Length == 4)
                {
                    if(parameters[1] != "-1")
                    {
                        result.URL = parameters[1];
                    }
                }
            }
            
            return result;
        }

        public string ConstructURL(string action)
        {
            action = URLHelpers.CombineURL(this.settings.url, action);
            action = URLHelpers.FixPrefix(action);
            return action;
        }

        public string GetHash(byte[] buffer)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);
            
            StringBuilder hashStr = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                hashStr.Append(hash[i].ToString("X2"));
            }

            return hashStr.ToString().ToLower();
        }
    }

    public class phpuushSettings
    {
        public string url = string.Empty;
        public string APIKey = string.Empty;
        public string email = string.Empty;
        public string password = string.Empty;
    }
}
