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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ShareX.HelpersLib
{
    public static class URLHelpers
    {
        public static void OpenURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                TaskEx.Run(() =>
                {
                    try
                    {
                        Process.Start(url);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e, string.Format("OpenURL({0}) failed", url));
                    }
                });
            }
        }

        private static string Encode(string text, string unreservedCharacters)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text)
                {
                    if (unreservedCharacters.Contains(c))
                    {
                        result.Append(c);
                    }
                    else
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());

                        foreach (byte b in bytes)
                        {
                            result.AppendFormat(CultureInfo.InvariantCulture, "%{0:X2}", b);
                        }
                    }
                }
            }

            return result.ToString();
        }

        public static string URLEncode(string text)
        {
            return Encode(text, Helpers.URLCharacters);
        }

        public static string URLPathEncode(string text)
        {
            return Encode(text, Helpers.URLPathCharacters);
        }

        public static string HtmlEncode(string text)
        {
            char[] chars = HttpUtility.HtmlEncode(text).ToCharArray();
            StringBuilder result = new StringBuilder(chars.Length + (int)(chars.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);

                if (value > 127)
                {
                    result.AppendFormat("&#{0};", value);
                }
                else
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        public static string CombineURL(string url1, string url2)
        {
            bool url1Empty = string.IsNullOrEmpty(url1);
            bool url2Empty = string.IsNullOrEmpty(url2);

            if (url1Empty && url2Empty)
            {
                return string.Empty;
            }

            if (url1Empty)
            {
                return url2;
            }

            if (url2Empty)
            {
                return url1;
            }

            if (url1.EndsWith("/"))
            {
                url1 = url1.Substring(0, url1.Length - 1);
            }

            if (url2.StartsWith("/"))
            {
                url2 = url2.Remove(0, 1);
            }

            return url1 + "/" + url2;
        }

        public static string CombineURL(params string[] urls)
        {
            return urls.Aggregate(CombineURL);
        }

        public static bool IsValidURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                return !url.StartsWith("file://") && Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
            }

            return false;
        }

        public static bool IsValidURLRegex(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            // https://gist.github.com/729294
            string pattern =
                "^" +
                // protocol identifier
                "(?:(?:https?|ftp)://)" +
                // user:pass authentication
                "(?:\\S+(?::\\S*)?@)?" +
                "(?:" +
                // IP address exclusion
                // private & local networks
                "(?!10(?:\\.\\d{1,3}){3})" +
                "(?!127(?:\\.\\d{1,3}){3})" +
                "(?!169\\.254(?:\\.\\d{1,3}){2})" +
                "(?!192\\.168(?:\\.\\d{1,3}){2})" +
                "(?!172\\.(?:1[6-9]|2\\d|3[0-1])(?:\\.\\d{1,3}){2})" +
                // IP address dotted notation octets
                // excludes loopback network 0.0.0.0
                // excludes reserved space >= 224.0.0.0
                // excludes network & broacast addresses
                // (first & last IP address of each class)
                "(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])" +
                "(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}" +
                "(?:\\.(?:[1-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))" +
                "|" +
                // host name
                "(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)" +
                // domain name
                "(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*" +
                // TLD identifier
                "(?:\\.(?:[a-z\\u00a1-\\uffff]{2,}))" +
                ")" +
                // port number
                "(?::\\d{2,5})?" +
                // resource path
                "(?:/[^\\s]*)?" +
                "$";

            return Regex.IsMatch(url.Trim(), pattern, RegexOptions.IgnoreCase);
        }

        public static string AddSlash(string url, SlashType slashType)
        {
            return AddSlash(url, slashType, 1);
        }

        public static string AddSlash(string url, SlashType slashType, int count)
        {
            if (slashType == SlashType.Prefix)
            {
                if (url.StartsWith("/"))
                {
                    url = url.Remove(0, 1);
                }

                for (int i = 0; i < count; i++)
                {
                    url = "/" + url;
                }
            }
            else
            {
                if (url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }

                for (int i = 0; i < count; i++)
                {
                    url += "/";
                }
            }

            return url;
        }

        public static string GetFileName(string path, bool checkExtension = false)
        {
            if (path.Contains("/"))
            {
                path = path.Remove(0, path.LastIndexOf('/') + 1);
            }

            if (checkExtension && !Path.HasExtension(path))
            {
                return null;
            }

            return path;
        }

        public static string GetDirectoryPath(string path)
        {
            if (path.Contains("/"))
            {
                path = path.Substring(0, path.LastIndexOf('/'));
            }

            return path;
        }

        public static List<string> GetPaths(string path)
        {
            List<string> result = new List<string>();
            string temp = string.Empty;
            string[] dirs = path.Split('/');
            foreach (string dir in dirs)
            {
                if (!string.IsNullOrEmpty(dir))
                {
                    temp += "/" + dir;
                    result.Add(temp);
                }
            }

            return result;
        }

        private static readonly string[] URLPrefixes = new string[] { "http://", "https://", "ftp://", "ftps://", "file://" };

        public static string FixPrefix(string url)
        {
            if (!HasPrefix(url))
            {
                return "http://" + url;
            }

            return url;
        }

        public static bool HasPrefix(string url)
        {
            return URLPrefixes.Any(x => url.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
        }

        public static string RemovePrefixes(string url)
        {
            foreach (string prefix in URLPrefixes)
            {
                if (url.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    url = url.Remove(0, prefix.Length);
                    break;
                }
            }

            return url;
        }
    }
}