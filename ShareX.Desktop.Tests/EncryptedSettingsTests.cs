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

using ShareX.UploadersLib;
using System;
using System.IO;
using Xunit;

namespace ShareX.Desktop.Tests;

[Collection(PlatformServicesCollection.Name)]
public sealed class EncryptedSettingsTests : IDisposable
{
    private readonly PlatformServicesFixture fixture;
    private readonly string folder = Path.Combine(Path.GetTempPath(), "sharex-encrypted-" + Guid.NewGuid().ToString("N"));

    public EncryptedSettingsTests(PlatformServicesFixture fixture)
    {
        this.fixture = fixture;
        fixture.Platform.SecretsFake.Fail = false;
        Directory.CreateDirectory(folder);
    }

    public void Dispose()
    {
        fixture.Platform.SecretsFake.Fail = false;

        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    private static UploadersConfig WithSecret()
    {
        UploadersConfig config = new UploadersConfig { SupportDPAPIEncryption = true };
        CustomUploaderItem item = CustomUploaderItem.Init();
        item.RequestURL = "https://example.test/upload";
        config.CustomUploadersList.Add(item);
        config.VgymeUserKey = "the-user-key";
        return config;
    }

    [Fact]
    public void Save_EncryptsSecrets()
    {
        string path = Path.Combine(folder, "UploadersConfig.json");

        Assert.True(WithSecret().Save(path));

        string json = File.ReadAllText(path);
        Assert.DoesNotContain("the-user-key", json);
        Assert.Contains("$DPAPIEncrypted$", json);
        Assert.Equal("the-user-key", UploadersConfig.Load(path).VgymeUserKey);
    }

    [Fact]
    public void Save_WhenEncryptionFails_KeepsThePreviousFileAndNeverWritesPlainText()
    {
        string path = Path.Combine(folder, "UploadersConfig.json");
        Assert.True(WithSecret().Save(path));
        string before = File.ReadAllText(path);

        fixture.Platform.SecretsFake.Fail = true;
        bool saved = WithSecret().Save(path);
        fixture.Platform.SecretsFake.Fail = false;

        Assert.False(saved);
        Assert.Equal(before, File.ReadAllText(path));
        Assert.DoesNotContain("the-user-key", File.ReadAllText(path));
    }

    [Fact]
    public void Load_ValuesEncryptedElsewhere_BecomeEmptyInsteadOfGarbage()
    {
        string path = Path.Combine(folder, "UploadersConfig.json");
        Assert.True(WithSecret().Save(path));

        fixture.Platform.SecretsFake.Fail = true;
        UploadersConfig loaded = UploadersConfig.Load(path);
        fixture.Platform.SecretsFake.Fail = false;

        Assert.Null(loaded.VgymeUserKey);
    }
}
