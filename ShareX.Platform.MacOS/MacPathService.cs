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

namespace ShareX.Platform.MacOS;

/// <summary>Folders following Apple's File System Programming Guide.</summary>
public sealed class MacPathService : IPathService
{
    private readonly string home;

    public MacPathService()
        : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    {
    }

    public MacPathService(string home)
    {
        this.home = home;
    }

    public string LibraryDirectory => Path.Combine(home, "Library");

    public string GetConfigDirectory(string applicationName) => Path.Combine(LibraryDirectory, "Application Support", applicationName);

    public string GetDataDirectory(string applicationName) => GetConfigDirectory(applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(LibraryDirectory, "Caches", applicationName);

    public string GetDefaultPersonalFolder(string applicationName) => GetConfigDirectory(applicationName);

    public string GetPicturesDirectory() => Path.Combine(home, "Pictures");

    public string GetVideosDirectory() => Path.Combine(home, "Movies");

    public string GetDocumentsDirectory() => Path.Combine(home, "Documents");

    public string GetDesktopDirectory() => Path.Combine(home, "Desktop");
}
