/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
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

using Greenshot.Configuration;
using Greenshot.Drawing;
using System;
using System.Drawing;

namespace Greenshot.Memento
{
    /// <summary>
    /// The SurfaceCropMemento makes it possible to undo-redo an surface crop
    /// </summary>
    public class SurfaceBackgroundChangeMemento : IMemento
    {
        private Image image;
        private Surface surface;
        private Point offset;

        public SurfaceBackgroundChangeMemento(Surface surface, Point offset)
        {
            this.surface = surface;
            image = surface.Image;
            this.offset = new Point(-offset.X, -offset.Y);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
                surface = null;
            }
        }

        public bool Merge(IMemento otherMemento)
        {
            return false;
        }

        public LangKey ActionLanguageKey
        {
            get
            {
                //return LangKey.editor_crop;
                return LangKey.none;
            }
        }

        public IMemento Restore()
        {
            SurfaceBackgroundChangeMemento oldState = new SurfaceBackgroundChangeMemento(surface, offset);
            surface.UndoBackgroundChange(image, offset);
            surface.Invalidate();
            return oldState;
        }
    }
}