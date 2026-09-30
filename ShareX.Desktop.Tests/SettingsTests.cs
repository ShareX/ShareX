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

using ShareX.Desktop.Commands;
using ShareX.Desktop.Settings;
using System;
using System.IO;
using Xunit;

namespace ShareX.Desktop.Tests;

public sealed class SettingsTests : IDisposable
{
    private readonly string folder = Path.Combine(Path.GetTempPath(), "sharex-settings-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void Defaults_NeverUploadAndNeverNameAThirdParty()
    {
        DesktopSettings settings = new DesktopSettings();

        Assert.False(settings.UploadImage);
        Assert.Equal("", settings.ImageUploader);
        Assert.Equal("", settings.FileUploader);
        Assert.True(settings.SaveToFile);
        Assert.True(settings.CopyImageToClipboard);
    }

    [Fact]
    public void SaveThenLoad_RoundTrips()
    {
        string path = Path.Combine(folder, "nested", DesktopSettings.FileName);
        new DesktopSettings { ImageUploader = "CustomImageUploader", UploadImage = true, ScreenshotsFolder = "/x" }.Save(path);

        DesktopSettings loaded = DesktopSettings.Load(path);

        Assert.Equal("CustomImageUploader", loaded.ImageUploader);
        Assert.True(loaded.UploadImage);
        Assert.Equal("/x", loaded.ScreenshotsFolder);
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void Load_MissingOrDamagedFile_FallsBackToDefaults()
    {
        Directory.CreateDirectory(folder);
        string path = Path.Combine(folder, DesktopSettings.FileName);
        File.WriteAllText(path, "{ this is not json");

        Assert.True(DesktopSettings.Load(path).SaveToFile);
        Assert.True(DesktopSettings.Load(Path.Combine(folder, "missing.json")).SaveToFile);
    }

    [Fact]
    public void ToAfterCapture_AppliesOnlyTheOverridesThatAreSet()
    {
        DesktopSettings settings = new DesktopSettings { SaveToFile = true, CopyImageToClipboard = true, UploadImage = false, ShowNotifications = true };

        AfterCapture actions = settings.ToAfterCapture(new AfterCaptureOverrides { Upload = true, Save = false });

        Assert.False(actions.Save);
        Assert.True(actions.CopyImage);
        Assert.True(actions.Upload);
        Assert.False(actions.Edit);
        Assert.True(actions.Notify);
    }
}
