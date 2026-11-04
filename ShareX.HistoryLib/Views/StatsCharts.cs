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
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using ShareX.HistoryLib.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ShareX.HistoryLib;

/// <summary>
/// The stats charts' colors for one theme and accent: images in the accent color, recordings and other files in two hues
/// kept well away from it, each a step further from the surface for the busiest mark, and a ramp from the empty day to
/// the accent for the calendar.
/// </summary>
internal sealed class StatsPalette
{
    private static readonly Color DefaultAccent = Color.Parse("#3E83F2");
    // Candidates for recordings and other files, in order of preference, stepped for the dark and the light surface.
    private static readonly Color[] OtherHuesDark = Parse("#D95926", "#199E70", "#9085E9", "#D55181", "#C98500");
    private static readonly Color[] OtherHuesLight = Parse("#EB6834", "#1BAF7A", "#6250D6", "#E87BA4", "#EDA100");
    private static StatsPalette? _cached;

    private readonly IBrush[] _series;
    private readonly IBrush[] _peak;

    private StatsPalette(bool dark, Color accent)
    {
        IsDarkTheme = dark;
        Accent = accent;

        Color[] others = PickOtherHues(dark ? OtherHuesDark : OtherHuesLight, accent);
        Color[] series = [accent, others[0], others[1]];
        _series = series.Select(ToBrush).ToArray();
        _peak = series.Select(color => ToBrush(ToPeak(color, dark))).ToArray();

        Color empty = Color.Parse(dark ? "#313131" : "#EBEBEF");
        Ramp = new[]
        {
            empty,
            Mix(empty, accent, dark ? 0.38 : 0.3),
            Mix(empty, accent, dark ? 0.66 : 0.6),
            accent,
            ToPeak(accent, dark)
        }.Select(ToBrush).ToArray();
    }

    public bool IsDarkTheme { get; }

    public Color Accent { get; }

    /// <summary>Level 0 is a day with nothing, 4 the busiest.</summary>
    public IBrush[] Ramp { get; }

    public IBrush Series(HistoryStatsKind kind) => _series[(int)kind];

    public IBrush Peak(HistoryStatsKind kind) => _peak[(int)kind];

    public static StatsPalette Get(StyledElement element)
    {
        bool dark = IsDark(element);
        Color accent = element.TryFindResource("ShareX.Color.Accent.Start", element.ActualThemeVariant, out object? value) && value is Color color
            ? color
            : DefaultAccent;

        StatsPalette? cached = _cached;
        if (cached == null || cached.IsDarkTheme != dark || cached.Accent != accent)
        {
            _cached = cached = new StatsPalette(dark, accent);
        }

        return cached;
    }

    public static bool IsDark(StyledElement element)
    {
        for (ThemeVariant? variant = element.ActualThemeVariant; variant != null; variant = variant.InheritVariant)
        {
            if (variant == ThemeVariant.Dark) return true;
            if (variant == ThemeVariant.Light) return false;
        }

        return true;
    }

    /// <summary>The first two candidates whose hue is far enough from the accent's not to be mistaken for it.</summary>
    private static Color[] PickOtherHues(Color[] candidates, Color accent)
    {
        HsvColor accentHsv = accent.ToHsv();
        bool accentHasHue = accentHsv.S > 0.2 && accentHsv.V > 0.2;

        Color[] picked = candidates.
            Where(color => !accentHasHue || HueDistance(color.ToHsv().H, accentHsv.H) >= 45).
            Take(2).
            ToArray();

        return picked.Length == 2 ? picked : candidates.Take(2).ToArray();
    }

    private static double HueDistance(double a, double b)
    {
        double distance = Math.Abs(a - b) % 360;
        return distance > 180 ? 360 - distance : distance;
    }

    /// <summary>Lighter on the dark surface and darker on the light one, so the busiest mark stands further out.</summary>
    private static Color ToPeak(Color color, bool dark) => dark ? Mix(color, Colors.White, 0.45) : Mix(color, Colors.Black, 0.35);

    private static Color Mix(Color from, Color to, double amount) => Color.FromRgb(
        (byte)Math.Round(from.R + (to.R - from.R) * amount),
        (byte)Math.Round(from.G + (to.G - from.G) * amount),
        (byte)Math.Round(from.B + (to.B - from.B) * amount));

    private static IBrush ToBrush(Color color) => new SolidColorBrush(color).ToImmutable();

    private static Color[] Parse(params string[] colors) => colors.Select(Color.Parse).ToArray();
}

internal static class StatsFormat
{
    public static string Number(int value) => value.ToString("N0", CultureInfo.CurrentCulture);

    public static string Share(int value, int total)
    {
        if (total <= 0) return "0%";

        double share = value * 100.0 / total;
        // A decimal for the small shares, and for the large ones that would otherwise round up to 100%.
        string format = (share > 0 && share < 10) || (share > 99 && share < 100) ? "0.#" : "0";
        return share.ToString(format, CultureInfo.CurrentCulture) + "%";
    }

    public static string Days(int count) =>
        string.Format(count == 1 ? Strings.StatsWindow_OneDay : Strings.StatsWindow_Days, Number(count));

    /// <summary>
    /// Dates and day names sit in translated text, so they follow the UI language, keeping the regional format when the
    /// region speaks that language too.
    /// </summary>
    public static CultureInfo DateCulture
    {
        get
        {
            CultureInfo language = CultureInfo.CurrentUICulture;
            CultureInfo region = CultureInfo.CurrentCulture;
            return language.TwoLetterISOLanguageName == region.TwoLetterISOLanguageName || language.Equals(CultureInfo.InvariantCulture) ? region : language;
        }
    }

    public static string Date(DateTime date) => date.ToString(GetShortLongDatePattern(false), DateCulture);

    public static string DateWithDay(DateTime date) => date.ToString(GetShortLongDatePattern(true), DateCulture);

    public static string LongDateWithDay(DateTime date)
    {
        string pattern = DateCulture.DateTimeFormat.LongDatePattern;
        return date.ToString(pattern.Contains("dddd") ? pattern : "dddd " + pattern, DateCulture);
    }

    /// <summary>
    /// The culture's long date with the month abbreviated, and the day of the week abbreviated or left out, such as
    /// "18 Jun 2026" or "Thu, Jun 18, 2026".
    /// </summary>
    private static string GetShortLongDatePattern(bool withDayOfWeek)
    {
        string pattern = DateCulture.DateTimeFormat.LongDatePattern.Replace("MMMM", "MMM");

        if (pattern.Contains("dddd"))
        {
            return withDayOfWeek ? pattern.Replace("dddd", "ddd") : Regex.Replace(pattern, @"[\s,،]*dddd[\s,،]*", " ").Trim();
        }

        return withDayOfWeek ? "ddd " + pattern : pattern;
    }

    public static string Hour(int hour) => $"{hour % 24:00}:00";

    public static string HourSpan(int hour) => $"{Hour(hour)}–{Hour(hour + 1)}";

    public static string Compact(double value)
    {
        if (value >= 1_000_000) return (value / 1_000_000).ToString("0.#", CultureInfo.CurrentCulture) + "M";
        if (value >= 1_000) return (value / 1_000).ToString("0.#", CultureInfo.CurrentCulture) + "k";
        return value.ToString("0", CultureInfo.CurrentCulture);
    }

    public static string SeriesName(HistoryStatsKind kind) => kind switch
    {
        HistoryStatsKind.Image => Strings.StatsWindow_Images,
        HistoryStatsKind.Recording => Strings.StatsWindow_Recordings,
        _ => Strings.StatsWindow_OtherFiles
    };

    /// <summary>A title, then the count of each kind there is any of.</summary>
    public static string TallyToolTip(string title, HistoryStatsTally tally)
    {
        StringBuilder sb = new(title);

        if (tally.Total == 0)
        {
            sb.AppendLine().Append(Strings.StatsWindow_NothingYet);
            return sb.ToString();
        }

        sb.AppendLine().Append(string.Format(Strings.StatsWindow_TotalCount, Number(tally.Total)));

        foreach ((HistoryStatsKind kind, int count) in new[]
        {
            (HistoryStatsKind.Image, tally.Images),
            (HistoryStatsKind.Recording, tally.Recordings),
            (HistoryStatsKind.Other, tally.Other)
        })
        {
            if (count > 0)
            {
                sb.AppendLine().Append($"{SeriesName(kind)}: {Number(count)}");
            }
        }

        return sb.ToString();
    }
}

/// <summary>
/// A chart drawn by hand, with a tooltip for the mark under the pointer, recolored when the theme changes.
/// </summary>
public abstract class StatsChartControl : Control
{
    private object? _hoverKey;

    protected StatsChartControl()
    {
        // The palette comes from the theme and the accent color, both of which can change while the window is open.
        ActualThemeVariantChanged += (_, _) => InvalidateVisual();
        ResourcesChanged += (_, _) => InvalidateVisual();
    }

    private protected StatsPalette Palette => StatsPalette.Get(this);

    protected object? HoverKey => _hoverKey;

    protected IBrush TextBrush => FindBrush("ShareX.Brush.Text", Brushes.Gray);

    protected IBrush MutedTextBrush
    {
        get
        {
            IBrush text = TextBrush;
            return text is ISolidColorBrush solid ? new SolidColorBrush(solid.Color, 0.62) : text;
        }
    }

    protected IBrush SeparatorBrush => FindBrush("ShareX.Brush.Separator", Brushes.Gray);

    protected IBrush HoverBrush => FindBrush("ShareX.Brush.Control.Background.Hover", Brushes.Gray);

    protected abstract (object Key, string ToolTip)? HitTest(Point point);

    protected FormattedText CreateText(string text, double size, IBrush brush, FontWeight weight = FontWeight.Normal) =>
        new(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            new Typeface(TextElement.GetFontFamily(this), FontStyle.Normal, weight), size, brush);

    protected static Geometry DataEndRoundedRect(Rect rect, double radius)
    {
        // Rounded at the end away from the baseline, square where the bar stands.
        radius = Math.Min(radius, Math.Min(rect.Width / 2, rect.Height));
        StreamGeometry geometry = new();
        using (StreamGeometryContext context = geometry.Open())
        {
            context.BeginFigure(rect.BottomLeft, true);
            context.LineTo(new Point(rect.Left, rect.Top + radius));
            context.ArcTo(new Point(rect.Left + radius, rect.Top), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
            context.LineTo(new Point(rect.Right - radius, rect.Top));
            context.ArcTo(new Point(rect.Right, rect.Top + radius), new Size(radius, radius), 0, false, SweepDirection.Clockwise);
            context.LineTo(rect.BottomRight);
            context.EndFigure(true);
        }

        return geometry;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        UpdateHover(HitTest(e.GetPosition(this)));
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        UpdateHover(null);
    }

    private void UpdateHover((object Key, string ToolTip)? hit)
    {
        if (Equals(hit?.Key, _hoverKey))
        {
            return;
        }

        _hoverKey = hit?.Key;
        ToolTip.SetIsOpen(this, false);
        ToolTip.SetTip(this, hit?.ToolTip);

        if (hit != null)
        {
            ToolTip.SetIsOpen(this, true);
        }

        InvalidateVisual();
    }

    private IBrush FindBrush(string key, IBrush fallback) =>
        this.TryFindResource(key, ActualThemeVariant, out object? value) && value is IBrush brush ? brush : fallback;
}

/// <summary>One bar split into images, recordings and other files.</summary>
public sealed class StatsSplitBar : StatsChartControl
{
    private const double Gap = 2;

    public static readonly StyledProperty<HistoryStatsTally> TallyProperty =
        AvaloniaProperty.Register<StatsSplitBar, HistoryStatsTally>(nameof(Tally));

    static StatsSplitBar()
    {
        AffectsRender<StatsSplitBar>(TallyProperty);
        HeightProperty.OverrideDefaultValue<StatsSplitBar>(10);
    }

    public HistoryStatsTally Tally
    {
        get => GetValue(TallyProperty);
        set => SetValue(TallyProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        // Transparent, so the gaps between marks still get the pointer for the tooltips.
        context.DrawRectangle(Brushes.Transparent, null, new Rect(Bounds.Size));

        Rect bounds = new(Bounds.Size);
        HistoryStatsTally tally = Tally;

        if (tally.Total == 0)
        {
            context.DrawRectangle(Palette.Ramp[0], null, bounds, 3, 3);
            return;
        }

        foreach ((HistoryStatsKind kind, Rect rect) in GetSegments(bounds))
        {
            IBrush fill = Equals(HoverKey, kind) ? Palette.Peak(kind) : Palette.Series(kind);
            context.DrawRectangle(fill, null, rect, 3, 3);
        }
    }

    protected override (object Key, string ToolTip)? HitTest(Point point)
    {
        foreach ((HistoryStatsKind kind, Rect rect) in GetSegments(new Rect(Bounds.Size)))
        {
            if (point.X >= rect.Left - Gap && point.X <= rect.Right + Gap)
            {
                int count = kind switch
                {
                    HistoryStatsKind.Image => Tally.Images,
                    HistoryStatsKind.Recording => Tally.Recordings,
                    _ => Tally.Other
                };

                return (kind, $"{StatsFormat.SeriesName(kind)}: {StatsFormat.Number(count)} ({StatsFormat.Share(count, Tally.Total)})");
            }
        }

        return null;
    }

    private IEnumerable<(HistoryStatsKind Kind, Rect Rect)> GetSegments(Rect bounds)
    {
        HistoryStatsTally tally = Tally;
        (HistoryStatsKind Kind, int Count)[] parts = new (HistoryStatsKind Kind, int Count)[]
        {
            (HistoryStatsKind.Image, tally.Images),
            (HistoryStatsKind.Recording, tally.Recordings),
            (HistoryStatsKind.Other, tally.Other)
        }.Where(part => part.Count > 0).ToArray();

        if (parts.Length == 0)
        {
            yield break;
        }

        // Every segment gets at least a sliver, so a small share is still seen.
        double available = bounds.Width - Gap * (parts.Length - 1);
        double minimum = Math.Min(4, available / parts.Length);
        double scalable = available - minimum * parts.Length;
        double x = bounds.Left;

        foreach ((HistoryStatsKind kind, int count) in parts)
        {
            double width = minimum + scalable * count / tally.Total;
            yield return (kind, new Rect(x, bounds.Top, width, bounds.Height));
            x += width + Gap;
        }
    }
}

/// <summary>A legend's square for one kind of item, or for every step of the calendar's ramp.</summary>
public sealed class StatsSwatch : StatsChartControl
{
    public static readonly StyledProperty<HistoryStatsKind> KindProperty =
        AvaloniaProperty.Register<StatsSwatch, HistoryStatsKind>(nameof(Kind));

    public static readonly StyledProperty<bool> IsRampProperty =
        AvaloniaProperty.Register<StatsSwatch, bool>(nameof(IsRamp));

    private const double SquareSize = 10;
    private const double Gap = 3;

    static StatsSwatch()
    {
        AffectsRender<StatsSwatch>(KindProperty, IsRampProperty);
        AffectsMeasure<StatsSwatch>(IsRampProperty);
        IsHitTestVisibleProperty.OverrideDefaultValue<StatsSwatch>(false);
        VerticalAlignmentProperty.OverrideDefaultValue<StatsSwatch>(Avalonia.Layout.VerticalAlignment.Center);
    }

    public HistoryStatsKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public bool IsRamp
    {
        get => GetValue(IsRampProperty);
        set => SetValue(IsRampProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) =>
        IsRamp ? new Size(5 * SquareSize + 4 * Gap, SquareSize) : new Size(SquareSize, SquareSize);

    public override void Render(DrawingContext context)
    {
        if (!IsRamp)
        {
            context.DrawRectangle(Palette.Series(Kind), null, new Rect(0, 0, SquareSize, SquareSize), 2, 2);
            return;
        }

        for (int level = 0; level < 5; level++)
        {
            context.DrawRectangle(Palette.Ramp[level], null, new Rect(level * (SquareSize + Gap), 0, SquareSize, SquareSize), 2, 2);
        }
    }

    protected override (object Key, string ToolTip)? HitTest(Point point) => null;
}

/// <summary>A horizontal bar against a track, filled to <see cref="Fraction"/>.</summary>
public sealed class StatsMeter : StatsChartControl
{
    public static readonly StyledProperty<double> FractionProperty =
        AvaloniaProperty.Register<StatsMeter, double>(nameof(Fraction));

    public static readonly StyledProperty<bool> IsPeakProperty =
        AvaloniaProperty.Register<StatsMeter, bool>(nameof(IsPeak));

    static StatsMeter()
    {
        AffectsRender<StatsMeter>(FractionProperty, IsPeakProperty);
        HeightProperty.OverrideDefaultValue<StatsMeter>(10);
        IsHitTestVisibleProperty.OverrideDefaultValue<StatsMeter>(false);
    }

    public double Fraction
    {
        get => GetValue(FractionProperty);
        set => SetValue(FractionProperty, value);
    }

    public bool IsPeak
    {
        get => GetValue(IsPeakProperty);
        set => SetValue(IsPeakProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        Rect bounds = new(Bounds.Size);
        context.DrawRectangle(Palette.Ramp[0], null, bounds, 3, 3);

        double fraction = Math.Clamp(Fraction, 0, 1);
        if (fraction > 0)
        {
            Rect fill = bounds.WithWidth(Math.Max(bounds.Height / 2, bounds.Width * fraction));
            IBrush brush = IsPeak ? Palette.Peak(HistoryStatsKind.Image) : Palette.Series(HistoryStatsKind.Image);
            context.DrawRectangle(brush, null, fill, 3, 3);
        }
    }

    protected override (object Key, string ToolTip)? HitTest(Point point) => null;
}

/// <summary>
/// A square for each day, a column a week, filled by how many items there were that day: the past year, one calendar
/// year, or every year a row each.
/// </summary>
public sealed class StatsCalendar : StatsChartControl
{
    public const int PastYear = 0;
    public const int AllYears = -1;

    private const double Gap = 3;
    private const double MinStep = 11;
    private const double MaxStep = 20;
    private const double MonthRowHeight = 18;
    private const double RunGap = 14;
    private const int YearWeeks = 54;

    public static readonly StyledProperty<HistoryStats?> StatsProperty =
        AvaloniaProperty.Register<StatsCalendar, HistoryStats?>(nameof(Stats));

    /// <summary><see cref="PastYear"/>, <see cref="AllYears"/>, or a year.</summary>
    public static readonly StyledProperty<int> SpanProperty =
        AvaloniaProperty.Register<StatsCalendar, int>(nameof(Span));

    private sealed record Run(string? Label, DateTime WeekStart, DateTime From, DateTime To);

    private sealed record CalendarLayout(IReadOnlyList<Run> Runs, double Left, double Step, int[] Steps);

    private CalendarLayout? _layout;
    private double _layoutWidth;

    static StatsCalendar()
    {
        AffectsRender<StatsCalendar>(StatsProperty, SpanProperty);
        AffectsMeasure<StatsCalendar>(StatsProperty, SpanProperty);
    }

    public HistoryStats? Stats
    {
        get => GetValue(StatsProperty);
        set => SetValue(StatsProperty, value);
    }

    public int Span
    {
        get => GetValue(SpanProperty);
        set => SetValue(SpanProperty, value);
    }

    /// <summary>The first and last day the calendar shows, for the heading's count of days.</summary>
    public static (DateTime From, DateTime To)? GetRange(HistoryStats stats, int span)
    {
        if (stats.FirstDay is not DateTime first)
        {
            return null;
        }

        DateTime today = stats.Today;

        return span switch
        {
            PastYear => (Max(first, today.AddYears(-1).AddDays(1)), today),
            AllYears => (first, today),
            _ => (Max(first, new DateTime(span, 1, 1)), Min(today, new DateTime(span, 12, 31)))
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double width = double.IsInfinity(availableSize.Width) ? 760 : availableSize.Width;
        CalendarLayout? layout = CreateLayout(width);

        if (layout == null)
        {
            return new Size(0, 0);
        }

        double height = layout.Runs.Count * (MonthRowHeight + 7 * layout.Step - Gap) + (layout.Runs.Count - 1) * RunGap;
        return new Size(width, height);
    }

    public override void Render(DrawingContext context)
    {
        // Transparent, so the gaps between marks still get the pointer for the tooltips.
        context.DrawRectangle(Brushes.Transparent, null, new Rect(Bounds.Size));

        HistoryStats? stats = Stats;
        CalendarLayout? layout = CreateLayout(Bounds.Width);

        if (stats == null || layout == null)
        {
            return;
        }

        StatsPalette palette = Palette;
        IBrush muted = MutedTextBrush;
        IBrush text = TextBrush;
        double size = layout.Step - Gap;
        string[] dayNames = StatsFormat.DateCulture.DateTimeFormat.AbbreviatedDayNames;
        string[] monthNames = StatsFormat.DateCulture.DateTimeFormat.AbbreviatedMonthNames;
        double top = 0;

        foreach (Run run in layout.Runs)
        {
            double gridTop = top + MonthRowHeight;

            if (run.Label != null)
            {
                context.DrawText(CreateText(run.Label, 11, text, FontWeight.SemiBold), new Point(0, top));
            }

            // Every other weekday, as the rows are too close for all seven.
            for (int row = 0; row < 7; row += 2)
            {
                string name = dayNames[((int)stats.FirstDayOfWeek + row) % 7];
                FormattedText label = CreateText(name, 10, muted);
                context.DrawText(label, new Point(0, gridTop + row * layout.Step + (size - label.Height) / 2));
            }

            double lastMonthX = double.NegativeInfinity;
            for (DateTime month = new(run.From.Year, run.From.Month, 1); month <= run.To; month = month.AddMonths(1))
            {
                if (month < run.From)
                {
                    continue;
                }

                double x = layout.Left + (month - run.WeekStart).Days / 7 * layout.Step;
                if (x - lastMonthX >= 3 * layout.Step)
                {
                    context.DrawText(CreateText(monthNames[month.Month - 1], 10, muted), new Point(x, top));
                    lastMonthX = x;
                }
            }

            for (DateTime day = run.From; day <= run.To; day = day.AddDays(1))
            {
                int index = (day - run.WeekStart).Days;
                Rect cell = new(layout.Left + index / 7 * layout.Step, gridTop + index % 7 * layout.Step, size, size);

                if (day < stats.FirstDay)
                {
                    // Before the history began nothing is known, which isn't the same as a day with nothing.
                    context.DrawRectangle(null, new Pen(palette.Ramp[0], 1), cell.Deflate(0.5), 3, 3);
                    continue;
                }

                int count = stats.GetDay(day).Total;
                context.DrawRectangle(palette.Ramp[GetLevel(count, layout.Steps)], null, cell, 3, 3);

                if (Equals(HoverKey, day))
                {
                    context.DrawRectangle(null, new Pen(text, 1.5), cell.Inflate(1), 4, 4);
                }
            }

            top = gridTop + 7 * layout.Step - Gap + RunGap;
        }
    }

    protected override (object Key, string ToolTip)? HitTest(Point point)
    {
        HistoryStats? stats = Stats;
        CalendarLayout? layout = CreateLayout(Bounds.Width);

        if (stats == null || layout == null || point.X < layout.Left)
        {
            return null;
        }

        double top = 0;

        foreach (Run run in layout.Runs)
        {
            double gridTop = top + MonthRowHeight;
            double gridBottom = gridTop + 7 * layout.Step - Gap;

            if (point.Y >= gridTop && point.Y < gridBottom)
            {
                int column = (int)((point.X - layout.Left) / layout.Step);
                int row = Math.Min(6, (int)((point.Y - gridTop) / layout.Step));
                DateTime day = run.WeekStart.AddDays(column * 7 + row);

                if (day < run.From || day > run.To)
                {
                    return null;
                }

                string title = StatsFormat.LongDateWithDay(day);
                string tip = day < stats.FirstDay
                    ? title + Environment.NewLine + Strings.StatsWindow_BeforeHistory
                    : StatsFormat.TallyToolTip(title, stats.GetDay(day));
                return (day, tip);
            }

            top = gridBottom + RunGap;
        }

        return null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == StatsProperty || change.Property == SpanProperty)
        {
            _layout = null;
        }
    }

    private CalendarLayout? CreateLayout(double width)
    {
        if (_layout != null && _layoutWidth == width)
        {
            return _layout;
        }

        _layout = CreateLayoutCore(width);
        _layoutWidth = width;
        return _layout;
    }

    private CalendarLayout? CreateLayoutCore(double width)
    {
        HistoryStats? stats = Stats;
        if (stats?.FirstDay is not DateTime first || width <= 0)
        {
            return null;
        }

        DateTime today = stats.Today;
        int span = Span;
        double left = span == AllYears ? 40 : 32;
        double room = width - left + Gap;
        List<Run> runs = [];
        double step;

        if (span == PastYear)
        {
            // As many weeks as fit, up to a year.
            int weeks = Math.Clamp((int)(room / MinStep), 1, 53);
            step = Math.Min(MaxStep, room / weeks);
            DateTime weekStart = stats.GetWeekStart(today).AddDays(-7 * (weeks - 1));
            runs.Add(new Run(null, weekStart, weekStart, today));
        }
        else
        {
            step = Math.Clamp(room / YearWeeks, 6, MaxStep);
            IEnumerable<int> years = span == AllYears ? Enumerable.Range(first.Year, today.Year - first.Year + 1).Reverse() : [span];

            foreach (int year in years)
            {
                DateTime from = new(year, 1, 1);
                DateTime to = Min(new DateTime(year, 12, 31), today);
                runs.Add(new Run(span == AllYears ? year.ToString(CultureInfo.CurrentCulture) : null, stats.GetWeekStart(from), from, to));
            }
        }

        // The fill steps up at the quartiles of the busy days shown, so one very busy day doesn't wash out the rest.
        List<int> busy = [];
        foreach (Run run in runs)
        {
            foreach (KeyValuePair<DateTime, HistoryStatsTally> pair in stats.Daily)
            {
                if (pair.Key >= run.From && pair.Key <= run.To && pair.Value.Total > 0)
                {
                    busy.Add(pair.Value.Total);
                }
            }
        }

        busy.Sort();
        int[] steps = busy.Count == 0 ? [0, 0, 0] :
            [busy[(busy.Count - 1) / 4], busy[(busy.Count - 1) / 2], busy[(busy.Count - 1) * 3 / 4]];

        return new CalendarLayout(runs, left, step, steps);
    }

    private static int GetLevel(int count, int[] steps)
    {
        if (count <= 0) return 0;
        if (count <= steps[0]) return 1;
        if (count <= steps[1]) return 2;
        if (count <= steps[2]) return 3;
        return 4;
    }

    private static DateTime Max(DateTime a, DateTime b) => a > b ? a : b;

    private static DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
}

/// <summary>
/// A bar an hour of the day, of one kind or all, with the busiest hour picked out and each part of the day's share
/// under its quarter of the chart.
/// </summary>
public sealed class StatsHourChart : StatsChartControl
{
    private const double PlotTop = 8;
    private const double AxisLabelHeight = 20;
    private const double DayPartsHeight = 34;

    public static readonly StyledProperty<HistoryStats?> StatsProperty =
        AvaloniaProperty.Register<StatsHourChart, HistoryStats?>(nameof(Stats));

    public static readonly StyledProperty<HistoryStatsFilter> FilterProperty =
        AvaloniaProperty.Register<StatsHourChart, HistoryStatsFilter>(nameof(Filter));

    static StatsHourChart()
    {
        AffectsRender<StatsHourChart>(StatsProperty, FilterProperty);
    }

    public HistoryStats? Stats
    {
        get => GetValue(StatsProperty);
        set => SetValue(StatsProperty, value);
    }

    public HistoryStatsFilter Filter
    {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public static int? GetPeakHour(HistoryStats stats, HistoryStatsFilter filter)
    {
        int peak = -1;
        int max = 0;

        for (int hour = 0; hour < 24; hour++)
        {
            int count = stats.Hours[hour].Get(filter);
            if (count > max)
            {
                max = count;
                peak = hour;
            }
        }

        return peak >= 0 ? peak : null;
    }

    public override void Render(DrawingContext context)
    {
        // Transparent, so the gaps between marks still get the pointer for the tooltips.
        context.DrawRectangle(Brushes.Transparent, null, new Rect(Bounds.Size));

        HistoryStats? stats = Stats;
        if (stats == null || Bounds.Width <= 0)
        {
            return;
        }

        StatsPalette palette = Palette;
        HistoryStatsFilter filter = Filter;
        HistoryStatsKind kind = filter == HistoryStatsFilter.Recordings ? HistoryStatsKind.Recording : HistoryStatsKind.Image;
        IBrush muted = MutedTextBrush;
        IBrush text = TextBrush;
        Pen gridPen = new(SeparatorBrush, 1);
        int[] values = stats.Hours.Select(tally => tally.Get(filter)).ToArray();
        int? peak = GetPeakHour(stats, filter);
        (double step, double top) = GetScale(values.Max());
        (double plotLeft, double plotRight, double plotBottom) = GetPlot(top);
        double slot = (plotRight - plotLeft) / 24;

        for (double value = 0; value <= top + step / 2; value += step)
        {
            double y = plotBottom - (plotBottom - PlotTop) * value / top;
            context.DrawLine(gridPen, new Point(plotLeft, Math.Round(y) + 0.5), new Point(plotRight, Math.Round(y) + 0.5));
            FormattedText label = CreateText(StatsFormat.Compact(value), 10, muted);
            context.DrawText(label, new Point(plotLeft - 8 - label.Width, y - label.Height / 2));
        }

        for (int hour = 0; hour < 24; hour++)
        {
            Rect column = new(plotLeft + hour * slot, PlotTop, slot, plotBottom - PlotTop);

            if (Equals(HoverKey, hour))
            {
                context.DrawRectangle(HoverBrush, null, column, 3, 3);
            }

            if (values[hour] > 0)
            {
                double gap = Math.Max(2, slot * 0.22);
                double height = Math.Max(2, (plotBottom - PlotTop) * values[hour] / top);
                Rect bar = new(column.Left + gap / 2, plotBottom - height, slot - gap, height);
                IBrush fill = hour == peak ? palette.Peak(kind) : palette.Series(kind);
                context.DrawGeometry(fill, null, DataEndRoundedRect(bar, 4));
            }

            if (hour % 3 == 0)
            {
                FormattedText label = CreateText(StatsFormat.Hour(hour), 10, muted);
                context.DrawText(label, new Point(column.Center.X - label.Width / 2, plotBottom + 4));
            }
        }

        // Night, morning, afternoon and evening, each under its quarter of the day.
        string[] parts = [Strings.StatsWindow_Night, Strings.StatsWindow_Morning, Strings.StatsWindow_Afternoon, Strings.StatsWindow_Evening];
        int total = values.Sum();
        double quarter = (plotRight - plotLeft) / 4;
        double partsTop = plotBottom + AxisLabelHeight + 6;
        Pen partPen = new(SeparatorBrush, 2);

        for (int i = 0; i < parts.Length; i++)
        {
            double x0 = plotLeft + i * quarter + 2;
            double x1 = plotLeft + (i + 1) * quarter - 2;
            context.DrawLine(partPen, new Point(x0, partsTop), new Point(x1, partsTop));

            FormattedText name = CreateText(parts[i], 11, muted);
            FormattedText share = CreateText(StatsFormat.Share(values.Skip(i * 6).Take(6).Sum(), total), 11, text, FontWeight.SemiBold);
            double x = (x0 + x1 - name.Width - 4 - share.Width) / 2;
            context.DrawText(name, new Point(x, partsTop + 6));
            context.DrawText(share, new Point(x + name.Width + 4, partsTop + 6));
        }
    }

    protected override (object Key, string ToolTip)? HitTest(Point point)
    {
        HistoryStats? stats = Stats;
        if (stats == null)
        {
            return null;
        }

        (_, double top) = GetScale(stats.Hours.Max(tally => tally.Get(Filter)));
        (double plotLeft, double plotRight, double plotBottom) = GetPlot(top);

        if (point.X < plotLeft || point.X >= plotRight || point.Y > plotBottom + AxisLabelHeight)
        {
            return null;
        }

        int hour = Math.Clamp((int)((point.X - plotLeft) / ((plotRight - plotLeft) / 24)), 0, 23);
        return (hour, StatsFormat.TallyToolTip(StatsFormat.HourSpan(hour), stats.Hours[hour]));
    }

    private (double Left, double Right, double Bottom) GetPlot(double top)
    {
        double labelWidth = CreateText(StatsFormat.Compact(top), 10, Brushes.Gray).Width;
        return (labelWidth + 12, Bounds.Width, Bounds.Height - AxisLabelHeight - DayPartsHeight);
    }

    /// <summary>A round grid step (1, 2 or 5 × a power of ten) for about two lines, and the top of the scale.</summary>
    private static (double Step, double Top) GetScale(int max)
    {
        if (max <= 0)
        {
            return (1, 1);
        }

        double rough = max / 2.0;
        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(rough)));
        double normalized = rough / magnitude;
        double step = (normalized <= 1 ? 1 : normalized <= 2 ? 2 : normalized <= 5 ? 5 : 10) * magnitude;
        step = Math.Max(1, step);
        return (step, Math.Ceiling(max / step) * step);
    }
}
