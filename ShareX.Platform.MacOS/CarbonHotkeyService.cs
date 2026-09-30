using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ShareX.Platform.MacOS;

/// <summary>Global hotkeys with Carbon's RegisterEventHotKey, which needs no Accessibility permission.</summary>
/// <remarks>
/// Register from the main thread. Events are delivered by the application's main run loop, so
/// <see cref="HotkeyPressed"/> is raised on the main thread while the UI toolkit (Avalonia) runs it.
/// </remarks>
public sealed unsafe class CarbonHotkeyService : IHotkeyService
{
    private const uint Signature = 0x53485258; // 'SHRX'

    private readonly Dictionary<int, (IntPtr Ref, PlatformHotkey Hotkey)> registrations = new Dictionary<int, (IntPtr, PlatformHotkey)>();
    private GCHandle self;
    private IntPtr handlerRef;

    public CarbonHotkeyService()
    {
        self = GCHandle.Alloc(this);
        Carbon.EventTypeSpec spec = new Carbon.EventTypeSpec { eventClass = Carbon.kEventClassKeyboard, eventKind = Carbon.kEventHotKeyPressed };
        int status = Carbon.InstallEventHandler(Carbon.GetApplicationEventTarget(), (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, int>)&OnHotkeyEvent,
            1, &spec, GCHandle.ToIntPtr(self), out handlerRef);

        Support = status == 0 ? FeatureSupport.Supported : FeatureSupport.NotSupported($"InstallEventHandler failed with status {status}.");
    }

    public FeatureSupport Support { get; }

    public event EventHandler<HotkeyPressedEventArgs>? HotkeyPressed;

    public HotkeyRegistrationStatus Register(int id, PlatformHotkey hotkey)
    {
        if (!Support.IsSupported)
        {
            return HotkeyRegistrationStatus.NotSupported;
        }

        if (!MacKeyMap.TryGetKeyCode(hotkey.KeyCode, out uint keyCode))
        {
            return HotkeyRegistrationStatus.UnsupportedKey;
        }

        Unregister(id);

        Carbon.EventHotKeyID hotKeyId = new Carbon.EventHotKeyID { signature = Signature, id = (uint)id };
        int status = Carbon.RegisterEventHotKey(keyCode, MacKeyMap.ToCarbonModifiers(hotkey.Modifiers), hotKeyId,
            Carbon.GetApplicationEventTarget(), 0, out IntPtr hotKeyRef);

        switch (status)
        {
            case 0:
                registrations[id] = (hotKeyRef, hotkey);
                return HotkeyRegistrationStatus.Registered;
            case Carbon.eventHotKeyExistsErr:
                return HotkeyRegistrationStatus.InUse;
            case Carbon.eventHotKeyInvalidErr:
                return HotkeyRegistrationStatus.UnsupportedKey;
            default:
                return HotkeyRegistrationStatus.Failed;
        }
    }

    public bool Unregister(int id)
    {
        if (!registrations.Remove(id, out (IntPtr Ref, PlatformHotkey Hotkey) registration))
        {
            return false;
        }

        Carbon.UnregisterEventHotKey(registration.Ref);
        return true;
    }

    public void UnregisterAll()
    {
        foreach (int id in new List<int>(registrations.Keys))
        {
            Unregister(id);
        }
    }

    [UnmanagedCallersOnly]
    private static int OnHotkeyEvent(IntPtr nextHandler, IntPtr eventRef, IntPtr userData)
    {
        Carbon.EventHotKeyID hotKeyId;

        if (Carbon.GetEventParameter(eventRef, Carbon.kEventParamDirectObject, Carbon.typeEventHotKeyID, IntPtr.Zero,
            (nuint)sizeof(Carbon.EventHotKeyID), IntPtr.Zero, &hotKeyId) == 0 && hotKeyId.signature == Signature &&
            GCHandle.FromIntPtr(userData).Target is CarbonHotkeyService service)
        {
            try
            {
                service.OnHotkey((int)hotKeyId.id);
            }
            catch (Exception)
            {
                // Exceptions must not cross into native code.
            }
        }

        return 0;
    }

    private void OnHotkey(int id)
    {
        if (registrations.TryGetValue(id, out (IntPtr Ref, PlatformHotkey Hotkey) registration))
        {
            HotkeyPressed?.Invoke(this, new HotkeyPressedEventArgs(id, registration.Hotkey));
        }
    }

    public void Dispose()
    {
        UnregisterAll();

        if (handlerRef != IntPtr.Zero)
        {
            Carbon.RemoveEventHandler(handlerRef);
            handlerRef = IntPtr.Zero;
        }

        if (self.IsAllocated)
        {
            self.Free();
        }
    }
}
