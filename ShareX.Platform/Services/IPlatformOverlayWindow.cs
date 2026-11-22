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

#nullable enable

using System;
using System.Collections.Generic;

namespace ShareX.Platform;

/// <summary>Requested geometry and behavior for a native platform overlay window.</summary>
public sealed record OverlayWindowOptions(
    PlatformRectangle ScreenBounds,
    int PixelWidth,
    int PixelHeight,
    bool Transparent,
    string Namespace);

/// <summary>One monitor surface covered by an <see cref="IPlatformOverlayWindow"/>, in compositor logical coordinates.</summary>
public sealed record OverlayOutputInfo(
    string Name,
    PlatformRectangle LogicalBounds,
    double ScaleFactor,
    bool IsPrimary);

[Flags]
public enum OverlayInputModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Meta = 8,
    LeftMouseButton = 16,
    RightMouseButton = 32,
    MiddleMouseButton = 64,
    XButton1MouseButton = 128,
    XButton2MouseButton = 256
}

public enum OverlayPointerEventKind
{
    Move,
    Leave,
    LeftDown,
    LeftUp,
    RightDown,
    RightUp,
    MiddleDown,
    MiddleUp,
    X1Down,
    X1Up,
    X2Down,
    X2Up
}

public readonly record struct OverlayPointerEvent(
    OverlayPointerEventKind Kind,
    double LogicalX,
    double LogicalY,
    ulong Timestamp,
    OverlayInputModifiers Modifiers);

public readonly record struct OverlayWheelEvent(
    double LogicalX,
    double LogicalY,
    double DeltaX,
    double DeltaY,
    ulong Timestamp,
    OverlayInputModifiers Modifiers);

public readonly record struct OverlayKeyEvent(
    bool IsDown,
    uint EvdevKeycode,
    uint Keysym,
    string? Text,
    ulong Timestamp,
    OverlayInputModifiers Modifiers);

public enum OverlayCursorShape
{
    Default,
    Crosshair,
    Pointer,
    Grab,
    Grabbing,
    Move,
    Text,
    Wait,
    Progress,
    Help,
    NotAllowed,
    NResize,
    SResize,
    EResize,
    WResize,
    NEResize,
    NWResize,
    SEResize,
    SWResize,
    EWResize,
    NSResize,
    AllScroll,
    Copy,
    Alias,
    None
}

/// <summary>A pointer cursor to display over an <see cref="IPlatformOverlayWindow"/>.</summary>
public sealed class OverlayCursor
{
    public static OverlayCursor Default { get; } = new(OverlayCursorShape.Default, null, 0, 0, 0, 0, 1.0);

    private OverlayCursor(
        OverlayCursorShape shape,
        byte[]? bgraPixels,
        int width,
        int height,
        int hotspotX,
        int hotspotY,
        double scale)
    {
        Shape = shape;
        BgraPixels = bgraPixels;
        Width = width;
        Height = height;
        HotspotX = hotspotX;
        HotspotY = hotspotY;
        Scale = scale > 0 ? scale : 1.0;
    }

    public OverlayCursorShape Shape { get; }

    /// <summary>Premultiplied BGRA8888 cursor pixels when this is a custom bitmap cursor; otherwise null.</summary>
    public byte[]? BgraPixels { get; }

    public int Width { get; }

    public int Height { get; }

    public int HotspotX { get; }

    public int HotspotY { get; }

    public double Scale { get; }

    public bool IsBitmap => BgraPixels != null;

    public static OverlayCursor FromStandard(OverlayCursorShape shape) =>
        shape == OverlayCursorShape.Default ? Default : new OverlayCursor(shape, null, 0, 0, 0, 0, 1.0);

    public static OverlayCursor FromBitmap(byte[] bgraPixels, int width, int height, int hotspotX, int hotspotY, double scale = 1.0)
    {
        ArgumentNullException.ThrowIfNull(bgraPixels);
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (bgraPixels.Length < checked(width * height * 4))
        {
            throw new ArgumentException("Cursor pixel buffer is smaller than width * height * 4.", nameof(bgraPixels));
        }

        return new OverlayCursor(OverlayCursorShape.Default, bgraPixels, width, height, hotspotX, hotspotY, scale);
    }
}

/// <summary>
/// Platform-native interactive overlay surface spanning one or more monitors above panels and top bars
/// (for example, Wayland wlr-layer-shell overlay surfaces per monitor).
/// </summary>
public interface IPlatformOverlayWindow : IDisposable
{
    PlatformRectangle LogicalBounds { get; }

    IReadOnlyList<OverlayOutputInfo> Outputs { get; }

    double RenderScaling { get; }

    bool IsTransparent { get; }

    event Action<OverlayPointerEvent>? PointerEvent;

    event Action<OverlayWheelEvent>? WheelEvent;

    event Action<OverlayKeyEvent>? KeyEvent;

    event Action<int, int>? KeyRepeatInfoChanged;

    event Action? Activated;

    event Action? Deactivated;

    event Action? ClosedByCompositor;

    void Show();

    void Hide();

    void PresentFrame(IntPtr bgraPremulPixels, int width, int height, int stride);

    void SetCursor(OverlayCursor cursor);
}
