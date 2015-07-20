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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShareX.UploadersLib.URLShorteners
{
	public sealed class LnkUURLShortener : URLShortener
	{
		private const string API_ENDPOINT = "http://lnku.co/api/go/";
		public string API_KEY { get; set; }

		public override UploadResult ShortenURL(string url)
		{
			UploadResult result = new UploadResult { URL = url };

			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add("apikey", API_KEY);
			args.Add("action" , "shorten");
			args.Add("fullurl", url);

			string response = SendRequest(HttpMethod.GET, API_ENDPOINT, args);

			if (!string.IsNullOrEmpty(response))
			{
				LnkUURLShortenerResponse jsonResponse = JsonConvert.DeserializeObject<LnkUURLShortenerResponse>(response);

				if (jsonResponse != null)
				{
					result.ShortenedURL = jsonResponse.shorturl;
				}
			}

			return result;
		}
	}

	public class LnkUURLShortenerResponse
	{
		public string shortcode { get; set; }
		public string site { get; set; }
		public string shorturl { get; set; }
		public string fullurl { get; set; }
		public string title { get; set; }
	}
}
