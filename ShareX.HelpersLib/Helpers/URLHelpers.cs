#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ShareX.HelpersLib
{
    public static class URLHelpers
    {
        public const string URLCharacters = Helpers.Alphanumeric + "-._~"; // 45 46 95 126
        public const string URLPathCharacters = URLCharacters + "/"; // 47
        public const string ValidURLCharacters = URLPathCharacters + ":?#[]@!$&'()*+,;= ";

        private static readonly string[] URLPrefixes = new string[] { "http://", "https://", "ftp://", "ftps://", "file://", "//" };
        private static readonly char[] BidiControlCharacters = new char[] { '\u200E', '\u200F', '\u202A', '\u202B', '\u202C', '\u202D', '\u202E' };

        public static void OpenURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Task.Run(() =>
                {
                    try
                    {
                        using (Process process = new Process())
                        {
                            ProcessStartInfo psi = new ProcessStartInfo();

                            if (!string.IsNullOrEmpty(HelpersOptions.BrowserPath))
                            {
                                psi.FileName = HelpersOptions.BrowserPath;
                                psi.Arguments = url;
                            }
                            else
                            {
                                psi.FileName = url;
                            }

                            process.StartInfo = psi;
                            process.Start();
                        }

                        DebugHelper.WriteLine("URL opened: " + url);
                    }
                    catch (Exception e)
                    {
                        DebugHelper.WriteException(e, $"OpenURL({url}) failed");
                    }
                });
            }
        }

        public static string URLEncode(string text, bool isPath = false, bool ignoreEmoji = false)
        {
            if (ignoreEmoji)
            {
                return URLEncodeIgnoreEmoji(text, isPath);
            }

            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(text))
            {
                string unreservedCharacters;

                if (isPath)
                {
                    unreservedCharacters = URLPathCharacters;
                }
                else
                {
                    unreservedCharacters = URLCharacters;
                }

                foreach (char c in Encoding.UTF8.GetBytes(text))
                {
                    if (unreservedCharacters.IndexOf(c) != -1)
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "%{0:X2}", (int)c);
                    }
                }
            }

            return sb.ToString();
        }

        public static string URLEncodeIgnoreEmoji(string text, bool isPath = false)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                string remainingText = text.Substring(i);

                int emojiLength = Emoji.SearchEmoji(remainingText);

                if (emojiLength > 0)
                {
                    sb.Append(remainingText.Substring(0, emojiLength));
                    i += emojiLength - 1;
                }
                else
                {
                    sb.Append(URLEncode(remainingText.Substring(0, 1), isPath));
                }
            }

            return sb.ToString();
        }

        public static string RemoveBidiControlCharacters(string text)
        {
            return new string(text.Where(c => !BidiControlCharacters.Contains(c)).ToArray());
        }

        public static string ReplaceReservedCharacters(string text, string replace)
        {
            StringBuilder sb = new StringBuilder();

            string last = null;

            foreach (char c in text)
            {
                if (URLCharacters.Contains(c))
                {
                    last = c.ToString();
                }
                else if (last != replace)
                {
                    last = replace;
                }
                else
                {
                    continue;
                }

                sb.Append(last);
            }

            return sb.ToString();
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

        public static string JSONEncode(string text)
        {
            text = JsonConvert.ToString(text);
            return text.Substring(1, text.Length - 2);
        }

        public static string XMLEncode(string text)
        {
            return SecurityElement.Escape(text);
        }

        public static string URLDecode(string url, int count = 1)
        {
            string temp = null;

            for (int i = 0; i < count && url != temp; i++)
            {
                temp = url;
                url = HttpUtility.UrlDecode(url);
            }

            return url;
        }

        public static string CombineURL(string url1, string url2)
        {
            bool url1Empty = string.IsNullOrEmpty(url1);
            bool url2Empty = string.IsNullOrEmpty(url2);

            if (url1Empty && url2Empty)
            {
                return "";
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

        public static bool IsValidURL(string url, bool useRegex = true)
        {
            if (string.IsNullOrEmpty(url)) return false;

            url = url.Trim();

            if (useRegex)
            {
                // Source: https://gist.github.com/729294
                string pattern =
                    "^" +
                    // protocol identifier
                    "(?:(?:https?|ftp)://)" +
                    // user:pass authentication
                    "(?:\\S+(?::\\S*)?@)?" +
                    "(?:" +
                    // IP address exclusion
                    // private & local networks
                    "(?!(?:10|127)(?:\\.\\d{1,3}){3})" +
                    "(?!(?:169\\.254|192\\.168)(?:\\.\\d{1,3}){2})" +
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
                    "(?:(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)" +
                    // domain name
                    "(?:\\.(?:[a-z\\u00a1-\\uffff0-9]-*)*[a-z\\u00a1-\\uffff0-9]+)*" +
                    // TLD identifier
                    "(?:\\.(?:[a-z\\u00a1-\\uffff]{2,}))" +
                    // TLD may end with dot
                    "\\.?" +
                    ")" +
                    // port number
                    "(?::\\d{2,5})?" +
                    // resource path
                    "(?:[/?#]\\S*)?" +
                    "$";

                return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
            }

            return !url.StartsWith("file://") && Uri.IsWellFormedUriString(url, UriKind.Absolute);
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

        public static string GetFileName(string path)
        {
            if (path.Contains('/'))
            {
                path = path.Substring(path.LastIndexOf('/') + 1);
            }

            if (path.Contains('?'))
            {
                path = path.Remove(path.IndexOf('?'));
            }

            if (path.Contains('#'))
            {
                path = path.Remove(path.IndexOf('#'));
            }

            return path;
        }

        public static bool IsFileURL(string url)
        {
            int index = url.LastIndexOf('/');

            if (index < 0)
            {
                return false;
            }

            string path = url.Substring(index + 1);

            return !string.IsNullOrEmpty(path) && path.Contains(".");
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
            List<string> paths = new List<string>();

            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '/')
                {
                    string currentPath = path.Remove(i);

                    if (!string.IsNullOrEmpty(currentPath))
                    {
                        paths.Add(currentPath);
                    }
                }
                else if (i == path.Length - 1)
                {
                    paths.Add(path);
                }
            }

            return paths;
        }

        public static bool HasPrefix(string url)
        {
            return URLPrefixes.Any(x => url.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public static string GetPrefix(string url)
        {
            return URLPrefixes.FirstOrDefault(x => url.StartsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        public static string FixPrefix(string url, string prefix = "https://")
        {
            if (!string.IsNullOrEmpty(url) && !HasPrefix(url))
            {
                return prefix + url;
            }

            return url;
        }

        public static string ForcePrefix(string url, string prefix = "https://")
        {
            if (!string.IsNullOrEmpty(url))
            {
                url = prefix + RemovePrefixes(url);
            }

            return url;
        }

        public static string RemovePrefixes(string url)
        {
            foreach (string prefix in URLPrefixes)
            {
                if (url.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    url = url.Remove(0, prefix.Length);
                    break;
                }
            }

            return url;
        }

        public static string GetHostName(string url)
        {
            if (!string.IsNullOrEmpty(url) && Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                string host = uri.Host;

                if (!string.IsNullOrEmpty(host))
                {
                    if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                    {
                        host = host.Substring(4);
                    }

                    return host;
                }
            }

            return url;
        }

        public static string CreateQueryString(Dictionary<string, string> args, bool customEncoding = false)
        {
            if (args != null && args.Count > 0)
            {
                List<string> pairs = new List<string>();

                foreach (KeyValuePair<string, string> arg in args)
                {
                    string pair;

                    if (string.IsNullOrEmpty(arg.Value))
                    {
                        pair = arg.Key;
                    }
                    else
                    {
                        string value;

                        if (customEncoding)
                        {
                            value = URLEncode(arg.Value);
                        }
                        else
                        {
                            value = HttpUtility.UrlEncode(arg.Value);
                        }

                        pair = arg.Key + "=" + value;
                    }

                    pairs.Add(pair);
                }

                return string.Join("&", pairs);
            }

            return "";
        }

        public static string CreateQueryString(string url, Dictionary<string, string> args, bool customEncoding = false)
        {
            string query = CreateQueryString(args, customEncoding);

            if (!string.IsNullOrEmpty(query))
            {
                if (url.Contains("?"))
                {
                    return url + "&" + query;
                }
                else
                {
                    return url + "?" + query;
                }
            }

            return url;
        }

        public static string RemoveQueryString(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                int index = url.IndexOf("?");

                if (index > -1)
                {
                    return url.Remove(index);
                }
            }

            return url;
        }

        public static NameValueCollection ParseQueryString(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                int index = url.IndexOf("?");

                if (index > -1 && index + 1 < url.Length)
                {
                    string query = url.Substring(index + 1);
                    return HttpUtility.ParseQueryString(query);
                }
            }

            return null;
        }

        public static string BuildUri(string root, string path, string query = null)
        {
            UriBuilder builder = new UriBuilder(root);
            builder.Path = path;
            builder.Query = query;
            return builder.Uri.AbsoluteUri;
        }
    }
}