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

using Avalonia.Media.Imaging;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShareX.HelpersLib
{
    /// <summary>Clipboard and file manager helpers that go through <see cref="PlatformServices"/>, so they work on every OS.</summary>
    public static class PortableShell
    {
        public static void CopyText(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                Observe(PlatformServices.Current.Clipboard.SetTextAsync(text), "copy text");
            }
        }

        public static void CopyTextFromFile(string path)
        {
            if (File.Exists(path))
            {
                CopyText(File.ReadAllText(path));
            }
        }

        public static void CopyFiles(IEnumerable<string> paths)
        {
            string[] files = paths.Where(File.Exists).ToArray();

            if (files.Length > 0)
            {
                Observe(PlatformServices.Current.Clipboard.SetFilesAsync(files), "copy files");
            }
        }

        /// <summary>Decodes any image format Avalonia supports and puts it on the clipboard as a PNG.</summary>
        public static void CopyImageFromFile(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            using Bitmap bitmap = new Bitmap(path);
            using MemoryStream stream = new MemoryStream();
            bitmap.Save(stream);
            Observe(PlatformServices.Current.Clipboard.SetImageAsync(stream.ToArray()), "copy image");
        }

        public static void CopyImage(SkiaSharp.SKBitmap bitmap)
        {
            using SkiaSharp.SKData data = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
            Observe(PlatformServices.Current.Clipboard.SetImageAsync(data.ToArray()), "copy image");
        }

        public static bool OpenFolderWithFile(string path) => FileHelpers.OpenFolderWithFile(path);

        /// <summary>Clipboard writes finish in the background. A failure is logged instead of being lost with the task.</summary>
        private static async void Observe(Task<bool> task, string action)
        {
            try
            {
                if (!await task.ConfigureAwait(false))
                {
                    DebugHelper.WriteLine($"Clipboard: could not {action}. {PlatformServices.Current.Clipboard.Support.Reason}");
                }
            }
            catch (Exception e)
            {
                DebugHelper.WriteException(e, $"Clipboard: could not {action}");
            }
        }
    }
}
