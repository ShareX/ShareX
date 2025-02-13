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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShareX.HistoryLib
{
    public class HistoryFilter
    {
        public string Filename { get; set; }
        public string URL { get; set; }
        public bool FilterDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool FilterType { get; set; }
        public string Type { get; set; }
        public bool FilterHost { get; set; }
        public string Host { get; set; }

        public int MaxItemCount { get; set; }
        public bool SearchInTags { get; set; } = true;

        public HistoryFilter()
        {
        }

        public IEnumerable<HistoryItem> ApplyFilter(IEnumerable<HistoryItem> historyItems)
        {
            if (FilterType && !string.IsNullOrEmpty(Type))
            {
                historyItems = historyItems.Where(x => !string.IsNullOrEmpty(x.Type) && x.Type.Equals(Type, StringComparison.InvariantCultureIgnoreCase));
            }

            if (FilterHost && !string.IsNullOrEmpty(Host))
            {
                historyItems = historyItems.Where(x => !string.IsNullOrEmpty(x.Host) && x.Host.Contains(Host, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!string.IsNullOrEmpty(Filename))
            {
                string pattern = Regex.Escape(Filename).Replace("\\?", ".").Replace("\\*", ".*");
                Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                historyItems = historyItems.Where(x => (x.FileName != null && regex.IsMatch(x.FileName)) ||
                    (SearchInTags && x.Tags != null && x.Tags.Any(tag => regex.IsMatch(tag.Value))));
            }

            if (!string.IsNullOrEmpty(URL))
            {
                historyItems = historyItems.Where(x => x.URL != null && x.URL.Contains(URL, StringComparison.InvariantCultureIgnoreCase));
            }

            if (FilterDate)
            {
                historyItems = historyItems.Where(x => x.DateTime.Date >= FromDate && x.DateTime.Date <= ToDate);
            }

            if (MaxItemCount > 0)
            {
                historyItems = historyItems.Take(MaxItemCount);
            }

            return historyItems;
        }
    }
}