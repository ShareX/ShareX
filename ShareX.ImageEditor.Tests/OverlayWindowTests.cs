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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using ShareX.AvaloniaUI.Input;
using ShareX.AvaloniaUI.Integration;
using ShareX.AvaloniaUI.Windows;
using ShareX.ImageEditor.Integration;
using ShareX.ImageEditor.Presentation.ViewModels;
using ShareX.Platform;
using ShareX.ScreenCaptureLib;
using ShareX.ScreenCaptureLib.Presentation.RegionCapture;
using SkiaSharp;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Text.Json;
using Xunit;
using DrawingRectangle = System.Drawing.Rectangle;

namespace ShareX.ImageEditor.Tests;

public sealed class OverlayWindowTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    [Theory]
    [InlineData(1u, 0xff1bu, Key.Escape, PhysicalKey.Escape)]
    [InlineData(28u, 0xff0du, Key.Return, PhysicalKey.Enter)]
    [InlineData(30u, 0x0061u, Key.A, PhysicalKey.A)]
    [InlineData(2u, 0x0031u, Key.D1, PhysicalKey.Digit1)]
    [InlineData(59u, 0xffbeu, Key.F1, PhysicalKey.F1)]
    [InlineData(88u, 0xffc9u, Key.F12, PhysicalKey.F12)]
    [InlineData(103u, 0xff52u, Key.Up, PhysicalKey.ArrowUp)]
    [InlineData(15u, 0u, Key.Tab, PhysicalKey.Tab)]
    [InlineData(111u, 0u, Key.Delete, PhysicalKey.Delete)]
    public void OverlayKeyMap_ResolvesEvdevAndKeysyms(
        uint evdevKeycode, uint keysym, Key expectedKey, PhysicalKey expectedPhysical)
    {
        (Key actualKey, PhysicalKey actualPhysical) = OverlayKeyMap.Resolve(evdevKeycode, keysym);
        Assert.Equal(expectedKey, actualKey);
        Assert.Equal(expectedPhysical, actualPhysical);
    }

    [Theory]
    [InlineData("cursor-loader")]
    [InlineData("region-drag")]
    [InlineData("region-escape")]
    [InlineData("transparent-drag")]
    public async Task AvaloniaOverlayScenarios_RunCleanlyInIsolatedUiProcess(string mode)
    {
        if (!HasUiSession())
        {
            return;
        }

        await RunOverlayFixtureProcessAsync(mode);
    }

    /// <summary>
    /// Live Hyprland end-to-end test: constructs a real <see cref="RegionCaptureWindow"/> backed by
    /// <c>WaylandLayerOverlayWindow</c>, verifies it maps on layer 3 ("overlay") via <c>hyprctl layers -j</c>,
    /// measures first-frame render time, and closes cleanly. Runs only when SHAREX_WAYLAND_OVERLAY_TEST=1.
    /// </summary>
    [Fact]
    public async Task LiveHyprland_RegionCaptureWindowMapsOnOverlayLayer()
    {
        if (!OperatingSystem.IsLinux() ||
            Environment.GetEnvironmentVariable("SHAREX_WAYLAND_OVERLAY_TEST") != "1" ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return;
        }

        await RunOverlayFixtureProcessAsync("live-hyprland");
    }

    [Fact]
    public async Task LiveHyprland_RegionCaptureWindowUsesNativeAvaloniaLayerShell()
    {
        if (!OperatingSystem.IsLinux() ||
            Environment.GetEnvironmentVariable("SHAREX_WAYLAND_OVERLAY_TEST") != "1" ||
            string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        {
            return;
        }

        await RunOverlayFixtureProcessAsync("live-hyprland-native");
    }

    private static async Task RunOverlayFixtureProcessAsync(string mode)
    {
        ProcessStartInfo info = new("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        info.ArgumentList.Add(typeof(NativeProcessFixture).Assembly.Location);
        info.ArgumentList.Add("--overlay-window");
        info.ArgumentList.Add(mode);

        using Process process = Assert.IsType<Process>(Process.Start(info));
        Task<string> output = process.StandardOutput.ReadToEndAsync();
        Task<string> error = process.StandardError.ReadToEndAsync();
        try
        {
            await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(20));
            string diagnostics = await output.WaitAsync(TimeSpan.FromSeconds(2)) +
                await error.WaitAsync(TimeSpan.FromSeconds(2));
            Assert.True(process.ExitCode == 0, diagnostics);
            Assert.Contains("overlay fixture passed: " + mode, diagnostics);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
    }

    internal static int RunFixture(string[] args)
    {
        try
        {
            string mode = args.Single();
            AvaloniaBootstrapper.EnsureInitialized();
            DesktopServices.Run(async () =>
            {
                try
                {
                    switch (mode)
                    {
                        case "cursor-loader":
                            VerifyCursorAssetLoader();
                            break;
                        case "region-drag":
                            await VerifyRegionCaptureDragAcrossTopBarAsync();
                            break;
                        case "region-escape":
                            await VerifyRegionCaptureEscapeCancelsAsync();
                            break;
                        case "transparent-drag":
                            await VerifyTransparentOverlayDragAndClearAsync();
                            break;
                        case "live-hyprland":
                            await VerifyLiveHyprlandRegionCaptureOverlayAsync();
                            break;
                        case "live-hyprland-native":
                            await VerifyLiveHyprlandNativeAvaloniaLayerShellAsync();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(args), mode, "Unknown overlay fixture mode.");
                    }

                    return true;
                }
                finally
                {
                    AvaloniaBootstrapper.Shutdown();
                }
            });

            Console.WriteLine("overlay fixture passed: " + mode);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void VerifyCursorAssetLoader()
    {
        OverlayCursor cross = CursorAssetLoader.ToOverlayCursor(new Cursor(StandardCursorType.Cross));
        Assert.False(cross.IsBitmap);
        Assert.Equal(OverlayCursorShape.Crosshair, cross.Shape);

        OverlayCursor hand = CursorAssetLoader.ToOverlayCursor(new Cursor(StandardCursorType.Hand));
        Assert.Equal(OverlayCursorShape.Pointer, hand.Shape);

        OverlayCursor sizeAll = CursorAssetLoader.ToOverlayCursor(new Cursor(StandardCursorType.SizeAll));
        Assert.Equal(OverlayCursorShape.Move, sizeAll.Shape);

        OverlayCursor none = CursorAssetLoader.ToOverlayCursor(new Cursor(StandardCursorType.None));
        Assert.Equal(OverlayCursorShape.None, none.Shape);

        OverlayCursor nullCursor = CursorAssetLoader.ToOverlayCursor(null);
        Assert.Equal(OverlayCursorShape.Default, nullCursor.Shape);

        Cursor customCrosshair = CursorAssetLoader.GetCrosshairCursor(1.25);
        OverlayCursor mapped = CursorAssetLoader.ToOverlayCursor(customCrosshair);
        if (PlatformServices.IsInitialized && PlatformServices.Current.Windows.PrefersSystemCursors)
        {
            Assert.False(mapped.IsBitmap);
            Assert.Equal(OverlayCursorShape.Crosshair, mapped.Shape);
        }
        else
        {
            Assert.True(mapped.IsBitmap);
            Assert.True(mapped.Width >= 32 && mapped.Height >= 32);
            Assert.NotNull(mapped.BgraPixels);
        }
    }

    private static async Task VerifyRegionCaptureDragAcrossTopBarAsync()
    {
        FakePlatformOverlayWindow fakeOverlay = new(
            new PlatformRectangle(0, 0, 3200, 1080),
            renderScaling: 1.0,
            transparent: false,
            [
                new OverlayOutputInfo("DP-1", new PlatformRectangle(0, 0, 1920, 1080), 1.0, true),
                new OverlayOutputInfo("HDMI-A-1", new PlatformRectangle(1920, 0, 1280, 1080), 1.0, false)
            ]);

        SKBitmap screenshot = new(3200, 1080, SKColorType.Bgra8888, SKAlphaType.Premul);
        screenshot.Erase(new SKColor(20, 90, 180, 255));
        using (SKCanvas canvas = new(screenshot))
        using (SKPaint paint = new() { Color = new SKColor(240, 200, 40, 255) })
        {
            canvas.DrawRect(new SKRect(100, 5, 2500, 600), paint);
        }

        AvaloniaRegionCaptureRequest request = new()
        {
            Screenshot = screenshot,
            ScreenshotIsOpaque = true,
            ScreenBounds = new DrawingRectangle(0, 0, 3200, 1080),
            EnableAnnotations = false,
            RegionCaptureOptions = new RegionCaptureOptions
            {
                QuickCapture = true,
                DetectWindows = false,
                ShowInfo = false,
                ShowMagnifier = false,
                BackgroundDimStrength = 20
            },
            ImageEditorOptions = new ImageEditorOptions()
        };

        RegionCaptureWindow window = new(request, allowPlatformOverlay: true, customOverlay: fakeOverlay);
        Task<AvaloniaRegionCaptureResult?> captureTask = window.CaptureAsync();

        Assert.True(fakeOverlay.Shown);
        Assert.True(fakeOverlay.PresentCount >= 1);
        Assert.NotNull(fakeOverlay.LastFrame);
        Assert.True(HasNonZeroRgbPixel(fakeOverlay.LastFrame!), "Initial rendered overlay frame should contain visible screenshot pixels.");

        // Start drag directly over the top-bar band (y = 5 on monitor 1) and cross to monitor 2 (x = 2500).
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.Move, 100, 5, 10, OverlayInputModifiers.None));
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.LeftDown, 100, 5, 11, OverlayInputModifiers.LeftMouseButton));
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.Move, 1200, 300, 12, OverlayInputModifiers.LeftMouseButton));
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.Move, 2500, 600, 13, OverlayInputModifiers.LeftMouseButton));
        await PumpUiAsync(DispatcherPriority.Render);
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.LeftUp, 2500, 600, 14, OverlayInputModifiers.None));

        AvaloniaRegionCaptureResult? result = await captureTask.WaitAsync(Timeout);
        Assert.NotNull(result);
        using (result.Image)
        {
            // RegionSelectionOverlay.CreatePixelRectangle includes both endpoints: (2500 - 100 + 1) x (600 - 5 + 1).
            Assert.Equal(new DrawingRectangle(100, 5, 2401, 596), result.ScreenRectangle);
            Assert.Equal(2401, result.Image.Width);
            Assert.Equal(596, result.Image.Height);
        }

        Assert.True(fakeOverlay.Disposed);
    }

    private static async Task VerifyRegionCaptureEscapeCancelsAsync()
    {
        FakePlatformOverlayWindow fakeOverlay = new(
            new PlatformRectangle(0, 0, 1280, 720),
            renderScaling: 1.0,
            transparent: false,
            [new OverlayOutputInfo("eDP-1", new PlatformRectangle(0, 0, 1280, 720), 1.0, true)]);

        SKBitmap screenshot = new(1280, 720, SKColorType.Bgra8888, SKAlphaType.Premul);
        screenshot.Erase(new SKColor(40, 50, 60, 255));

        AvaloniaRegionCaptureRequest request = new()
        {
            Screenshot = screenshot,
            ScreenshotIsOpaque = true,
            ScreenBounds = new DrawingRectangle(0, 0, 1280, 720),
            EnableAnnotations = false,
            RegionCaptureOptions = new RegionCaptureOptions
            {
                QuickCapture = true,
                DetectWindows = false,
                ShowInfo = false,
                ShowMagnifier = false
            },
            ImageEditorOptions = new ImageEditorOptions()
        };

        RegionCaptureWindow window = new(request, allowPlatformOverlay: true, customOverlay: fakeOverlay);
        Task<AvaloniaRegionCaptureResult?> captureTask = window.CaptureAsync();
        await PumpUiAsync(DispatcherPriority.Loaded);

        fakeOverlay.EmitKey(new OverlayKeyEvent(true, 1, 0xff1b, null, 20, OverlayInputModifiers.None));
        fakeOverlay.EmitKey(new OverlayKeyEvent(false, 1, 0xff1b, null, 21, OverlayInputModifiers.None));

        AvaloniaRegionCaptureResult? result = await captureTask.WaitAsync(Timeout);
        Assert.Null(result);
        Assert.True(fakeOverlay.Disposed);
    }

    private static async Task VerifyTransparentOverlayDragAndClearAsync()
    {
        FakePlatformOverlayWindow fakeOverlay = new(
            new PlatformRectangle(0, 0, 640, 480),
            renderScaling: 1.0,
            transparent: true,
            [new OverlayOutputInfo("DP-1", new PlatformRectangle(0, 0, 640, 480), 1.0, true)]);

        RegionCaptureTransparentWindow window = new(
            new DrawingRectangle(0, 0, 640, 480),
            lockCursorToWindow: false,
            customOverlay: fakeOverlay,
            allowPlatformOverlay: true);

        Task<DrawingRectangle?> selectTask = window.SelectAsync();

        Assert.True(fakeOverlay.Shown);
        Assert.NotNull(fakeOverlay.LastFrame);
        Assert.False(HasOpaquePixel(fakeOverlay.LastFrame!), "Initial transparent overlay must leave all pixels transparent (alpha == 0).");

        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.LeftDown, 50, 5, 1, OverlayInputModifiers.LeftMouseButton));
        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.Move, 250, 105, 2, OverlayInputModifiers.LeftMouseButton));
        await PumpUiAsync(DispatcherPriority.Render);

        Assert.NotNull(fakeOverlay.LastFrame);
        Assert.True(HasOpaquePixel(fakeOverlay.LastFrame!), "Selection drag should draw visible dashed outline on transparent overlay.");

        fakeOverlay.EmitPointer(new OverlayPointerEvent(OverlayPointerEventKind.LeftUp, 250, 105, 3, OverlayInputModifiers.None));
        DrawingRectangle? selection = await selectTask.WaitAsync(Timeout);

        Assert.Equal(new DrawingRectangle(50, 5, 201, 101), selection);
        Assert.NotNull(fakeOverlay.LastFrame);
        Assert.False(HasOpaquePixel(fakeOverlay.LastFrame!), "Final frame before close must clear the selection outline.");
        Assert.True(fakeOverlay.Disposed);
    }

    private static async Task VerifyLiveHyprlandRegionCaptureOverlayAsync()
    {
        IPlatformOverlayWindow? liveOverlay = TryCreateRealWaylandOverlay(
            new OverlayWindowOptions(PlatformRectangle.Empty, 1, 1, false, "sharex-region-capture"));
        Assert.NotNull(liveOverlay);

        PlatformRectangle logical = liveOverlay.LogicalBounds;
        int pxW = Math.Max(1, (int)Math.Round(logical.Width * liveOverlay.Outputs[0].ScaleFactor));
        int pxH = Math.Max(1, (int)Math.Round(logical.Height * liveOverlay.Outputs[0].ScaleFactor));
        liveOverlay.Dispose();

        liveOverlay = TryCreateRealWaylandOverlay(
            new OverlayWindowOptions(logical, pxW, pxH, false, "sharex-region-capture"));
        Assert.NotNull(liveOverlay);

        SKBitmap screenshot = new(pxW, pxH, SKColorType.Bgra8888, SKAlphaType.Premul);
        screenshot.Erase(new SKColor(28, 36, 48, 255));

        AvaloniaRegionCaptureRequest request = new()
        {
            Screenshot = screenshot,
            ScreenshotIsOpaque = true,
            ScreenBounds = new DrawingRectangle(logical.X, logical.Y, logical.Width, logical.Height),
            EnableAnnotations = false,
            RegionCaptureOptions = new RegionCaptureOptions
            {
                QuickCapture = true,
                DetectWindows = false,
                ShowInfo = false,
                ShowMagnifier = false
            },
            ImageEditorOptions = new ImageEditorOptions()
        };

        RegionCaptureWindow window = new(request, allowPlatformOverlay: true, customOverlay: liveOverlay);
        Stopwatch sw = Stopwatch.StartNew();
        Task<AvaloniaRegionCaptureResult?> captureTask = window.CaptureAsync();
        await PumpUiAsync(DispatcherPriority.Render);
        sw.Stop();

        Thread.Sleep(120);
        Assert.True(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            $"Expected 'sharex-region-capture' on Hyprland overlay layer 3 (startup {sw.ElapsedMilliseconds} ms).");

        window.Close();
        AvaloniaRegionCaptureResult? result = await captureTask.WaitAsync(Timeout);
        Assert.Null(result);

        Thread.Sleep(100);
        Assert.False(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            "'sharex-region-capture' remained mapped after RegionCaptureWindow closed.");
    }

    private static async Task VerifyLiveHyprlandNativeAvaloniaLayerShellAsync()
    {
        using Process proc = Process.Start(new ProcessStartInfo("hyprctl", "monitors -j")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true
        }) ?? throw new InvalidOperationException("Failed to start hyprctl.");
        string json = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        Assert.Equal(0, proc.ExitCode);

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement monitor = doc.RootElement.EnumerateArray().FirstOrDefault(m =>
            m.TryGetProperty("focused", out JsonElement focused) && focused.GetBoolean());
        if (monitor.ValueKind == JsonValueKind.Undefined)
        {
            monitor = doc.RootElement.EnumerateArray().First();
        }

        int x = (int)Math.Round(monitor.GetProperty("x").GetDouble());
        int y = (int)Math.Round(monitor.GetProperty("y").GetDouble());
        int width = monitor.GetProperty("width").GetInt32();
        int height = monitor.GetProperty("height").GetInt32();
        double scale = monitor.TryGetProperty("scale", out JsonElement scaleProp) && scaleProp.GetDouble() > 0
            ? scaleProp.GetDouble()
            : 1;
        int logicalWidth = Math.Max(1, (int)Math.Round(width / scale));
        int logicalHeight = Math.Max(1, (int)Math.Round(height / scale));

        SKBitmap screenshot = new(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, SKAlphaType.Premul);
        screenshot.Erase(new SKColor(28, 36, 48, 255));

        AvaloniaRegionCaptureRequest request = new()
        {
            Screenshot = screenshot,
            ScreenshotIsOpaque = true,
            ScreenBounds = new DrawingRectangle(x, y, logicalWidth, logicalHeight),
            EnableAnnotations = false,
            RegionCaptureOptions = new RegionCaptureOptions
            {
                QuickCapture = true,
                DetectWindows = false,
                ShowInfo = false,
                ShowMagnifier = false,
                ActiveMonitorMode = true
            },
            ImageEditorOptions = new ImageEditorOptions()
        };

        RegionCaptureWindow window = new(request, allowPlatformOverlay: true, customOverlay: null);
        Assert.True(WaylandProperties.IsWaylandBackend(window));
        Assert.True(WaylandProperties.IsLayerShellSupported(window));
        Assert.Equal(WaylandLayer.Overlay, WaylandProperties.GetLayer(window));

        Task<AvaloniaRegionCaptureResult?> captureTask = window.CaptureAsync();
        await PumpUiAsync(DispatcherPriority.Render);
        await Task.Delay(200);

        Assert.False(captureTask.IsFaulted, captureTask.Exception?.ToString());
        Assert.True(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            "Expected native Avalonia.Wayland layer-shell 'sharex-region-capture' on Hyprland overlay layer 3.");

        window.Close();
        AvaloniaRegionCaptureResult? result = await captureTask.WaitAsync(Timeout);
        Assert.Null(result);

        await Task.Delay(120);
        Assert.False(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            "Native layer-shell 'sharex-region-capture' remained mapped after RegionCaptureWindow closed.");

        RegionCaptureTransparentWindow transparentWindow = new(
            new DrawingRectangle(x, y, logicalWidth, logicalHeight),
            lockCursorToWindow: false,
            customOverlay: null,
            allowPlatformOverlay: true);
        Assert.True(WaylandProperties.IsWaylandBackend(transparentWindow));
        Assert.True(WaylandProperties.IsLayerShellSupported(transparentWindow));
        Assert.Equal(WaylandLayer.Overlay, WaylandProperties.GetLayer(transparentWindow));

        Task<DrawingRectangle?> selectTask = transparentWindow.SelectAsync();
        await PumpUiAsync(DispatcherPriority.Render);
        await Task.Delay(200);

        Assert.False(selectTask.IsFaulted, selectTask.Exception?.ToString());
        Assert.True(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            "Expected native Avalonia.Wayland layer-shell 'sharex-region-capture' on Hyprland overlay layer 3 for RegionCaptureTransparentWindow.");

        transparentWindow.Close();
        DrawingRectangle? rect = await selectTask.WaitAsync(Timeout);
        Assert.Null(rect);

        await Task.Delay(120);
        Assert.False(IsLayerMapped("sharex-region-capture", expectedLevel: 3),
            "Native layer-shell 'sharex-region-capture' remained mapped after RegionCaptureTransparentWindow closed.");
    }

    private static IPlatformOverlayWindow? TryCreateRealWaylandOverlay(OverlayWindowOptions options)
    {
        string baseDir = AppContext.BaseDirectory;
        string? linuxAsmPath = new[]
        {
            Path.Combine(baseDir, "ShareX.Platform.Linux.dll"),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "ShareX.Platform.Linux", "bin", "Release", "linux-x64", "ShareX.Platform.Linux.dll")),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "ShareX.Platform.Linux", "bin", "Debug", "linux-x64", "ShareX.Platform.Linux.dll")),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "ShareX.Platform.Linux", "bin", "Release", "linux-x64", "ShareX.Platform.Linux.dll")),
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "ShareX.Platform.Linux", "bin", "Release", "net11.0", "ShareX.Platform.Linux.dll"))
        }.FirstOrDefault(File.Exists);

        if (linuxAsmPath == null)
        {
            return null;
        }

        Assembly linuxAsm = AssemblyLoadContext.Default.LoadFromAssemblyPath(linuxAsmPath);
        Type? type = linuxAsm.GetType("ShareX.Platform.Linux.Wayland.WaylandLayerOverlayWindow");
        MethodInfo? tryCreate = type?.GetMethod("TryCreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        return (IPlatformOverlayWindow?)tryCreate?.Invoke(null, [options]);
    }

    private static bool IsLayerMapped(string ns, int expectedLevel)
    {
        using Process proc = Process.Start(new ProcessStartInfo("hyprctl", "layers -j")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true
        }) ?? throw new InvalidOperationException("Failed to start hyprctl.");
        string json = proc.StandardOutput.ReadToEnd();
        proc.WaitForExit();
        if (proc.ExitCode != 0)
        {
            return false;
        }

        using JsonDocument doc = JsonDocument.Parse(json);
        foreach (JsonProperty monitor in doc.RootElement.EnumerateObject())
        {
            if (!monitor.Value.TryGetProperty("levels", out JsonElement levels))
            {
                continue;
            }

            if (!levels.TryGetProperty(expectedLevel.ToString(), out JsonElement levelArr) ||
                levelArr.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (JsonElement surface in levelArr.EnumerateArray())
            {
                if (surface.TryGetProperty("namespace", out JsonElement nsProp) &&
                    nsProp.GetString() == ns)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasUiSession()
    {
        if (OperatingSystem.IsLinux())
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")) ||
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY"));
        }

        return true;
    }

    private static async Task PumpUiAsync(DispatcherPriority priority)
    {
        await Dispatcher.UIThread.InvokeAsync(() => { }, priority);
    }

    private static bool HasNonZeroRgbPixel(byte[] bgraPixels)
    {
        for (int i = 0; i + 3 < bgraPixels.Length; i += 4)
        {
            if (bgraPixels[i] > 10 || bgraPixels[i + 1] > 10 || bgraPixels[i + 2] > 10)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasOpaquePixel(byte[] bgraPixels)
    {
        for (int i = 3; i < bgraPixels.Length; i += 4)
        {
            if (bgraPixels[i] > 0)
            {
                return true;
            }
        }

        return false;
    }

    private sealed class FakePlatformOverlayWindow : IPlatformOverlayWindow
    {
        public FakePlatformOverlayWindow(
            PlatformRectangle logicalBounds,
            double renderScaling,
            bool transparent,
            IReadOnlyList<OverlayOutputInfo> outputs)
        {
            LogicalBounds = logicalBounds;
            RenderScaling = renderScaling;
            IsTransparent = transparent;
            Outputs = outputs;
        }

        public PlatformRectangle LogicalBounds { get; }
        public IReadOnlyList<OverlayOutputInfo> Outputs { get; }
        public double RenderScaling { get; }
        public bool IsTransparent { get; }

        public bool Shown { get; private set; }
        public bool Disposed { get; private set; }
        public int PresentCount { get; private set; }
        public byte[]? LastFrame { get; private set; }
        public OverlayCursor LastCursor { get; private set; } = OverlayCursor.Default;

        public event Action<OverlayPointerEvent>? PointerEvent;
        public event Action<OverlayWheelEvent>? WheelEvent;
        public event Action<OverlayKeyEvent>? KeyEvent;
        public event Action<int, int>? KeyRepeatInfoChanged;
        public event Action? Activated;
        public event Action? Deactivated;
        public event Action? ClosedByCompositor;

        public void Show()
        {
            Shown = true;
            Activated?.Invoke();
        }

        public void Hide()
        {
            Shown = false;
            Deactivated?.Invoke();
        }

        public void PresentFrame(IntPtr bgraPremulPixels, int width, int height, int stride)
        {
            PresentCount++;
            int byteLength = checked(height * stride);
            if (LastFrame == null || LastFrame.Length != byteLength)
            {
                LastFrame = new byte[byteLength];
            }

            Marshal.Copy(bgraPremulPixels, LastFrame, 0, byteLength);
        }

        public void SetCursor(OverlayCursor cursor)
        {
            LastCursor = cursor;
        }

        public void EmitPointer(OverlayPointerEvent ev) => PointerEvent?.Invoke(ev);

        public void EmitKey(OverlayKeyEvent ev) => KeyEvent?.Invoke(ev);

        public void EmitWheel(OverlayWheelEvent ev) => WheelEvent?.Invoke(ev);

        public void EmitClosedByCompositor() => ClosedByCompositor?.Invoke();

        public void EmitKeyRepeatInfo(int rate, int delay) => KeyRepeatInfoChanged?.Invoke(rate, delay);

        public void Dispose()
        {
            Disposed = true;
            Shown = false;
        }
    }
}
