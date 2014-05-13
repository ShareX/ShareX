#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (C) 2007-2014 ShareX Developers

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

namespace CodeWorks
{
    public class RegionArea
    {
        public int RegionStartIndex { get; set; }
        public int RegionEndIndex { get; set; }

        public int RegionLength
        {
            get { return RegionEndIndex - RegionStartIndex + 1; }
        }

        public string RegionName { get; set; }

        public int RegionIndexOffset { get; set; }

        private RegionAreaManager manager;

        public RegionArea(RegionAreaManager regionAreaManager)
        {
            manager = regionAreaManager;
        }

        public RegionArea(RegionAreaManager regionAreaManager, int regionStartIndex)
            : this(regionAreaManager)
        {
            RegionStartIndex = regionStartIndex;
        }

        public string GetRegionText()
        {
            return manager.Text.Substring(RegionStartIndex + RegionIndexOffset, RegionLength);
        }

        public string RemoveRegionText()
        {
            return manager.Text.Remove(RegionStartIndex + RegionIndexOffset, RegionLength);
        }

        public string ReplaceRegionText(string replaceWith)
        {
            string result = string.Empty;

            if (RegionStartIndex + RegionIndexOffset > 0)
            {
                result = manager.Text.Substring(0, RegionStartIndex + RegionIndexOffset);
            }

            result += replaceWith + manager.Text.Substring(RegionEndIndex + RegionIndexOffset);

            return result;
        }
    }
}