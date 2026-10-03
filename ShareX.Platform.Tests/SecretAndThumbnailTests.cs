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
        // Example from the specification, rooted on the host's drive when running on Windows.
        string root = Path.GetPathRoot(Path.GetTempPath())!;
        string path = Path.Combine(root, "home", "jens", "photo with spaces.png");
        string uri = "file:///" + root.Replace('\\', '/').TrimStart('/') + "home/jens/photo%20with%20spaces.png";
        Assert.Equal(Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(uri))).ToLowerInvariant() + ".png",
            FreedesktopThumbnailService.GetThumbnailName(path));
    }

    /// <summary>A PNG signature, a tEXt chunk and IEND: enough for the chunk reader, like a real thumbnailer's output.</summary>
    private static byte[] ThumbnailPng(params (string Key, string Value)[] texts)
    {
        using MemoryStream stream = new MemoryStream();
        stream.Write(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

        void Chunk(string type, byte[] data)
        {
            stream.Write(new[] { (byte)(data.Length >> 24), (byte)(data.Length >> 16), (byte)(data.Length >> 8), (byte)data.Length });
            stream.Write(Encoding.ASCII.GetBytes(type));
            stream.Write(data);
            stream.Write(new byte[4]); // CRC, not checked by the reader
        }

        foreach ((string key, string value) in texts)
        {
            Chunk("tEXt", Encoding.Latin1.GetBytes(key + "\0" + value));
        }

        Chunk("IEND", Array.Empty<byte>());
        return stream.ToArray();
    }

    [Fact]
    public void ReadTextChunk_FindsTheKeywordAndStopsAtImageData()
    {
        byte[] png = ThumbnailPng(("Thumb::URI", "file:///a.png"), ("Thumb::MTime", "1700000000"));

        Assert.Equal("1700000000", FreedesktopThumbnailService.ReadTextChunk(png, "Thumb::MTime"));
        Assert.Equal("file:///a.png", FreedesktopThumbnailService.ReadTextChunk(png, "Thumb::URI"));
        Assert.Null(FreedesktopThumbnailService.ReadTextChunk(png, "Thumb::Size"));
        Assert.Null(FreedesktopThumbnailService.ReadTextChunk(new byte[] { 1, 2, 3 }, "Thumb::MTime"));
    }

    [UnixFact]
    public void GetThumbnail_ReturnsOnlyThumbnailsThatMatchTheFilesModificationTime()
    {
        string cache = Path.Combine(Path.GetTempPath(), "sharex-thumbs-" + Guid.NewGuid().ToString("N"));

        try
        {
            string image = Path.Combine(cache, "pictures", "a.png");
            Directory.CreateDirectory(Path.GetDirectoryName(image)!);
            File.WriteAllBytes(image, new byte[] { 9 });
            File.SetLastWriteTimeUtc(image, new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc));
            long mtime = new DateTimeOffset(File.GetLastWriteTimeUtc(image)).ToUnixTimeSeconds();

            string thumbnail = Path.Combine(cache, "thumbnails", "normal", FreedesktopThumbnailService.GetThumbnailName(image));
            Directory.CreateDirectory(Path.GetDirectoryName(thumbnail)!);
            byte[] current = ThumbnailPng(("Thumb::MTime", mtime.ToString(System.Globalization.CultureInfo.InvariantCulture)));
            File.WriteAllBytes(thumbnail, current);
            FreedesktopThumbnailService service = new FreedesktopThumbnailService(Path.Combine(cache, "thumbnails"));

            Assert.Equal(current, service.GetThumbnail(image, 100, 100));

            // The user edited the picture after the file manager made the thumbnail.
            File.SetLastWriteTimeUtc(image, new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc));
            Assert.Null(service.GetThumbnail(image, 100, 100));

            // A thumbnail without Thumb::MTime is invalid under the specification.
            File.WriteAllBytes(thumbnail, ThumbnailPng());
            Assert.Null(service.GetThumbnail(image, 100, 100));

            Assert.Null(service.GetThumbnail(Path.Combine(cache, "pictures", "missing.png"), 100, 100));
        }
        finally
        {
            Directory.Delete(cache, true);
        }
    }
}
