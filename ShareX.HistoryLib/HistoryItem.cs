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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShareX.HistoryLib
{
    public class HistoryItem
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string DeletionURL { get; set; }
        public string ShortenedURL { get; set; }

        [Browsable(false)]
        public Dictionary<string, string> Tags { get; set; }

        [JsonIgnore, DisplayName("Tags[WindowTitle]")]
        public string TagsWindowTitle
        {
            get
            {
                if (Tags != null && Tags.TryGetValue("WindowTitle", out string value))
                {
                    return value;
                }

                return null;
            }
        }

        [JsonIgnore, DisplayName("Tags[ProcessName]")]
        public string TagsProcessName
        {
            get
            {
                if (Tags != null && Tags.TryGetValue("ProcessName", out string value))
                {
                    return value;
                }

                return null;
            }
        }
    }
}