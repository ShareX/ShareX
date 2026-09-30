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

using ShareX.Platform.Linux;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace ShareX.Platform.Tests;

public sealed class KeyFileSecretProtectionTests : IDisposable
{
    private readonly string folder = Path.Combine(Path.GetTempPath(), "sharex-secret-" + Guid.NewGuid().ToString("N"));

    private string KeyPath => Path.Combine(folder, "secret.key");

    public void Dispose()
    {
        if (Directory.Exists(folder))
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void Protect_RoundTripsAndDoesNotStorePlainText()
    {
        KeyFileSecretProtectionService service = new KeyFileSecretProtectionService(KeyPath);
        byte[] secret = Encoding.UTF8.GetBytes("imgur-token-123");

        byte[] protectedData = service.Protect(secret);

        Assert.DoesNotContain("imgur-token", Encoding.UTF8.GetString(protectedData));
        Assert.Equal(secret, service.Unprotect(protectedData));
    }

    [Fact]
    public void Protect_UsesANewNonceEachTime()
    {
        KeyFileSecretProtectionService service = new KeyFileSecretProtectionService(KeyPath);
        byte[] secret = Encoding.UTF8.GetBytes("same");

        Assert.NotEqual(service.Protect(secret), service.Protect(secret));
    }

    [Fact]
    public void Unprotect_ReadsDataProtectedByAnotherInstanceWithTheSameKeyFile()
    {
        byte[] protectedData = new KeyFileSecretProtectionService(KeyPath).Protect(Encoding.UTF8.GetBytes("x"), Encoding.UTF8.GetBytes("entropy"));

        Assert.Equal("x", Encoding.UTF8.GetString(new KeyFileSecretProtectionService(KeyPath).Unprotect(protectedData, Encoding.UTF8.GetBytes("entropy"))));
    }

    [Fact]
    public void Unprotect_RejectsWrongEntropyTamperingAndOtherUsersKeys()
    {
        KeyFileSecretProtectionService service = new KeyFileSecretProtectionService(KeyPath);
        byte[] protectedData = service.Protect(Encoding.UTF8.GetBytes("secret"), Encoding.UTF8.GetBytes("a"));

        Assert.ThrowsAny<CryptographicException>(() => service.Unprotect(protectedData, Encoding.UTF8.GetBytes("b")));

        byte[] tampered = (byte[])protectedData.Clone();
        tampered[^1] ^= 1;
        Assert.ThrowsAny<CryptographicException>(() => service.Unprotect(tampered, Encoding.UTF8.GetBytes("a")));

        KeyFileSecretProtectionService other = new KeyFileSecretProtectionService(Path.Combine(folder, "other.key"));
        Assert.ThrowsAny<CryptographicException>(() => other.Unprotect(protectedData, Encoding.UTF8.GetBytes("a")));
        Assert.Throws<CryptographicException>(() => service.Unprotect(new byte[4]));
    }

    [UnixFact]
    public void KeyFile_IsReadableOnlyByTheOwner()
    {
        new KeyFileSecretProtectionService(KeyPath).Protect(new byte[] { 1 });

        Assert.Equal(UnixFileMode.UserRead | UnixFileMode.UserWrite, File.GetUnixFileMode(KeyPath));
    }
}

public class FreedesktopThumbnailTests
{
    [Fact]
    public void GetThumbnailName_IsTheMd5OfTheFileUri()
    {
        // Example from the specification: file:///home/jens/photo%20with%20spaces.png
        Assert.Equal(Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes("file:///home/jens/photo%20with%20spaces.png"))).ToLowerInvariant() + ".png",
            FreedesktopThumbnailService.GetThumbnailName("/home/jens/photo with spaces.png"));
    }

    [UnixFact]
    public void GetThumbnail_ReturnsTheCachedPngAndNullWhenMissing()
    {
        string cache = Path.Combine(Path.GetTempPath(), "sharex-thumbs-" + Guid.NewGuid().ToString("N"));

        try
        {
            string image = Path.Combine(cache, "pictures", "a.png");
            string name = FreedesktopThumbnailService.GetThumbnailName(image);
            Directory.CreateDirectory(Path.Combine(cache, "thumbnails", "normal"));
            File.WriteAllBytes(Path.Combine(cache, "thumbnails", "normal", name), new byte[] { 1, 2, 3 });
            FreedesktopThumbnailService service = new FreedesktopThumbnailService(Path.Combine(cache, "thumbnails"));

            Assert.Equal(new byte[] { 1, 2, 3 }, service.GetThumbnail(image, 100, 100));
            Assert.Null(service.GetThumbnail(Path.Combine(cache, "pictures", "missing.png"), 100, 100));
        }
        finally
        {
            Directory.Delete(cache, true);
        }
    }
}
