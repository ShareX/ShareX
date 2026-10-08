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

using ShareX.Platform.MacOS.Native;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ShareX.Platform.MacOS;

/// <summary>
/// ShareX's menu bar icon as an NSStatusItem of its own. Avalonia's tray icon opens its menu on every click; this one reports left,
/// right (or Control) and middle clicks, so a left click runs the configured action and a right click shows ShareX's menu, as in
/// the Windows notification area.
/// </summary>
[SupportedOSPlatform("macos")]
public sealed class MacTrayService : ITrayService
{
    public FeatureSupport Support => FeatureSupport.Supported;

    public FeatureSupport IconAreaSupport => FeatureSupport.Supported;

    public FeatureSupport MiddleClickSupport => FeatureSupport.Supported;

    public FeatureSupport RightButtonSupport => FeatureSupport.Supported;

    /// <summary>Null, for Avalonia's tray icon, when the status item cannot be created.</summary>
    public ITraySession? CreateSession(Action<Exception> onUnhandledException)
    {
        try
        {
            return new MacTraySession(onUnhandledException);
        }
        catch (Exception e) when (e is EntryPointNotFoundException or InvalidOperationException)
        {
            return null;
        }
    }
}

[SupportedOSPlatform("macos")]
internal sealed unsafe class MacTraySession : ITraySession
{
    // NSEventTypeLeftMouseUp, NSEventTypeRightMouseUp, NSEventTypeOtherMouseUp and their masks.
    private const nint LeftMouseUp = 2, RightMouseUp = 4, OtherMouseUp = 26;
    private const nint ActionMask = (1 << 2) | (1 << 4) | (1 << 26);
    private const nint ControlKeyFlag = 1 << 18;
    private const nint VariableStatusItemLength = -1;
    // Menu bar icons are 18 points high.
    private const double IconSize = 18;

    private static readonly Dictionary<IntPtr, MacTraySession> sessions = new Dictionary<IntPtr, MacTraySession>();
    private static IntPtr targetClass;

    private readonly Action<Exception> onUnhandledException;
    private IntPtr statusItem;
    private IntPtr target;
    private string toolTipText = string.Empty;
    private bool visible;

    public event Action<TrayMouseButton>? MouseDown;
    public event Action<TrayMouseButton>? MouseUp;
    // macOS does not ask menu bar icons to close.
    public event Action? CloseRequested { add { } remove { } }

    public MacTraySession(Action<Exception> onUnhandledException)
    {
        this.onUnhandledException = onUnhandledException;

        ObjC.WithAutoreleasePool(() =>
        {
            target = ObjC.Send(ObjC.Send(GetTargetClass(), "alloc"), "init");
            IntPtr statusBar = ObjC.Send(ObjC.GetClass("NSStatusBar"), "systemStatusBar");
            statusItem = ObjC.Send(ObjC.Send(statusBar, "statusItemWithLength:", VariableStatusItemLength), "retain");
            IntPtr button = ObjC.Send(statusItem, "button");

            if (statusItem == IntPtr.Zero || button == IntPtr.Zero)
            {
                throw new InvalidOperationException("The menu bar did not create a status item.");
            }

            ObjC.Send(button, "setTarget:", target);
            ObjC.Send(button, "setAction:", ObjC.Selector("statusItemClicked:"));
            ObjC.Send(button, "sendActionOn:", ActionMask);
            ObjC.SendBoolArg(statusItem, "setVisible:", false);
            return true;
        });

        sessions[target] = this;
    }

    public bool Visible
    {
        get => visible;
        set
        {
            visible = value;
            if (statusItem != IntPtr.Zero) ObjC.SendBoolArg(statusItem, "setVisible:", value);
        }
    }

    public string ToolTipText
    {
        get => toolTipText;
        set
        {
            toolTipText = value ?? string.Empty;
            if (statusItem == IntPtr.Zero) return;
            IntPtr text = CoreFoundation.CreateString(toolTipText);
            try
            {
                ObjC.Send(ObjC.Send(statusItem, "button"), "setToolTip:", text);
            }
            finally
            {
                CoreFoundation.CFRelease(text);
            }
        }
    }

    public TimeSpan DoubleClickTime =>
        TimeSpan.FromSeconds(((delegate* unmanaged<IntPtr, IntPtr, double>)ObjC.MsgSend)(ObjC.GetClass("NSEvent"), ObjC.Selector("doubleClickInterval")));

    public void SetIcon(byte[] png)
    {
        if (statusItem == IntPtr.Zero) return;

        ObjC.WithAutoreleasePool(() =>
        {
            IntPtr button = ObjC.Send(statusItem, "button");
            IntPtr image = IntPtr.Zero;

            fixed (byte* bytes = png)
            {
                IntPtr data = CoreFoundation.CFDataCreate(IntPtr.Zero, bytes, png.Length);
                if (data != IntPtr.Zero)
                {
                    image = ObjC.Send(ObjC.Send(ObjC.GetClass("NSImage"), "alloc"), "initWithData:", data);
                    CoreFoundation.CFRelease(data);
                }
            }

            if (image == IntPtr.Zero)
            {
                // Never leave an empty, invisible item.
                IntPtr title = CoreFoundation.CreateString("ShareX");
                ObjC.Send(button, "setTitle:", title);
                CoreFoundation.CFRelease(title);
                return true;
            }

            ((delegate* unmanaged<IntPtr, IntPtr, CoreGraphics.CGPoint, void>)ObjC.MsgSend)(image, ObjC.Selector("setSize:"),
                new CoreGraphics.CGPoint { X = IconSize, Y = IconSize });
            ObjC.Send(button, "setImage:", image);
            ObjC.Send(image, "release");
            return true;
        });
    }

    public void Dispose()
    {
        if (statusItem == IntPtr.Zero) return;

        sessions.Remove(target);
        ObjC.Send(ObjC.Send(ObjC.GetClass("NSStatusBar"), "systemStatusBar"), "removeStatusItem:", statusItem);
        ObjC.Send(statusItem, "release");
        ObjC.Send(target, "release");
        statusItem = IntPtr.Zero;
        target = IntPtr.Zero;
    }

    private void OnClicked()
    {
        IntPtr application = ObjC.Send(ObjC.GetClass("NSApplication"), "sharedApplication");
        IntPtr currentEvent = ObjC.Send(application, "currentEvent");
        nint type = currentEvent != IntPtr.Zero ? ObjC.SendNInt(currentEvent, "type") : LeftMouseUp;
        bool control = currentEvent != IntPtr.Zero && (ObjC.SendNInt(currentEvent, "modifierFlags") & ControlKeyFlag) != 0;

        TrayMouseButton button = type switch
        {
            RightMouseUp => TrayMouseButton.Right,
            OtherMouseUp => TrayMouseButton.Middle,
            // Control-click is the right click of a one-button mouse or trackpad.
            _ => control ? TrayMouseButton.Right : TrayMouseButton.Left
        };

        // The button reports the click on release; a menu opens on the right release, as on Windows.
        MouseDown?.Invoke(button);
        MouseUp?.Invoke(button);
    }

    private static IntPtr GetTargetClass()
    {
        if (targetClass != IntPtr.Zero) return targetClass;

        IntPtr existing = ObjC.LookUpClass("ShareXStatusItemTarget");
        if (existing != IntPtr.Zero) return targetClass = existing;

        IntPtr created = ObjC.AllocateClassPair(ObjC.GetClass("NSObject"), "ShareXStatusItemTarget");
        if (created == IntPtr.Zero || !ObjC.AddMethod(created, ObjC.Selector("statusItemClicked:"),
            (IntPtr)(delegate* unmanaged<IntPtr, IntPtr, IntPtr, void>)&OnStatusItemClicked, "v@:@"))
        {
            throw new InvalidOperationException("The status item target class could not be created.");
        }

        ObjC.RegisterClassPair(created);
        return targetClass = created;
    }

    [UnmanagedCallersOnly]
    private static void OnStatusItemClicked(IntPtr self, IntPtr selector, IntPtr sender)
    {
        if (!sessions.TryGetValue(self, out MacTraySession? session)) return;

        try
        {
            session.OnClicked();
        }
        catch (Exception e)
        {
            // Exceptions must not cross into native code; the host reports them on the UI thread.
            session.onUnhandledException(e);
        }
    }
}
