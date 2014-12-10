/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2014 Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Plugin.Drawing;
using GreenshotPlugin;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Greenshot.Drawing
{
    /// <summary>
    /// Description of IconContainer.
    /// </summary>
    [Serializable]
    public class IconContainer : DrawableContainer, IIconContainer
    {
        protected Icon icon;

        public IconContainer(Surface parent)
            : base(parent)
        {
        }

        public IconContainer(Surface parent, string filename)
            : base(parent)
        {
            Load(filename);
        }

        public Icon Icon
        {
            set
            {
                if (icon != null)
                {
                    icon.Dispose();
                }
                icon = (Icon)value.Clone();
                Width = value.Width;
                Height = value.Height;
            }
            get
            {
                return icon;
            }
        }

        /**
         * This Dispose is called from the Dispose and the Destructor.
         * When disposing==true all non-managed resources should be freed too!
         */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (icon != null)
                {
                    icon.Dispose();
                }
            }
            icon = null;
            base.Dispose(disposing);
        }

        public void Load(string filename)
        {
            if (File.Exists(filename))
            {
                using (Icon fileIcon = new Icon(filename))
                {
                    Icon = fileIcon;
                    LOG.Debug("Loaded file: " + filename + " with resolution: " + Height + "," + Width);
                }
            }
        }

        public override void Draw(Graphics graphics, RenderMode rm)
        {
            if (icon != null)
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawIcon(icon, Bounds);
            }
        }

        public override bool HasDefaultSize
        {
            get
            {
                return true;
            }
        }

        public override Size DefaultSize
        {
            get
            {
                return icon.Size;
            }
        }
    }
}