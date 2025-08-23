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
using ShareX.HistoryLib.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShareX.HistoryLib
{
    public static class HistoryHelpers
    {
        public static string OutputStats(List<HistoryItem> historyItems)
        {
            string empty = "(empty)";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Resources.HistoryItemCounts);
            sb.AppendLine(Resources.HistoryStats_Total + " " + historyItems.Count);

            IEnumerable<string> types = historyItems.
                GroupBy(x => x.Type).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("{0}: {1} ({2:N0}%)", x.Key, x.Count(), x.Count() / (float)historyItems.Count * 100));

            sb.AppendLine(string.Join(Environment.NewLine, types));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_YearlyUsages);

            IEnumerable<string> yearlyUsages = historyItems.
                GroupBy(x => x.DateTime.Year).
                OrderByDescending(x => x.Key).
                Select(x => string.Format("{0}: {1} ({2:N0}%)", x.Key, x.Count(), x.Count() / (float)historyItems.Count * 100));

            sb.AppendLine(string.Join(Environment.NewLine, yearlyUsages));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_FileExtensions);

            IEnumerable<string> fileExtensions = historyItems.
                Where(x => !string.IsNullOrEmpty(x.FileName) && !x.FileName.EndsWith(")")).
                Select(x => FileHelpers.GetFileNameExtension(x.FileName)).
                GroupBy(x => string.IsNullOrWhiteSpace(x) ? empty : x).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.AppendLine(string.Join(Environment.NewLine, fileExtensions));

            sb.AppendLine();
            sb.AppendLine(Resources.HistoryStats_Hosts);

            IEnumerable<string> hosts = historyItems.
                GroupBy(x => string.IsNullOrWhiteSpace(x.Host) ? empty : x.Host).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.AppendLine(string.Join(Environment.NewLine, hosts));

            sb.AppendLine();
            sb.AppendLine(Resources.ProcessNames);

            IEnumerable<string> processNames = historyItems.
                GroupBy(x => string.IsNullOrWhiteSpace(x.TagsProcessName) ? empty : x.TagsProcessName).
                OrderByDescending(x => x.Count()).
                Select(x => string.Format("[{0}] {1}", x.Count(), x.Key));

            sb.Append(string.Join(Environment.NewLine, processNames));

            return sb.ToString();
        }
    }
}