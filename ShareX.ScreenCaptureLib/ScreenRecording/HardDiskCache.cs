#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ShareX.ScreenCaptureLib
{
    public class HardDiskCache : ImageCache
    {
        public int Count
        {
            get
            {
                if (indexList != null)
                {
                    return indexList.Count;
                }

                return 0;
            }
        }

        private FileStream fsCache;
        private List<LocationInfo> indexList;

        public HardDiskCache(ScreenRecordingOptions options)
        {
            Options = options;
            FileHelpers.CreateDirectoryFromFilePath(Options.OutputPath);
            fsCache = new FileStream(Options.OutputPath, FileMode.Create, FileAccess.Write, FileShare.Read);
            indexList = new List<LocationInfo>();
        }

        protected override void WriteFrame(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                long position = fsCache.Position;
                ms.CopyStreamTo(fsCache);
                indexList.Add(new LocationInfo(position, fsCache.Length - position));
            }
        }

        public override void Dispose()
        {
            if (fsCache != null)
            {
                fsCache.Dispose();
            }

            base.Dispose();
        }

        public IEnumerable<Image> GetImageEnumerator()
        {
            if (!IsWorking && File.Exists(Options.OutputPath) && indexList != null && indexList.Count > 0)
            {
                using (FileStream fsCache = new FileStream(Options.OutputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    foreach (LocationInfo index in indexList)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            fsCache.CopyStreamTo64(ms, index.Location, (int)index.Length);
                            yield return Image.FromStream(ms);
                        }
                    }
                }
            }
        }
    }
}