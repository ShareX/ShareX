/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2013  Thomas Braun, Jens Klingen, Robin Krom
 *
 * For more information see: http://getgreenshot.org/
 * The Greenshot project is hosted on Sourceforge: http://sourceforge.net/projects/greenshot/
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GreenshotPlugin.Core
{
    public static class EncryptionHelper
    {
        private const string RGBIV = "dlgjowejgogkklwj";
        private const string KEY = "lsjvkwhvwujkagfauguwcsjgu2wueuff";

        /// <summary>
        /// A simply rijndael aes encryption, can be used to store passwords
        /// </summary>
        /// <param name="ClearText">the string to call upon</param>
        /// <returns>an encryped string in base64 form</returns>
        public static string Encrypt(this string ClearText)
        {
            string returnValue = ClearText;
            try
            {
                byte[] clearTextBytes = Encoding.ASCII.GetBytes(ClearText);
                using (SymmetricAlgorithm rijn = SymmetricAlgorithm.Create())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] rgbIV = Encoding.ASCII.GetBytes(RGBIV);
                        byte[] key = Encoding.ASCII.GetBytes(KEY);
                        using (CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write))
                        {
                            cs.Write(clearTextBytes, 0, clearTextBytes.Length);

                            cs.Close();
                        }
                        returnValue = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error encrypting, error: ", ex.Message);
            }
            return returnValue;
        }

        /// <summary>
        /// A simply rijndael aes decryption, can be used to store passwords
        /// </summary>
        /// <param name="EncryptedText">a base64 encoded rijndael encrypted string</param>
        /// <returns>Decrypeted text</returns>
        public static string Decrypt(this string EncryptedText)
        {
            string returnValue = EncryptedText;
            try
            {
                byte[] encryptedTextBytes = Convert.FromBase64String(EncryptedText);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (SymmetricAlgorithm rijn = SymmetricAlgorithm.Create())
                    {
                        byte[] rgbIV = Encoding.ASCII.GetBytes(RGBIV);
                        byte[] key = Encoding.ASCII.GetBytes(KEY);

                        using (CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);

                            cs.Close();
                        }
                    }
                    return Encoding.ASCII.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error decrypting {0}, error: ", EncryptedText, ex.Message);
            }

            return EncryptedText;
        }
    }
}