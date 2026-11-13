#region License Information (GPL v3)
/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team
*/
#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;

namespace ShareX.HelpersLib;

public sealed class LinuxHotkeyHost : IHotkeyHost
{
    private readonly HashSet<HotkeyInfo> registeredHotkeys = new();

    public event HotkeyEventHandler HotkeyPress;
    public event EventHandler Closed;
    public event EventHandler<NativeWindowMessageEventArgs> NativeMessageReceived;

    public IntPtr Handle => IntPtr.Zero;
    public bool IsDisposed { get; private set; }
    public int HotkeyRepeatLimit { get; set; } = 1000;

    public LinuxHotkeyHost()
    {
    }

    public void Initialize()
    {
    }

    public void RegisterHotkey(HotkeyInfo hotkeyInfo)
    {
        if (hotkeyInfo == null || hotkeyInfo.Status == HotkeyStatus.Registered) return;
        if (!hotkeyInfo.IsValidHotkey)
        {
            hotkeyInfo.Status = HotkeyStatus.NotConfigured;
            return;
        }

        if (hotkeyInfo.ID == 0)
        {
            hotkeyInfo.ID = (ushort)(registeredHotkeys.Count + 1);
        }
        registeredHotkeys.Add(hotkeyInfo);
        hotkeyInfo.Status = HotkeyStatus.Registered;
    }

    public bool UnregisterHotkey(HotkeyInfo hotkeyInfo)
    {
        if (hotkeyInfo == null) return false;
        registeredHotkeys.Remove(hotkeyInfo);
        hotkeyInfo.Status = HotkeyStatus.NotConfigured;
        return true;
    }

    public void TriggerHotkey(ushort id, InputKey key, Modifiers modifier)
    {
        HotkeyPress?.Invoke(id, key, modifier);
    }

    public void Close()
    {
        if (IsDisposed) return;
        Closed?.Invoke(this, EventArgs.Empty);
        registeredHotkeys.Clear();
        IsDisposed = true;
    }

    public void Dispose() => Close();
}
