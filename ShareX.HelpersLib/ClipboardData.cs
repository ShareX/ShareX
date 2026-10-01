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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HelpersLib;

// A snapshot owns managed values, so callers never retain native clipboard objects.
public sealed class ClipboardData
{
    private readonly Dictionary<string, object> values = new(StringComparer.OrdinalIgnoreCase);
    public string[] GetFormats() => values.Keys.ToArray();
    public object GetData(string format) => values.TryGetValue(format, out object value) ? value : null;
    public void SetData(string format, object value) => values[format] = value;
}

public static class ClipboardDataFormats
{
    public const string Text = "Text";
    public const string UnicodeText = "UnicodeText";
    public const string FileDrop = "FileDrop";
    public const string Bitmap = "Bitmap";
    public const string Dib = "DeviceIndependentBitmap";
    public const string Html = "HTML Format";
}
