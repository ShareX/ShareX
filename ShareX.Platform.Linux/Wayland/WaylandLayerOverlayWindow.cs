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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ShareX.Platform.Linux.Wayland;

using static WaylandNative;
using static WaylandOverlayGeometry;

/// <summary>
/// Interactive native Wayland overlay spanning one or more monitors via <c>zwlr_layer_shell_v1</c> on the
/// <c>overlay</c> layer (above top bars such as Waybar/Quickshell and docks). Runs a dedicated Wayland thread so
/// protocol callbacks never collide with Avalonia's X11 thread.
/// </summary>
internal sealed unsafe class WaylandLayerOverlayWindow : IPlatformOverlayWindow
{
    private const uint LayerOverlay = 3;
    private const uint AnchorTop = 1;
    private const uint AnchorBottom = 2;
    private const uint AnchorLeft = 4;
    private const uint AnchorRight = 8;
    private const uint AnchorAll = AnchorTop | AnchorBottom | AnchorLeft | AnchorRight;
    private const uint KeyboardInteractivityExclusive = 1;

    private const uint SeatCapPointer = 1;
    private const uint SeatCapKeyboard = 2;

    private const int MaxBuffersPerSurface = 3;

    private readonly OverlayWindowOptions options;
    private readonly int wakeEventFd;
    private readonly Thread thread;
    private readonly ManualResetEventSlim initReady = new(false);
    private readonly ManualResetEventSlim showReady = new(false);
    private readonly ManualResetEventSlim hideReady = new(false);
    private readonly ConcurrentQueue<Action> commandQueue = new();

    private readonly object frameLock = new();
    private byte[]? pendingPixels;
    private int pendingWidth;
    private int pendingHeight;
    private int pendingStride;
    private long latestFrameId;
    private volatile bool hasPendingFrame;

    private readonly object cursorLock = new();
    private OverlayCursor pendingCursor = OverlayCursor.Default;
    private volatile bool hasPendingCursor;

    private GCHandle selfHandle;
    private bool initSucceeded;
    private volatile bool volatileDisposed;

    // Wayland thread state only:
    private IntPtr display;
    private IntPtr registry;
    private IntPtr compositor;
    private uint compositorVersion;
    private IntPtr shm;
    private uint shmVersion;
    private IntPtr seat;
    private uint seatVersion;
    private IntPtr layerShell;
    private uint layerShellVersion;
    private IntPtr xdgOutputManager;
    private uint xdgOutputManagerVersion;
    private IntPtr viewporter;
    private uint viewporterVersion;
    private IntPtr cursorShapeManager;
    private uint cursorShapeManagerVersion;

    private IntPtr pointer;
    private uint pointerVersion;
    private IntPtr cursorShapeDevice;
    private IntPtr cursorTheme;
    private int cursorThemeScale;
    private IntPtr cursorSurface;
    private IntPtr cursorViewport;
    private WaylandShmBuffer? cursorBuffer;

    private IntPtr keyboard;
    private uint keyboardVersion;
    private IntPtr xkbContext;
    private IntPtr xkbKeymap;
    private IntPtr xkbState;
    private OverlayInputModifiers keyboardModifiers;

    private readonly Dictionary<IntPtr, DispatchTarget> dispatchTargets = new();
    private readonly List<OutputState> allOutputs = new();
    private readonly List<OutputState> targetOutputs = new();
    private readonly Dictionary<long, OutputState> surfaceToOutput = new();

    private WaylandPointerFrameAccumulator? pointerFrame;
    private bool surfacesVisible;
    private int keyboardFocusedSurfaces;
    private bool compositorClosed;
    private volatile bool stopRequested;

    private WaylandLayerOverlayWindow(OverlayWindowOptions options, int wakeEventFd)
    {
        this.options = options;
        this.wakeEventFd = wakeEventFd;
        selfHandle = GCHandle.Alloc(this, GCHandleType.Normal);
        thread = new Thread(ThreadMain)
        {
            Name = "ShareX Wayland Overlay",
            IsBackground = true
        };
    }

    public PlatformRectangle LogicalBounds { get; private set; }

    public IReadOnlyList<OverlayOutputInfo> Outputs { get; private set; } = Array.Empty<OverlayOutputInfo>();

    public double RenderScaling { get; private set; } = 1.0;

    public bool IsTransparent => options.Transparent;

    public event Action<OverlayPointerEvent>? PointerEvent;
    public event Action<OverlayWheelEvent>? WheelEvent;
    public event Action<OverlayKeyEvent>? KeyEvent;
    public event Action<int, int>? KeyRepeatInfoChanged;
    public event Action? Activated;
    public event Action? Deactivated;
    public event Action? ClosedByCompositor;

    public static WaylandLayerOverlayWindow? TryCreate(OverlayWindowOptions options)
    {
        if (Environment.GetEnvironmentVariable("SHAREX_DISABLE_WAYLAND_LAYER_OVERLAY") == "1")
        {
            return null;
        }

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")) || !IsWaylandClientAvailable)
        {
            return null;
        }

        int wakeFd = EventFd(0, EFD_NONBLOCK | EFD_CLOEXEC);
        if (wakeFd < 0)
        {
            return null;
        }

        WaylandLayerOverlayWindow window = new(options, wakeFd);
        window.thread.Start();

        if (!window.initReady.Wait(TimeSpan.FromSeconds(3)) || !window.initSucceeded)
        {
            window.Dispose();
            return null;
        }

        return window;
    }

    public void Show()
    {
        if (volatileDisposed)
        {
            return;
        }

        showReady.Reset();
        EnqueueCommand(ShowOnWaylandThread);
        showReady.Wait(TimeSpan.FromSeconds(2));
    }

    public void Hide()
    {
        if (volatileDisposed)
        {
            return;
        }

        hideReady.Reset();
        EnqueueCommand(HideOnWaylandThread);
        hideReady.Wait(TimeSpan.FromSeconds(1));
    }

    public void PresentFrame(IntPtr bgraPremulPixels, int width, int height, int stride)
    {
        if (volatileDisposed || bgraPremulPixels == IntPtr.Zero || width <= 0 || height <= 0 || stride < width * 4)
        {
            return;
        }

        int targetStride = checked(width * 4);
        int needed = checked(targetStride * height);

        lock (frameLock)
        {
            if (pendingPixels == null || pendingPixels.Length < needed)
            {
                pendingPixels = new byte[needed];
            }

            fixed (byte* dst = pendingPixels)
            {
                byte* src = (byte*)bgraPremulPixels;
                if (stride == targetStride)
                {
                    Buffer.MemoryCopy(src, dst, needed, needed);
                }
                else
                {
                    for (int y = 0; y < height; y++)
                    {
                        Buffer.MemoryCopy(src + (long)y * stride, dst + (long)y * targetStride, targetStride, targetStride);
                    }
                }
            }

            pendingWidth = width;
            pendingHeight = height;
            pendingStride = targetStride;
            latestFrameId++;
            hasPendingFrame = true;
        }

        SignalWake();
    }

    public void SetCursor(OverlayCursor cursor)
    {
        if (volatileDisposed)
        {
            return;
        }

        lock (cursorLock)
        {
            pendingCursor = cursor;
            hasPendingCursor = true;
        }

        SignalWake();
    }

    public void Dispose()
    {
        if (volatileDisposed)
        {
            return;
        }

        volatileDisposed = true;
        stopRequested = true;
        SignalWake();

        if (thread.IsAlive && Thread.CurrentThread != thread)
        {
            thread.Join(TimeSpan.FromSeconds(2));
        }

        if (!thread.IsAlive)
        {
            initReady.Dispose();
            showReady.Dispose();
            hideReady.Dispose();
        }
    }

    private void EnqueueCommand(Action command)
    {
        commandQueue.Enqueue(command);
        SignalWake();
    }

    private void SignalWake()
    {
        ulong one = 1;
        Write(wakeEventFd, &one, (nuint)sizeof(ulong));
    }

    private void DrainWake()
    {
        ulong dummy;
        while (Read(wakeEventFd, &dummy, (nuint)sizeof(ulong)) > 0)
        {
        }
    }

    private void ThreadMain()
    {
        try
        {
            if (!InitializeWayland())
            {
                initSucceeded = false;
                initReady.Set();
                return;
            }

            initSucceeded = true;
            initReady.Set();
            RunEventLoop();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Wayland overlay failed: {ex.Message}");
            if (!initReady.IsSet)
            {
                initSucceeded = false;
                initReady.Set();
            }
            showReady.Set();
            hideReady.Set();
        }
        finally
        {
            CleanupWayland();
            Close(wakeEventFd);
            if (selfHandle.IsAllocated)
            {
                selfHandle.Free();
            }
        }
    }

    private bool InitializeWayland()
    {
        display = WlDisplayConnect(null);
        if (display == IntPtr.Zero)
        {
            return false;
        }

        // Create XKB context before any roundtrip so initial wl_keyboard.keymap events are never dropped.
        if (IsXkbAvailable)
        {
            xkbContext = XkbContextNew(XKB_CONTEXT_NO_FLAGS);
        }

        WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;

        WlArgument* regArgs = stackalloc WlArgument[1];
        regArgs[0].Pointer = IntPtr.Zero;
        // wl_display.get_registry (opcode 1)
        registry = WlProxyMarshalArrayFlags(display, 1, ifaces.WlRegistry, 1, 0, regArgs);
        if (registry == IntPtr.Zero)
        {
            return false;
        }

        RegisterDispatcher(registry, DispatchKind.Registry, null);

        // Roundtrip #1: receive all globals and bind required protocols + outputs.
        if (WlDisplayRoundtrip(display) < 0)
        {
            return false;
        }

        if (compositor == IntPtr.Zero || shm == IntPtr.Zero || seat == IntPtr.Zero ||
            layerShell == IntPtr.Zero || allOutputs.Count == 0)
        {
            return false;
        }

        WlArgument* xdgArgs = stackalloc WlArgument[2];
        if (xdgOutputManager != IntPtr.Zero)
        {
            foreach (OutputState output in allOutputs)
            {
                xdgArgs[0].Pointer = IntPtr.Zero;
                xdgArgs[1].Pointer = output.WlOutput;
                // zxdg_output_manager_v1.get_xdg_output (opcode 1)
                output.XdgOutput = WlProxyMarshalArrayFlags(
                    xdgOutputManager, 1, ifaces.ZXdgOutputV1, xdgOutputManagerVersion, 0, xdgArgs);
                if (output.XdgOutput != IntPtr.Zero)
                {
                    RegisterDispatcher(output.XdgOutput, DispatchKind.XdgOutput, output);
                }
            }
        }

        // Roundtrip #2: receive wl_output geometry/mode/scale, zxdg_output logical coords, and wl_seat capabilities.
        if (WlDisplayRoundtrip(display) < 0)
        {
            return false;
        }

        // Roundtrip #3 if pointer/keyboard were just bound, to receive initial wl_keyboard.keymap & repeat_info.
        if (pointer != IntPtr.Zero || keyboard != IntPtr.Zero)
        {
            if (WlDisplayRoundtrip(display) < 0)
            {
                return false;
            }
        }

        for (int i = 0; i < allOutputs.Count; i++)
        {
            allOutputs[i].UpdateLogicalBounds(isFirst: i == 0);
        }

        TargetOutputSelection<OutputState> selection = SelectTargetOutputs(
            allOutputs,
            o => o.LogicalBounds,
            options.ScreenBounds);

        if (selection.Selected.Count == 0 || selection.LogicalBounds.IsEmpty)
        {
            return false;
        }

        targetOutputs.Clear();
        targetOutputs.AddRange(selection.Selected);
        LogicalBounds = selection.LogicalBounds;

        bool hasPrimary = false;
        foreach (OutputState output in targetOutputs)
        {
            if (output.LogicalBounds.X == 0 && output.LogicalBounds.Y == 0)
            {
                hasPrimary = true;
                break;
            }
        }

        List<OverlayOutputInfo> outputInfos = new(targetOutputs.Count);
        for (int i = 0; i < targetOutputs.Count; i++)
        {
            OutputState output = targetOutputs[i];
            bool isPrimary = hasPrimary
                ? output.LogicalBounds.X == 0 && output.LogicalBounds.Y == 0
                : i == 0;
            outputInfos.Add(new OverlayOutputInfo(
                output.Name ?? $"output-{i}",
                output.LogicalBounds,
                output.ScaleFactor,
                isPrimary));
        }

        Outputs = outputInfos;
        RenderScaling = ComputeRenderScaling(
            LogicalBounds,
            options.PixelWidth,
            options.PixelHeight,
            options.Transparent);

        pointerFrame = new WaylandPointerFrameAccumulator(
            LogicalBounds,
            surfaceId => surfaceToOutput.TryGetValue(surfaceId, out OutputState? output) ? output.LogicalBounds : null);

        return true;
    }

    private void RunEventLoop()
    {
        int wlFd = WlDisplayGetFd(display);
        PollFd* pollFds = stackalloc PollFd[2];
        pollFds[0].Fd = wlFd;
        pollFds[0].Events = POLLIN;
        pollFds[1].Fd = wakeEventFd;
        pollFds[1].Events = POLLIN;

        while (!stopRequested)
        {
            DrainCommands();

            if (stopRequested)
            {
                break;
            }

            if (hasPendingCursor)
            {
                ApplyPendingCursor();
            }

            if (hasPendingFrame && surfacesVisible)
            {
                SubmitPendingFrame();
            }

            while (WlDisplayPrepareRead(display) != 0)
            {
                if (WlDisplayDispatchPending(display) < 0)
                {
                    return;
                }

                DrainCommands();
                if (stopRequested)
                {
                    return;
                }
            }

            if (WlDisplayFlush(display) < 0 && Marshal.GetLastPInvokeError() != EAGAIN)
            {
                WlDisplayCancelRead(display);
                return;
            }

            if (stopRequested || commandQueue.Count > 0 || (hasPendingFrame && surfacesVisible) || hasPendingCursor)
            {
                WlDisplayCancelRead(display);
                continue;
            }

            int pollResult = Poll(pollFds, 2, -1);
            if (pollResult < 0)
            {
                WlDisplayCancelRead(display);
                if (Marshal.GetLastPInvokeError() == EINTR)
                {
                    continue;
                }

                return;
            }

            if ((pollFds[0].Revents & POLLIN) != 0)
            {
                if (WlDisplayReadEvents(display) < 0 || WlDisplayDispatchPending(display) < 0)
                {
                    return;
                }
            }
            else
            {
                WlDisplayCancelRead(display);
            }

            if ((pollFds[0].Revents & (POLLERR | POLLHUP)) != 0 || WlDisplayGetError(display) != 0)
            {
                return;
            }

            if ((pollFds[1].Revents & POLLIN) != 0)
            {
                DrainWake();
            }
        }
    }

    private void DrainCommands()
    {
        while (commandQueue.TryDequeue(out Action? action))
        {
            action();
        }
    }

    private void ShowOnWaylandThread()
    {
        try
        {
            if (stopRequested || surfacesVisible)
            {
                showReady.Set();
                return;
            }

            WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;
            IntPtr nsPtr = Marshal.StringToCoTaskMemUTF8(options.Namespace);

            WlArgument* surfArgs = stackalloc WlArgument[1];
            WlArgument* vpArgs = stackalloc WlArgument[2];
            WlArgument* layerArgs = stackalloc WlArgument[5];
            WlArgument* a1 = stackalloc WlArgument[1];
            WlArgument* a2 = stackalloc WlArgument[2];
            WlArgument* a4 = stackalloc WlArgument[4];

            try
            {
                foreach (OutputState output in targetOutputs)
                {
                    surfArgs[0].Pointer = IntPtr.Zero;
                    // wl_compositor.create_surface (opcode 0)
                    IntPtr wlSurface = WlProxyMarshalArrayFlags(compositor, 0, ifaces.WlSurface, compositorVersion, 0, surfArgs);
                    if (wlSurface == IntPtr.Zero)
                    {
                        continue;
                    }

                    output.WlSurface = wlSurface;
                    surfaceToOutput[wlSurface.ToInt64()] = output;

                    if (viewporter != IntPtr.Zero)
                    {
                        vpArgs[0].Pointer = IntPtr.Zero;
                        vpArgs[1].Pointer = wlSurface;
                        // wp_viewporter.get_viewport (opcode 1)
                        output.Viewport = WlProxyMarshalArrayFlags(viewporter, 1, ifaces.WpViewport, viewporterVersion, 0, vpArgs);
                    }

                    layerArgs[0].Pointer = IntPtr.Zero;
                    layerArgs[1].Pointer = wlSurface;
                    layerArgs[2].Pointer = output.WlOutput;
                    layerArgs[3].UInt32 = LayerOverlay;
                    layerArgs[4].Pointer = nsPtr;
                    // zwlr_layer_shell_v1.get_layer_surface (opcode 0)
                    IntPtr layerSurface = WlProxyMarshalArrayFlags(
                        layerShell, 0, ifaces.ZwlrLayerSurfaceV1, layerShellVersion, 0, layerArgs);

                    if (layerSurface == IntPtr.Zero)
                    {
                        continue;
                    }

                    output.LayerSurface = layerSurface;
                    output.Configured = false;
                    RegisterDispatcher(layerSurface, DispatchKind.LayerSurface, output);

                    uint layerSurfVer = WlProxyGetVersion(layerSurface);

                    // set_anchor (opcode 1): all four edges
                    a1[0].UInt32 = AnchorAll;
                    WlProxyMarshalArrayFlags(layerSurface, 1, null, layerSurfVer, 0, a1);

                    // set_size (opcode 0): (0, 0) to fill output
                    a2[0].UInt32 = 0;
                    a2[1].UInt32 = 0;
                    WlProxyMarshalArrayFlags(layerSurface, 0, null, layerSurfVer, 0, a2);

                    // set_exclusive_zone (opcode 2): -1 so surface covers bars/docks/reserved areas
                    a1[0].Int32 = -1;
                    WlProxyMarshalArrayFlags(layerSurface, 2, null, layerSurfVer, 0, a1);

                    // set_margin (opcode 3): (0, 0, 0, 0)
                    a4[0].Int32 = 0;
                    a4[1].Int32 = 0;
                    a4[2].Int32 = 0;
                    a4[3].Int32 = 0;
                    WlProxyMarshalArrayFlags(layerSurface, 3, null, layerSurfVer, 0, a4);

                    // set_keyboard_interactivity (opcode 4): EXCLUSIVE (1)
                    a1[0].UInt32 = KeyboardInteractivityExclusive;
                    WlProxyMarshalArrayFlags(layerSurface, 4, null, layerSurfVer, 0, a1);

                    // wl_surface.commit (opcode 6) — triggers initial configure without attaching a buffer
                    WlProxyMarshalArrayFlags(wlSurface, 6, null, compositorVersion, 0, null);
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(nsPtr);
            }

            surfacesVisible = true;

            // Pump roundtrips until every created layer surface has received and acked its initial configure.
            Stopwatch wait = Stopwatch.StartNew();
            while (!stopRequested && wait.Elapsed < TimeSpan.FromMilliseconds(1500))
            {
                bool allConfigured = true;
                foreach (OutputState output in targetOutputs)
                {
                    if (output.LayerSurface != IntPtr.Zero && !output.Configured)
                    {
                        allConfigured = false;
                        break;
                    }
                }

                if (allConfigured)
                {
                    break;
                }

                if (WlDisplayRoundtrip(display) < 0)
                {
                    break;
                }
            }

            if (hasPendingFrame)
            {
                SubmitPendingFrame();
            }

            Activated?.Invoke();
        }
        finally
        {
            showReady.Set();
        }
    }

    private void HideOnWaylandThread()
    {
        try
        {
            if (!surfacesVisible)
            {
                hideReady.Set();
                return;
            }

            DestroyOverlaySurfaces();
            surfacesVisible = false;
            WlDisplayRoundtrip(display);
        }
        finally
        {
            hideReady.Set();
        }
    }

    private void SubmitPendingFrame()
    {
        byte[]? pixels;
        int width;
        int height;
        int stride;
        long frameId;

        lock (frameLock)
        {
            if (pendingPixels == null)
            {
                hasPendingFrame = false;
                return;
            }

            pixels = pendingPixels;
            width = pendingWidth;
            height = pendingHeight;
            stride = pendingStride;
            frameId = latestFrameId;
            hasPendingFrame = false;
        }

        bool anyCommitted = false;
        foreach (OutputState output in targetOutputs)
        {
            if (CommitOutputFrame(output, frameId, pixels, width, height, stride))
            {
                anyCommitted = true;
            }
        }

        if (anyCommitted)
        {
            WlDisplayFlush(display);
        }
    }

    private bool CommitOutputFrame(OutputState output, long frameId, byte[] pixels, int width, int height, int stride)
    {
        if (output.WlSurface == IntPtr.Zero ||
            !output.Configured ||
            output.ConfiguredWidth <= 0 ||
            output.ConfiguredHeight <= 0 ||
            output.CommittedFrameId == frameId ||
            output.FrameCallback != IntPtr.Zero)
        {
            return false;
        }

        PlatformRectangle slice = ComputeSurfaceSlice(
            LogicalBounds,
            width,
            height,
            output.LogicalBounds,
            output.ConfiguredWidth,
            output.ConfiguredHeight);

        if (slice.IsEmpty)
        {
            return false;
        }

        int bufW = output.Viewport != IntPtr.Zero ? slice.Width : output.ConfiguredWidth;
        int bufH = output.Viewport != IntPtr.Zero ? slice.Height : output.ConfiguredHeight;

        WaylandShmBuffer? shmBuf = AcquireBuffer(output, bufW, bufH);
        if (shmBuf == null)
        {
            return false;
        }

        shmBuf.CopyFrom(pixels, width, height, stride, slice);

        WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;
        uint surfVer = WlProxyGetVersion(output.WlSurface);
        if (output.Viewport != IntPtr.Zero)
        {
            WlArgument* vpArgs = stackalloc WlArgument[2];
            vpArgs[0].Int32 = output.ConfiguredWidth;
            vpArgs[1].Int32 = output.ConfiguredHeight;
            // wp_viewport.set_destination (opcode 2)
            WlProxyMarshalArrayFlags(output.Viewport, 2, null, WlProxyGetVersion(output.Viewport), 0, vpArgs);
        }
        else if (surfVer >= 3)
        {
            WlArgument* scaleArg = stackalloc WlArgument[1];
            scaleArg[0].Int32 = 1;
            // wl_surface.set_buffer_scale (opcode 8)
            WlProxyMarshalArrayFlags(output.WlSurface, 8, null, surfVer, 0, scaleArg);
        }

        if (!options.Transparent && !output.OpaqueRegionApplied)
        {
            WlArgument* regArg = stackalloc WlArgument[1];
            regArg[0].Pointer = IntPtr.Zero;
            // wl_compositor.create_region (opcode 1)
            IntPtr region = WlProxyMarshalArrayFlags(compositor, 1, ifaces.WlRegion, compositorVersion, 0, regArg);
            if (region != IntPtr.Zero)
            {
                uint regVer = WlProxyGetVersion(region);
                WlArgument* rectArgs = stackalloc WlArgument[4];
                rectArgs[0].Int32 = 0;
                rectArgs[1].Int32 = 0;
                rectArgs[2].Int32 = output.ConfiguredWidth;
                rectArgs[3].Int32 = output.ConfiguredHeight;
                // wl_region.add (opcode 1)
                WlProxyMarshalArrayFlags(region, 1, null, regVer, 0, rectArgs);

                WlArgument* opqArg = stackalloc WlArgument[1];
                opqArg[0].Pointer = region;
                // wl_surface.set_opaque_region (opcode 4)
                WlProxyMarshalArrayFlags(output.WlSurface, 4, null, surfVer, 0, opqArg);

                // wl_region.destroy (opcode 0)
                WlProxyMarshalArrayFlags(region, 0, null, regVer, WL_MARSHAL_FLAG_DESTROY, null);
                output.OpaqueRegionApplied = true;
            }
        }

        // Request per-output wl_surface.frame callback (opcode 3) so commits pace with compositor vsync.
        WlArgument* cbArgs = stackalloc WlArgument[1];
        cbArgs[0].Pointer = IntPtr.Zero;
        IntPtr frameCb = WlProxyMarshalArrayFlags(output.WlSurface, 3, ifaces.WlCallback, surfVer, 0, cbArgs);
        if (frameCb != IntPtr.Zero)
        {
            output.FrameCallback = frameCb;
            RegisterDispatcher(frameCb, DispatchKind.FrameCallback, output);
        }

        WlArgument* attachArgs = stackalloc WlArgument[3];
        attachArgs[0].Pointer = shmBuf.WlBuffer;
        attachArgs[1].Int32 = 0;
        attachArgs[2].Int32 = 0;
        // wl_surface.attach (opcode 1)
        WlProxyMarshalArrayFlags(output.WlSurface, 1, null, surfVer, 0, attachArgs);

        WlArgument* dmgArgs = stackalloc WlArgument[4];
        dmgArgs[0].Int32 = 0;
        dmgArgs[1].Int32 = 0;
        dmgArgs[2].Int32 = bufW;
        dmgArgs[3].Int32 = bufH;
        if (surfVer >= 4)
        {
            // wl_surface.damage_buffer (opcode 9)
            WlProxyMarshalArrayFlags(output.WlSurface, 9, null, surfVer, 0, dmgArgs);
        }
        else
        {
            dmgArgs[2].Int32 = output.ConfiguredWidth;
            dmgArgs[3].Int32 = output.ConfiguredHeight;
            // wl_surface.damage (opcode 2)
            WlProxyMarshalArrayFlags(output.WlSurface, 2, null, surfVer, 0, dmgArgs);
        }

        // wl_surface.commit (opcode 6)
        WlProxyMarshalArrayFlags(output.WlSurface, 6, null, surfVer, 0, null);
        shmBuf.Busy = true;
        output.CommittedFrameId = frameId;
        return true;
    }

    private void MarkPendingFrameIfStale(OutputState output)
    {
        if (!surfacesVisible || stopRequested)
        {
            return;
        }

        lock (frameLock)
        {
            if (pendingPixels != null && output.CommittedFrameId < latestFrameId)
            {
                hasPendingFrame = true;
            }
        }
    }

    private void MarkPendingFrameIfAnyStale()
    {
        if (!surfacesVisible || stopRequested)
        {
            return;
        }

        lock (frameLock)
        {
            if (pendingPixels == null)
            {
                return;
            }

            foreach (OutputState output in targetOutputs)
            {
                if (output.CommittedFrameId < latestFrameId)
                {
                    hasPendingFrame = true;
                    break;
                }
            }
        }
    }

    private WaylandShmBuffer? AcquireBuffer(OutputState output, int width, int height)
    {
        for (int i = output.Buffers.Count - 1; i >= 0; i--)
        {
            WaylandShmBuffer existing = output.Buffers[i];
            if (existing.Width == width && existing.Height == height)
            {
                if (!existing.Busy)
                {
                    return existing;
                }
            }
            else if (!existing.Busy)
            {
                dispatchTargets.Remove(existing.WlBuffer);
                existing.Dispose();
                output.Buffers.RemoveAt(i);
            }
        }

        if (output.Buffers.Count >= MaxBuffersPerSurface)
        {
            // All slots are busy; coalesce until compositor releases one.
            return null;
        }

        WaylandShmBuffer created = WaylandShmBuffer.Create(shm, shmVersion, width, height);
        RegisterDispatcher(created.WlBuffer, DispatchKind.Buffer, created);
        output.Buffers.Add(created);
        return created;
    }

    private void ApplyPendingCursor()
    {
        OverlayCursor cursor;
        lock (cursorLock)
        {
            cursor = pendingCursor;
            hasPendingCursor = false;
        }

        ApplyCursorNow(cursor);
    }

    private void ApplyCursorNow(OverlayCursor cursor)
    {
        if (pointer == IntPtr.Zero || pointerFrame == null || pointerFrame.LastEnterSerial == 0)
        {
            return;
        }

        uint serial = pointerFrame.LastEnterSerial;

        if (!cursor.IsBitmap)
        {
            if (cursor.Shape == OverlayCursorShape.None)
            {
                WlArgument* args = stackalloc WlArgument[4];
                args[0].UInt32 = serial;
                args[1].Pointer = IntPtr.Zero;
                args[2].Int32 = 0;
                args[3].Int32 = 0;
                // wl_pointer.set_cursor (opcode 0)
                WlProxyMarshalArrayFlags(pointer, 0, null, pointerVersion, 0, args);
                WlDisplayFlush(display);
                return;
            }

            if (cursorShapeDevice != IntPtr.Zero && MapCursorShape(cursor.Shape) is uint wpShape)
            {
                WlArgument* shapeArgs = stackalloc WlArgument[2];
                shapeArgs[0].UInt32 = serial;
                shapeArgs[1].UInt32 = wpShape;
                // wp_cursor_shape_device_v1.set_shape (opcode 1)
                WlProxyMarshalArrayFlags(
                    cursorShapeDevice, 1, null, WlProxyGetVersion(cursorShapeDevice), 0, shapeArgs);
                WlDisplayFlush(display);
                return;
            }

            if (IsWaylandCursorAvailable)
            {
                ApplyWaylandCursorTheme(cursor.Shape, serial);
            }

            return;
        }

        byte[]? pixels = cursor.BgraPixels;
        if (pixels == null || !EnsureCursorSurface())
        {
            return;
        }

        if (cursorBuffer == null || cursorBuffer.Width != cursor.Width || cursorBuffer.Height != cursor.Height)
        {
            if (cursorBuffer != null)
            {
                dispatchTargets.Remove(cursorBuffer.WlBuffer);
                cursorBuffer.Dispose();
                cursorBuffer = null;
            }

            cursorBuffer = WaylandShmBuffer.Create(shm, shmVersion, cursor.Width, cursor.Height);
            RegisterDispatcher(cursorBuffer.WlBuffer, DispatchKind.Buffer, cursorBuffer);
        }

        cursorBuffer.CopyFull(pixels, cursor.Width * 4);

        uint cursorSurfVer = WlProxyGetVersion(cursorSurface);
        double scale = cursor.Scale > 0 ? cursor.Scale : 1.0;
        int logicalW = Math.Max(1, (int)Math.Round(cursor.Width / scale));
        int logicalH = Math.Max(1, (int)Math.Round(cursor.Height / scale));
        int logicalHotX;
        int logicalHotY;

        if (cursorViewport != IntPtr.Zero)
        {
            WlArgument* vpArgs = stackalloc WlArgument[2];
            vpArgs[0].Int32 = logicalW;
            vpArgs[1].Int32 = logicalH;
            WlProxyMarshalArrayFlags(cursorViewport, 2, null, WlProxyGetVersion(cursorViewport), 0, vpArgs);
            logicalHotX = Math.Clamp((int)Math.Round(cursor.HotspotX / scale), 0, logicalW - 1);
            logicalHotY = Math.Clamp((int)Math.Round(cursor.HotspotY / scale), 0, logicalH - 1);
        }
        else
        {
            logicalHotX = cursor.HotspotX;
            logicalHotY = cursor.HotspotY;
        }

        WlArgument* attachArgs = stackalloc WlArgument[3];
        attachArgs[0].Pointer = cursorBuffer.WlBuffer;
        attachArgs[1].Int32 = 0;
        attachArgs[2].Int32 = 0;
        WlProxyMarshalArrayFlags(cursorSurface, 1, null, cursorSurfVer, 0, attachArgs);

        WlArgument* dmgArgs = stackalloc WlArgument[4];
        dmgArgs[0].Int32 = 0;
        dmgArgs[1].Int32 = 0;
        dmgArgs[2].Int32 = cursor.Width;
        dmgArgs[3].Int32 = cursor.Height;
        if (cursorSurfVer >= 4)
        {
            WlProxyMarshalArrayFlags(cursorSurface, 9, null, cursorSurfVer, 0, dmgArgs);
        }
        else
        {
            WlProxyMarshalArrayFlags(cursorSurface, 2, null, cursorSurfVer, 0, dmgArgs);
        }

        WlProxyMarshalArrayFlags(cursorSurface, 6, null, cursorSurfVer, 0, null);

        WlArgument* setCursorArgs = stackalloc WlArgument[4];
        setCursorArgs[0].UInt32 = serial;
        setCursorArgs[1].Pointer = cursorSurface;
        setCursorArgs[2].Int32 = logicalHotX;
        setCursorArgs[3].Int32 = logicalHotY;
        WlProxyMarshalArrayFlags(pointer, 0, null, pointerVersion, 0, setCursorArgs);
        WlDisplayFlush(display);
    }

    private bool EnsureCursorSurface()
    {
        if (cursorSurface != IntPtr.Zero)
        {
            return true;
        }

        WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;
        WlArgument* surfArgs = stackalloc WlArgument[1];
        surfArgs[0].Pointer = IntPtr.Zero;
        cursorSurface = WlProxyMarshalArrayFlags(compositor, 0, ifaces.WlSurface, compositorVersion, 0, surfArgs);
        if (cursorSurface == IntPtr.Zero)
        {
            return false;
        }

        if (viewporter != IntPtr.Zero)
        {
            WlArgument* vpArgs = stackalloc WlArgument[2];
            vpArgs[0].Pointer = IntPtr.Zero;
            vpArgs[1].Pointer = cursorSurface;
            cursorViewport = WlProxyMarshalArrayFlags(viewporter, 1, ifaces.WpViewport, viewporterVersion, 0, vpArgs);
        }

        return true;
    }

    private void ApplyWaylandCursorTheme(OverlayCursorShape shape, uint serial)
    {
        if (shm == IntPtr.Zero || pointerFrame == null || !EnsureCursorSurface())
        {
            return;
        }

        int scale = 1;
        if (surfaceToOutput.TryGetValue(pointerFrame.ActiveSurfaceId, out OutputState? activeOutput))
        {
            scale = Math.Max(1, activeOutput.IntegerScale);
        }

        if (cursorTheme == IntPtr.Zero || cursorThemeScale != scale)
        {
            if (cursorTheme != IntPtr.Zero)
            {
                WlCursorThemeDestroy(cursorTheme);
                cursorTheme = IntPtr.Zero;
            }

            string? themeName = Environment.GetEnvironmentVariable("XCURSOR_THEME");
            int baseSize = 24;
            if (int.TryParse(Environment.GetEnvironmentVariable("XCURSOR_SIZE"), out int parsedSize) && parsedSize > 0)
            {
                baseSize = Math.Clamp(parsedSize, 8, 128);
            }

            cursorTheme = WlCursorThemeLoad(string.IsNullOrEmpty(themeName) ? null : themeName, baseSize * scale, shm);
            cursorThemeScale = scale;
        }

        if (cursorTheme == IntPtr.Zero)
        {
            return;
        }

        WlCursor* wlCursor = null;
        foreach (string name in GetXCursorCandidateNames(shape))
        {
            wlCursor = WlCursorThemeGetCursor(cursorTheme, name);
            if (wlCursor != null && wlCursor->ImageCount > 0)
            {
                break;
            }

            wlCursor = null;
        }

        if (wlCursor == null)
        {
            wlCursor = WlCursorThemeGetCursor(cursorTheme, "left_ptr");
        }

        if (wlCursor == null || wlCursor->ImageCount == 0 || wlCursor->Images == null || wlCursor->Images[0] == null)
        {
            return;
        }

        WlCursorImage* img = wlCursor->Images[0];
        IntPtr buf = WlCursorImageGetBuffer((IntPtr)img);
        if (buf == IntPtr.Zero)
        {
            return;
        }

        uint cursorSurfVer = WlProxyGetVersion(cursorSurface);
        int imgW = (int)img->Width;
        int imgH = (int)img->Height;
        int logicalW = Math.Max(1, imgW / scale);
        int logicalH = Math.Max(1, imgH / scale);

        if (cursorViewport != IntPtr.Zero)
        {
            WlArgument* vpArgs = stackalloc WlArgument[2];
            vpArgs[0].Int32 = logicalW;
            vpArgs[1].Int32 = logicalH;
            WlProxyMarshalArrayFlags(cursorViewport, 2, null, WlProxyGetVersion(cursorViewport), 0, vpArgs);
        }
        else if (cursorSurfVer >= 3)
        {
            WlArgument* scaleArg = stackalloc WlArgument[1];
            scaleArg[0].Int32 = scale;
            WlProxyMarshalArrayFlags(cursorSurface, 8, null, cursorSurfVer, 0, scaleArg);
        }

        WlArgument* attachArgs = stackalloc WlArgument[3];
        attachArgs[0].Pointer = buf;
        attachArgs[1].Int32 = 0;
        attachArgs[2].Int32 = 0;
        WlProxyMarshalArrayFlags(cursorSurface, 1, null, cursorSurfVer, 0, attachArgs);

        WlArgument* dmgArgs = stackalloc WlArgument[4];
        dmgArgs[0].Int32 = 0;
        dmgArgs[1].Int32 = 0;
        dmgArgs[2].Int32 = imgW;
        dmgArgs[3].Int32 = imgH;
        if (cursorSurfVer >= 4)
        {
            WlProxyMarshalArrayFlags(cursorSurface, 9, null, cursorSurfVer, 0, dmgArgs);
        }
        else
        {
            dmgArgs[2].Int32 = logicalW;
            dmgArgs[3].Int32 = logicalH;
            WlProxyMarshalArrayFlags(cursorSurface, 2, null, cursorSurfVer, 0, dmgArgs);
        }

        WlProxyMarshalArrayFlags(cursorSurface, 6, null, cursorSurfVer, 0, null);

        WlArgument* setCursorArgs = stackalloc WlArgument[4];
        setCursorArgs[0].UInt32 = serial;
        setCursorArgs[1].Pointer = cursorSurface;
        setCursorArgs[2].Int32 = (int)(img->HotspotX / (uint)scale);
        setCursorArgs[3].Int32 = (int)(img->HotspotY / (uint)scale);
        WlProxyMarshalArrayFlags(pointer, 0, null, pointerVersion, 0, setCursorArgs);
        WlDisplayFlush(display);
    }

    private static string[] GetXCursorCandidateNames(OverlayCursorShape shape) => shape switch
    {
        OverlayCursorShape.Crosshair => ["crosshair", "tcross"],
        OverlayCursorShape.Pointer => ["pointer", "hand2", "hand"],
        OverlayCursorShape.Grab => ["grab", "openhand", "hand2"],
        OverlayCursorShape.Grabbing => ["grabbing", "closedhand", "fleur"],
        OverlayCursorShape.Move => ["move", "fleur"],
        OverlayCursorShape.Text => ["text", "xterm"],
        OverlayCursorShape.Wait => ["wait", "watch"],
        OverlayCursorShape.Progress => ["progress", "left_ptr_watch"],
        OverlayCursorShape.Help => ["help", "question_arrow"],
        OverlayCursorShape.NotAllowed => ["not-allowed", "crossed_circle"],
        OverlayCursorShape.NResize => ["n-resize", "top_side"],
        OverlayCursorShape.SResize => ["s-resize", "bottom_side"],
        OverlayCursorShape.EResize => ["e-resize", "right_side"],
        OverlayCursorShape.WResize => ["w-resize", "left_side"],
        OverlayCursorShape.NEResize => ["ne-resize", "top_right_corner"],
        OverlayCursorShape.NWResize => ["nw-resize", "top_left_corner"],
        OverlayCursorShape.SEResize => ["se-resize", "bottom_right_corner"],
        OverlayCursorShape.SWResize => ["sw-resize", "bottom_left_corner"],
        OverlayCursorShape.EWResize => ["ew-resize", "sb_h_double_arrow"],
        OverlayCursorShape.NSResize => ["ns-resize", "sb_v_double_arrow"],
        OverlayCursorShape.AllScroll => ["all-scroll", "fleur"],
        OverlayCursorShape.Copy => ["copy", "dnd-copy"],
        OverlayCursorShape.Alias => ["alias", "dnd-link"],
        _ => ["default", "left_ptr"]
    };

    private void RegisterDispatcher(IntPtr proxy, DispatchKind kind, object? state)
    {
        dispatchTargets[proxy] = new DispatchTarget(kind, state);
        WlProxyAddDispatcher(proxy, &NativeDispatch, (IntPtr)selfHandle, IntPtr.Zero);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static int NativeDispatch(IntPtr dispatcherData, IntPtr target, uint opcode, IntPtr msg, WlArgument* args)
    {
        try
        {
            if (dispatcherData == IntPtr.Zero)
            {
                return 0;
            }

            GCHandle handle = GCHandle.FromIntPtr(dispatcherData);
            if (!handle.IsAllocated || handle.Target is not WaylandLayerOverlayWindow self)
            {
                return 0;
            }

            if (!self.dispatchTargets.TryGetValue(target, out DispatchTarget entry))
            {
                return 0;
            }

            self.HandleDispatch(entry, target, opcode, args);
        }
        catch (Exception ex)
        {
            // Never allow a managed exception to unwind across libwayland-client's C stack.
            Debug.WriteLine($"Wayland dispatcher error: {ex}");
        }

        return 0;
    }

    private void HandleDispatch(DispatchTarget entry, IntPtr target, uint opcode, WlArgument* args)
    {
        switch (entry.Kind)
        {
            case DispatchKind.Registry:
                HandleRegistryEvent(opcode, args);
                break;
            case DispatchKind.Output:
                HandleOutputEvent((OutputState)entry.State!, opcode, args);
                break;
            case DispatchKind.XdgOutput:
                HandleXdgOutputEvent((OutputState)entry.State!, opcode, args);
                break;
            case DispatchKind.Seat:
                HandleSeatEvent(opcode, args);
                break;
            case DispatchKind.Pointer:
                HandlePointerEvent(opcode, args);
                break;
            case DispatchKind.Keyboard:
                HandleKeyboardEvent(opcode, args);
                break;
            case DispatchKind.LayerSurface:
                HandleLayerSurfaceEvent((OutputState)entry.State!, target, opcode, args);
                break;
            case DispatchKind.Buffer:
                if (opcode == 0 && entry.State is WaylandShmBuffer buffer)
                {
                    buffer.Busy = false;
                    MarkPendingFrameIfAnyStale();
                }
                break;

            case DispatchKind.FrameCallback:
                if (opcode == 0 && entry.State is OutputState cbOutput)
                {
                    dispatchTargets.Remove(target);
                    WlProxyDestroy(target);
                    if (cbOutput.FrameCallback == target)
                    {
                        cbOutput.FrameCallback = IntPtr.Zero;
                    }

                    MarkPendingFrameIfStale(cbOutput);
                }
                break;
        }
    }

    private void HandleRegistryEvent(uint opcode, WlArgument* args)
    {
        if (opcode != 0)
        {
            return;
        }

        uint name = args[0].UInt32;
        string? ifaceName = Marshal.PtrToStringUTF8(args[1].Pointer);
        uint version = args[2].UInt32;
        if (string.IsNullOrEmpty(ifaceName))
        {
            return;
        }

        WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;

        switch (ifaceName)
        {
            case "wl_compositor":
                if (compositor == IntPtr.Zero)
                {
                    compositorVersion = Math.Min(version, 6u);
                    compositor = BindGlobal(name, ifaceName, ifaces.WlCompositor, compositorVersion);
                }
                break;

            case "wl_shm":
                if (shm == IntPtr.Zero)
                {
                    shmVersion = Math.Min(version, 1u);
                    shm = BindGlobal(name, ifaceName, ifaces.WlShm, shmVersion);
                }
                break;

            case "wl_seat":
                if (seat == IntPtr.Zero)
                {
                    seatVersion = Math.Min(version, 8u);
                    seat = BindGlobal(name, ifaceName, ifaces.WlSeat, seatVersion);
                    if (seat != IntPtr.Zero)
                    {
                        RegisterDispatcher(seat, DispatchKind.Seat, null);
                    }
                }
                break;

            case "zwlr_layer_shell_v1":
                if (layerShell == IntPtr.Zero)
                {
                    layerShellVersion = Math.Min(version, 4u);
                    layerShell = BindGlobal(name, ifaceName, ifaces.ZwlrLayerShellV1, layerShellVersion);
                }
                break;

            case "zxdg_output_manager_v1":
                if (xdgOutputManager == IntPtr.Zero)
                {
                    xdgOutputManagerVersion = Math.Min(version, 3u);
                    xdgOutputManager = BindGlobal(name, ifaceName, ifaces.ZXdgOutputManagerV1, xdgOutputManagerVersion);
                }
                break;

            case "wp_viewporter":
                if (viewporter == IntPtr.Zero)
                {
                    viewporterVersion = Math.Min(version, 1u);
                    viewporter = BindGlobal(name, ifaceName, ifaces.WpViewporter, viewporterVersion);
                }
                break;

            case "wp_cursor_shape_manager_v1":
                if (cursorShapeManager == IntPtr.Zero)
                {
                    cursorShapeManagerVersion = Math.Min(version, 1u);
                    cursorShapeManager = BindGlobal(name, ifaceName, ifaces.WpCursorShapeManagerV1, cursorShapeManagerVersion);
                }
                break;

            case "wl_output":
            {
                uint bindVer = Math.Min(version, 4u);
                IntPtr wlOutput = BindGlobal(name, ifaceName, ifaces.WlOutput, bindVer);
                if (wlOutput != IntPtr.Zero)
                {
                    OutputState output = new(name, wlOutput, bindVer);
                    allOutputs.Add(output);
                    RegisterDispatcher(wlOutput, DispatchKind.Output, output);
                }
                break;
            }
        }
    }

    private IntPtr BindGlobal(uint name, string interfaceName, WlInterface* iface, uint bindVersion)
    {
        IntPtr namePtr = Marshal.StringToCoTaskMemUTF8(interfaceName);
        try
        {
            WlArgument* bindArgs = stackalloc WlArgument[4];
            bindArgs[0].UInt32 = name;
            bindArgs[1].Pointer = namePtr;
            bindArgs[2].UInt32 = bindVersion;
            bindArgs[3].Pointer = IntPtr.Zero;
            // wl_registry.bind (opcode 0)
            return WlProxyMarshalArrayFlags(registry, 0, iface, bindVersion, 0, bindArgs);
        }
        finally
        {
            Marshal.FreeCoTaskMem(namePtr);
        }
    }

    private static void HandleOutputEvent(OutputState output, uint opcode, WlArgument* args)
    {
        switch (opcode)
        {
            case 0: // geometry(x, y, physW, physH, subpixel, make, model, transform)
                output.RawX = args[0].Int32;
                output.RawY = args[1].Int32;
                output.Transform = args[7].Int32;
                break;
            case 1: // mode(flags, width, height, refresh)
                if ((args[0].UInt32 & 1) != 0 || (output.ModeWidth == 0 && output.ModeHeight == 0))
                {
                    output.ModeWidth = args[1].Int32;
                    output.ModeHeight = args[2].Int32;
                }
                break;
            case 3: // scale(factor)
                output.IntegerScale = Math.Max(1, args[0].Int32);
                break;
            case 4: // name(string)
                output.Name = Marshal.PtrToStringUTF8(args[0].Pointer);
                break;
        }
    }

    private static void HandleXdgOutputEvent(OutputState output, uint opcode, WlArgument* args)
    {
        switch (opcode)
        {
            case 0: // logical_position(x, y)
                output.XdgX = args[0].Int32;
                output.XdgY = args[1].Int32;
                output.HasXdgPosition = true;
                break;
            case 1: // logical_size(width, height)
                output.XdgWidth = args[0].Int32;
                output.XdgHeight = args[1].Int32;
                output.HasXdgSize = true;
                break;
            case 3: // name(string)
                output.Name ??= Marshal.PtrToStringUTF8(args[0].Pointer);
                break;
        }
    }

    private void HandleSeatEvent(uint opcode, WlArgument* args)
    {
        if (opcode != 0)
        {
            return;
        }

        uint caps = args[0].UInt32;
        WaylandProtocolInterfaces.InterfaceTable ifaces = WaylandProtocolInterfaces.All;

        if ((caps & SeatCapPointer) != 0 && pointer == IntPtr.Zero)
        {
            WlArgument* pArgs = stackalloc WlArgument[1];
            pArgs[0].Pointer = IntPtr.Zero;
            // wl_seat.get_pointer (opcode 0)
            pointer = WlProxyMarshalArrayFlags(seat, 0, ifaces.WlPointer, seatVersion, 0, pArgs);
            if (pointer != IntPtr.Zero)
            {
                pointerVersion = seatVersion;
                RegisterDispatcher(pointer, DispatchKind.Pointer, null);

                if (cursorShapeManager != IntPtr.Zero)
                {
                    WlArgument* csArgs = stackalloc WlArgument[2];
                    csArgs[0].Pointer = IntPtr.Zero;
                    csArgs[1].Pointer = pointer;
                    // wp_cursor_shape_manager_v1.get_pointer (opcode 1)
                    cursorShapeDevice = WlProxyMarshalArrayFlags(
                        cursorShapeManager, 1, ifaces.WpCursorShapeDeviceV1, cursorShapeManagerVersion, 0, csArgs);
                }
            }
        }

        if ((caps & SeatCapKeyboard) != 0 && keyboard == IntPtr.Zero)
        {
            WlArgument* kArgs = stackalloc WlArgument[1];
            kArgs[0].Pointer = IntPtr.Zero;
            // wl_seat.get_keyboard (opcode 1)
            keyboard = WlProxyMarshalArrayFlags(seat, 1, ifaces.WlKeyboard, seatVersion, 0, kArgs);
            if (keyboard != IntPtr.Zero)
            {
                keyboardVersion = seatVersion;
                RegisterDispatcher(keyboard, DispatchKind.Keyboard, null);
            }
        }
    }

    private void HandlePointerEvent(uint opcode, WlArgument* args)
    {
        if (pointerFrame == null)
        {
            return;
        }

        switch (opcode)
        {
            case 0: // enter(serial, surface, surface_x, surface_y)
                pointerFrame.EnqueueEnter(
                    args[1].Pointer.ToInt64(),
                    args[0].UInt32,
                    args[2].FixedToDouble(),
                    args[3].FixedToDouble());
                break;

            case 1: // leave(serial, surface)
                pointerFrame.EnqueueLeave(args[1].Pointer.ToInt64(), args[0].UInt32);
                break;

            case 2: // motion(time, surface_x, surface_y)
                pointerFrame.EnqueueMotion(
                    args[0].UInt32,
                    args[1].FixedToDouble(),
                    args[2].FixedToDouble());
                break;

            case 3: // button(serial, time, button, state)
                pointerFrame.EnqueueButton(args[1].UInt32, args[2].UInt32, args[3].UInt32 == 1);
                break;

            case 4: // axis(time, axis, value)
                pointerFrame.EnqueueAxis(args[0].UInt32, args[1].UInt32, args[2].FixedToDouble());
                break;

            case 5: // frame()
                FlushPointerFrame();
                break;

            case 8: // axis_discrete(axis, discrete)
                pointerFrame.EnqueueAxisDiscrete(args[0].UInt32, args[1].Int32);
                break;

            case 9: // axis_value120(axis, value120)
                pointerFrame.EnqueueAxisValue120(args[0].UInt32, args[1].Int32);
                break;
        }

        if (pointerVersion < 5 && opcode <= 4)
        {
            FlushPointerFrame();
        }
    }

    private void FlushPointerFrame()
    {
        if (pointerFrame == null)
        {
            return;
        }

        pointerFrame.FlushFrame(
            keyboardModifiers,
            ev => PointerEvent?.Invoke(ev),
            wheel => WheelEvent?.Invoke(wheel));

        if (pointerFrame.PendingCursorApply)
        {
            pointerFrame.ClearPendingCursorApply();
            OverlayCursor activeCursor;
            lock (cursorLock)
            {
                activeCursor = pendingCursor;
                hasPendingCursor = false;
            }
            ApplyCursorNow(activeCursor);
        }
    }

    private void HandleKeyboardEvent(uint opcode, WlArgument* args)
    {
        switch (opcode)
        {
            case 0: // keymap(format, fd, size)
            {
                uint format = args[0].UInt32;
                int fd = args[1].Fd;
                uint size = args[2].UInt32;
                LoadKeymap(format, fd, size);
                break;
            }

            case 1: // enter(serial, surface, keys)
                if (surfaceToOutput.ContainsKey(args[1].Pointer.ToInt64()))
                {
                    bool wasFocused = keyboardFocusedSurfaces > 0;
                    keyboardFocusedSurfaces++;
                    if (!wasFocused)
                    {
                        Activated?.Invoke();
                    }
                }
                break;

            case 2: // leave(serial, surface)
                if (surfaceToOutput.ContainsKey(args[1].Pointer.ToInt64()))
                {
                    keyboardFocusedSurfaces = Math.Max(0, keyboardFocusedSurfaces - 1);
                    if (keyboardFocusedSurfaces == 0 && surfacesVisible && !stopRequested)
                    {
                        Deactivated?.Invoke();
                    }
                }
                break;

            case 3: // key(serial, time, key, state)
            {
                uint time = args[1].UInt32;
                uint evdevKey = args[2].UInt32;
                bool isDown = args[3].UInt32 == 1;
                ResolveKey(evdevKey, isDown, out uint keysym, out string? text);
                KeyEvent?.Invoke(new OverlayKeyEvent(isDown, evdevKey, keysym, text, time, keyboardModifiers));
                break;
            }

            case 4: // modifiers(serial, mods_depressed, mods_latched, mods_locked, group)
                if (xkbState != IntPtr.Zero)
                {
                    XkbStateUpdateMask(
                        xkbState,
                        args[1].UInt32,
                        args[2].UInt32,
                        args[3].UInt32,
                        0,
                        0,
                        args[4].UInt32);
                    OverlayInputModifiers mods = OverlayInputModifiers.None;
                    if (XkbStateModNameIsActive(xkbState, "Shift", XKB_STATE_MODS_EFFECTIVE) > 0) mods |= OverlayInputModifiers.Shift;
                    if (XkbStateModNameIsActive(xkbState, "Control", XKB_STATE_MODS_EFFECTIVE) > 0) mods |= OverlayInputModifiers.Control;
                    if (XkbStateModNameIsActive(xkbState, "Mod1", XKB_STATE_MODS_EFFECTIVE) > 0) mods |= OverlayInputModifiers.Alt;
                    if (XkbStateModNameIsActive(xkbState, "Mod4", XKB_STATE_MODS_EFFECTIVE) > 0) mods |= OverlayInputModifiers.Meta;
                    keyboardModifiers = mods;
                }
                break;

            case 5: // repeat_info(rate, delay)
                KeyRepeatInfoChanged?.Invoke(args[0].Int32, args[1].Int32);
                break;
        }
    }

    private void LoadKeymap(uint format, int fd, uint size)
    {
        try
        {
            if (xkbContext == IntPtr.Zero)
            {
                return;
            }

            IntPtr newMap;
            if (format == WL_KEYBOARD_KEYMAP_FORMAT_NO_KEYMAP)
            {
                newMap = XkbKeymapNewFromNames(xkbContext, IntPtr.Zero, XKB_KEYMAP_COMPILE_NO_FLAGS);
            }
            else if (format == XKB_KEYMAP_FORMAT_TEXT_V1 && size > 1)
            {
                IntPtr mapped = Mmap(IntPtr.Zero, size, PROT_READ, MAP_PRIVATE, fd, 0);
                if (mapped == MAP_FAILED || mapped == IntPtr.Zero)
                {
                    return;
                }

                try
                {
                    newMap = XkbKeymapNewFromBuffer(
                        xkbContext,
                        (byte*)mapped,
                        size - 1,
                        XKB_KEYMAP_FORMAT_TEXT_V1,
                        XKB_KEYMAP_COMPILE_NO_FLAGS);
                }
                finally
                {
                    Munmap(mapped, size);
                }
            }
            else
            {
                return;
            }

            if (newMap == IntPtr.Zero)
            {
                return;
            }

            IntPtr newState = XkbStateNew(newMap);
            if (newState == IntPtr.Zero)
            {
                XkbKeymapUnref(newMap);
                return;
            }

            if (xkbState != IntPtr.Zero) XkbStateUnref(xkbState);
            if (xkbKeymap != IntPtr.Zero) XkbKeymapUnref(xkbKeymap);
            xkbKeymap = newMap;
            xkbState = newState;
        }
        finally
        {
            if (fd >= 0)
            {
                Close(fd);
            }
        }
    }

    private void ResolveKey(uint evdevKeycode, bool isDown, out uint keysym, out string? text)
    {
        keysym = 0;
        text = null;

        if (xkbState == IntPtr.Zero)
        {
            return;
        }

        uint xkbKey = evdevKeycode + 8;
        keysym = XkbStateKeyGetOneSym(xkbState, xkbKey);

        if (!isDown || (keyboardModifiers & (OverlayInputModifiers.Control | OverlayInputModifiers.Alt | OverlayInputModifiers.Meta)) != 0)
        {
            return;
        }

        int needed = XkbStateKeyGetUtf8(xkbState, xkbKey, null, 0);
        if (needed <= 0)
        {
            return;
        }

        int bufSize = needed + 1;
        byte* buf = stackalloc byte[bufSize];
        XkbStateKeyGetUtf8(xkbState, xkbKey, buf, (nuint)bufSize);
        string raw = Encoding.UTF8.GetString(buf, needed);

        if (raw.Length == 1 && (raw[0] < ' ' || raw[0] == '\x7f'))
        {
            return;
        }

        text = raw;
    }

    private void HandleLayerSurfaceEvent(OutputState output, IntPtr layerSurface, uint opcode, WlArgument* args)
    {
        switch (opcode)
        {
            case 0: // configure(serial, width, height)
            {
                uint serial = args[0].UInt32;
                int width = (int)args[1].UInt32;
                int height = (int)args[2].UInt32;
                if (width > 0) output.ConfiguredWidth = width;
                if (height > 0) output.ConfiguredHeight = height;
                output.Configured = true;
                output.OpaqueRegionApplied = false;

                WlArgument* ackArgs = stackalloc WlArgument[1];
                ackArgs[0].UInt32 = serial;
                // zwlr_layer_surface_v1.ack_configure (opcode 6)
                WlProxyMarshalArrayFlags(layerSurface, 6, null, WlProxyGetVersion(layerSurface), 0, ackArgs);
                MarkPendingFrameIfStale(output);
                break;
            }

            case 1: // closed()
                if (!compositorClosed && !stopRequested)
                {
                    compositorClosed = true;
                    ClosedByCompositor?.Invoke();
                }
                break;
        }
    }

    private void DestroyOverlaySurfaces()
    {
        foreach (OutputState output in targetOutputs)
        {
            if (output.LayerSurface != IntPtr.Zero)
            {
                dispatchTargets.Remove(output.LayerSurface);
                // zwlr_layer_surface_v1.destroy (opcode 7)
                WlProxyMarshalArrayFlags(
                    output.LayerSurface, 7, null, WlProxyGetVersion(output.LayerSurface), WL_MARSHAL_FLAG_DESTROY, null);
                output.LayerSurface = IntPtr.Zero;
            }

            if (output.Viewport != IntPtr.Zero)
            {
                // wp_viewport.destroy (opcode 0)
                WlProxyMarshalArrayFlags(
                    output.Viewport, 0, null, WlProxyGetVersion(output.Viewport), WL_MARSHAL_FLAG_DESTROY, null);
                output.Viewport = IntPtr.Zero;
            }

            if (output.FrameCallback != IntPtr.Zero)
            {
                dispatchTargets.Remove(output.FrameCallback);
                WlProxyDestroy(output.FrameCallback);
                output.FrameCallback = IntPtr.Zero;
            }

            if (output.WlSurface != IntPtr.Zero)
            {
                surfaceToOutput.Remove(output.WlSurface.ToInt64());
                // wl_surface.destroy (opcode 0)
                WlProxyMarshalArrayFlags(
                    output.WlSurface, 0, null, WlProxyGetVersion(output.WlSurface), WL_MARSHAL_FLAG_DESTROY, null);
                output.WlSurface = IntPtr.Zero;
            }

            foreach (WaylandShmBuffer buf in output.Buffers)
            {
                dispatchTargets.Remove(buf.WlBuffer);
                buf.Dispose();
            }

            output.Buffers.Clear();
            output.Configured = false;
            output.OpaqueRegionApplied = false;
            output.CommittedFrameId = 0;
        }
    }

    private void CleanupWayland()
    {
        if (display == IntPtr.Zero)
        {
            return;
        }

        DestroyOverlaySurfaces();

        if (cursorBuffer != null)
        {
            dispatchTargets.Remove(cursorBuffer.WlBuffer);
            cursorBuffer.Dispose();
            cursorBuffer = null;
        }

        if (cursorViewport != IntPtr.Zero)
        {
            WlProxyMarshalArrayFlags(cursorViewport, 0, null, WlProxyGetVersion(cursorViewport), WL_MARSHAL_FLAG_DESTROY, null);
            cursorViewport = IntPtr.Zero;
        }

        if (cursorSurface != IntPtr.Zero)
        {
            WlProxyMarshalArrayFlags(cursorSurface, 0, null, WlProxyGetVersion(cursorSurface), WL_MARSHAL_FLAG_DESTROY, null);
            cursorSurface = IntPtr.Zero;
        }

        if (cursorShapeDevice != IntPtr.Zero)
        {
            WlProxyMarshalArrayFlags(cursorShapeDevice, 0, null, WlProxyGetVersion(cursorShapeDevice), WL_MARSHAL_FLAG_DESTROY, null);
            cursorShapeDevice = IntPtr.Zero;
        }

        if (cursorTheme != IntPtr.Zero)
        {
            WlCursorThemeDestroy(cursorTheme);
            cursorTheme = IntPtr.Zero;
        }

        DestroyOptionalProtocolProxy(ref pointer, 1, 3);
        DestroyOptionalProtocolProxy(ref keyboard, 0, 3);
        DestroyOptionalProtocolProxy(ref seat, 3, 5);
        DestroyOptionalProtocolProxy(ref layerShell, 1, 3);
        DestroySimpleDestructorProxy(ref xdgOutputManager, 0);
        DestroySimpleDestructorProxy(ref viewporter, 0);
        DestroySimpleDestructorProxy(ref cursorShapeManager, 0);

        foreach (OutputState output in allOutputs)
        {
            if (output.XdgOutput != IntPtr.Zero)
            {
                dispatchTargets.Remove(output.XdgOutput);
                WlProxyMarshalArrayFlags(output.XdgOutput, 0, null, WlProxyGetVersion(output.XdgOutput), WL_MARSHAL_FLAG_DESTROY, null);
                output.XdgOutput = IntPtr.Zero;
            }

            if (output.WlOutput != IntPtr.Zero)
            {
                dispatchTargets.Remove(output.WlOutput);
                if (output.Version >= 3)
                {
                    WlProxyMarshalArrayFlags(output.WlOutput, 0, null, output.Version, WL_MARSHAL_FLAG_DESTROY, null);
                }
                else
                {
                    WlProxyDestroy(output.WlOutput);
                }
                output.WlOutput = IntPtr.Zero;
            }
        }

        if (shm != IntPtr.Zero)
        {
            WlProxyDestroy(shm);
            shm = IntPtr.Zero;
        }

        if (compositor != IntPtr.Zero)
        {
            WlProxyDestroy(compositor);
            compositor = IntPtr.Zero;
        }

        if (registry != IntPtr.Zero)
        {
            dispatchTargets.Remove(registry);
            WlProxyDestroy(registry);
            registry = IntPtr.Zero;
        }

        // Roundtrip before disconnect ensures the compositor unmaps the layer surfaces immediately
        // (critical when transparent capture takes a screenshot right after the overlay closes).
        WlDisplayRoundtrip(display);
        WlDisplayDisconnect(display);
        display = IntPtr.Zero;

        if (xkbState != IntPtr.Zero)
        {
            XkbStateUnref(xkbState);
            xkbState = IntPtr.Zero;
        }

        if (xkbKeymap != IntPtr.Zero)
        {
            XkbKeymapUnref(xkbKeymap);
            xkbKeymap = IntPtr.Zero;
        }

        if (xkbContext != IntPtr.Zero)
        {
            XkbContextUnref(xkbContext);
            xkbContext = IntPtr.Zero;
        }
    }

    private void DestroyOptionalProtocolProxy(ref IntPtr proxy, uint releaseOpcode, uint minReleaseVersion)
    {
        if (proxy == IntPtr.Zero)
        {
            return;
        }

        dispatchTargets.Remove(proxy);
        uint ver = WlProxyGetVersion(proxy);
        if (ver >= minReleaseVersion)
        {
            WlProxyMarshalArrayFlags(proxy, releaseOpcode, null, ver, WL_MARSHAL_FLAG_DESTROY, null);
        }
        else
        {
            WlProxyDestroy(proxy);
        }

        proxy = IntPtr.Zero;
    }

    private void DestroySimpleDestructorProxy(ref IntPtr proxy, uint destroyOpcode)
    {
        if (proxy == IntPtr.Zero)
        {
            return;
        }

        dispatchTargets.Remove(proxy);
        WlProxyMarshalArrayFlags(proxy, destroyOpcode, null, WlProxyGetVersion(proxy), WL_MARSHAL_FLAG_DESTROY, null);
        proxy = IntPtr.Zero;
    }

    private enum DispatchKind
    {
        Registry,
        Output,
        XdgOutput,
        Seat,
        Pointer,
        Keyboard,
        LayerSurface,
        Buffer,
        FrameCallback
    }

    private readonly record struct DispatchTarget(DispatchKind Kind, object? State);

    private sealed class OutputState(uint registryName, IntPtr wlOutput, uint version)
    {
        public uint RegistryName { get; } = registryName;
        public IntPtr WlOutput { get; set; } = wlOutput;
        public uint Version { get; } = version;
        public IntPtr XdgOutput { get; set; }
        public IntPtr WlSurface { get; set; }
        public IntPtr LayerSurface { get; set; }
        public IntPtr Viewport { get; set; }
        public IntPtr FrameCallback { get; set; }
        public long CommittedFrameId { get; set; }
        public List<WaylandShmBuffer> Buffers { get; } = new();

        public int RawX { get; set; }
        public int RawY { get; set; }
        public int ModeWidth { get; set; }
        public int ModeHeight { get; set; }
        public int Transform { get; set; }
        public int IntegerScale { get; set; } = 1;
        public bool HasXdgPosition { get; set; }
        public int XdgX { get; set; }
        public int XdgY { get; set; }
        public bool HasXdgSize { get; set; }
        public int XdgWidth { get; set; }
        public int XdgHeight { get; set; }
        public string? Name { get; set; }

        public PlatformRectangle LogicalBounds { get; private set; }
        public double ScaleFactor { get; private set; } = 1.0;

        public int ConfiguredWidth { get; set; }
        public int ConfiguredHeight { get; set; }
        public bool Configured { get; set; }
        public bool OpaqueRegionApplied { get; set; }

        public void UpdateLogicalBounds(bool isFirst)
        {
            _ = isFirst;
            LogicalBounds = ComputeOutputLogicalBounds(
                RawX,
                RawY,
                ModeWidth,
                ModeHeight,
                Transform,
                IntegerScale,
                HasXdgPosition,
                XdgX,
                XdgY,
                HasXdgSize,
                XdgWidth,
                XdgHeight);

            int physW = (Transform & 1) != 0 ? ModeHeight : ModeWidth;
            if (LogicalBounds.Width > 0 && physW > 0)
            {
                ScaleFactor = physW / (double)LogicalBounds.Width;
            }
            else
            {
                ScaleFactor = Math.Max(1, IntegerScale);
            }

            if (LogicalBounds.Width > 0 && LogicalBounds.Height > 0)
            {
                ConfiguredWidth = LogicalBounds.Width;
                ConfiguredHeight = LogicalBounds.Height;
            }
        }
    }
}
