#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2025 ShareX Team

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
            byte[] encryptedData = ProtectedData.Protect(data, optionalEntropy, scope);
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
            byte[] decryptedData = ProtectedData.Unprotect(encryptedData, optionalEntropy, scope);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}