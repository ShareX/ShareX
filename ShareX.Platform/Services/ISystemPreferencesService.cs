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

namespace ShareX.Platform;

/// <summary>User preferences the operating system owns, so ShareX's windows behave like every other window.</summary>
public interface ISystemPreferencesService
{
    /// <summary>Lines to scroll for one mouse wheel notch. Windows lets the user change it; elsewhere it is 3.</summary>
    int WheelScrollLines { get; }

    /// <summary>
    /// Whether the system surfaces that hold tray icons (the Windows task bar) use the light theme, so tray glyphs can be drawn in a
    /// contrasting colour. Null where the platform does not say; callers then follow ShareX's own theme.
    /// </summary>
    bool? SystemUsesLightTheme { get; }
}

/// <summary>The defaults used on platforms that do not expose these preferences.</summary>
public sealed class DefaultSystemPreferencesService : ISystemPreferencesService
{
    public const int DefaultWheelScrollLines = 3;

    public int WheelScrollLines => DefaultWheelScrollLines;

    public bool? SystemUsesLightTheme => null;
}
