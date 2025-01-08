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
using System;

namespace ShareX
{
    public class RecentTask
    {
        public string FilePath { get; set; }

        public string FileName
        {
            get
            {
                string text = "";

                if (!string.IsNullOrEmpty(FilePath))
                {
                    text = FilePath;
                }
                else if (!string.IsNullOrEmpty(URL))
                {
                    text = URL;
                }

                return FileHelpers.GetFileNameSafe(text);
            }
        }

        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string DeletionURL { get; set; }
        public string ShortenedURL { get; set; }

        public DateTime Time { get; set; }

        public string TrayMenuText
        {
            get
            {
                string text = ToString().Truncate(50, "...", false);

                return string.Format("[{0:HH:mm:ss}] {1}", Time, text);
            }
        }

        public RecentTask()
        {
            Time = DateTime.Now;
        }

        public override string ToString()
        {
            string text = "";

            if (!string.IsNullOrEmpty(ShortenedURL))
            {
                text = ShortenedURL;
            }
            else if (!string.IsNullOrEmpty(URL))
            {
                text = URL;
            }
            else if (!string.IsNullOrEmpty(FilePath))
            {
                text = FilePath;
            }

            return text;
        }
    }
}