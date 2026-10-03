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

using Avalonia.Threading;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShareX.HelpersLib;

/// <summary>Registers ShareX's hotkeys with the platform's <see cref="IHotkeyService"/> and raises presses on the UI thread.</summary>
/// <remarks>
/// Replaces the hotkey part of WindowsHotkeyHost. The configurable repeat limit is applied here, as v22 did in its hotkey window, so every
/// platform throttles held keys the same way; the platform services report every press including native repeats.
/// </remarks>
public sealed class HotkeyRegistrar : IDisposable
{
    private readonly IHotkeyService service;
    private readonly Func<int> getRepeatLimit;
    private readonly Action<Action> dispatch;
    private readonly Stopwatch repeatLimitTimer = Stopwatch.StartNew();
    private readonly HashSet<HotkeyInfo> registeredHotkeys = new();
    private ushort lastId;
    private bool disposed;

    /// <summary>Raised on the UI thread with the <see cref="HotkeyInfo.ID"/> of the pressed hotkey.</summary>
    public event Action<ushort> HotkeyPress;

    /// <param name="getRepeatLimit">Milliseconds a hotkey is ignored after it fires; read on every press so settings apply at once. Zero disables it.</param>
    public HotkeyRegistrar(Func<int> getRepeatLimit)
        : this(PlatformServices.Current.Hotkeys, getRepeatLimit, action => Dispatcher.UIThread.Post(action))
    {
    }

    /// <param name="dispatch">Runs the handler on the UI thread. Exceptions thrown there reach the application's handler.</param>
    public HotkeyRegistrar(IHotkeyService service, Func<int> getRepeatLimit, Action<Action> dispatch)
    {
        this.service = service;
        this.getRepeatLimit = getRepeatLimit;
        this.dispatch = dispatch;
        service.HotkeyPressed += OnHotkeyPressed;
    }

    public FeatureSupport Support => service.Support;

    public void RegisterHotkey(HotkeyInfo hotkeyInfo)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (hotkeyInfo == null || hotkeyInfo.Status == HotkeyStatus.Registered)
        {
            return;
        }

        if (!hotkeyInfo.IsValidHotkey)
        {
            hotkeyInfo.Status = HotkeyStatus.NotConfigured;
            return;
        }

        if (hotkeyInfo.ID == 0)
        {
            hotkeyInfo.ID = NextId();
        }

        HotkeyRegistrationStatus status = service.Register(hotkeyInfo.ID, ToPlatformHotkey(hotkeyInfo));

        if (status == HotkeyRegistrationStatus.Registered)
        {
            lock (registeredHotkeys)
            {
                registeredHotkeys.Add(hotkeyInfo);
            }

            hotkeyInfo.Status = HotkeyStatus.Registered;
        }
        else
        {
            DebugHelper.WriteLine($"Unable to register hotkey ({status}): {hotkeyInfo}");
            hotkeyInfo.ID = 0;
            hotkeyInfo.Status = HotkeyStatus.Failed;
        }
    }

    public bool UnregisterHotkey(HotkeyInfo hotkeyInfo)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (hotkeyInfo == null)
        {
            return false;
        }

        if (hotkeyInfo.ID > 0 && service.Unregister(hotkeyInfo.ID))
        {
            lock (registeredHotkeys)
            {
                registeredHotkeys.Remove(hotkeyInfo);
            }

            hotkeyInfo.ID = 0;
            hotkeyInfo.Status = HotkeyStatus.NotConfigured;
            return true;
        }

        hotkeyInfo.Status = HotkeyStatus.Failed;
        return false;
    }

    public void UnregisterAll()
    {
        HotkeyInfo[] hotkeys;

        lock (registeredHotkeys)
        {
            hotkeys = registeredHotkeys.ToArray();
            registeredHotkeys.Clear();
        }

        foreach (HotkeyInfo hotkey in hotkeys)
        {
            service.Unregister(hotkey.ID);
            hotkey.ID = 0;
            hotkey.Status = HotkeyStatus.NotConfigured;
        }
    }

    public void Dispose()
    {
        if (!disposed)
        {
            UnregisterAll();
            service.HotkeyPressed -= OnHotkeyPressed;
            disposed = true;
        }
    }

    /// <summary>Saved hotkeys store Windows virtual key codes and modifier bits, which is what <see cref="PlatformHotkey"/> uses on every platform.</summary>
    public static PlatformHotkey ToPlatformHotkey(HotkeyInfo hotkeyInfo) =>
        new PlatformHotkey((int)hotkeyInfo.KeyCode, (HotkeyModifiers)(int)hotkeyInfo.ModifiersEnum);

    private ushort NextId()
    {
        lock (registeredHotkeys)
        {
            do
            {
                lastId = lastId == ushort.MaxValue ? (ushort)1 : (ushort)(lastId + 1);
            }
            while (registeredHotkeys.Any(x => x.ID == lastId));

            return lastId;
        }
    }

    private void OnHotkeyPressed(object sender, HotkeyPressedEventArgs e)
    {
        ushort id = (ushort)e.Id;

        dispatch(() =>
        {
            if (!disposed && CheckRepeatLimitTime())
            {
                HotkeyPress?.Invoke(id);
            }
        });
    }

    private bool CheckRepeatLimitTime()
    {
        int repeatLimit = getRepeatLimit();

        if (repeatLimit > 0)
        {
            if (repeatLimitTimer.ElapsedMilliseconds < repeatLimit)
            {
                return false;
            }

            repeatLimitTimer.Restart();
        }

        return true;
    }
}
