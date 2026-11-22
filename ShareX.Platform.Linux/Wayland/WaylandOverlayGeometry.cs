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

namespace ShareX.Platform.Linux.Wayland;

/// <summary>
/// Pure geometry, DPI scaling, buffer slicing, and pointer-frame state machine for multi-monitor Wayland layer-shell
/// overlays. Isolated from native handles so all coordinate math and monitor transitions can be unit tested directly.
/// </summary>
internal static class WaylandOverlayGeometry
{
    internal const uint BtnLeft = 0x110;
    internal const uint BtnRight = 0x111;
    internal const uint BtnMiddle = 0x112;
    internal const uint BtnSide = 0x113;
    internal const uint BtnExtra = 0x114;

    private const double AxisUnitsPerDetent = 15.0;
    private const OverlayInputModifiers ButtonModifierMask =
        OverlayInputModifiers.LeftMouseButton |
        OverlayInputModifiers.RightMouseButton |
        OverlayInputModifiers.MiddleMouseButton |
        OverlayInputModifiers.XButton1MouseButton |
        OverlayInputModifiers.XButton2MouseButton;

    internal readonly record struct TargetOutputSelection<T>(IReadOnlyList<T> Selected, PlatformRectangle LogicalBounds);

    internal static TargetOutputSelection<T> SelectTargetOutputs<T>(
        IReadOnlyList<T> allOutputs,
        Func<T, PlatformRectangle> getBounds,
        PlatformRectangle requestedBounds)
    {
        List<T> valid = new(allOutputs.Count);
        foreach (T item in allOutputs)
        {
            if (!getBounds(item).IsEmpty)
            {
                valid.Add(item);
            }
        }

        if (valid.Count == 0)
        {
            return new TargetOutputSelection<T>(Array.Empty<T>(), PlatformRectangle.Empty);
        }

        List<T> chosen = new(valid.Count);

        if (!requestedBounds.IsEmpty)
        {
            foreach (T item in valid)
            {
                if (!getBounds(item).Intersect(requestedBounds).IsEmpty)
                {
                    chosen.Add(item);
                }
            }
        }

        if (chosen.Count == 0)
        {
            chosen.AddRange(valid);
        }

        PlatformRectangle union = getBounds(chosen[0]);
        for (int i = 1; i < chosen.Count; i++)
        {
            union = union.Union(getBounds(chosen[i]));
        }

        return new TargetOutputSelection<T>(chosen, union);
    }

    internal static double ComputeRenderScaling(PlatformRectangle logicalBounds, int pixelWidth, int pixelHeight, bool transparent)
    {
        if (transparent || logicalBounds.Width <= 0 || logicalBounds.Height <= 0 || pixelWidth <= 0 || pixelHeight <= 0)
        {
            return 1.0;
        }

        double scaleX = pixelWidth / (double)logicalBounds.Width;
        return double.IsFinite(scaleX) && scaleX > 0 ? scaleX : 1.0;
    }

    internal static PlatformRectangle ComputeSurfaceSlice(
        PlatformRectangle overlayLogicalBounds,
        int framebufferWidth,
        int framebufferHeight,
        PlatformRectangle outputLogicalBounds,
        int configuredWidth,
        int configuredHeight)
    {
        if (overlayLogicalBounds.Width <= 0 || overlayLogicalBounds.Height <= 0 ||
            framebufferWidth <= 0 || framebufferHeight <= 0 ||
            configuredWidth <= 0 || configuredHeight <= 0)
        {
            return PlatformRectangle.Empty;
        }

        double scaleX = framebufferWidth / (double)overlayLogicalBounds.Width;
        double scaleY = framebufferHeight / (double)overlayLogicalBounds.Height;

        double localLeft = outputLogicalBounds.X - overlayLogicalBounds.X;
        double localTop = outputLogicalBounds.Y - overlayLogicalBounds.Y;

        int left = Math.Clamp((int)Math.Round(localLeft * scaleX), 0, framebufferWidth - 1);
        int top = Math.Clamp((int)Math.Round(localTop * scaleY), 0, framebufferHeight - 1);
        int right = Math.Clamp((int)Math.Round((localLeft + configuredWidth) * scaleX), left + 1, framebufferWidth);
        int bottom = Math.Clamp((int)Math.Round((localTop + configuredHeight) * scaleY), top + 1, framebufferHeight);

        return new PlatformRectangle(left, top, right - left, bottom - top);
    }

    internal static (double X, double Y) ToOverlayLogicalPoint(
        PlatformRectangle overlayLogicalBounds,
        PlatformRectangle surfaceLogicalBounds,
        double surfaceLocalX,
        double surfaceLocalY)
    {
        return (
            surfaceLogicalBounds.X - overlayLogicalBounds.X + surfaceLocalX,
            surfaceLogicalBounds.Y - overlayLogicalBounds.Y + surfaceLocalY);
    }

    /// <summary>
    /// Computes an output's logical bounds, preferring <c>zxdg_output_v1</c> logical position/size and falling back to
    /// <c>wl_output</c> geometry/mode divided by integer scale (swapping dimensions for 90°/270° transforms).
    /// </summary>
    internal static PlatformRectangle ComputeOutputLogicalBounds(
        int x,
        int y,
        int modeWidth,
        int modeHeight,
        int transform,
        int integerScale,
        bool hasXdgPosition,
        int xdgX,
        int xdgY,
        bool hasXdgSize,
        int xdgWidth,
        int xdgHeight)
    {
        int outX = hasXdgPosition ? xdgX : x;
        int outY = hasXdgPosition ? xdgY : y;

        if (hasXdgSize && xdgWidth > 0 && xdgHeight > 0)
        {
            return new PlatformRectangle(outX, outY, xdgWidth, xdgHeight);
        }

        if (modeWidth <= 0 || modeHeight <= 0)
        {
            return PlatformRectangle.Empty;
        }

        // Wayland wl_output.transform odd values (1=90, 3=270, 5=flipped-90, 7=flipped-270) swap width and height.
        bool rotated = (transform & 1) != 0;
        int physW = rotated ? modeHeight : modeWidth;
        int physH = rotated ? modeWidth : modeHeight;
        int scale = Math.Max(1, integerScale);

        return new PlatformRectangle(
            outX,
            outY,
            Math.Max(1, (int)Math.Round(physW / (double)scale)),
            Math.Max(1, (int)Math.Round(physH / (double)scale)));
    }

    internal static uint? MapCursorShape(OverlayCursorShape shape) => shape switch
    {
        OverlayCursorShape.Default => 1,
        OverlayCursorShape.Help => 3,
        OverlayCursorShape.Pointer => 4,
        OverlayCursorShape.Progress => 5,
        OverlayCursorShape.Wait => 6,
        OverlayCursorShape.Crosshair => 8,
        OverlayCursorShape.Text => 9,
        OverlayCursorShape.Alias => 11,
        OverlayCursorShape.Copy => 12,
        OverlayCursorShape.Move => 13,
        OverlayCursorShape.NotAllowed => 15,
        OverlayCursorShape.Grab => 16,
        OverlayCursorShape.Grabbing => 17,
        OverlayCursorShape.EResize => 18,
        OverlayCursorShape.NResize => 19,
        OverlayCursorShape.NEResize => 20,
        OverlayCursorShape.NWResize => 21,
        OverlayCursorShape.SResize => 22,
        OverlayCursorShape.SEResize => 23,
        OverlayCursorShape.SWResize => 24,
        OverlayCursorShape.WResize => 25,
        OverlayCursorShape.EWResize => 26,
        OverlayCursorShape.NSResize => 27,
        OverlayCursorShape.AllScroll => 32,
        OverlayCursorShape.None => null,
        _ => 1
    };

    /// <summary>
    /// Groups <c>wl_pointer</c> events within each <c>wl_pointer.frame</c>, preserves button modifier state, merges
    /// sibling-surface Leave+Enter transitions without breaking Avalonia drags, and translates coordinates into the
    /// unified overlay space.
    /// </summary>
    internal sealed class WaylandPointerFrameAccumulator
    {
        private readonly PlatformRectangle overlayBounds;
        private readonly Func<long, PlatformRectangle?> getSurfaceBounds;

        private readonly List<QueuedEvent> events = new();
        private long activeSurfaceId;
        private uint lastEnterSerial;
        private double lastLocalX;
        private double lastLocalY;
        private OverlayInputModifiers buttonModifiers;
        private bool hasAxis;
        private double axisRawX;
        private double axisRawY;
        private double axisV120X;
        private double axisV120Y;
        private bool axisV120SeenX;
        private bool axisV120SeenY;
        private ulong axisTimestamp;

        public WaylandPointerFrameAccumulator(PlatformRectangle overlayBounds, Func<long, PlatformRectangle?> getSurfaceBounds)
        {
            this.overlayBounds = overlayBounds;
            this.getSurfaceBounds = getSurfaceBounds;
        }

        public uint LastEnterSerial => lastEnterSerial;

        public long ActiveSurfaceId => activeSurfaceId;

        public bool PendingCursorApply { get; private set; }

        public void ClearPendingCursorApply() => PendingCursorApply = false;

        public void EnqueueEnter(long surfaceId, uint serial, double localX, double localY)
        {
            events.Add(new QueuedEvent(QueuedKind.Enter, surfaceId, serial, 0, localX, localY, 0, false));
        }

        public void EnqueueLeave(long surfaceId, uint serial)
        {
            events.Add(new QueuedEvent(QueuedKind.Leave, surfaceId, serial, 0, 0, 0, 0, false));
        }

        public void EnqueueMotion(ulong timestamp, double localX, double localY)
        {
            events.Add(new QueuedEvent(QueuedKind.Motion, 0, 0, timestamp, localX, localY, 0, false));
        }

        public void EnqueueButton(ulong timestamp, uint button, bool pressed)
        {
            events.Add(new QueuedEvent(QueuedKind.Button, 0, 0, timestamp, 0, 0, button, pressed));
        }

        public void EnqueueAxis(ulong timestamp, uint axis, double value)
        {
            axisTimestamp = timestamp;
            if (axis == 0)
            {
                axisRawY += value;
            }
            else if (axis == 1)
            {
                axisRawX += value;
            }

            EnsureAxisMarker();
        }

        public void EnqueueAxisDiscrete(uint axis, int discrete)
        {
            if (axis == 0)
            {
                axisV120Y += discrete;
                axisV120SeenY = true;
            }
            else if (axis == 1)
            {
                axisV120X += discrete;
                axisV120SeenX = true;
            }

            EnsureAxisMarker();
        }

        public void EnqueueAxisValue120(uint axis, int value120)
        {
            if (axis == 0)
            {
                axisV120Y += value120 / 120.0;
                axisV120SeenY = true;
            }
            else if (axis == 1)
            {
                axisV120X += value120 / 120.0;
                axisV120SeenX = true;
            }

            EnsureAxisMarker();
        }

        private void EnsureAxisMarker()
        {
            if (!hasAxis)
            {
                hasAxis = true;
                events.Add(new QueuedEvent(QueuedKind.Axis, 0, 0, 0, 0, 0, 0, false));
            }
        }

        public void FlushFrame(
            OverlayInputModifiers keyboardModifiers,
            Action<OverlayPointerEvent> emitPointer,
            Action<OverlayWheelEvent> emitWheel)
        {
            try
            {
                bool hasKnownEnterAfterLeave = false;
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    if (events[i].Kind == QueuedKind.Enter && getSurfaceBounds(events[i].SurfaceId) != null)
                    {
                        hasKnownEnterAfterLeave = true;
                        break;
                    }
                }

                foreach (QueuedEvent ev in events)
                {
                    switch (ev.Kind)
                    {
                        case QueuedKind.Enter:
                        {
                            PlatformRectangle? bounds = getSurfaceBounds(ev.SurfaceId);
                            if (bounds == null)
                            {
                                break;
                            }

                            activeSurfaceId = ev.SurfaceId;
                            lastEnterSerial = ev.Serial;
                            lastLocalX = ev.LocalX;
                            lastLocalY = ev.LocalY;
                            PendingCursorApply = true;

                            (double gx, double gy) = ToOverlayLogicalPoint(overlayBounds, bounds.Value, lastLocalX, lastLocalY);
                            emitPointer(new OverlayPointerEvent(
                                OverlayPointerEventKind.Move,
                                gx,
                                gy,
                                0,
                                buttonModifiers | keyboardModifiers));
                            break;
                        }

                        case QueuedKind.Leave:
                        {
                            // A frame with Leave(surfaceA) + Enter(surfaceB) is moving between two monitors of the same
                            // overlay; also keep tracking if a mouse button is currently held down during a drag.
                            if (hasKnownEnterAfterLeave || (buttonModifiers & ButtonModifierMask) != 0)
                            {
                                if (activeSurfaceId == ev.SurfaceId && !hasKnownEnterAfterLeave)
                                {
                                    // Keep activeSurfaceId during implicit button grab so motion offsets remain relative to it.
                                }
                                break;
                            }

                            if (activeSurfaceId == ev.SurfaceId)
                            {
                                PlatformRectangle? bounds = getSurfaceBounds(ev.SurfaceId);
                                activeSurfaceId = 0;
                                lastEnterSerial = 0;
                                (double gx, double gy) = bounds != null
                                    ? ToOverlayLogicalPoint(overlayBounds, bounds.Value, lastLocalX, lastLocalY)
                                    : (0.0, 0.0);
                                emitPointer(new OverlayPointerEvent(
                                    OverlayPointerEventKind.Leave,
                                    gx,
                                    gy,
                                    0,
                                    buttonModifiers | keyboardModifiers));
                            }
                            break;
                        }

                        case QueuedKind.Motion:
                        {
                            lastLocalX = ev.LocalX;
                            lastLocalY = ev.LocalY;
                            PlatformRectangle? bounds = getSurfaceBounds(activeSurfaceId);
                            if (bounds == null)
                            {
                                break;
                            }

                            (double gx, double gy) = ToOverlayLogicalPoint(overlayBounds, bounds.Value, lastLocalX, lastLocalY);
                            emitPointer(new OverlayPointerEvent(
                                OverlayPointerEventKind.Move,
                                gx,
                                gy,
                                ev.Timestamp,
                                buttonModifiers | keyboardModifiers));
                            break;
                        }

                        case QueuedKind.Button:
                        {
                            OverlayPointerEventKind? kind = MapButton(ev.Button, ev.Pressed);
                            if (kind == null)
                            {
                                break;
                            }

                            OverlayInputModifiers mod = ButtonToModifier(ev.Button);
                            if (ev.Pressed)
                            {
                                buttonModifiers |= mod;
                            }
                            else
                            {
                                buttonModifiers &= ~mod;
                            }

                            PlatformRectangle? bounds = getSurfaceBounds(activeSurfaceId);
                            if (bounds == null)
                            {
                                break;
                            }

                            (double gx, double gy) = ToOverlayLogicalPoint(overlayBounds, bounds.Value, lastLocalX, lastLocalY);
                            emitPointer(new OverlayPointerEvent(
                                kind.Value,
                                gx,
                                gy,
                                ev.Timestamp,
                                buttonModifiers | keyboardModifiers));
                            break;
                        }

                        case QueuedKind.Axis:
                        {
                            double dx = axisV120SeenX ? axisV120X : axisRawX / AxisUnitsPerDetent;
                            double dy = axisV120SeenY ? axisV120Y : axisRawY / AxisUnitsPerDetent;
                            if (dx == 0 && dy == 0)
                            {
                                break;
                            }

                            PlatformRectangle? bounds = getSurfaceBounds(activeSurfaceId);
                            if (bounds == null)
                            {
                                break;
                            }

                            (double gx, double gy) = ToOverlayLogicalPoint(overlayBounds, bounds.Value, lastLocalX, lastLocalY);
                            // Wayland: positive axis is scroll down/right; Avalonia: positive Y is scroll up.
                            emitWheel(new OverlayWheelEvent(
                                gx,
                                gy,
                                -dx,
                                -dy,
                                axisTimestamp,
                                buttonModifiers | keyboardModifiers));
                            break;
                        }
                    }
                }
            }
            finally
            {
                events.Clear();
                hasAxis = false;
                axisRawX = 0;
                axisRawY = 0;
                axisV120X = 0;
                axisV120Y = 0;
                axisV120SeenX = false;
                axisV120SeenY = false;
            }
        }

        private static OverlayPointerEventKind? MapButton(uint button, bool pressed) => button switch
        {
            BtnLeft => pressed ? OverlayPointerEventKind.LeftDown : OverlayPointerEventKind.LeftUp,
            BtnRight => pressed ? OverlayPointerEventKind.RightDown : OverlayPointerEventKind.RightUp,
            BtnMiddle => pressed ? OverlayPointerEventKind.MiddleDown : OverlayPointerEventKind.MiddleUp,
            BtnSide => pressed ? OverlayPointerEventKind.X1Down : OverlayPointerEventKind.X1Up,
            BtnExtra => pressed ? OverlayPointerEventKind.X2Down : OverlayPointerEventKind.X2Up,
            _ => null
        };

        private static OverlayInputModifiers ButtonToModifier(uint button) => button switch
        {
            BtnLeft => OverlayInputModifiers.LeftMouseButton,
            BtnRight => OverlayInputModifiers.RightMouseButton,
            BtnMiddle => OverlayInputModifiers.MiddleMouseButton,
            BtnSide => OverlayInputModifiers.XButton1MouseButton,
            BtnExtra => OverlayInputModifiers.XButton2MouseButton,
            _ => OverlayInputModifiers.None
        };

        private enum QueuedKind
        {
            Enter,
            Leave,
            Motion,
            Button,
            Axis
        }

        private readonly record struct QueuedEvent(
            QueuedKind Kind,
            long SurfaceId,
            uint Serial,
            ulong Timestamp,
            double LocalX,
            double LocalY,
            uint Button,
            bool Pressed);
    }
}
