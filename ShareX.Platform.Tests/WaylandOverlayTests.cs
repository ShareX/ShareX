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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using ShareX.Platform.Linux.Wayland;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class WaylandOverlayTests
{
    private sealed record FakeOutput(string Name, PlatformRectangle Bounds);

    [Fact]
    public void SelectTargetOutputs_ResolvesDualSingleAndNegativeMonitorLayouts()
    {
        FakeOutput[] linaryxMonitors =
        [
            new("DP-1", new PlatformRectangle(0, 0, 1920, 1080)),
            new("HDMI-A-1", new PlatformRectangle(1920, 0, 1280, 1080))
        ];

        WaylandOverlayGeometry.TargetOutputSelection<FakeOutput> fullDesktop =
            WaylandOverlayGeometry.SelectTargetOutputs(
                linaryxMonitors,
                o => o.Bounds,
                new PlatformRectangle(0, 0, 3200, 1080));

        Assert.Equal(2, fullDesktop.Selected.Count);
        Assert.Equal(new PlatformRectangle(0, 0, 3200, 1080), fullDesktop.LogicalBounds);

        WaylandOverlayGeometry.TargetOutputSelection<FakeOutput> singleMonitor =
            WaylandOverlayGeometry.SelectTargetOutputs(
                linaryxMonitors,
                o => o.Bounds,
                new PlatformRectangle(1920, 0, 1280, 1080));

        Assert.Single(singleMonitor.Selected);
        Assert.Equal("HDMI-A-1", singleMonitor.Selected[0].Name);
        Assert.Equal(new PlatformRectangle(1920, 0, 1280, 1080), singleMonitor.LogicalBounds);

        FakeOutput[] negativeOriginMonitors =
        [
            new("Left", new PlatformRectangle(-1920, 0, 1920, 1080)),
            new("Right", new PlatformRectangle(0, 0, 2560, 1440))
        ];

        WaylandOverlayGeometry.TargetOutputSelection<FakeOutput> negativeSpan =
            WaylandOverlayGeometry.SelectTargetOutputs(
                negativeOriginMonitors,
                o => o.Bounds,
                new PlatformRectangle(-1920, 0, 4480, 1440));

        Assert.Equal(2, negativeSpan.Selected.Count);
        Assert.Equal(new PlatformRectangle(-1920, 0, 4480, 1440), negativeSpan.LogicalBounds);

        WaylandOverlayGeometry.TargetOutputSelection<FakeOutput> unmatchedFallback =
            WaylandOverlayGeometry.SelectTargetOutputs(
                linaryxMonitors,
                o => o.Bounds,
                new PlatformRectangle(9000, 9000, 100, 100));

        Assert.Equal(2, unmatchedFallback.Selected.Count);
        Assert.Equal(new PlatformRectangle(0, 0, 3200, 1080), unmatchedFallback.LogicalBounds);
    }

    [Theory]
    [InlineData(3072, 1728, 3840, 2160, false, 1.25)]
    [InlineData(3072, 1728, 3840, 2160, true, 1.0)]
    [InlineData(3200, 1080, 3200, 1080, false, 1.0)]
    public void ComputeRenderScaling_HandlesFractionalAndTransparentOverlays(
        int logicalW, int logicalH, int pixelW, int pixelH, bool transparent, double expected)
    {
        double actual = WaylandOverlayGeometry.ComputeRenderScaling(
            new PlatformRectangle(0, 0, logicalW, logicalH),
            pixelW,
            pixelH,
            transparent);

        Assert.Equal(expected, actual, 6);
    }

    [Fact]
    public void ComputeSurfaceSlice_MapsOutputsIntoFramebufferCoordinates()
    {
        PlatformRectangle union = new(0, 0, 3200, 1080);
        PlatformRectangle leftSlice = WaylandOverlayGeometry.ComputeSurfaceSlice(
            union, 3200, 1080, new PlatformRectangle(0, 0, 1920, 1080), 1920, 1080);
        PlatformRectangle rightSlice = WaylandOverlayGeometry.ComputeSurfaceSlice(
            union, 3200, 1080, new PlatformRectangle(1920, 0, 1280, 1080), 1280, 1080);

        Assert.Equal(new PlatformRectangle(0, 0, 1920, 1080), leftSlice);
        Assert.Equal(new PlatformRectangle(1920, 0, 1280, 1080), rightSlice);

        PlatformRectangle hiDpiSlice = WaylandOverlayGeometry.ComputeSurfaceSlice(
            new PlatformRectangle(0, 0, 3072, 1728),
            3840,
            2160,
            new PlatformRectangle(0, 0, 3072, 1728),
            3072,
            1728);

        Assert.Equal(new PlatformRectangle(0, 0, 3840, 2160), hiDpiSlice);

        PlatformRectangle negUnion = new(-1920, 0, 4480, 1440);
        PlatformRectangle negSlice = WaylandOverlayGeometry.ComputeSurfaceSlice(
            negUnion, 4480, 1440, new PlatformRectangle(-1920, 0, 1920, 1080), 1920, 1080);
        Assert.Equal(new PlatformRectangle(0, 0, 1920, 1080), negSlice);
    }

    [Fact]
    public void ComputeOutputLogicalBounds_UsesXdgOutputOrFallsBackWithRotation()
    {
        PlatformRectangle xdgFrac = WaylandOverlayGeometry.ComputeOutputLogicalBounds(
            0, 0, 3840, 2160, transform: 0, integerScale: 2,
            hasXdgPosition: true, xdgX: 100, xdgY: 50,
            hasXdgSize: true, xdgWidth: 3072, xdgHeight: 1728);

        Assert.Equal(new PlatformRectangle(100, 50, 3072, 1728), xdgFrac);

        PlatformRectangle fallbackScaled = WaylandOverlayGeometry.ComputeOutputLogicalBounds(
            0, 0, 3840, 2160, transform: 0, integerScale: 2,
            hasXdgPosition: false, xdgX: 0, xdgY: 0,
            hasXdgSize: false, xdgWidth: 0, xdgHeight: 0);

        Assert.Equal(new PlatformRectangle(0, 0, 1920, 1080), fallbackScaled);

        PlatformRectangle rotated90 = WaylandOverlayGeometry.ComputeOutputLogicalBounds(
            1920, 0, 1920, 1080, transform: 1, integerScale: 1,
            hasXdgPosition: false, xdgX: 0, xdgY: 0,
            hasXdgSize: false, xdgWidth: 0, xdgHeight: 0);

        Assert.Equal(new PlatformRectangle(1920, 0, 1080, 1920), rotated90);
    }

    [Fact]
    public void PointerFrame_MergesCrossMonitorDragOverTopBarWithoutLeave()
    {
        PlatformRectangle union = new(0, 0, 3200, 1080);
        Dictionary<long, PlatformRectangle> surfaceBounds = new()
        {
            [1] = new PlatformRectangle(0, 0, 1920, 1080),
            [2] = new PlatformRectangle(1920, 0, 1280, 1080)
        };

        WaylandOverlayGeometry.WaylandPointerFrameAccumulator acc = new(
            union,
            id => surfaceBounds.TryGetValue(id, out PlatformRectangle b) ? b : null);

        List<OverlayPointerEvent> pointerEvents = [];
        List<OverlayWheelEvent> wheelEvents = [];

        // Enter surface 1 directly over the top bar area (y = 5) and press left button.
        acc.EnqueueEnter(1, serial: 10, localX: 100, localY: 5);
        acc.EnqueueButton(100, WaylandOverlayGeometry.BtnLeft, pressed: true);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);

        // Compositor keeps implicit grab on surface 1 as pointer crosses x=1920 to surface 2.
        acc.EnqueueMotion(101, localX: 2500, localY: 600);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);

        // Compositor switches pointer focus via Leave(1) + Enter(2) in the same frame while button is held.
        acc.EnqueueLeave(1, serial: 11);
        acc.EnqueueEnter(2, serial: 12, localX: 580, localY: 600);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);

        // Also verify split-frame Leave(2) then Enter(1) while left button is still held emits no Leave.
        acc.EnqueueLeave(2, serial: 13);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);
        acc.EnqueueEnter(1, serial: 14, localX: 1900, localY: 610);
        acc.EnqueueMotion(102, localX: 1910, localY: 615);
        acc.EnqueueButton(103, WaylandOverlayGeometry.BtnLeft, pressed: false);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);

        // With no button held, a Leave without sibling Enter emits OverlayPointerEventKind.Leave.
        acc.EnqueueLeave(1, serial: 15);
        acc.FlushFrame(OverlayInputModifiers.None, pointerEvents.Add, wheelEvents.Add);

        Assert.Equal(OverlayPointerEventKind.Move, pointerEvents[0].Kind);
        Assert.Equal(100, pointerEvents[0].LogicalX);
        Assert.Equal(5, pointerEvents[0].LogicalY);

        Assert.Equal(OverlayPointerEventKind.LeftDown, pointerEvents[1].Kind);
        Assert.True(pointerEvents[1].Modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton));

        Assert.Equal(OverlayPointerEventKind.Move, pointerEvents[2].Kind);
        Assert.Equal(2500, pointerEvents[2].LogicalX);
        Assert.Equal(600, pointerEvents[2].LogicalY);

        Assert.Equal(OverlayPointerEventKind.Move, pointerEvents[3].Kind);
        Assert.Equal(2500, pointerEvents[3].LogicalX);
        Assert.Equal(600, pointerEvents[3].LogicalY);

        // All events except the very last one must not be Leave.
        for (int i = 0; i < pointerEvents.Count - 1; i++)
        {
            Assert.NotEqual(OverlayPointerEventKind.Leave, pointerEvents[i].Kind);
        }

        Assert.Equal(OverlayPointerEventKind.LeftUp, pointerEvents[^2].Kind);
        Assert.False(pointerEvents[^2].Modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton));
        Assert.Equal(OverlayPointerEventKind.Leave, pointerEvents[^1].Kind);
        Assert.Empty(wheelEvents);
    }

    [Fact]
    public void PointerFrame_TracksMultiButtonModifiersAndWheelDirection()
    {
        PlatformRectangle union = new(0, 0, 1920, 1080);
        WaylandOverlayGeometry.WaylandPointerFrameAccumulator acc = new(
            union,
            id => id == 1 ? union : null);

        List<OverlayPointerEvent> pointerEvents = [];
        List<OverlayWheelEvent> wheelEvents = [];

        acc.EnqueueEnter(1, serial: 1, localX: 500, localY: 500);
        acc.EnqueueButton(10, WaylandOverlayGeometry.BtnLeft, pressed: true);
        acc.EnqueueButton(11, WaylandOverlayGeometry.BtnRight, pressed: true);
        acc.EnqueueButton(12, WaylandOverlayGeometry.BtnRight, pressed: false);
        acc.EnqueueButton(13, WaylandOverlayGeometry.BtnLeft, pressed: false);
        acc.EnqueueAxis(14, axis: 0, value: 15.0);
        acc.EnqueueAxisValue120(axis: 0, value120: 120);
        acc.FlushFrame(OverlayInputModifiers.Shift, pointerEvents.Add, wheelEvents.Add);

        Assert.True(pointerEvents[1].Modifiers.HasFlag(OverlayInputModifiers.Shift | OverlayInputModifiers.LeftMouseButton));
        Assert.True(pointerEvents[2].Modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton | OverlayInputModifiers.RightMouseButton));
        Assert.True(pointerEvents[3].Modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton));
        Assert.False(pointerEvents[3].Modifiers.HasFlag(OverlayInputModifiers.RightMouseButton));
        Assert.False(pointerEvents[4].Modifiers.HasFlag(OverlayInputModifiers.LeftMouseButton));

        OverlayWheelEvent wheel = Assert.Single(wheelEvents);
        // axis_value120 takes precedence (+120 down on Wayland => -1.0 in Avalonia coordinates).
        Assert.Equal(-1.0, wheel.DeltaY, 6);
        Assert.True(wheel.Modifiers.HasFlag(OverlayInputModifiers.Shift));
    }

    [Fact]
    public void TryCreate_ReturnsNullWhenEscapeHatchIsEnabled()
    {
        string? prior = Environment.GetEnvironmentVariable("SHAREX_DISABLE_WAYLAND_LAYER_OVERLAY");
        try
        {
            Environment.SetEnvironmentVariable("SHAREX_DISABLE_WAYLAND_LAYER_OVERLAY", "1");
            WaylandLayerOverlayWindow? window = WaylandLayerOverlayWindow.TryCreate(
                new OverlayWindowOptions(new PlatformRectangle(0, 0, 1920, 1080), 1920, 1080, false, "sharex-test"));
            Assert.Null(window);
        }
        finally
        {
            Environment.SetEnvironmentVariable("SHAREX_DISABLE_WAYLAND_LAYER_OVERLAY", prior);
        }
    }

    /// <summary>
    /// Live Hyprland verification test. Maps real zwlr_layer_shell_v1 surfaces on layer 3 ("overlay")
    /// only when SHAREX_WAYLAND_OVERLAY_TEST=1 is set.
    /// </summary>
    [LinuxFact]
    public void LiveHyprland_MapsLayerSurfaceAboveTopBarsAndCleanlyUnmaps()
    {
        if (Environment.GetEnvironmentVariable("SHAREX_WAYLAND_OVERLAY_TEST") != "1" ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return;
        }

        const string ns = "sharex-region-capture-test";
        WaylandLayerOverlayWindow? overlay = WaylandLayerOverlayWindow.TryCreate(
            new OverlayWindowOptions(PlatformRectangle.Empty, 1, 1, false, ns));

        Assert.NotNull(overlay);
        try
        {
            int pxW = Math.Max(1, (int)Math.Round(overlay.LogicalBounds.Width * overlay.Outputs[0].ScaleFactor));
            int pxH = Math.Max(1, (int)Math.Round(overlay.LogicalBounds.Height * overlay.Outputs[0].ScaleFactor));
            byte[] pixels = new byte[checked(pxW * pxH * 4)];
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 220;     // B
                pixels[i + 1] = 120; // G
                pixels[i + 2] = 40;  // R
                pixels[i + 3] = 255; // A
            }

            IntPtr ptr = Marshal.AllocHGlobal(pixels.Length);
            try
            {
                Marshal.Copy(pixels, 0, ptr, pixels.Length);
                overlay.PresentFrame(ptr, pxW, pxH, pxW * 4);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            overlay.SetCursor(OverlayCursor.FromStandard(OverlayCursorShape.Crosshair));
            overlay.Show();
            Thread.Sleep(150);

            using (JsonDocument layers = ReadHyprctlLayers())
            {
                Assert.True(TryFindLayerSurface(layers.RootElement, ns, out int level, out int x, out int y, out int w, out int h),
                    $"Expected layer namespace '{ns}' in `hyprctl layers -j`.");
                Assert.Equal(3, level);
                Assert.True(w > 0 && h > 0);
                Assert.True(x <= overlay.LogicalBounds.X && y <= overlay.LogicalBounds.Y);
            }

            byte[] customCursor = new byte[16 * 16 * 4];
            for (int i = 0; i < customCursor.Length; i += 4)
            {
                customCursor[i] = 255;
                customCursor[i + 1] = 255;
                customCursor[i + 2] = 255;
                customCursor[i + 3] = 255;
            }
            overlay.SetCursor(OverlayCursor.FromBitmap(customCursor, 16, 16, 8, 8, 1.0));
            Thread.Sleep(50);
        }
        finally
        {
            overlay.Dispose();
        }

        Thread.Sleep(100);
        using (JsonDocument after = ReadHyprctlLayers())
        {
            Assert.False(TryFindLayerSurface(after.RootElement, ns, out _, out _, out _, out _, out _),
                $"Layer namespace '{ns}' remained mapped after Dispose().");
        }
    }

    private static JsonDocument ReadHyprctlLayers()
    {
        using Process proc = Process.Start(new ProcessStartInfo("hyprctl", "layers -j")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true
        }) ?? throw new InvalidOperationException("Failed to start hyprctl.");
        string json = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        Assert.Equal(0, proc.ExitCode);
        return JsonDocument.Parse(json);
    }

    private static bool TryFindLayerSurface(
        JsonElement root,
        string ns,
        out int foundLevel,
        out int x,
        out int y,
        out int w,
        out int h)
    {
        foreach (JsonProperty monitor in root.EnumerateObject())
        {
            if (!monitor.Value.TryGetProperty("levels", out JsonElement levels))
            {
                continue;
            }

            foreach (JsonProperty levelProp in levels.EnumerateObject())
            {
                if (!int.TryParse(levelProp.Name, out int lvl) || levelProp.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (JsonElement surface in levelProp.Value.EnumerateArray())
                {
                    if (surface.TryGetProperty("namespace", out JsonElement nsProp) &&
                        nsProp.GetString() == ns)
                    {
                        foundLevel = lvl;
                        x = surface.TryGetProperty("x", out JsonElement xp) ? xp.GetInt32() : 0;
                        y = surface.TryGetProperty("y", out JsonElement yp) ? yp.GetInt32() : 0;
                        w = surface.TryGetProperty("w", out JsonElement wp) ? wp.GetInt32() : 0;
                        h = surface.TryGetProperty("h", out JsonElement hp) ? hp.GetInt32() : 0;
                        return true;
                    }
                }
            }
        }

        foundLevel = -1;
        x = y = w = h = 0;
        return false;
    }
}
