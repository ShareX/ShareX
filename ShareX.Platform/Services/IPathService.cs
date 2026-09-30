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

/// <summary>Resolves per user folders following each platform's conventions.</summary>
/// <remarks>
/// Windows: %APPDATA%, %LOCALAPPDATA%, Documents.
/// macOS: ~/Library/Application Support, ~/Library/Caches.
/// Linux: $XDG_CONFIG_HOME, $XDG_DATA_HOME, $XDG_CACHE_HOME and xdg-user-dirs.
/// </remarks>
public interface IPathService
{
    /// <summary>Folder for settings files, for example %APPDATA%\ShareX or $XDG_CONFIG_HOME/ShareX.</summary>
    string GetConfigDirectory(string applicationName);

    /// <summary>Folder for application data such as history databases.</summary>
    string GetDataDirectory(string applicationName);

    /// <summary>Folder for disposable caches.</summary>
    string GetCacheDirectory(string applicationName);

    /// <summary>The folder ShareX uses for its settings, history and logs when the user has not chosen a custom one.</summary>
    string GetDefaultPersonalFolder(string applicationName);

    /// <summary>The user's pictures folder, used as the parent of the screenshots folder.</summary>
    string GetPicturesDirectory();

    /// <summary>The user's videos folder.</summary>
    string GetVideosDirectory();

    /// <summary>The user's documents folder.</summary>
    string GetDocumentsDirectory();

    /// <summary>The user's desktop folder.</summary>
    string GetDesktopDirectory();
}
