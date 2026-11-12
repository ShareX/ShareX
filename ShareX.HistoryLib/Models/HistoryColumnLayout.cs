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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HistoryLib;

/// <summary>
/// Columns of the history list, in display order. <see cref="Icon"/> is always shown and cannot be resized.
/// </summary>
public enum HistoryColumn
{
    Icon,
    Date,
    FileName,
    FilePath,
    Type,
    Host,
    URL,
    ShortenedURL,
    ProcessName,
    WindowTitle,
    Tag
}

public class HistoryColumnSettings
{
    public string Name { get; set; } = "";
    public bool Visible { get; set; }
    public double Width { get; set; }
}

/// <summary>
/// Which history list columns are shown and how wide they are. The last visible column fills the remaining space.
/// </summary>
public sealed class HistoryColumnLayout
{
    public const double IconWidth = 18;
    public const double MinColumnWidth = 40;

    private sealed record ColumnDefault(bool Visible, double Width);

    private static readonly Dictionary<HistoryColumn, ColumnDefault> Defaults = new()
    {
        [HistoryColumn.Date] = new(true, 105),
        [HistoryColumn.FileName] = new(true, 220),
        [HistoryColumn.FilePath] = new(false, 250),
        [HistoryColumn.Type] = new(false, 60),
        [HistoryColumn.Host] = new(false, 120),
        [HistoryColumn.URL] = new(true, 250),
        [HistoryColumn.ShortenedURL] = new(false, 180),
        [HistoryColumn.ProcessName] = new(false, 110),
        [HistoryColumn.WindowTitle] = new(false, 200),
        [HistoryColumn.Tag] = new(false, 120)
    };

    public static IReadOnlyList<HistoryColumn> ConfigurableColumns { get; } =
        Enum.GetValues<HistoryColumn>().Where(column => column != HistoryColumn.Icon).ToArray();

    private readonly Dictionary<HistoryColumn, bool> _visible = new();
    private readonly Dictionary<HistoryColumn, double> _widths = new();

    public event EventHandler? Changed;

    public HistoryColumnLayout()
    {
        Reset();
    }

    public bool IsVisible(HistoryColumn column) =>
        column == HistoryColumn.Icon || (_visible.TryGetValue(column, out bool visible) && visible);

    public double GetWidth(HistoryColumn column) =>
        column == HistoryColumn.Icon ? IconWidth : _widths.TryGetValue(column, out double width) ? width : MinColumnWidth;

    /// <summary>The visible column that fills the remaining width, or null when only the icon is visible.</summary>
    public HistoryColumn? FillColumn =>
        ConfigurableColumns.Where(IsVisible).Select(column => (HistoryColumn?)column).LastOrDefault();

    public int VisibleCount => ConfigurableColumns.Count(IsVisible);

    public void SetVisible(HistoryColumn column, bool visible)
    {
        if (column == HistoryColumn.Icon || IsVisible(column) == visible)
        {
            return;
        }

        // Keep at least one text column so the list never shows only icons.
        if (!visible && VisibleCount <= 1)
        {
            return;
        }

        _visible[column] = visible;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Resize(HistoryColumn column, double delta)
    {
        if (column == HistoryColumn.Icon)
        {
            return;
        }

        double width = Math.Max(MinColumnWidth, GetWidth(column) + delta);

        if (width != GetWidth(column))
        {
            _widths[column] = width;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Reset()
    {
        foreach ((HistoryColumn column, ColumnDefault value) in Defaults)
        {
            _visible[column] = value.Visible;
            _widths[column] = value.Width;
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Load(IEnumerable<HistoryColumnSettings>? settings)
    {
        Reset();

        if (settings != null)
        {
            foreach (HistoryColumnSettings setting in settings)
            {
                if (setting == null || !Enum.TryParse(setting.Name, out HistoryColumn column) || !Defaults.ContainsKey(column))
                {
                    continue;
                }

                _visible[column] = setting.Visible;
                if (setting.Width > 0)
                {
                    _widths[column] = Math.Max(MinColumnWidth, setting.Width);
                }
            }

            if (VisibleCount == 0)
            {
                _visible[HistoryColumn.FileName] = true;
            }
        }

        Changed?.Invoke(this, EventArgs.Empty);
    }

    public List<HistoryColumnSettings> ToSettings() => ConfigurableColumns.Select(column => new HistoryColumnSettings
    {
        Name = column.ToString(),
        Visible = IsVisible(column),
        Width = Math.Round(GetWidth(column))
    }).ToList();
}
