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
using ShareX.Platform.Windows;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using Xunit;

namespace ShareX.ImageEditor.Tests;

[Collection("Windows signature verification")]
public sealed class WindowsCodeSignatureTests
{
    [WindowsCodeSignatureFact]
    public void MissingMalformedAndUnsignedFilesAreNeverTrusted()
    {
        if (!OperatingSystem.IsWindows()) return;
        using SignatureFixture fixture = new();
        WindowsCodeSignatureService service = new();
        Assert.True(service.Support.IsSupported);
        foreach (string? path in new string?[] { null, "", "\0", fixture.Directory, fixture.PathFor("missing.exe") })
        {
            Assert.False(service.IsTrusted(path!));
        }

        string malformed = fixture.Write("malformed.exe", [0x4d, 0x5a, 0]);
        string unsigned = fixture.Write("unsigned.dll", File.ReadAllBytes(typeof(WindowsCodeSignatureTests).Assembly.Location));
        for (int repeat = 0; repeat < 12; repeat++)
        {
            Assert.False(service.IsTrusted(malformed));
            Assert.False(service.IsTrusted(unsigned));
            AssertExclusiveAccess(malformed);
            AssertExclusiveAccess(unsigned);
        }
    }

    [WindowsCodeSignatureFact(true)]
    public void TrustedSignedCopyAndTamperedCopyAreDistinguishedWithoutRetainingFiles()
    {
        if (!OperatingSystem.IsWindows()) return;
        string source = Path.GetFullPath(Environment.GetEnvironmentVariable("SHAREX_TEST_SIGNED_WINDOWS_BINARY")!);
        byte[] original = File.ReadAllBytes(source);
        Assert.True(original.Length > 0x40);
        using SignatureFixture fixture = new();
        string signed = fixture.Write("signed fixture 背景.exe", original);
        byte[] tampered = (byte[])original.Clone();
        // Change a reserved DOS-header byte covered by Authenticode, keeping the PE header/signature table intact.
        tampered[0x20] ^= 1;
        string changed = fixture.Write("tampered fixture.exe", tampered);
        using (FileStream stream = File.OpenRead(changed))
        using (PEReader reader = new(stream))
        {
            Assert.NotNull(reader.PEHeaders.PEHeader);
        }

        WindowsCodeSignatureService service = new();
        for (int repeat = 0; repeat < 12; repeat++)
        {
            Assert.True(service.IsTrusted(signed), "The opt-in signed fixture must be trusted by the updater's Windows policy.");
            Assert.False(service.IsTrusted(changed));
            AssertExclusiveAccess(signed);
            AssertExclusiveAccess(changed);
        }
        Assert.Equal(original, File.ReadAllBytes(signed));
        Assert.Equal(tampered, File.ReadAllBytes(changed));
        Assert.Equal(SHA256.HashData(original), SHA256.HashData(File.ReadAllBytes(source)));
    }

    private static void AssertExclusiveAccess(string path)
    {
        using FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(stream.CanRead && stream.CanWrite);
    }

    private sealed class SignatureFixture : IDisposable
    {
        private const string Prefix = "sharex-signature-verification-";
        public string Directory { get; } = Path.Combine(Path.GetTempPath(), Prefix + Guid.NewGuid().ToString("N"));

        public SignatureFixture() => System.IO.Directory.CreateDirectory(Directory);
        public string PathFor(string name) => Path.Combine(Directory, name);
        public string Write(string name, byte[] bytes)
        {
            string path = PathFor(name);
            File.WriteAllBytes(path, bytes);
            return path;
        }

        public void Dispose()
        {
            string target = Path.GetFullPath(Directory);
            string temp = Path.TrimEndingDirectorySeparator(Path.GetFullPath(Path.GetTempPath()));
            if (!string.Equals(Path.GetDirectoryName(target), temp, StringComparison.OrdinalIgnoreCase) ||
                !Path.GetFileName(target).StartsWith(Prefix, StringComparison.Ordinal))
                throw new InvalidOperationException("Signature fixture cleanup must remain inside its own temporary directory.");
            if (System.IO.Directory.Exists(target)) System.IO.Directory.Delete(target, true);
        }
    }
}

public sealed class WindowsCodeSignatureFactAttribute : FactAttribute
{
    public WindowsCodeSignatureFactAttribute(bool signedFixture = false)
    {
        if (!OperatingSystem.IsWindows()) Skip = "Requires Windows Authenticode verification.";
        else if (signedFixture && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("SHAREX_TEST_SIGNED_WINDOWS_BINARY")))
            Skip = "Set SHAREX_TEST_SIGNED_WINDOWS_BINARY to an already trusted signed Windows binary, such as the installed Microsoft .NET host.";
    }
}

[CollectionDefinition("Windows signature verification", DisableParallelization = true)]
public sealed class WindowsSignatureVerificationCollection { }