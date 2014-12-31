#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2015 ShareX Developers

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

using System.Collections.Generic;

namespace ShareX.UploadersLib.TextUploaders
{
    public sealed class Pastee : TextUploader
    {
        public string Lexer { get; set; }
        public int TimeToLive { get; set; } // Days
        public string Key { get; set; }

        public Pastee()
        {
            Lexer = "text";
            TimeToLive = 30;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            UploadResult ur = new UploadResult();

            if (!string.IsNullOrEmpty(text))
            {
                Dictionary<string, string> arguments = new Dictionary<string, string>();
                arguments.Add("lexer", Lexer);
                arguments.Add("content", text);
                arguments.Add("ttl", (TimeToLive * 86400).ToString());

                if (!string.IsNullOrEmpty(Key))
                {
                    arguments.Add("encrypt", "checked");
                    arguments.Add("key", Key);
                }

                ur.URL = SendRequest(HttpMethod.POST, "https://pastee.org/submit", arguments, responseType: ResponseType.RedirectionURL);
            }

            return ur;
        }
    }
}