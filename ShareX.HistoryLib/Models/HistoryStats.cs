#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

#nullable enable

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HistoryLib;

public enum HistoryStatsKind
{
    Image,
    Recording,
    Other
}

public enum HistoryStatsFilter
{
    All,
    Images,
    Recordings
}

public struct HistoryStatsTally
{
    public int Images;
    public int Recordings;
    public int Other;

    public readonly int Total => Images + Recordings + Other;

    public void Add(HistoryStatsKind kind)
    {
        switch (kind)
        {
            case HistoryStatsKind.Image:
                Images++;
                break;
            case HistoryStatsKind.Recording:
                Recordings++;
                break;
            default:
                Other++;
                break;
        }
    }

    public readonly int Get(HistoryStatsFilter filter) => filter switch
    {
        HistoryStatsFilter.Images => Images,
        HistoryStatsFilter.Recordings => Recordings,
        _ => Total
    };
}

public sealed record HistoryStatsEntry(string Name, int Count);

/// <summary>
/// Everything the stats window shows, counted from the items in the history database.
/// </summary>
public sealed class HistoryStats
{
    private const int TopEntryCount = 6;

    public DateTime Today { get; private init; }
    public DayOfWeek FirstDayOfWeek { get; private init; }
    public HistoryStatsTally Total { get; private set; }
    public SortedDictionary<DateTime, HistoryStatsTally> Daily { get; } = new();
    public HistoryStatsTally[] Hours { get; } = new HistoryStatsTally[24];
    /// <summary>Indexed by <see cref="DayOfWeek"/>.</summary>
    public HistoryStatsTally[] Weekdays { get; } = new HistoryStatsTally[7];
    public DateTime? FirstDay { get; private set; }
    public IReadOnlyList<HistoryStatsEntry> Apps { get; private set; } = [];
    public IReadOnlyList<HistoryStatsEntry> Destinations { get; private set; } = [];
    public IReadOnlyList<HistoryStatsEntry> FileTypes { get; private set; } = [];

    public HistoryStatsTally TodayTally { get; private set; }
    /// <summary>From the start of this week to today, and the same days of the week before.</summary>
    public int ThisWeek { get; private set; }
    public int LastWeekSoFar { get; private set; }
    /// <summary>Days in a row with an item, up to today, or yesterday while today has none yet.</summary>
    public int Streak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateTime LongestStreakEnd { get; private set; }
    public DateTime? BusiestDay { get; private set; }
    public int BusiestDayCount { get; private set; }

    public int DaysCounted => FirstDay is DateTime first ? Math.Max(0, (Today - first).Days) + 1 : 0;

    public static HistoryStats Calculate(IEnumerable<HistoryItem> historyItems, DateTime now, DayOfWeek firstDayOfWeek)
    {
        HistoryStats stats = new()
        {
            Today = now.Date,
            FirstDayOfWeek = firstDayOfWeek
        };

        HistoryStatsTally total = default;
        Dictionary<string, int> apps = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> destinations = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> fileTypes = new(StringComparer.OrdinalIgnoreCase);

        foreach (HistoryItem item in historyItems)
        {
            if (item == null || item.DateTime == DateTime.MinValue)
            {
                continue;
            }

            HistoryStatsKind kind = GetKind(item);
            DateTime day = item.DateTime.Date;

            total.Add(kind);
            stats.Hours[item.DateTime.Hour].Add(kind);
            stats.Weekdays[(int)item.DateTime.DayOfWeek].Add(kind);

            stats.Daily.TryGetValue(day, out HistoryStatsTally dayTally);
            dayTally.Add(kind);
            stats.Daily[day] = dayTally;

            Increment(apps, item.TagsProcessName);
            Increment(destinations, item.Host);
            Increment(fileTypes, GetFileType(item));
        }

        stats.Total = total;
        stats.FirstDay = stats.Daily.Count > 0 ? stats.Daily.Keys.First() : null;
        stats.Apps = GetTopEntries(apps);
        stats.Destinations = GetTopEntries(destinations);
        stats.FileTypes = GetTopEntries(fileTypes);
        stats.CalculateHighlights();

        return stats;
    }

    public static HistoryStatsKind GetKind(HistoryItem item)
    {
        string fileName = !string.IsNullOrEmpty(item.FileName) ? item.FileName : item.FilePath;
        string extension = FileHelpers.GetFileNameExtension(fileName, checkSecondExtension: false);

        // GIF screen recordings are saved as images, but they are recordings.
        if (FileHelpers.IsVideoFile(fileName) || extension.Equals("gif", StringComparison.OrdinalIgnoreCase))
        {
            return HistoryStatsKind.Recording;
        }

        if (string.Equals(item.Type, "Image", StringComparison.OrdinalIgnoreCase) || FileHelpers.IsImageFile(fileName))
        {
            return HistoryStatsKind.Image;
        }

        return HistoryStatsKind.Other;
    }

    public int CountDays(DateTime from, DateTime to, Func<HistoryStatsTally, bool> predicate)
    {
        int count = 0;

        foreach (KeyValuePair<DateTime, HistoryStatsTally> pair in Daily)
        {
            if (pair.Key >= from && pair.Key <= to && predicate(pair.Value))
            {
                count++;
            }
        }

        return count;
    }

    public HistoryStatsTally GetDay(DateTime day) => Daily.TryGetValue(day.Date, out HistoryStatsTally tally) ? tally : default;

    public DateTime GetWeekStart(DateTime day)
    {
        int offset = ((int)day.DayOfWeek - (int)FirstDayOfWeek + 7) % 7;
        return day.Date.AddDays(-offset);
    }

    private void CalculateHighlights()
    {
        TodayTally = GetDay(Today);

        DateTime weekStart = GetWeekStart(Today);
        ThisWeek = SumDays(weekStart, Today);
        LastWeekSoFar = SumDays(weekStart.AddDays(-7), Today.AddDays(-7));

        DateTime day = TodayTally.Total > 0 ? Today : Today.AddDays(-1);
        while (GetDay(day).Total > 0)
        {
            Streak++;
            day = day.AddDays(-1);
        }

        int run = 0;
        DateTime previous = DateTime.MinValue;

        foreach (KeyValuePair<DateTime, HistoryStatsTally> pair in Daily)
        {
            int count = pair.Value.Total;
            if (count <= 0)
            {
                continue;
            }

            run = run > 0 && previous.AddDays(1) == pair.Key ? run + 1 : 1;
            previous = pair.Key;

            if (run > LongestStreak)
            {
                LongestStreak = run;
                LongestStreakEnd = pair.Key;
            }

            if (count > BusiestDayCount)
            {
                BusiestDayCount = count;
                BusiestDay = pair.Key;
            }
        }
    }

    private int SumDays(DateTime from, DateTime to)
    {
        int sum = 0;

        for (DateTime day = from; day <= to; day = day.AddDays(1))
        {
            sum += GetDay(day).Total;
        }

        return sum;
    }

    private static string? GetFileType(HistoryItem item)
    {
        string fileName = !string.IsNullOrEmpty(item.FileName) ? item.FileName : item.FilePath;
        string extension = FileHelpers.GetFileNameExtension(fileName, checkSecondExtension: false);

        // Skip what isn't really an extension, such as the end of a file name with a dot in it.
        if (extension.Length is 0 or > 6 || !extension.All(char.IsLetterOrDigit))
        {
            return null;
        }

        return extension.ToUpperInvariant();
    }

    private static void Increment(Dictionary<string, int> counts, string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            name = name.Trim();
            counts[name] = counts.TryGetValue(name, out int count) ? count + 1 : 1;
        }
    }

    private static IReadOnlyList<HistoryStatsEntry> GetTopEntries(Dictionary<string, int> counts)
    {
        List<HistoryStatsEntry> entries = counts.
            OrderByDescending(x => x.Value).
            ThenBy(x => x.Key, StringComparer.OrdinalIgnoreCase).
            Select(x => new HistoryStatsEntry(x.Key, x.Value)).
            ToList();

        // Past the top few, the rest are folded into one "other" entry, which has no name.
        if (entries.Count > TopEntryCount + 1)
        {
            int rest = entries.Skip(TopEntryCount).Sum(x => x.Count);
            entries = entries.Take(TopEntryCount).ToList();
            entries.Add(new HistoryStatsEntry(string.Empty, rest));
        }

        return entries;
    }
}
