/*
 * Greenshot - a free and open source screenshot tool
 * Copyright (C) 2007-2015 Thomas Braun, Jens Klingen, Robin Krom
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
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GreenshotPlugin.Core
{
    public static class StringExtensions
    {
        private const string RGBIV = "dlgjowejgogkklwj";
        private const string KEY = "lsjvkwhvwujkagfauguwcsjgu2wueuff";

        /// <summary>
        /// Format a string with the specified object
        /// </summary>
        /// <param name="format">String with formatting, like {name}</param>
        /// <param name="source">Object used for the formatting</param>
        /// <returns>Formatted string</returns>
        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        /// <summary>
        /// Format the string "format" with the source
        /// </summary>
        /// <param name="format"></param>
        /// <param name="provider"></param>
        /// <param name="source">object with properties, if a property has the type IDictionary string,string it can used these parameters too</param>
        /// <returns>Formatted string</returns>
        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }

            IDictionary<string, object> properties = new Dictionary<string, object>();
            foreach (var propertyInfo in source.GetType().GetProperties())
            {
                if (propertyInfo.CanRead && propertyInfo.CanWrite)
                {
                    object value = propertyInfo.GetValue(source, null);
                    if (propertyInfo.PropertyType != typeof(IDictionary<string, string>))
                    {
                        properties.Add(propertyInfo.Name, value);
                    }
                    else
                    {
                        IDictionary<string, string> dictionary = (IDictionary<string, string>)value;
                        foreach (var propertyKey in dictionary.Keys)
                        {
                            properties.Add(propertyKey, dictionary[propertyKey]);
                        }
                    }
                }
            }

            Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            List<object> values = new List<object>();
            string rewrittenFormat = r.Replace(format, delegate (Match m)
            {
                Group startGroup = m.Groups["start"];
                Group propertyGroup = m.Groups["property"];
                Group formatGroup = m.Groups["format"];
                Group endGroup = m.Groups["end"];

                object value;
                if (properties.TryGetValue(propertyGroup.Value, out value))
                {
                    values.Add(value);
                }
                else
                {
                    values.Add(source);
                }
                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }

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
                SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] rgbIV = Encoding.ASCII.GetBytes(RGBIV);
                    byte[] key = Encoding.ASCII.GetBytes(KEY);
                    using (CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write))
                    {
                        cs.Write(clearTextBytes, 0, clearTextBytes.Length);
                        cs.FlushFinalBlock();

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
                    SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();

                    byte[] rgbIV = Encoding.ASCII.GetBytes(RGBIV);
                    byte[] key = Encoding.ASCII.GetBytes(KEY);

                    using (CryptoStream cs = new CryptoStream(ms, rijn.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
                        cs.FlushFinalBlock();
                        returnValue = Encoding.ASCII.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                LOG.ErrorFormat("Error decrypting {0}, error: ", EncryptedText, ex.Message);
            }

            return returnValue;
        }

        /// <summary>
        /// Read "streamextensions" :)
        /// </summary>
        /// <param name="input">Stream</param>
        /// <param name="output">Stream</param>
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}