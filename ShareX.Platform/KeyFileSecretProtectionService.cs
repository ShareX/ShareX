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

using System;
using System.IO;
using System.Security.Cryptography;

namespace ShareX.Platform;

/// <summary>
/// AES-GCM with a random per-user key stored in a file only the current user can read, the same protection model as an SSH private key.
/// Used on macOS and Linux, where there is no DPAPI equivalent that works without a running keyring.
/// </summary>
public sealed class KeyFileSecretProtectionService : ISecretProtectionService
{
    private const int KeySize = 32;
    private const int NonceSize = 12;
    private const int TagSize = 16;

    private readonly string keyPath;
    private readonly object sync = new object();
    private byte[]? key;

    public KeyFileSecretProtectionService(string keyPath)
    {
        this.keyPath = keyPath ?? throw new ArgumentNullException(nameof(keyPath));
    }

    public byte[] Protect(byte[] data, byte[]? entropy = null)
    {
        ArgumentNullException.ThrowIfNull(data);

        byte[] nonce = RandomNumberGenerator.GetBytes(NonceSize);
        byte[] cipher = new byte[data.Length];
        byte[] tag = new byte[TagSize];

        using (AesGcm aes = new AesGcm(GetKey(), TagSize))
        {
            aes.Encrypt(nonce, data, cipher, tag, entropy);
        }

        byte[] result = new byte[NonceSize + TagSize + cipher.Length];
        nonce.CopyTo(result, 0);
        tag.CopyTo(result, NonceSize);
        cipher.CopyTo(result, NonceSize + TagSize);
        return result;
    }

    public byte[] Unprotect(byte[] protectedData, byte[]? entropy = null)
    {
        ArgumentNullException.ThrowIfNull(protectedData);

        if (protectedData.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("The protected value is too short.");
        }

        byte[] nonce = protectedData[..NonceSize];
        byte[] tag = protectedData[NonceSize..(NonceSize + TagSize)];
        byte[] cipher = protectedData[(NonceSize + TagSize)..];
        byte[] plain = new byte[cipher.Length];

        using (AesGcm aes = new AesGcm(GetKey(), TagSize))
        {
            aes.Decrypt(nonce, cipher, tag, plain, entropy);
        }

        return plain;
    }

    private byte[] GetKey()
    {
        lock (sync)
        {
            if (key != null)
            {
                return key;
            }

            if (File.Exists(keyPath))
            {
                byte[] existing = File.ReadAllBytes(keyPath);

                if (existing.Length == KeySize)
                {
                    return key = existing;
                }
            }

            string? folder = Path.GetDirectoryName(keyPath);

            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }

            byte[] created = RandomNumberGenerator.GetBytes(KeySize);

            // Create with owner-only permissions from the start so the key is never readable by others, even briefly.
            FileStreamOptions options = new FileStreamOptions { Mode = FileMode.Create, Access = FileAccess.Write, Share = FileShare.None };

            if (!OperatingSystem.IsWindows())
            {
                options.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            }

            using (FileStream stream = new FileStream(keyPath, options))
            {
                stream.Write(created);
            }

            return key = created;
        }
    }
}
