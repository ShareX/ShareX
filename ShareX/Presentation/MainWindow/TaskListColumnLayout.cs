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
using System.ComponentModel;
using System.Linq;

namespace ShareX;

/// <summary>
/// Widths of the resizable task list columns. The URL column is not stored: it fills the remaining space.
/// </summary>
public sealed class TaskListColumnLayout : INotifyPropertyChanged
{
    private const double MinColumnWidth = 24;
    private static readonly double[] DefaultWidths = [150, 65, 125, 80, 55, 70];

    private readonly double[] _widths = (double[])DefaultWidths.Clone();

    public double FilenameWidth => _widths[0];
    public double StatusWidth => _widths[1];
    public double ProgressWidth => _widths[2];
    public double SpeedWidth => _widths[3];
    public double ElapsedWidth => _widths[4];
    public double RemainingWidth => _widths[5];

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Load(IReadOnlyList<int>? widths)
    {
        for (int i = 0; i < _widths.Length; i++)
        {
            _widths[i] = widths != null && i < widths.Count && widths[i] > 0
                ? Math.Max(MinColumnWidth, widths[i])
                : DefaultWidths[i];
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    public void Resize(int column, double delta)
    {
        if (column < 0 || column >= _widths.Length)
        {
            return;
        }

        double width = Math.Max(MinColumnWidth, _widths[column] + delta);

        if (width != _widths[column])
        {
            _widths[column] = width;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(GetPropertyName(column)));
        }
    }

    public List<int> ToSettings() => _widths.Select(x => (int)Math.Round(x)).ToList();

    private static string GetPropertyName(int column) => column switch
    {
        0 => nameof(FilenameWidth),
        1 => nameof(StatusWidth),
        2 => nameof(ProgressWidth),
        3 => nameof(SpeedWidth),
        4 => nameof(ElapsedWidth),
        _ => nameof(RemainingWidth)
    };
}
