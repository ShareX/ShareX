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

using Greenshot.Drawing;
using System;

namespace Greenshot.Memento
{
    /// <summary>
    /// The TextChangeMemento makes it possible to undo-redo an IDrawableContainer move
    /// </summary>
    public class TextChangeMemento : IMemento
    {
        private TextContainer textContainer;
        private string oldText;

        public TextChangeMemento(TextContainer textContainer)
        {
            this.textContainer = textContainer;
            oldText = textContainer.Text;
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
                textContainer = null;
            }
        }

        public bool Merge(IMemento otherMemento)
        {
            TextChangeMemento other = otherMemento as TextChangeMemento;
            if (other != null)
            {
                if (other.textContainer.Equals(textContainer))
                {
                    // Match, do not store anything as the initial state is what we want.
                    return true;
                }
            }
            return false;
        }

        public IMemento Restore()
        {
            // Before
            textContainer.Invalidate();
            TextChangeMemento oldState = new TextChangeMemento(textContainer);
            textContainer.ChangeText(oldText, false);
            // After
            textContainer.Invalidate();
            return oldState;
        }
    }
}