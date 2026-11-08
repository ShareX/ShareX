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

using ShareX.HelpersLib;
using ShareX.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ShareX.Tools.Tests;

/// <summary>FileHelpers.OpenFolder hands the folder to the platform shell, with Explorer's trailing separator only on Windows.</summary>
// Shares the global PlatformServices with the inspector tests, so it runs in their non-parallel collection.
[Collection("Inspector native window")]
public sealed class OpenFolderTests
{
    [Fact]
    public void OpenFolderPassesAnExistingPathToTheShell()
    {
        string folder = Directory.CreateTempSubdirectory("sharex-open-folder-").FullName;
        RecordingShell shell = new(result: true);
        PlatformServices.Initialize(new ShellOnlyPlatform(shell));

        try
        {
            Assert.True(FileHelpers.OpenFolder(folder, allowMessageBox: false));
            string opened = Assert.Single(shell.Opened);

            if (OperatingSystem.IsWindows())
            {
                Assert.Equal(folder + @"\", opened);
            }
            else
            {
                // A backslash is part of the name here: xdg-open and open were handed a folder that does not exist.
                Assert.Equal(folder, opened);
                Assert.True(Directory.Exists(opened));
            }
        }
        finally
        {
            PlatformServices.Shutdown();
            Directory.Delete(folder);
        }
    }

    [Fact]
    public void OpenFolderReportsWhenNothingOpenedIt()
    {
        string folder = Directory.CreateTempSubdirectory("sharex-open-folder-").FullName;
        PlatformServices.Initialize(new ShellOnlyPlatform(new RecordingShell(result: false)));

        try
        {
            Assert.False(FileHelpers.OpenFolder(folder, allowMessageBox: false));
        }
        finally
        {
            PlatformServices.Shutdown();
            Directory.Delete(folder);
        }
    }

    private sealed class RecordingShell(bool result) : IShellService
    {
        public List<string> Opened { get; } = [];
        public bool OpenUrl(string url) => throw new NotImplementedException();
        public bool OpenPath(string path) { Opened.Add(path); return result; }
        public bool RevealInFileManager(string path) => throw new NotImplementedException();
        public string? GetMimeType(string extension) => null;
        public FeatureSupport DesktopIconsSupport => FeatureSupport.NotSupported("test");
        public bool? AreDesktopIconsVisible() => null;
        public bool SetDesktopIconsVisible(bool visible) => false;
        public string? FindProgram(string executableName) => null;
    }

    private sealed class ShellOnlyPlatform(IShellService shell) : IPlatformServices
    {
        public IShellService Shell => shell;
        public IWindowManagementService WindowManagement => throw new NotImplementedException();
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
}
