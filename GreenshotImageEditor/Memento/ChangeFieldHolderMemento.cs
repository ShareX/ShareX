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
using Greenshot.Drawing.Fields;
using Greenshot.Plugin.Drawing;
using System;

namespace Greenshot.Memento
{
    /// <summary>
    /// The ChangeFieldHolderMemento makes it possible to undo-redo an IDrawableContainer move
    /// </summary>
    public class ChangeFieldHolderMemento : IMemento
    {
        private IDrawableContainer drawableContainer;
        private Field fieldToBeChanged;
        private object oldValue;

        public ChangeFieldHolderMemento(IDrawableContainer drawableContainer, Field fieldToBeChanged)
        {
            this.drawableContainer = drawableContainer;
            this.fieldToBeChanged = fieldToBeChanged;
            oldValue = fieldToBeChanged.Value;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            //if (disposing) { }
            drawableContainer = null;
        }

        public LangKey ActionLanguageKey
        {
            get
            {
                return LangKey.none;
            }
        }

        public bool Merge(IMemento otherMemento)
        {
            ChangeFieldHolderMemento other = otherMemento as ChangeFieldHolderMemento;
            if (other != null)
            {
                if (other.drawableContainer.Equals(drawableContainer))
                {
                    if (other.fieldToBeChanged.Equals(fieldToBeChanged))
                    {
                        // Match, do not store anything as the initial state is what we want.
                        return true;
                    }
                }
            }
            return false;
        }

        public IMemento Restore()
        {
            // Before
            drawableContainer.Invalidate();
            ChangeFieldHolderMemento oldState = new ChangeFieldHolderMemento(drawableContainer, fieldToBeChanged);
            fieldToBeChanged.Value = oldValue;
            // After
            drawableContainer.Invalidate();
            return oldState;
        }
    }
}