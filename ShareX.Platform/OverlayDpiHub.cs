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

namespace ShareX.Platform;

/// <summary>
/// DPI changes for ShareX's own overlays. The Windows window procedure reports them here; the geometry log subscribes.
/// Events that arrive before a subscriber are kept and delivered on the next subscribe, so a change during window
/// creation is not lost.
/// </summary>
public static class OverlayDpiHub
{
    private static readonly object gate = new();
    private static readonly List<PendingDpi> pending = new();
    private static Action<long, uint, bool>? dpiChanged;
    private static Action<long>? windowDestroyed;

    public static event Action<long, uint, bool>? DpiChanged
    {
        add
        {
            lock (gate)
            {
                dpiChanged += value;
                foreach (PendingDpi change in pending.ToArray())
                {
                    if (!change.Destroyed)
                    {
                        value?.Invoke(change.Handle, change.Dpi, change.Suppressed);
                    }
                }
            }
        }
        remove
        {
            lock (gate)
            {
                dpiChanged -= value;
            }
        }
    }

    public static event Action<long>? WindowDestroyed
    {
        add
        {
            lock (gate)
            {
                windowDestroyed += value;
            }
        }
        remove
        {
            lock (gate)
            {
                windowDestroyed -= value;
            }
        }
    }

    public static void NoteDpiChanged(long windowHandle, uint newDpi, bool suppressed)
    {
        Action<long, uint, bool>? handler;
        lock (gate)
        {
            handler = dpiChanged;
            if (handler == null)
            {
                pending.Add(new PendingDpi(windowHandle, newDpi, suppressed, false));
            }
        }

        handler?.Invoke(windowHandle, newDpi, suppressed);
    }

    public static void NoteWindowDestroyed(long windowHandle)
    {
        Action<long>? handler;
        lock (gate)
        {
            handler = windowDestroyed;
            if (handler == null)
            {
                pending.Add(new PendingDpi(windowHandle, 0, false, true));
            }
            else
            {
                pending.RemoveAll(change => change.Handle == windowHandle);
            }
        }

        handler?.Invoke(windowHandle);
    }

    private readonly record struct PendingDpi(long Handle, uint Dpi, bool Suppressed, bool Destroyed);
}
