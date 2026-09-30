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

namespace ShareX.Platform.Windows;

/// <summary>Known folders on Windows. The personal folder stays in Documents\ShareX so existing installs keep their settings.</summary>
public sealed class WindowsPathService : IPathService
{
    public string GetConfigDirectory(string applicationName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);

    public string GetDataDirectory(string applicationName) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), applicationName);

    public string GetCacheDirectory(string applicationName) => Path.Combine(GetDataDirectory(applicationName), "Cache");

    public string GetDefaultPersonalFolder(string applicationName) => Path.Combine(GetDocumentsDirectory(), applicationName);

    public string GetPicturesDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

    public string GetVideosDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

    public string GetDocumentsDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    public string GetDesktopDirectory() => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
}
