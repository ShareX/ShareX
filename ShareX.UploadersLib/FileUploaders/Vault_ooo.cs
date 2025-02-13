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

using Newtonsoft.Json;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ShareX.UploadersLib.FileUploaders
{
    public class Vault_oooFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Vault_ooo;

        public override bool CheckConfig(UploadersConfig config)
        {
            return true;
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Vault_ooo();
        }
    }

    public sealed class Vault_ooo : FileUploader
    {
        private const string APIURL = "https://vault.ooo";
        private const int PBKDF2_ITERATIONS = 10000;
        private const int AES_KEY_SIZE = 256; // Bits
        private const int AES_BLOCK_SIZE = 128; // Bits
        private const int BYTE_CHUNK_SIZE = 256 * 1024 * 381; // Copied from web client (99 MB)
        private static readonly DateTime ORIGIN_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public override UploadResult Upload(Stream stream, string fileName)
        {
            #region Calculating sizes

            long fileSize = stream.Length;
            int chunks = (int)Math.Ceiling((double)fileSize / BYTE_CHUNK_SIZE);
            long fullUploadSize = 16; // 16 bytes header

            List<long> uploadSizes = new List<long>();
            uploadSizes.Add(0);
            LoopStartEnd((chunkStart, chunkEnd, i) =>
            {
                int chunkLength = chunkEnd - chunkStart;
                fullUploadSize += chunkLength + 16 - (chunkLength % 16);
                uploadSizes.Add(fullUploadSize);
            }, chunks, fileSize);

            #endregion

            string randomKey = GenerateRandomKey();
            byte[] randomKeyBytes = Encoding.UTF8.GetBytes(randomKey);
            Vault_oooCryptoData cryptoData = DeriveCryptoData(randomKeyBytes);

            #region Building filename

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            string encryptedFileName;
            using (MemoryStream ms = new MemoryStream()) // Encrypting file name
            {
                ms.Write(cryptoData.Salt, 0, cryptoData.Salt.Length);
                byte[] encryptedFn = EncryptBytes(cryptoData, fileNameBytes);
                ms.Write(encryptedFn, 0, encryptedFn.Length);
                encryptedFileName = Helpers.BytesToHex(ms.ToArray());
            }
            string bytesLengthHex = fullUploadSize.ToString("X4"); // To Hex
            DateTime expiryTime = DateTime.UtcNow.AddDays(30); // Defaults from the web client
            string expiryTimeHex = ((long)(expiryTime - ORIGIN_TIME).TotalSeconds).ToString("X4"); // Expiry date in UNIX seconds in hex
            string fullFileName = $"{expiryTimeHex}-b-{bytesLengthHex}-{encryptedFileName}".ToLower();

            #endregion

            Dictionary<string, string> requestHeaders = new Dictionary<string, string>();
            requestHeaders.Add("X-Get-Raw-File", "1");
            Dictionary<string, long> postRequestJson = new Dictionary<string, long>();
            postRequestJson.Add("chunks", chunks);
            postRequestJson.Add("fileLength", fullUploadSize);

            string postResult = SendRequest(HttpMethod.POST, URLHelpers.CombineURL(APIURL, fullFileName), JsonConvert.SerializeObject(postRequestJson), RequestHelpers.ContentTypeJSON, requestHeaders);
            Vault_oooMetaInfo metaInfo = JsonConvert.DeserializeObject<Vault_oooMetaInfo>(postResult);

            if (string.IsNullOrEmpty(metaInfo.UrlPathName))
                throw new InvalidOperationException("No correct metaInfo returned");

            #region Upload in chunks

            List<byte> dumpStash = new List<byte>();
            LoopStartEnd((chunkStart, chunkEnd, i) =>
            {
                int chunkLength = chunkEnd - chunkStart;
                byte[] plainBytes = new byte[chunkLength];
                stream.Read(plainBytes, 0, chunkLength);

                byte[] encryptedBytes = EncryptBytes(cryptoData, plainBytes);

                int prependSize = 0;
                if (dumpStash.Count > 0)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Write(dumpStash.ToArray(), 0, dumpStash.Count);
                        ms.Write(encryptedBytes, 0, encryptedBytes.Length);
                        encryptedBytes = ms.ToArray();
                    }

                    prependSize = dumpStash.Count;
                    dumpStash.Clear();
                }

                if (encryptedBytes.Length + (i == 0 ? 16 : 0) > BYTE_CHUNK_SIZE) // 16 bytes for the salt header
                {
                    dumpStash.AddRange(encryptedBytes.Skip(BYTE_CHUNK_SIZE - (i == 0 ? 16 : 0)));
                    encryptedBytes = encryptedBytes.Take(BYTE_CHUNK_SIZE - (i == 0 ? 16 : 0)).ToArray();
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (i == 0)
                    {
                        ms.Write(Encoding.UTF8.GetBytes("Salted__"), 0, 8); // Write header
                        ms.Write(cryptoData.Salt, 0, cryptoData.Salt.Length); // 8 bytes
                        ms.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    else
                    {
                        ms.Write(encryptedBytes, 0, encryptedBytes.Length); // Write encrypted bytes
                    }

                    NameValueCollection headers = new NameValueCollection();
                    headers.Add("X-Get-Raw-File", "1");
                    int uploadChunkStart = (int)(uploadSizes[i] - prependSize);
                    headers.Add("X-Put-Chunk-Start", uploadChunkStart.ToString());
                    headers.Add("X-Put-Chunk-End", (uploadChunkStart + ms.Length).ToString());
                    headers.Add("X-Put-JWT", metaInfo.Token);

                    SendRequest(HttpMethod.PUT, URLHelpers.CombineURL(APIURL, metaInfo.UrlPathName), ms, "application/octet-stream", null, headers);
                }
            }, chunks, fileSize);

            #endregion

            UploadResult res = new UploadResult();
            res.IsURLExpected = true;
            res.URL = URLHelpers.CombineURL(APIURL, metaInfo.UrlPathName) + "#" + randomKey; // Full url with the encryption key

            return res;
        }

        private delegate void StartEndCallback(int chunkStart, int chunkEnd, int i);

        private static void LoopStartEnd(StartEndCallback callback, int chunks, long fileSize)
        {
            int lastChunkEnd = 0;
            for (int i = 0; i < chunks; i++)
            {
                int chunkStart, chunkEnd;

                if (i == 0)
                {
                    chunkStart = 0;
                    lastChunkEnd = chunkEnd = (int)Math.Min(fileSize, BYTE_CHUNK_SIZE);
                }
                else
                {
                    chunkStart = lastChunkEnd;
                    lastChunkEnd = chunkEnd = (int)Math.Min(fileSize, lastChunkEnd + BYTE_CHUNK_SIZE);
                }

                callback(chunkStart, chunkEnd, i);
            }
        }

        #region Crypto

        private static string GenerateRandomKey()
        {
            return Guid.NewGuid().ToString(); // The web client uses random uuids as keys
        }

        private static Vault_oooCryptoData DeriveCryptoData(byte[] key)
        {
            byte[] salt = new byte[8]; // 8 bytes salt like in the web client
            RandomNumberGenerator rng = RandomNumberGenerator.Create(); // Cryptographically secure
            rng.GetBytes(salt);

            Rfc2898DeriveBytes rfcDeriver = new Rfc2898DeriveBytes(key, salt, PBKDF2_ITERATIONS, HashAlgorithmName.SHA256);

            return new Vault_oooCryptoData
            {
                Salt = salt,
                Key = rfcDeriver.GetBytes(AES_KEY_SIZE / 8), // Derive the bytes from the rfcDeriver; Divide by 8 to input byte count
                IV = rfcDeriver.GetBytes(AES_BLOCK_SIZE / 8)
            };
        }

        private static byte[] EncryptBytes(Vault_oooCryptoData crypto, byte[] bytes)
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.Mode = CipherMode.CBC;
                aes.KeySize = AES_KEY_SIZE;
                aes.BlockSize = AES_BLOCK_SIZE;

                aes.Key = crypto.Key;
                aes.IV = crypto.IV;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(crypto.Key, crypto.IV), CryptoStreamMode.Write))
                    {
                        cs.Write(bytes, 0, bytes.Length); // Write all bytes into the CryptoStream
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }

        private class Vault_oooCryptoData
        {
            public byte[] Salt { get; set; }
            public byte[] Key { get; set; }
            public byte[] IV { get; set; }
        }

        #endregion

        private class Vault_oooMetaInfo
        {
            [JsonProperty("urlPathName")]
            public string UrlPathName { get; set; }

            [JsonProperty("token")]
            public string Token { get; set; }
        }
    }
}