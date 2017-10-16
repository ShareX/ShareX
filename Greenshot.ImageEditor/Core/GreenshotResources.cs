/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.ComponentModel;
using System.Drawing;

namespace GreenshotPlugin.Core
{
    /// <summary>
    /// Centralized storage of the icons & bitmaps
    /// </summary>
    public static class GreenshotResources
    {
        private static readonly ComponentResourceManager greenshotResources = new ComponentResourceManager(typeof(GreenshotResources));

        public static Image getImage(string imageName)
        {
            return (Image)greenshotResources.GetObject(imageName);
        }

        public static Icon getIcon(string imageName)
        {
            return (Icon)greenshotResources.GetObject(imageName);
        }

        public static Icon getGreenshotIcon()
        {
            return getIcon("Greenshot.Icon");
        }

        public static Image getGreenshotImage()
        {
            return getImage("Greenshot.Image");
        }
    }
}