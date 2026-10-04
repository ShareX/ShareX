#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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

namespace ShareX.HelpersLib
{
    /// <summary>
    /// Runs actions at once, or, while held, queues them and runs them in order on release. Startup holds it so that warnings
    /// raised while initializing appear after the command line work the start was for (a browser extension upload, a capture).
    /// Not thread safe: use it from the UI thread.
    /// </summary>
    public sealed class DeferredActionGate
    {
        private readonly List<Action> queued = new List<Action>();
        private bool held;
        private bool discarded;

        public bool IsHeld => held;

        public void Hold()
        {
            discarded = false;
            held = true;
        }

        /// <summary>Discards pending and future actions until the next hold, so shutdown cannot present late warnings.</summary>
        public void Discard()
        {
            discarded = true;
            held = false;
            queued.Clear();
        }

        public void Run(Action action)
        {
            if (discarded) return;
            if (held)
            {
                queued.Add(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>Stops holding and runs the queued actions in order. Actions they queue run at once.</summary>
        public void Release()
        {
            held = false;
            Action[] actions = queued.ToArray();
            queued.Clear();

            foreach (Action action in actions)
            {
                if (discarded) break;
                action();
            }
        }
    }
}
