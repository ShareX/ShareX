#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2008-2014 ShareX Developers

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
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace UploadersLib.OtherServices
{
    public class Lyricsfly : Uploader
    {
        public string UserID { get; set; }

        public string AppID { get; set; }

        public Lyricsfly(string userID, string appID)
        {
            UserID = userID;
            AppID = appID;
        }

        /// <summary>
        /// To search by artist and title combination
        /// </summary>
        public Lyrics SearchLyrics(string artist, string title)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("i", UserID + AppID);
            arguments.Add("a", FixText(artist));
            arguments.Add("t", FixText(title));
            string response = SendRequest(HttpMethod.GET, "http://lyricsfly.com/api/api.php", arguments);
            return ParseResponse(response);
        }

        /// <summary>
        /// To search by lyrics text string
        /// </summary>
        public Lyrics SearchLyrics(string text)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("i", UserID + AppID);
            arguments.Add("l", FixText(text));
            string response = SendRequest(HttpMethod.GET, "http://lyricsfly.com/api/txt-api.php", arguments);
            return ParseResponse(response);
        }

        /// <summary>
        /// Function to edit lyrics (incomplete)
        /// </summary>
        /// <param name="lyrics"></param>
        public void EditLyrics(Lyrics lyrics)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("cs", lyrics.Checksum);
            arguments.Add("id", lyrics.SongID);
            string response = SendRequest(HttpMethod.GET, "http://lyricsfly.com/search/correction.php?", arguments);
            ParseResponse(response);
        }

        private Lyrics ParseResponse(string response)
        {
            if (!string.IsNullOrEmpty(response))
            {
                XDocument xdoc = XDocument.Parse(response);
                XElement xele = xdoc.Element("start");

                if (xele != null)
                {
                    string status = xele.GetElementValue("status");
                    if (!string.IsNullOrEmpty(status))
                    {
                        switch (status)
                        {
                            case "200": // OK - Results are returned. All parameters checked ok.
                            case "300": // TESTING LIMITED - Temporary access. Limited content. All parameters checked ok.
                                xele = xele.Element("sg");
                                if (xele != null)
                                {
                                    Lyrics lyric = new Lyrics();
                                    lyric.Checksum = xele.GetElementValue("cs");
                                    lyric.SongID = xele.GetElementValue("id");
                                    lyric.ArtistName = xele.GetElementValue("ar");
                                    lyric.Title = xele.GetElementValue("tt");
                                    lyric.AlbumName = xele.GetElementValue("al");
                                    lyric.Text = xele.GetElementValue("tx").Replace("[br]", string.Empty);

                                    return lyric;
                                }
                                break;
                            case "204": // NO CONTENT
                                Errors.Add("Parameter query returned no results. All parameters checked ok.");
                                break;
                            case "400": // MISSING KEY
                                Errors.Add("Parameter “i” missing. Authorization failed.");
                                break;
                            case "401": // UNAUTHORIZED
                                Errors.Add("Parameter “i” invalid. Authorization failed.");
                                break;
                            case "402": // LIMITED TIME
                                Errors.Add("Query request too soon. Limit query requests. Time of delay is shown in <delay> tag in milliseconds.");
                                break;
                            case "406": // QUERY TOO SHORT
                                Errors.Add("Query request string is too short. All other parameters checked ok.");
                                break;
                            default:
                                Errors.Add("Unknown status.");
                                break;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Because our database varies with many html format encodings including international characters,
        /// we recommend that you replace all quotes, ampersands and all other special and international characters with "%".
        /// Simply put; if the character is not [A-Z a-z 0-9] or space, just substitute "%" for it to get most out of your results.
        /// </summary>
        private string FixText(string text)
        {
            char[] chars = text.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (!Char.IsLetterOrDigit(chars[i]) && chars[i] != ' ')
                {
                    chars[i] = '%';
                }
            }

            return new string(chars);
        }
    }

    public class Lyrics
    {
        /// <summary>
        /// cs - checksum (for original URL link back construction)
        /// </summary>
        public string Checksum { get; set; }

        /// <summary>
        /// id - song ID in the database (for original URL link back construction)
        /// </summary>
        public string SongID { get; set; }

        /// <summary>
        /// ar - artist name
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// tt - title of the song
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// al - album name
        /// </summary>
        public string AlbumName { get; set; }

        /// <summary>
        /// tx - lyrics text separated by [br] for line break
        /// </summary>
        public string Text { get; set; }
    }
}