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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using ShareX.AvaloniaUI.Imaging;
using ShareX.AvaloniaUI.Theming;
using ShareX.HelpersLib;
using ShareX.HistoryLib.Localization;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.HistoryLib;

public sealed record StatsRow(string Label, double LabelWidth, double Fraction, string Value, string ToolTip, bool IsPeak);

public partial class StatsWindow : Window
{
    private readonly HistoryManagerSQLite _historyManager;
    private readonly Action<SKBitmap>? _shareImage;
    private readonly List<int> _spans = [];
    private HistoryStats? _stats;
    private HistoryStatsFilter _hourFilter = HistoryStatsFilter.All;
    private bool _isLoading;
    private bool _suppressSpanChange;

    public StatsWindow()
    {
        _historyManager = null!;
        InitializeComponent();
        RequestedThemeVariant = ThemeManager.GetCurrentTheme();
    }

    public StatsWindow(HistoryManagerSQLite historyManager, Action<SKBitmap>? shareImage) : this()
    {
        _historyManager = historyManager;
        _shareImage = shareImage;
        ShareButton.IsVisible = shareImage != null;
        Opened += OnOpened;
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        Opened -= OnOpened;
        await RefreshAsync();
    }

    private async void OnRefreshClick(object? sender, RoutedEventArgs e) => await RefreshAsync();

    private void OnShareClick(object? sender, RoutedEventArgs e)
    {
        if (_shareImage == null)
        {
            return;
        }

        SKBitmap image;

        try
        {
            image = RenderSnapshot();
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            return;
        }

        _shareImage(image);
    }

    /// <summary>Draws the whole page, including what is scrolled out of view, without the buttons in the title.</summary>
    private SKBitmap RenderSnapshot()
    {
        double scaling = RenderScaling;
        PixelSize pixelSize = PixelSize.FromSize(SnapshotPanel.Bounds.Size, scaling);
        double previousOpacity = ActionsPanel.Opacity;
        ActionsPanel.Opacity = 0;

        try
        {
            using RenderTargetBitmap bitmap = new(pixelSize, new Vector(96 * scaling, 96 * scaling));
            bitmap.Render(SnapshotPanel);
            return BitmapConversionHelpers.ToSKBitmap(bitmap);
        }
        finally
        {
            ActionsPanel.Opacity = previousOpacity;
        }
    }

    private async Task RefreshAsync()
    {
        if (_isLoading)
        {
            return;
        }

        _isLoading = true;
        RefreshButton.IsEnabled = false;

        if (_stats == null)
        {
            StatusText.Text = Strings.HistoryWindow_CalculatingStatistics;
        }

        try
        {
            List<HistoryItem> historyItems = await _historyManager.GetHistoryItemsAsync();
            DateTime now = DateTime.Now;
            DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            HistoryStats stats = await Task.Run(() => HistoryStats.Calculate(historyItems, now, firstDayOfWeek));
            ShowStats(stats);
        }
        catch (Exception ex)
        {
            DebugHelper.WriteException(ex);
            ContentPanel.IsVisible = false;
            ShareButton.IsEnabled = false;
            StatusPanel.IsVisible = true;
            StatusText.Text = ex.Message;
        }
        finally
        {
            _isLoading = false;
            RefreshButton.IsEnabled = true;
        }
    }

    private void ShowStats(HistoryStats stats)
    {
        _stats = stats;

        bool hasItems = stats.Total.Total > 0 && stats.FirstDay != null;
        StatusPanel.IsVisible = !hasItems;
        StatusText.Text = Strings.StatsWindow_NoHistory;
        ContentPanel.IsVisible = hasItems;
        ShareButton.IsEnabled = hasItems;
        SubtitleText.Text = stats.FirstDay is DateTime first
            ? string.Format(Strings.StatsWindow_CountingSince, StatsFormat.Date(first), StatsFormat.Days(stats.DaysCounted))
            : string.Empty;

        if (!hasItems)
        {
            return;
        }

        ShowTotal(stats);
        ShowHighlights(stats);
        ShowCalendarSpans(stats);
        ShowHours();
        ShowWeekdays(stats);
        ShowEntries(AppsList, AppsEmptyText, stats.Apps);
        ShowEntries(DestinationsList, DestinationsEmptyText, stats.Destinations);
        ShowEntries(FileTypesList, FileTypesEmptyText, stats.FileTypes);
    }

    private void ShowTotal(HistoryStats stats)
    {
        HistoryStatsTally total = stats.Total;
        TotalText.Text = StatsFormat.Number(total.Total);
        RateText.Text = GetRate(total.Total, stats.DaysCounted);
        SplitBar.Tally = total;
        ImagesCountText.Text = StatsFormat.Number(total.Images);
        ImagesShareText.Text = StatsFormat.Share(total.Images, total.Total);
        RecordingsCountText.Text = StatsFormat.Number(total.Recordings);
        RecordingsShareText.Text = StatsFormat.Share(total.Recordings, total.Total);
        OtherCountText.Text = StatsFormat.Number(total.Other);
        OtherShareText.Text = StatsFormat.Share(total.Other, total.Total);
    }

    private void ShowHighlights(HistoryStats stats)
    {
        TodayText.Text = StatsFormat.Number(stats.TodayTally.Total);
        SetNote(TodayNoteText, GetTallySummary(stats.TodayTally));

        WeekText.Text = StatsFormat.Number(stats.Past7Days);
        UpdateWeekChange();

        StreakText.Text = StatsFormat.Days(stats.Streak);
        string streakNote = stats.LongestStreak <= 0 ? string.Empty :
            stats.Streak >= stats.LongestStreak ? Strings.StatsWindow_LongestYet :
            string.Format(Strings.StatsWindow_LongestStreak, StatsFormat.Number(stats.LongestStreak), StatsFormat.Date(stats.LongestStreakEnd));
        SetNote(StreakNoteText, streakNote);

        if (stats.BusiestDay is DateTime busiestDay)
        {
            BusiestDayText.Text = StatsFormat.Number(stats.BusiestDayCount);
            SetNote(BusiestDayNoteText, StatsFormat.DateWithDay(busiestDay));
        }
        else
        {
            BusiestDayText.Text = "—";
            SetNote(BusiestDayNoteText, Strings.StatsWindow_NothingYet);
        }
    }

    private void UpdateWeekChange()
    {
        if (_stats == null)
        {
            return;
        }

        if (_stats.Previous7Days == 0)
        {
            WeekChangeText.Text = string.Empty;
            WeekChangeText.IsVisible = false;
            SetNote(WeekNoteText, _stats.Past7Days == 0 ? Strings.StatsWindow_NonePrevious7DaysEither : Strings.StatsWindow_NoneInPrevious7Days);
            return;
        }

        double change = (_stats.Past7Days - _stats.Previous7Days) * 100.0 / _stats.Previous7Days;
        bool up = change >= 0;
        WeekChangeText.IsVisible = true;
        WeekChangeText.Text = (up ? "▲ " : "▼ ") + Math.Abs(change).ToString("0", CultureInfo.CurrentCulture) + "%";
        WeekChangeText.Classes.Set("change-up", up);
        WeekChangeText.Classes.Set("change-down", !up);
        SetNote(WeekNoteText, Strings.StatsWindow_VsPrevious7Days);
    }

    private void ShowCalendarSpans(HistoryStats stats)
    {
        int selected = SpanComboBox.SelectedIndex >= 0 && SpanComboBox.SelectedIndex < _spans.Count ? _spans[SpanComboBox.SelectedIndex] : StatsCalendar.PastYear;
        int firstYear = stats.FirstDay?.Year ?? stats.Today.Year;

        _spans.Clear();
        _spans.Add(StatsCalendar.PastYear);
        List<string> names = [Strings.StatsWindow_PastYear];

        if (firstYear < stats.Today.Year)
        {
            _spans.Add(StatsCalendar.AllYears);
            names.Add(Strings.StatsWindow_AllYears);

            for (int year = stats.Today.Year; year >= firstYear; year--)
            {
                _spans.Add(year);
                names.Add(year.ToString(CultureInfo.CurrentCulture));
            }
        }

        _suppressSpanChange = true;
        SpanComboBox.ItemsSource = names;
        SpanComboBox.SelectedIndex = Math.Max(0, _spans.IndexOf(selected));
        SpanComboBox.IsVisible = _spans.Count > 1;
        _suppressSpanChange = false;

        Calendar.Stats = stats;
        ShowCalendarSpan();
    }

    private void OnSpanSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (!_suppressSpanChange)
        {
            ShowCalendarSpan();
        }
    }

    private void ShowCalendarSpan()
    {
        if (_stats == null)
        {
            return;
        }

        int span = SpanComboBox.SelectedIndex >= 0 && SpanComboBox.SelectedIndex < _spans.Count ? _spans[SpanComboBox.SelectedIndex] : StatsCalendar.PastYear;
        Calendar.Span = span;

        if (StatsCalendar.GetRange(_stats, span) is (DateTime from, DateTime to))
        {
            int days = Math.Max(0, (to - from).Days) + 1;
            int activeDays = _stats.CountDays(from, to, tally => tally.Total > 0);
            CalendarNoteText.Text = string.Format(Strings.StatsWindow_ActiveDays, StatsFormat.Number(activeDays), StatsFormat.Days(days));
        }
        else
        {
            CalendarNoteText.Text = string.Empty;
        }
    }

    private void OnHourFilterClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: HistoryStatsFilter filter })
        {
            _hourFilter = filter;
            ShowHours();
        }
    }

    private void ShowHours()
    {
        FilterAllButton.Classes.Set("active", _hourFilter == HistoryStatsFilter.All);
        FilterImagesButton.Classes.Set("active", _hourFilter == HistoryStatsFilter.Images);
        FilterRecordingsButton.Classes.Set("active", _hourFilter == HistoryStatsFilter.Recordings);

        HourChart.Stats = _stats;
        HourChart.Filter = _hourFilter;

        int? peak = _stats != null ? StatsHourChart.GetPeakHour(_stats, _hourFilter) : null;
        HoursNoteText.Text = peak is int hour ? string.Format(Strings.StatsWindow_BusiestTime, StatsFormat.HourSpan(hour)) : string.Empty;
    }

    private void ShowWeekdays(HistoryStats stats)
    {
        DateTimeFormatInfo format = StatsFormat.DateCulture.DateTimeFormat;
        int total = stats.Weekdays.Sum(tally => tally.Total);
        int max = stats.Weekdays.Max(tally => tally.Total);
        DayOfWeek? peak = max > 0 ? (DayOfWeek)Array.FindIndex(stats.Weekdays, tally => tally.Total == max) : null;

        WeekdaysList.ItemsSource = Enumerable.Range(0, 7).Select(i =>
        {
            DayOfWeek day = (DayOfWeek)(((int)stats.FirstDayOfWeek + i) % 7);
            HistoryStatsTally tally = stats.Weekdays[(int)day];
            return new StatsRow(format.GetAbbreviatedDayName(day), 44, max > 0 ? tally.Total / (double)max : 0,
                StatsFormat.Share(tally.Total, total), StatsFormat.TallyToolTip(format.GetDayName(day), tally), day == peak);
        }).ToArray();

        WeekdaysNoteText.Text = peak is DayOfWeek peakDay ? string.Format(Strings.StatsWindow_BusiestOn, format.GetDayName(peakDay)) : string.Empty;
    }

    private static void ShowEntries(ItemsControl list, TextBlock emptyText, IReadOnlyList<HistoryStatsEntry> entries)
    {
        int total = entries.Sum(entry => entry.Count);
        int max = entries.Count > 0 ? entries.Max(entry => entry.Count) : 0;

        list.ItemsSource = entries.Select(entry =>
        {
            string name = string.IsNullOrEmpty(entry.Name) ? Strings.StatsWindow_Other : entry.Name;
            string share = StatsFormat.Share(entry.Count, total);
            return new StatsRow(name, 150, max > 0 ? entry.Count / (double)max : 0, share,
                $"{name}: {StatsFormat.Number(entry.Count)} ({share})", false);
        }).ToArray();

        emptyText.IsVisible = entries.Count == 0;
    }

    private static void SetNote(TextBlock textBlock, string text)
    {
        textBlock.Text = text;
        ToolTip.SetTip(textBlock, string.IsNullOrEmpty(text) ? null : text);
    }

    /// <summary>"about 26 a day", or a week for the occasional user.</summary>
    private static string GetRate(int total, int days)
    {
        double perDay = total / (double)Math.Max(1, days);
        return perDay >= 1
            ? string.Format(Strings.StatsWindow_AboutADay, Round(perDay))
            : string.Format(Strings.StatsWindow_AboutAWeek, Round(perDay * 7));
    }

    private static string Round(double value) =>
        value.ToString(value >= 10 ? "N0" : "0.#", CultureInfo.CurrentCulture);

    /// <summary>What there was of each kind, without plurals: "Images 12 · Recordings 2".</summary>
    private static string GetTallySummary(HistoryStatsTally tally)
    {
        if (tally.Total == 0)
        {
            return Strings.StatsWindow_NothingYet;
        }

        IEnumerable<string> parts = new[]
        {
            (HistoryStatsKind.Image, tally.Images),
            (HistoryStatsKind.Recording, tally.Recordings),
            (HistoryStatsKind.Other, tally.Other)
        }.Where(part => part.Item2 > 0).Select(part => $"{StatsFormat.SeriesName(part.Item1)} {StatsFormat.Number(part.Item2)}");

        return string.Join(" · ", parts);
    }
}
