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

using ShareX.HelpersLib;

namespace ShareX.ImageEffectsLib
{
    internal static class ImageEffectPathHelpers
    {
        public static bool TryGetSafeLocalFilePath(string path, out string safePath)
        {
            return TryGetSafeLocalPath(path, false, out safePath);
        }

        public static bool TryGetSafeLocalFolderPath(string path, out string safePath)
        {
            return TryGetSafeLocalPath(path, true, out safePath);
        }

        public static bool IsPathInFolder(string path, string folderPath)
        {
            if (!TryGetSafeLocalPath(path, false, out string fullPath) ||
                !TryGetSafeLocalPath(folderPath, true, out string fullFolderPath))
            {
                return false;
            }

            fullFolderPath = Path.TrimEndingDirectorySeparator(fullFolderPath);
            fullPath = Path.TrimEndingDirectorySeparator(fullPath);

            return fullPath.Equals(fullFolderPath, StringComparison.OrdinalIgnoreCase) ||
                fullPath.StartsWith(fullFolderPath + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryGetSafeLocalPath(string path, bool isFolder, out string safePath)
        {
            safePath = null;

            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            try
            {
                path = FileHelpers.ExpandFolderVariables(path, true);

                if (Uri.TryCreate(path, UriKind.Absolute, out Uri uri))
                {
                    if (!uri.IsFile || uri.IsUnc)
                    {
                        return false;
                    }

                    path = uri.LocalPath;
                }

                if (IsRemoteOrDevicePath(path))
                {
                    return false;
                }

                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                safePath = Path.GetFullPath(path);

                if (IsRemoteOrDevicePath(safePath) || HasReparsePoint(safePath, isFolder))
                {
                    safePath = null;
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e);
            }

            return false;
        }

        private static bool IsRemoteOrDevicePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return true;
            }

            path = path.Trim();

            if (path.StartsWith(@"\\", StringComparison.Ordinal) ||
                path.StartsWith("//", StringComparison.Ordinal))
            {
                return true;
            }

            string root = Path.GetPathRoot(path);
            return !string.IsNullOrEmpty(root) &&
                (root.StartsWith(@"\\", StringComparison.Ordinal) ||
                root.StartsWith("//", StringComparison.Ordinal));
        }

        private static bool HasReparsePoint(string path, bool isFolder)
        {
            string currentPath = isFolder ? path : Path.GetDirectoryName(path);

            while (!string.IsNullOrEmpty(currentPath))
            {
                if (Directory.Exists(currentPath) && HasReparsePoint(new DirectoryInfo(currentPath).Attributes))
                {
                    return true;
                }

                DirectoryInfo parent = Directory.GetParent(currentPath);

                if (parent == null)
                {
                    break;
                }

                currentPath = parent.FullName;
            }

            if (!isFolder && File.Exists(path) && HasReparsePoint(File.GetAttributes(path)))
            {
                return true;
            }

            return false;
        }

        private static bool HasReparsePoint(FileAttributes attributes)
        {
            return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }
    }
}
