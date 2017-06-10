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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;

namespace ShareX
{
    public class NewsManager
    {
        public List<NewsItem> NewsItems { get; private set; } = new List<NewsItem>();
        public bool IsUnread { get; private set; }

        public void UpdateNews()
        {
            try
            {
                NewsItems = GetNews();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        private List<NewsItem> GetNews()
        {
            using (WebClient wc = new WebClient())
            {
                wc.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                wc.Headers.Add(HttpRequestHeader.UserAgent, ShareXResources.UserAgent);
                wc.Proxy = HelpersOptions.CurrentProxy.GetWebProxy();

                string url = URLHelpers.CombineURL(Links.URL_WEBSITE, "news.json");
                string response = wc.DownloadString(url);

                if (!string.IsNullOrEmpty(response))
                {
                    return JsonConvert.DeserializeObject<List<NewsItem>>(response);
                }
            }

            return null;
        }

        private void ExportNews(List<NewsItem> newsItems)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(newsItems, settings);
            File.WriteAllText("news.json", json);
        }

        private void ExportExample()
        {
            List<NewsItem> newsItems = new List<NewsItem>()
            {
                new NewsItem() { DateTime = new DateTime(2017, 06, 20), Text = "ShareX has been released on Windows Store!" },
                new NewsItem() { DateTime = new DateTime(2017, 06, 20), Text = "ShareX 11.8.0 has been released.", URL = "https://getsharex.com/changelog" },
                new NewsItem() { DateTime = new DateTime(2017, 04, 14), Text = "We now have a Discord server!", URL = "https://discord.gg/E4R3Qa9" },
                new NewsItem() { DateTime = new DateTime(2016, 6, 10), Text = "We now have a Patreon page!", URL = "https://www.patreon.com/ShareX" },
                new NewsItem() { DateTime = new DateTime(2015, 10, 2), Text = "ShareX has been released on Steam!", URL = "http://store.steampowered.com/app/400040/" }
            };

            ExportNews(newsItems);
        }
    }
}