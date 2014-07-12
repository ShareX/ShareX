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

using System;
using System.Collections.Generic;
using HelpersLib;

namespace CodeWorks
{
    public class RegionAreaManager
    {
        public string Text { get; set; }
        public List<RegionArea> RegionAreas { get; private set; }

        public RegionAreaManager(string text)
        {
            Text = text;
        }

        public List<RegionArea> GetRegionAreas()
        {
            RegionAreas = new List<RegionArea>();

            if (!string.IsNullOrEmpty(Text))
            {
                bool searchingRegionStart = true;

                StringLineReader reader = new StringLineReader(Text);
                string line;
                RegionArea regionArea = null;
                int index = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (searchingRegionStart)
                    {
                        if (line.StartsWith("#region", StringComparison.InvariantCulture))
                        {
                            searchingRegionStart = false;
                            regionArea = new RegionArea(this, index);
                            if (line.Length > 8) regionArea.RegionName = line.Substring(8);
                        }
                    }
                    else
                    {
                        if (line.StartsWith("#endregion", StringComparison.InvariantCulture))
                        {
                            searchingRegionStart = true;
                            regionArea.RegionEndIndex = reader.Position - 1;
                            RegionAreas.Add(regionArea);
                        }
                    }

                    index = reader.Position;
                }
            }

            return RegionAreas;
        }
    }
}