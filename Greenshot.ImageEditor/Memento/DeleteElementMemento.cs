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

using Greenshot.Drawing;
using Greenshot.Plugin.Drawing;
using System;

namespace Greenshot.Memento
{
    /// <summary>
    /// The DeleteElementMemento makes it possible to undo deleting an element
    /// </summary>
    public class DeleteElementMemento : IMemento
    {
        private IDrawableContainer drawableContainer;
        private Surface surface;

        public DeleteElementMemento(Surface surface, IDrawableContainer drawableContainer)
        {
            this.surface = surface;
            this.drawableContainer = drawableContainer;
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
                if (drawableContainer != null)
                {
                    drawableContainer.Dispose();
                    drawableContainer = null;
                }
            }
        }

        public bool Merge(IMemento otherMemento)
        {
            return false;
        }

        public IMemento Restore()
        {
            // Before
            drawableContainer.Invalidate();

            AddElementMemento oldState = new AddElementMemento(surface, drawableContainer);
            surface.AddElement(drawableContainer, false);
            // The container has a selected flag which represents the state at the moment it was deleted.
            if (drawableContainer.Selected)
            {
                surface.SelectElement(drawableContainer);
            }

            // After
            drawableContainer.Invalidate();
            return oldState;
        }
    }
}