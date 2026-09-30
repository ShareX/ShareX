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
using System.IO;

namespace ShareX.Desktop.Settings;

/// <summary>Reloads DesktopSettings.json when it changes, so edits apply to the next capture without restarting.</summary>
public sealed class DesktopSettingsStore
{
    private readonly string path;
    private readonly object sync = new object();
    private DesktopSettings current;
    private DateTime loadedWriteTime;

    public DesktopSettingsStore(string path)
    {
        this.path = path;
        loadedWriteTime = GetWriteTime();
        current = DesktopSettings.Load(path);
    }

    public string FilePath => path;

    public DesktopSettings Current
    {
        get
        {
            lock (sync)
            {
                DateTime writeTime = GetWriteTime();

                if (writeTime != loadedWriteTime)
                {
                    loadedWriteTime = writeTime;
                    current = DesktopSettings.Load(path);
                }

                return current;
            }
        }
    }

    private DateTime GetWriteTime()
    {
        try
        {
            return File.Exists(path) ? File.GetLastWriteTimeUtc(path) : DateTime.MinValue;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return loadedWriteTime;
        }
    }
}
