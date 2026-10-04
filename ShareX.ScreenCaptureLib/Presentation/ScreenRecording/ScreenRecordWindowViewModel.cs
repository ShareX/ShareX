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
using System.Threading;

namespace ShareX.ScreenCaptureLib;

/// <summary>
/// Stops recording-window callbacks from acting on resources after UI disposal.
/// Dispatchers execute callbacks on the UI thread; workers may queue them from another thread.
/// </summary>
public sealed class ScreenRecordWindowViewModel
{
    private int _closed;

    public bool IsClosed => Volatile.Read(ref _closed) != 0;

    public bool TryClose() => Interlocked.Exchange(ref _closed, 1) == 0;

    public bool TryRun(Action action)
    {
        if (IsClosed) return false;
        action();
        return true;
    }

    public bool Dispatch(Action<Action> dispatch, Action action)
    {
        if (IsClosed) return false;
        // Closing can occur while a callback waits in either the post or invoke queue.
        dispatch(() => TryRun(action));
        return true;
    }
}
