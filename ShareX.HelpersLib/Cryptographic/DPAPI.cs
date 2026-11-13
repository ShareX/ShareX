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
using System.Text;

namespace ShareX.HelpersLib
{
    // https://en.wikipedia.org/wiki/Data_Protection_API
    public static class DPAPI
    {
        public static string Encrypt(string stringToEncrypt, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] data = Encoding.UTF8.GetBytes(stringToEncrypt);
            return Encrypt(data, optionalEntropy, scope);
        }

        public static string Encrypt(byte[] data, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] entropyData = null;
            if (optionalEntropy != null)
            {
                entropyData = Encoding.UTF8.GetBytes(optionalEntropy);
            }
            return Encrypt(data, entropyData, scope);
        }

        public static string Encrypt(byte[] data, byte[] optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] encryptedData;
            if (OperatingSystem.IsWindows())
            {
                encryptedData = ProtectedData.Protect(data, optionalEntropy, scope);
            }
            else
            {
                encryptedData = SymmetricProtect(data, optionalEntropy);
            }
            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string encryptedString, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedString);
            return Decrypt(encryptedData, optionalEntropy, scope);
        }

        public static string Decrypt(byte[] encryptedData, string optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] entropyData = null;
            if (optionalEntropy != null)
            {
                entropyData = Encoding.UTF8.GetBytes(optionalEntropy);
            }
            return Decrypt(encryptedData, entropyData, scope);
        }

        public static string Decrypt(byte[] encryptedData, byte[] optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            byte[] decryptedData;
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    decryptedData = ProtectedData.Unprotect(encryptedData, optionalEntropy, scope);
                }
                catch (PlatformNotSupportedException)
                {
                    decryptedData = SymmetricUnprotect(encryptedData, optionalEntropy);
                }
                catch
                {
                    decryptedData = SymmetricUnprotect(encryptedData, optionalEntropy);
                }
            }
            else
            {
                decryptedData = SymmetricUnprotect(encryptedData, optionalEntropy);
            }
            return Encoding.UTF8.GetString(decryptedData);
        }

        private static byte[] GetKey(byte[] optionalEntropy)
        {
            using (SHA256 sha = SHA256.Create())
            {
                string seed = Environment.UserName + "@" + Environment.MachineName + "_ShareX_Key";
                byte[] seedBytes = Encoding.UTF8.GetBytes(seed);
                if (optionalEntropy != null && optionalEntropy.Length > 0)
                {
                    byte[] combined = new byte[seedBytes.Length + optionalEntropy.Length];
                    Buffer.BlockCopy(seedBytes, 0, combined, 0, seedBytes.Length);
                    Buffer.BlockCopy(optionalEntropy, 0, combined, seedBytes.Length, optionalEntropy.Length);
                    return sha.ComputeHash(combined);
                }
                return sha.ComputeHash(seedBytes);
            }
        }

        private static byte[] SymmetricProtect(byte[] data, byte[] optionalEntropy)
        {
            if (data == null || data.Length == 0) return data ?? Array.Empty<byte>();

            try
            {
                byte[] key = GetKey(optionalEntropy);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.GenerateIV();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(aes.IV, 0, aes.IV.Length);
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return data;
            }
        }

        private static byte[] SymmetricUnprotect(byte[] encryptedData, byte[] optionalEntropy)
        {
            if (encryptedData == null || encryptedData.Length == 0) return encryptedData ?? Array.Empty<byte>();

            if (encryptedData.Length <= 16)
            {
                return encryptedData;
            }

            try
            {
                byte[] key = GetKey(optionalEntropy);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    byte[] iv = new byte[16];
                    Buffer.BlockCopy(encryptedData, 0, iv, 0, 16);
                    aes.IV = iv;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedData, 16, encryptedData.Length - 16);
                            cs.FlushFinalBlock();
                        }
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return encryptedData;
            }
        }
    }
}
