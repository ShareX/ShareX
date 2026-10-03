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

using ShareX.Platform;
using ShareX.Platform.Windows;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Xunit;

namespace ShareX.Tools.Tests;

[Collection("Inspector native window")]
public sealed class InspectWindowTests
{
    [WindowsInspectorFact]
    public void InspectorUsesPortableDetailsAndClearsClosedWindows()
    {
        if (!OperatingSystem.IsWindows()) return;
        Exception? failure = null;
        Thread thread = new(() => { try { VerifyInspector(); } catch (Exception exception) { failure = exception; } }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(15)), "Inspector verification did not complete.");
        if (failure != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failure).Throw();
    }

    private static void Equal<T>(T expected, T actual) => Assert.Equal(expected, actual);

    [Fact]
    public void InspectorRespectsOperationSupportEvenWhenMetadataExists()
    {
        WithFakeManagement(service =>
        {
            service.ChildSupport = FeatureSupport.NotSupported("Whole windows only.");
            service.TopMostSupport = FeatureSupport.NotSupported("Topmost is unavailable.");
            service.OpacitySupport = FeatureSupport.NotSupported("Opacity is unavailable.");
            using InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(new IntPtr(1), true);
            Assert.True(viewModel.HasSelection);
            Assert.True(viewModel.CanPickWindow);
            Assert.False(viewModel.CanPickControl);
            Assert.Equal(service.ChildSupport.Reason, viewModel.PickControlSupportReason);
            Assert.False(viewModel.CanChangeTopMost);
            Assert.False(viewModel.CanChangeOpacity);
            Assert.Equal(service.TopMostSupport.Reason, viewModel.TopMostSupportReason);
            Assert.Equal(service.OpacitySupport.Reason, viewModel.OpacitySupportReason);

            viewModel.IsTopMost = true;
            viewModel.Opacity = 50;
            Assert.Equal(0, service.TopMostChanges);
            Assert.Equal(0, service.OpacityChanges);
            int lookups = service.Lookups;
            viewModel.SelectWindow(new IntPtr(2), false);
            Assert.Equal(lookups, service.Lookups);
            Assert.True(viewModel.IsTopLevelWindow);
        });
    }

    [Fact]
    public void InspectorExplainsPerWindowUnavailableSettings()
    {
        WithFakeManagement(service =>
        {
            service.TopMost = null;
            service.Opacity = null;
            using InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(new IntPtr(1), true);
            Assert.True(viewModel.HasSelection);
            Assert.False(viewModel.CanChangeTopMost);
            Assert.False(viewModel.CanChangeOpacity);
            string reason = Localization.Strings.InspectWindowViewModel_Selected_window_setting_unavailable;
            Assert.Equal(reason, viewModel.TopMostSupportReason);
            Assert.Equal(reason, viewModel.OpacitySupportReason);

            viewModel.SelectWindow(new IntPtr(1), false);
            Assert.False(viewModel.CanChangeTopMost);
            Assert.False(viewModel.CanChangeOpacity);
            Assert.Null(viewModel.TopMostSupportReason);
            Assert.Null(viewModel.OpacitySupportReason);
        });
    }

    [Fact]
    public void InspectorClearsSelectionWhenInspectionBecomesUnavailable()
    {
        WithFakeManagement(service =>
        {
            using InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(new IntPtr(1), true);
            Assert.True(viewModel.HasSelection);
            int lookups = service.Lookups;
            service.InspectSupport = FeatureSupport.NotSupported("Inspection is unavailable in this session.");
            viewModel.RefreshCommand.Execute(null);
            Assert.False(viewModel.HasSelection);
            Assert.Empty(viewModel.Details);
            Assert.False(viewModel.CanRefresh);
            Assert.False(viewModel.CanPickWindow);
            Assert.False(viewModel.CanPickControl);
            Assert.Equal(service.InspectSupport.Reason, viewModel.PickWindowSupportReason);
            Assert.Equal(service.InspectSupport.Reason, viewModel.PickControlSupportReason);
            viewModel.SelectWindow(new IntPtr(2), true);
            viewModel.ReloadWindowList();
            Assert.Equal(lookups, service.Lookups);
            Assert.Empty(viewModel.Windows);
        });
    }

    private static void WithFakeManagement(Action<FakeWindowManagementService> action)
    {
        FakeWindowManagementService service = new();
        PlatformServices.Initialize(new InspectorPlatformFixture(service));
        try { action(service); }
        finally { PlatformServices.Shutdown(); }
    }

    private sealed class FakeWindowManagementService : IWindowManagementService
    {
        public FeatureSupport Support => FeatureSupport.Supported;
        public FeatureSupport BorderlessSupport => FeatureSupport.Supported;
        public FeatureSupport InspectSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport ChildSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport TopMostSupport { get; set; } = FeatureSupport.Supported;
        public FeatureSupport OpacitySupport { get; set; } = FeatureSupport.Supported;
        public bool? TopMost { get; set; } = false;
        public byte? Opacity { get; set; } = 255;
        public int Lookups { get; private set; }
        public int TopMostChanges { get; private set; }
        public int OpacityChanges { get; private set; }
        public FeatureSupport GetSupport(WindowManagementFeature feature) => feature switch
        {
            WindowManagementFeature.Inspect => InspectSupport,
            WindowManagementFeature.ChildControls => ChildSupport,
            WindowManagementFeature.TopMost => TopMostSupport,
            WindowManagementFeature.Opacity => OpacitySupport,
            _ => BorderlessSupport
        };
        public WindowDetails GetDetails(long handle)
        {
            Lookups++;
            return new(handle, "Fixture window", "Fixture", "Fixture", null, 1,
                new PlatformRectangle(0, 0, 100, 80), null, [], [], TopMost, Opacity);
        }
        public byte[]? GetIcon(long handle) => null;
        public long GetWindowAt(PlatformPoint point, bool topLevel) => throw new NotImplementedException();
        public bool ToggleBorderless(long handle, bool useWorkingArea) => throw new NotImplementedException();
        public bool SetTopMost(long handle, bool value) { TopMostChanges++; TopMost = value; return true; }
        public bool SetOpacity(long handle, byte value) { OpacityChanges++; Opacity = value; return true; }
    }

    // Any unexpected OS/service access fails this synthetic fixture instead of touching the desktop.
    private sealed class InspectorPlatformFixture(IWindowManagementService management) : IPlatformServices
    {
        public IWindowManagementService WindowManagement => management;
        public PlatformInfo Info => throw new NotImplementedException();
        public IPathService Paths => throw new NotImplementedException();
        public IStartupService Startup => throw new NotImplementedException();
        public IClipboardService Clipboard => throw new NotImplementedException();
        public IScreenCaptureService ScreenCapture => throw new NotImplementedException();
        public IScreenRecordingService ScreenRecording => throw new NotImplementedException();
        public IHotkeyService Hotkeys => throw new NotImplementedException();
        public IWindowService Windows => throw new NotImplementedException();
        public IInputService Input => throw new NotImplementedException();
        public INotificationService Notifications => throw new NotImplementedException();
        public IShellService Shell => throw new NotImplementedException();
        public IShellIntegrationService ShellIntegration => throw new NotImplementedException();
        public ICredentialService Credentials => throw new NotImplementedException();
        public ISecretProtectionService Secrets => throw new NotImplementedException();
        public IThumbnailService Thumbnails => throw new NotImplementedException();
        public ISystemPreferencesService Preferences => throw new NotImplementedException();
        public ISystemInfoService SystemInfo => throw new NotImplementedException();
        public ICodeSignatureService CodeSignature => throw new NotImplementedException();
        public IOcrService Ocr => throw new NotImplementedException();
        public ITaskbarService Taskbar => throw new NotImplementedException();
        public ISystemGraphicsService Graphics => throw new NotImplementedException();
        public IPrintService Printing => throw new NotImplementedException();
        public ISoundService Sounds => throw new NotImplementedException();
        public IDesktopWallpaperService Wallpaper => throw new NotImplementedException();
        public IApplicationSessionService Session => throw new NotImplementedException();
        public ITrayService Tray => throw new NotImplementedException();
        public IApplicationLaunchService ApplicationLaunch => throw new NotImplementedException();
        public void Dispose() { }
    }

    [SupportedOSPlatform("windows")]
    private static void VerifyInspector()
    {
        PlatformServices.Initialize(new WindowsPlatformServices());
        // Off-screen tool window: visible to enumeration without putting a window on the user's desktop.
        IntPtr window = CreateWindowExW(0x08000080, "STATIC", "ShareX inspector verification", 0x10000000,
            -32000, -32000, 100, 80, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (window == IntPtr.Zero) throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());

        try
        {
            Equal(true, PlatformServices.Current.Windows.GetWindows().Any(item => item.Handle == window.ToInt64()));
            PlatformRectangle client = PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.ClientBounds!.Value;
            Equal(0, client.X);
            Equal(0, client.Y);
            Equal(false, client.IsEmpty);
            using ShareX.Tools.InspectWindowViewModel viewModel = new();
            viewModel.SelectWindow(window, true);
            Equal(true, viewModel.HasSelection);
            Equal("ShareX inspector verification", viewModel.SelectedTitle);
            Equal(true, viewModel.CanChangeTopMost);
            Equal(true, viewModel.CanChangeOpacity);
            Equal(10, viewModel.Details.Count);
            viewModel.IsTopMost = true;
            Equal(true, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.IsTopMost!.Value);
            viewModel.Opacity = 50;
            Equal((byte)128, PlatformServices.Current.WindowManagement.GetDetails(window.ToInt64())!.Opacity!.Value);
            viewModel.SelectWindow(window, false);
            Equal(false, viewModel.CanChangeTopMost);
            Equal(false, viewModel.CanChangeOpacity);
            DestroyWindow(window);
            window = IntPtr.Zero;
            viewModel.RefreshCommand.Execute(null);
            Equal(false, viewModel.HasSelection);
            Equal(0, viewModel.Details.Count);
        }
        finally
        {
            if (window != IntPtr.Zero) DestroyWindow(window);
            PlatformServices.Shutdown();
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(uint extendedStyle, string className, string title, uint style,
        int x, int y, int width, int height, IntPtr parent, IntPtr menu, IntPtr instance, IntPtr parameter);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);
}

[CollectionDefinition("Inspector native window", DisableParallelization = true)]
public sealed class InspectorNativeWindowCollection { }

public sealed class WindowsInspectorFactAttribute : FactAttribute
{
    public WindowsInspectorFactAttribute()
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires a native Windows inspector window.";
    }
}
