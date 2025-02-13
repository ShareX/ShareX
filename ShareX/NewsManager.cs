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
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX
{
    public class NewsManager
    {
        public List<NewsItem> NewsItems { get; private set; } = new List<NewsItem>();
        public DateTime LastReadDate { get; set; }
        public bool IsUnread => UnreadCount > 0;
        public int UnreadCount => NewsItems != null ? NewsItems.Count(x => x.IsUnread) : 0;

        public async Task UpdateNews()
        {
            try
            {
                NewsItems = await GetNews();
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }
        }

        public void UpdateUnread()
        {
            if (NewsItems != null)
            {
                foreach (NewsItem newsItem in NewsItems)
                {
                    newsItem.IsUnread = newsItem.DateTime > LastReadDate;
                }
            }
        }

        private async Task<List<NewsItem>> GetNews()
        {
            string url = URLHelpers.CombineURL(Links.Website, "news.json");
            string response = await WebHelpers.DownloadStringAsync(url);

            if (!string.IsNullOrEmpty(response))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };

                return JsonConvert.DeserializeObject<List<NewsItem>>(response, settings);
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
    }
}