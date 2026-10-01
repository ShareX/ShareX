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

using MessageBox = ShareX.AvaloniaUI.MessageBox;
using MessageBoxButtons = ShareX.AvaloniaUI.MessageBoxButtons;
using MessageBoxIcon = ShareX.AvaloniaUI.MessageBoxIcon;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using System;

namespace ShareX.HelpersLib
{
    // Windows-only members of FileHelpers, kept in the same namespace so existing call sites keep compiling.
    public static class FileHelpersWindows
    {
        extension(FileHelpers)
        {
            public static string BrowseFile(IWin32Window window = null, string title = null)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        ofd.Title = title;
                    }

                    if (ofd.ShowDialog(window) == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;

                        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        {
                            return filePath;
                        }
                    }
                }

                return null;
            }

            public static bool BrowseFile(TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false, string filter = "")
            {
                return BrowseFile("ShareX - " + Localization.Strings.Helpers_BrowseFile_Choose_file, tb, initialDirectory, detectSpecialFolders, filter);
            }

            public static bool BrowseFile(string title, TextBox tb, string initialDirectory = "", bool detectSpecialFolders = false, string filter = "")
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = title;
                    ofd.Filter = filter;

                    try
                    {
                        string path = tb.Text;

                        if (detectSpecialFolders)
                        {
                            path = FileHelpers.ExpandFolderVariables(path);
                        }

                        if (!string.IsNullOrEmpty(path))
                        {
                            path = Path.GetDirectoryName(path);

                            if (Directory.Exists(path))
                            {
                                ofd.InitialDirectory = path;
                            }
                        }
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(ofd.InitialDirectory) && !string.IsNullOrEmpty(initialDirectory))
                        {
                            ofd.InitialDirectory = initialDirectory;
                        }
                    }

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = ofd.FileName;

                        if (detectSpecialFolders)
                        {
                            fileName = FileHelpers.GetVariableFolderPath(fileName);
                        }

                        tb.Text = fileName;

                        return true;
                    }
                }

                return false;
            }

            public static bool BrowseFolder(TextBox tb, string initialDirectory = null, bool detectSpecialFolders = false)
            {
                return BrowseFolder("ShareX - " + Localization.Strings.Helpers_BrowseFolder_Choose_folder, tb, initialDirectory, detectSpecialFolders);
            }

            public static bool BrowseFolder(string title, TextBox tb, string initialDirectory = null, bool detectSpecialFolders = false)
            {
                string path = tb.Text;

                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                {
                    initialDirectory = path;
                }

                string selectedPath = BrowseFolder(title, initialDirectory);

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    tb.Text = detectSpecialFolders ? FileHelpers.GetVariableFolderPath(selectedPath) : selectedPath;
                    return true;
                }

                return false;
            }

            public static string BrowseFolder(string title = null, string initialDirectory = null)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        fbd.Description = title;
                        fbd.UseDescriptionForTitle = true;
                    }

                    if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
                    {
                        fbd.InitialDirectory = initialDirectory;
                    }

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        return fbd.SelectedPath;
                    }
                }

                return null;
            }

        }
    }
}
