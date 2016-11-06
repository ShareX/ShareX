#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

// Credits: https://github.com/Upload

using Newtonsoft.Json;
using Security.Cryptography;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Up1FileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Up1;

        public override Icon ServiceIcon => Resources.Up1;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Up1(config.Up1Host, config.Up1Key);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpUp1;
    }

    public sealed class Up1 : FileUploader
    {
        /*
         * Up1 is an encrypted image uploader. The idea is that any URL (for example, https://up1.ca/#hsd2mdSuIkzTUR6saZpn1Q) contains
         * what is called a "seed". In this case, the seed is "hsd2mdSuIkzTUR6saZpn1Q". With this, we use sha512(seed) (output 64 bytes)
         * in order to derive an AES key (32 bytes), a CCM IV (16 bytes), and a server-side identifier (16 bytes). These are used to
         * encrypt and store the data.
         *
         * Within the encrypted blob, There is a double-null-terminated UTF-16 JSON object that contains metadata like the filename and mimetype.
         * This is prepended to the image data.
         */

        private const int MacSize = 64;

        public string SystemUrl { get; set; }
        public string ApiKey { get; set; }

        public Up1(string systemUrl, string apiKey)
        {
            if (string.IsNullOrEmpty(systemUrl))
            {
                SystemUrl = "https://share.riseup.net";
            }
            else
            {
                SystemUrl = systemUrl;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                ApiKey = "59Mnk5nY6eCn4bi9GvfOXhMH54E7Bh6EMJXtyJfs";
            }
            else
            {
                ApiKey = apiKey;
            }
        }

        /* Since we're dealing with URLs, the regular base64 encoding using +, / and = will mess things up
         * Therefore we'll use the URL base64 standard (as defined in the RFC)
         */

        private static string UrlBase64Encode(byte[] input)
        {
            return Convert.ToBase64String(input).Replace("=", "").Replace("+", "-").Replace("/", "_");
        }

        /* SJCL (the Javascript library powering the crypto in the browser) depends on the length of the input
         * in order to calculate CCM's IV length. This is the algorithm it uses.
         */

        private static long FindIVLen(long bufferLength)
        {
            if (bufferLength < 0xFFFF) return 15 - 2;
            if (bufferLength < 0xFFFFFF) return 15 - 3;
            return 15 - 4;
        }

        /* Given an input seed, derive the required output.
         */

        private static void DeriveParams(byte[] seed, out byte[] key, out byte[] iv, out string ident)
        {
            // Hash the output using sha512
            byte[] seed_output;

            using (SHA512CryptoServiceProvider sha512csp = new SHA512CryptoServiceProvider())
            {
                seed_output = sha512csp.ComputeHash(seed);
            }

            // Take key from first 32 bytes
            key = new byte[32];
            Buffer.BlockCopy(seed_output, 0, key, 0, 32);

            // Take IV from next 16 bytes
            iv = new byte[16];
            Buffer.BlockCopy(seed_output, 32, iv, 0, 16);

            // Take server identifier (the "ident") from last 16 bytes and base64url encode it
            byte[] ident_raw = new byte[16];
            Buffer.BlockCopy(seed_output, 48, ident_raw, 0, 16);
            ident = UrlBase64Encode(ident_raw);
        }

        private static MemoryStream Encrypt(Stream source, string fileName, out string seed_encoded, out string ident)
        {
            // Randomly generate a new seed for upload
            byte[] seed = new byte[16];

            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(seed);
            }

            seed_encoded = UrlBase64Encode(seed);

            // Derive the parameters (key, IV, ident) from the seed
            byte[] key, iv;
            DeriveParams(seed, out key, out iv, out ident);

            // Create a new String->String map for JSON blob, and define filename and metadata
            Dictionary<string, string> metadataMap = new Dictionary<string, string>();
            metadataMap["mime"] = Helpers.IsTextFile(fileName) ? "text/plain" : Helpers.GetMimeType(fileName);
            metadataMap["name"] = fileName;

            // Encode the metadata with UTF-16 and a double-null-byte terminator, and append data
            // Unfortunately, the CCM cipher mode can't stream the encryption, and so we have to GetBytes() on the source.
            // We do limit the source to 50MB however
            byte[] data = Encoding.BigEndianUnicode.GetBytes(JsonConvert.SerializeObject(metadataMap)).Concat(new byte[] { 0, 0 }).Concat(source.GetBytes()).ToArray();

            // Calculate the length of the CCM IV and copy it over
            long ccmIVLen = FindIVLen(data.Length);
            byte[] ccmIV = new byte[ccmIVLen];
            Array.Copy(iv, ccmIV, ccmIVLen);

            // http://blogs.msdn.com/b/shawnfa/archive/2009/03/17/authenticated-symmetric-encryption-in-net.aspx
            using (AuthenticatedAesCng aes = new AuthenticatedAesCng())
            {
                aes.CngMode = CngChainingMode.Ccm;
                aes.Key = key;
                aes.IV = ccmIV;
                aes.TagSize = MacSize;

                MemoryStream ms = new MemoryStream();

                using (IAuthenticatedCryptoTransform encryptor = aes.CreateAuthenticatedEncryptor())
                {
                    CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    byte[] tag = encryptor.GetTag();
                    ms.Write(tag, 0, tag.Length);
                    return ms;
                }
            }
        }

        public override UploadResult Upload(Stream input, string fileName)
        {
            // Make sure the file (or memory stream) is less than 50MB
            if (input.Length > 50000000)
            {
                throw new ArgumentException("Input files for Up1 cannot be more than 50MB in size.");
            }

            // Initialize the encrypted stream
            UploadResult result;
            string seed, ident;

            using (MemoryStream encryptedStream = Encrypt(input, fileName, out seed, out ident))
            {
                // Set up the form upload
                Dictionary<string, string> uploadArgs = new Dictionary<string, string>();
                uploadArgs["ident"] = ident;
                uploadArgs["api_key"] = ApiKey;

                // Upload and stream encrypt
                result = UploadData(encryptedStream, URLHelpers.CombineURL(SystemUrl, "up"), "blob", "file", uploadArgs);
            }

            if (result.IsSuccess)
            {
                // Return the output URLs
                result.URL = URLHelpers.CombineURL(SystemUrl, "#" + seed);

                Up1Response response = JsonConvert.DeserializeObject<Up1Response>(result.Response);

                if (response != null)
                {
                    result.DeletionURL = URLHelpers.CombineURL(SystemUrl, string.Format("del?ident={0}&delkey={1}", ident, response.DelKey));
                }
            }

            return result;
        }

        private class Up1Response
        {
            public string DelKey { get; set; }
        }
    }
}