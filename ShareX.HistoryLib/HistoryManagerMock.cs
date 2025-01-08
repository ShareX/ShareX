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
using System.Collections.Generic;
using System.Linq;

namespace ShareX.HistoryLib
{
    public class HistoryManagerMock : HistoryManager
    {
        private int itemCount = 10000;

        public HistoryManagerMock(string filePath) : base(filePath)
        {
        }

        protected override List<HistoryItem> Load(string filePath)
        {
            List<HistoryItem> items = new List<HistoryItem>();

            for (int i = 0; i < itemCount; i++)
            {
                items.Add(CreateMockHistoryItem());
            }

            return items.OrderBy(x => x.DateTime).ToList();
        }

        private HistoryItem CreateMockHistoryItem()
        {
            string fileName = $"ShareX_{Helpers.GetRandomAlphanumeric(10)}.png";

            HistoryItem historyItem = new HistoryItem()
            {
                FileName = fileName,
                FilePath = @"..\..\..\ShareX.HelpersLib\Resources\ShareX_Logo.png",
                DateTime = DateTime.Now.AddSeconds(-RandomFast.Next(0, 1000000)),
                Type = "Image",
                Host = "Amazon S3",
                URL = "https://i.example.com/" + fileName,
                ThumbnailURL = "https://t.example.com/" + fileName,
                DeletionURL = "https://d.example.com/" + fileName,
                ShortenedURL = "https://s.example.com/" + fileName
            };

            return historyItem;
        }

        protected override bool Append(string filePath, IEnumerable<HistoryItem> historyItems)
        {
            return true;
        }
    }
}