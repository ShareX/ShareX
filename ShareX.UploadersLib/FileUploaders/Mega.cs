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

    You should have received a copy of the license along with this program; if not,
    write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA 02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShareX.UploadersLib.FileUploaders
{
    public class MegaFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Mega;

        public override bool CheckConfig(UploadersConfig config)
        {
            return !string.IsNullOrWhiteSpace(config.MegaEmail) &&
                !string.IsNullOrWhiteSpace(config.MegaPassword);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Mega(config.MegaEmail, config.MegaPassword, config.MegaTwoFactorAuthenticationCode);
        }
    }

    public sealed class Mega : FileUploader
    {
        private const string ApiURL = "https://g.api.mega.co.nz/cs";
        private const string PublicFileURL = "https://mega.nz/file/";
        private const int InitialChunkSize = 128 * 1024;
        private const int MaximumChunkSize = 1024 * 1024;

        private long sequenceNumber = RandomNumberGenerator.GetInt32(int.MaxValue);
        private string sessionID;
        private byte[] masterKey;

        public string Email { get; }
        public string Password { get; }
        public string TwoFactorAuthenticationCode { get; }

        public Mega(string email, string password, string twoFactorAuthenticationCode = null)
        {
            Email = email?.Trim();
            Password = password;
            TwoFactorAuthenticationCode = twoFactorAuthenticationCode;
        }

        protected override async Task<UploadResult> UploadCoreAsync(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            UploadResult result = new UploadResult();
            bool wasProgressReportingEnabled = AllowReportProgress;
            AllowReportProgress = false;

            try
            {
                if (stream == null || !stream.CanRead)
                {
                    throw new InvalidOperationException("The upload stream is not readable.");
                }

                if (!stream.CanSeek)
                {
                    throw new NotSupportedException("MEGA uploads require a seekable stream.");
                }

                long fileSize = stream.Length - stream.Position;
                await LoginAsync(cancellationToken).ConfigureAwait(false);
                string rootNodeID = await GetRootNodeIDAsync(cancellationToken).ConfigureAwait(false);
                string uploadURL = await GetUploadURLAsync(fileSize, cancellationToken).ConfigureAwait(false);

                string completionHandle = null;
                long uploadPosition = 0;
                ProgressManager progress = new ProgressManager(fileSize);

                using MegaFileCipher cipher = new MegaFileCipher();
                foreach (int chunkSize in GetChunkSizes(fileSize))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    byte[] chunk = new byte[chunkSize];
                    if (chunkSize > 0)
                    {
                        await stream.ReadExactlyAsync(chunk.AsMemory(), cancellationToken).ConfigureAwait(false);
                        cipher.EncryptChunk(chunk);
                    }

                    using MemoryStream chunkStream = new MemoryStream(chunk, writable: false);
                    string response = await SendRequestAsync(HttpMethod.POST, $"{uploadURL}/{uploadPosition}", chunkStream,
                        RequestHelpers.ContentTypeOctetStream, cancellationToken: cancellationToken).ConfigureAwait(false);

                    if (response == null)
                    {
                        throw new MegaRequestException("MEGA rejected the upload request.");
                    }

                    response = response.Trim();
                    if (long.TryParse(response, out long uploadError) && uploadError < 0)
                    {
                        throw new MegaApiException(uploadError);
                    }

                    if (!string.IsNullOrEmpty(response))
                    {
                        completionHandle = response;
                    }

                    uploadPosition += chunkSize;
                    if (wasProgressReportingEnabled && fileSize > 0 && progress.UpdateProgress(chunkSize))
                    {
                        OnProgressChanged(progress);
                    }
                }

                if (string.IsNullOrWhiteSpace(completionHandle))
                {
                    throw new MegaRequestException("MEGA did not return an upload completion handle.");
                }

                byte[] fullFileKey = cipher.CreateFullFileKey();
                byte[] encryptedAttributes = EncryptAttributes(fileName, cipher.FileKey);
                byte[] encryptedFileKey = TransformEcb(fullFileKey, masterKey, encrypt: true);

                JToken createNodeResponse = await SendApiRequestAsync(new JObject
                {
                    ["a"] = "p",
                    ["t"] = rootNodeID,
                    ["n"] = new JArray
                    {
                        new JObject
                        {
                            ["h"] = completionHandle,
                            ["t"] = 0,
                            ["a"] = ToBase64URL(encryptedAttributes),
                            ["k"] = ToBase64URL(encryptedFileKey)
                        }
                    }
                }, cancellationToken).ConfigureAwait(false);

                string nodeID = createNodeResponse.SelectToken("f[0].h")?.Value<string>();
                if (string.IsNullOrWhiteSpace(nodeID))
                {
                    throw new MegaRequestException("MEGA created the file but did not return its node handle.");
                }

                JToken publicHandleResponse = await SendApiRequestAsync(new JObject
                {
                    ["a"] = "l",
                    ["n"] = nodeID
                }, cancellationToken).ConfigureAwait(false);

                string publicHandle = publicHandleResponse.Value<string>();
                if (string.IsNullOrWhiteSpace(publicHandle))
                {
                    throw new MegaRequestException("MEGA did not return a public file handle.");
                }

                result.Response = createNodeResponse.ToString(Formatting.None);
                result.URL = $"{PublicFileURL}{publicHandle}#{ToBase64URL(fullFileKey)}";
                result.IsSuccess = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                if (Errors.Count == 0)
                {
                    Errors.Add(e.Message);
                }
            }
            finally
            {
                AllowReportProgress = wasProgressReportingEnabled;
            }

            return result;
        }

        private async Task LoginAsync(CancellationToken cancellationToken)
        {
            JToken preLoginResponse = await SendApiRequestAsync(new JObject
            {
                ["a"] = "us0",
                ["user"] = Email
            }, cancellationToken, includeSession: false).ConfigureAwait(false);

            int accountVersion = preLoginResponse.Value<int?>("v") ?? 0;
            string salt = preLoginResponse.Value<string>("s");
            MegaAuthenticationInfo authentication = CreateAuthenticationInfo(Email, Password, accountVersion, salt);

            JObject request = new JObject
            {
                ["a"] = "us",
                ["user"] = Email,
                ["uh"] = authentication.UserHash
            };

            if (!string.IsNullOrWhiteSpace(TwoFactorAuthenticationCode))
            {
                request["mfa"] = TwoFactorAuthenticationCode.Trim();
            }

            JToken loginResponse = await SendApiRequestAsync(request, cancellationToken, includeSession: false).ConfigureAwait(false);
            masterKey = TransformEcb(FromBase64URL(loginResponse.Value<string>("k")), authentication.PasswordKey, encrypt: false);

            string encryptedSessionID = loginResponse.Value<string>("csid");
            if (!string.IsNullOrWhiteSpace(encryptedSessionID))
            {
                byte[] encryptedPrivateKey = FromBase64URL(loginResponse.Value<string>("privk"));
                sessionID = DecryptSessionID(encryptedSessionID, encryptedPrivateKey, masterKey);
                return;
            }

            string temporarySessionID = loginResponse.Value<string>("tsid");
            if (!ValidateTemporarySessionID(temporarySessionID, masterKey))
            {
                throw new MegaRequestException("MEGA returned an invalid login session.");
            }

            sessionID = temporarySessionID;
        }

        private async Task<string> GetRootNodeIDAsync(CancellationToken cancellationToken)
        {
            JToken response = await SendApiRequestAsync(new JObject
            {
                ["a"] = "f",
                ["c"] = 1
            }, cancellationToken).ConfigureAwait(false);

            string rootNodeID = response["f"]?
                .Children<JToken>()
                .FirstOrDefault(node => node.Value<int?>("t") == 2)?
                .Value<string>("h");

            if (string.IsNullOrWhiteSpace(rootNodeID))
            {
                throw new MegaRequestException("MEGA did not return the Cloud Drive root folder.");
            }

            return rootNodeID;
        }

        private async Task<string> GetUploadURLAsync(long fileSize, CancellationToken cancellationToken)
        {
            JToken response = await SendApiRequestAsync(new JObject
            {
                ["a"] = "u",
                ["ssl"] = 2,
                ["v"] = 3,
                ["s"] = fileSize
            }, cancellationToken).ConfigureAwait(false);

            string uploadURL = response.Value<string>("p");
            if (string.IsNullOrWhiteSpace(uploadURL))
            {
                throw new MegaRequestException("MEGA did not return an upload URL.");
            }

            return uploadURL.TrimEnd('/');
        }

        private async Task<JToken> SendApiRequestAsync(JObject request, CancellationToken cancellationToken, bool includeSession = true)
        {
            long requestID = Interlocked.Increment(ref sequenceNumber);
            string url = $"{ApiURL}?id={requestID}";
            if (includeSession && !string.IsNullOrWhiteSpace(sessionID))
            {
                url += "&sid=" + Uri.EscapeDataString(sessionID);
            }

            byte[] requestBody = Encoding.UTF8.GetBytes(new JArray(request).ToString(Formatting.None));
            string hashcash = null;

            for (int attempt = 0; attempt < 2; attempt++)
            {
                NameValueCollection headers = new NameValueCollection
                {
                    ["Accept"] = "application/json"
                };
                if (!string.IsNullOrWhiteSpace(hashcash))
                {
                    headers["X-Hashcash"] = hashcash;
                }

                using MemoryStream requestStream = new MemoryStream(requestBody, writable: false);
                ResponseInfo responseInfo = await GetResponseAsync(HttpMethod.POST, url, requestStream,
                    RequestHelpers.ContentTypeJSON, headers: headers, allowNon2xxResponses: true,
                    cancellationToken: cancellationToken).ConfigureAwait(false);

                if (responseInfo == null)
                {
                    throw new MegaRequestException("MEGA API request failed.");
                }

                if (responseInfo.StatusCode == HttpStatusCode.PaymentRequired)
                {
                    string challenge = responseInfo.Headers?["X-Hashcash"];
                    if (attempt > 0 || string.IsNullOrWhiteSpace(challenge))
                    {
                        throw new MegaRequestException("MEGA proof-of-work verification failed.");
                    }

                    hashcash = await Task.Run(() => GenerateHashcashToken(challenge, cancellationToken), cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                if (!responseInfo.IsSuccess)
                {
                    throw new MegaRequestException($"MEGA API request failed with HTTP status {(int)responseInfo.StatusCode}.");
                }

                if (string.IsNullOrWhiteSpace(responseInfo.ResponseText))
                {
                    throw new MegaRequestException("MEGA returned an empty API response.");
                }

                JArray responseArray;
                try
                {
                    responseArray = JArray.Parse(responseInfo.ResponseText);
                }
                catch (JsonException e)
                {
                    throw new MegaRequestException("MEGA returned an invalid API response.", e);
                }

                if (responseArray.Count == 0)
                {
                    throw new MegaRequestException("MEGA returned an empty API response.");
                }

                JToken responseToken = responseArray[0];
                if (responseToken.Type == JTokenType.Integer)
                {
                    long errorCode = responseToken.Value<long>();
                    if (errorCode < 0)
                    {
                        throw new MegaApiException(errorCode);
                    }
                }

                return responseToken;
            }

            throw new MegaRequestException("MEGA proof-of-work verification failed.");
        }

        internal static string GenerateHashcashToken(string challenge, CancellationToken cancellationToken = default)
        {
            string[] parts = challenge?.Split(':');
            if (parts == null || parts.Length < 4 || parts[0] != "1" || !byte.TryParse(parts[1], out byte easiness))
            {
                throw new MegaRequestException("MEGA returned an invalid proof-of-work challenge.");
            }

            string tokenText = parts[3];
            byte[] token = FromBase64URL(tokenText);
            const int tokenLength = 48;
            const int prefixLength = 4;
            const int repeatCount = 262144;
            if (token.Length != tokenLength)
            {
                throw new MegaRequestException("MEGA returned an invalid proof-of-work token.");
            }

            byte[] buffer = new byte[prefixLength + repeatCount * tokenLength];
            Buffer.BlockCopy(token, 0, buffer, prefixLength, tokenLength);
            int filled = tokenLength;
            int tokenAreaLength = repeatCount * tokenLength;
            while (filled < tokenAreaLength)
            {
                int copyLength = Math.Min(filled, tokenAreaLength - filled);
                Buffer.BlockCopy(buffer, prefixLength, buffer, prefixLength + filled, copyLength);
                filled += copyLength;
            }

            uint threshold = (uint)(((easiness & 63) << 1) + 1) << ((easiness >> 6) * 7 + 3);
            Span<byte> hash = stackalloc byte[32];

            for (uint nonce = 0; ; nonce++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                buffer[0] = (byte)(nonce >> 24);
                buffer[1] = (byte)(nonce >> 16);
                buffer[2] = (byte)(nonce >> 8);
                buffer[3] = (byte)nonce;
                SHA256.HashData(buffer, hash);

                uint firstWord = (uint)(hash[0] << 24 | hash[1] << 16 | hash[2] << 8 | hash[3]);
                if (firstWord <= threshold)
                {
                    return $"1:{tokenText}:{ToBase64URL(buffer[..prefixLength])}";
                }
            }
        }

        internal static MegaAuthenticationInfo CreateAuthenticationInfo(string email, string password, int accountVersion, string salt)
        {
            byte[] passwordBytes = PasswordToBytes(password);

            if (accountVersion == 2 && !string.IsNullOrWhiteSpace(salt))
            {
                byte[] derivedKey = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, FromBase64URL(salt), 100000,
                    HashAlgorithmName.SHA512, 32);
                return new MegaAuthenticationInfo
                {
                    PasswordKey = derivedKey[..16],
                    UserHash = ToBase64URL(derivedKey[16..])
                };
            }

            if (accountVersion == 1)
            {
                byte[] passwordKey = PrepareLegacyPasswordKey(passwordBytes);
                return new MegaAuthenticationInfo
                {
                    PasswordKey = passwordKey,
                    UserHash = CreateLegacyUserHash(email.ToLowerInvariant(), passwordKey)
                };
            }

            throw new NotSupportedException($"MEGA account version {accountVersion} is not supported.");
        }

        internal static byte[] PasswordToBytes(string password)
        {
            byte[] bytes = new byte[((password?.Length ?? 0) + 3) & ~3];
            for (int index = 0; index < (password?.Length ?? 0); index++)
            {
                bytes[index] = (byte)password[index];
            }
            return bytes;
        }

        private static byte[] PrepareLegacyPasswordKey(byte[] passwordBytes)
        {
            byte[] passwordKey =
            [
                0x93, 0xC4, 0x67, 0xE3, 0x7D, 0xB0, 0xC7, 0xA4,
                0xD1, 0xBE, 0x3F, 0x81, 0x01, 0x52, 0xCB, 0x56
            ];

            List<ICryptoTransform> encryptors = new List<ICryptoTransform>();
            try
            {
                for (int offset = 0; offset < passwordBytes.Length; offset += 16)
                {
                    byte[] key = new byte[16];
                    Buffer.BlockCopy(passwordBytes, offset, key, 0, Math.Min(16, passwordBytes.Length - offset));
                    Aes aes = CreateAes(key, CipherMode.ECB);
                    encryptors.Add(new OwnedCryptoTransform(aes, aes.CreateEncryptor()));
                }

                for (int iteration = 0; iteration < 65536; iteration++)
                {
                    foreach (ICryptoTransform encryptor in encryptors)
                    {
                        byte[] output = new byte[16];
                        encryptor.TransformBlock(passwordKey, 0, passwordKey.Length, output, 0);
                        passwordKey = output;
                    }
                }
            }
            finally
            {
                foreach (ICryptoTransform encryptor in encryptors)
                {
                    encryptor.Dispose();
                }
            }

            return passwordKey;
        }

        private static string CreateLegacyUserHash(string email, byte[] passwordKey)
        {
            byte[] hash = new byte[16];
            byte[] emailBytes = Encoding.UTF8.GetBytes(email);
            for (int index = 0; index < emailBytes.Length; index++)
            {
                hash[index % hash.Length] ^= emailBytes[index];
            }

            using Aes aes = CreateAes(passwordKey, CipherMode.ECB);
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            for (int iteration = 0; iteration < 16384; iteration++)
            {
                byte[] output = new byte[16];
                encryptor.TransformBlock(hash, 0, hash.Length, output, 0);
                hash = output;
            }

            byte[] result = new byte[8];
            Buffer.BlockCopy(hash, 0, result, 0, 4);
            Buffer.BlockCopy(hash, 8, result, 4, 4);
            return ToBase64URL(result);
        }

        private static string DecryptSessionID(string encryptedSessionID, byte[] encryptedPrivateKey, byte[] accountMasterKey)
        {
            int paddedLength = (encryptedPrivateKey.Length + 15) / 16 * 16;
            Array.Resize(ref encryptedPrivateKey, paddedLength);
            byte[] privateKey = TransformEcb(encryptedPrivateKey, accountMasterKey, encrypt: false);

            int offset = 0;
            BigInteger p = ReadMpi(privateKey, ref offset);
            BigInteger q = ReadMpi(privateKey, ref offset);
            BigInteger d = ReadMpi(privateKey, ref offset);
            _ = ReadMpi(privateKey, ref offset);

            byte[] encryptedSessionBytes = FromBase64URL(encryptedSessionID);
            int sessionOffset = 0;
            BigInteger encryptedSession = ReadMpi(encryptedSessionBytes, ref sessionOffset);
            BigInteger decryptedSession = BigInteger.ModPow(encryptedSession, d, p * q);
            byte[] sessionBytes = decryptedSession.ToByteArray(isUnsigned: true, isBigEndian: true);

            if (sessionBytes.Length < 43)
            {
                throw new CryptographicException("MEGA returned an invalid encrypted session.");
            }

            return ToBase64URL(sessionBytes[..43]);
        }

        private static bool ValidateTemporarySessionID(string temporarySessionID, byte[] accountMasterKey)
        {
            if (string.IsNullOrWhiteSpace(temporarySessionID))
            {
                return false;
            }

            byte[] sessionBytes = FromBase64URL(temporarySessionID);
            if (sessionBytes.Length != 32)
            {
                return false;
            }

            byte[] encryptedChallenge = sessionBytes[16..];
            byte[] challenge = TransformEcb(encryptedChallenge, accountMasterKey, encrypt: false);
            return CryptographicOperations.FixedTimeEquals(sessionBytes.AsSpan(0, 16), challenge);
        }

        private static BigInteger ReadMpi(byte[] data, ref int offset)
        {
            if (offset + 2 > data.Length)
            {
                throw new CryptographicException("Invalid MEGA MPI value.");
            }

            int bitLength = data[offset] * 256 + data[offset + 1];
            int byteLength = (bitLength + 7) / 8;
            offset += 2;

            if (byteLength == 0 || offset + byteLength > data.Length)
            {
                throw new CryptographicException("Invalid MEGA MPI value.");
            }

            BigInteger value = new BigInteger(data.AsSpan(offset, byteLength), isUnsigned: true, isBigEndian: true);
            offset += byteLength;
            return value;
        }

        private static byte[] EncryptAttributes(string fileName, byte[] fileKey)
        {
            string json = JsonConvert.SerializeObject(new { n = fileName }, Formatting.None);
            byte[] data = Encoding.UTF8.GetBytes("MEGA" + json);
            Array.Resize(ref data, (data.Length + 15) / 16 * 16);

            using Aes aes = CreateAes(fileKey, CipherMode.CBC);
            aes.IV = new byte[16];
            using ICryptoTransform encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        private static byte[] TransformEcb(byte[] data, byte[] key, bool encrypt)
        {
            if (data == null || data.Length == 0 || data.Length % 16 != 0)
            {
                throw new CryptographicException("Invalid MEGA AES block data.");
            }

            using Aes aes = CreateAes(key, CipherMode.ECB);
            using ICryptoTransform transform = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
            return transform.TransformFinalBlock(data, 0, data.Length);
        }

        private static Aes CreateAes(byte[] key, CipherMode mode)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.Mode = mode;
            aes.Padding = PaddingMode.None;
            return aes;
        }

        private static IEnumerable<int> GetChunkSizes(long fileSize)
        {
            if (fileSize == 0)
            {
                yield return 0;
                yield break;
            }

            long remaining = fileSize;
            int chunkSize = InitialChunkSize;
            while (remaining > 0)
            {
                int currentSize = (int)Math.Min(chunkSize, remaining);
                yield return currentSize;
                remaining -= currentSize;
                chunkSize = Math.Min(chunkSize + InitialChunkSize, MaximumChunkSize);
            }
        }

        internal static string ToBase64URL(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        internal static byte[] FromBase64URL(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException("Invalid MEGA Base64 value.");
            }

            string base64 = value.Replace('-', '+').Replace('_', '/');
            base64 = base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
            return Convert.FromBase64String(base64);
        }

        private sealed class MegaFileCipher : IDisposable
        {
            private readonly Aes aes;
            private readonly byte[] fileMac = new byte[16];
            private ulong counter;

            public byte[] FileKey { get; } = RandomNumberGenerator.GetBytes(16);
            public byte[] IV { get; } = RandomNumberGenerator.GetBytes(8);
            public byte[] MetaMac { get; private set; } = new byte[8];

            public MegaFileCipher()
            {
                aes = CreateAes(FileKey, CipherMode.ECB);
            }

            public void EncryptChunk(byte[] data)
            {
                byte[] chunkMac = new byte[16];
                Buffer.BlockCopy(IV, 0, chunkMac, 0, IV.Length);
                Buffer.BlockCopy(IV, 0, chunkMac, IV.Length, IV.Length);

                byte[] counterBlock = new byte[16];
                byte[] keyStream = new byte[16];
                byte[] plainBlock = new byte[16];

                for (int offset = 0; offset < data.Length; offset += 16)
                {
                    int blockLength = Math.Min(16, data.Length - offset);
                    Array.Clear(plainBlock);
                    Buffer.BlockCopy(data, offset, plainBlock, 0, blockLength);

                    Buffer.BlockCopy(IV, 0, counterBlock, 0, IV.Length);
                    WriteUInt64BigEndian(counterBlock, 8, counter++);
                    aes.EncryptEcb(counterBlock, keyStream, PaddingMode.None);

                    for (int index = 0; index < 16; index++)
                    {
                        chunkMac[index] ^= plainBlock[index];
                    }
                    byte[] encryptedChunkMac = new byte[16];
                    aes.EncryptEcb(chunkMac, encryptedChunkMac, PaddingMode.None);
                    chunkMac = encryptedChunkMac;

                    for (int index = 0; index < blockLength; index++)
                    {
                        data[offset + index] = (byte)(plainBlock[index] ^ keyStream[index]);
                    }
                }

                for (int index = 0; index < fileMac.Length; index++)
                {
                    fileMac[index] ^= chunkMac[index];
                }
                byte[] encryptedFileMac = new byte[16];
                aes.EncryptEcb(fileMac, encryptedFileMac, PaddingMode.None);
                Buffer.BlockCopy(encryptedFileMac, 0, fileMac, 0, fileMac.Length);

                for (int index = 0; index < 4; index++)
                {
                    MetaMac[index] = (byte)(fileMac[index] ^ fileMac[index + 4]);
                    MetaMac[index + 4] = (byte)(fileMac[index + 8] ^ fileMac[index + 12]);
                }
            }

            public byte[] CreateFullFileKey()
            {
                byte[] fullFileKey = new byte[32];
                for (int index = 0; index < 8; index++)
                {
                    fullFileKey[index] = (byte)(FileKey[index] ^ IV[index]);
                    fullFileKey[index + 16] = IV[index];
                }
                for (int index = 8; index < 16; index++)
                {
                    fullFileKey[index] = (byte)(FileKey[index] ^ MetaMac[index - 8]);
                    fullFileKey[index + 16] = MetaMac[index - 8];
                }
                return fullFileKey;
            }

            public void Dispose()
            {
                aes.Dispose();
            }

            private static void WriteUInt64BigEndian(byte[] buffer, int offset, ulong value)
            {
                for (int index = 7; index >= 0; index--)
                {
                    buffer[offset + index] = (byte)value;
                    value >>= 8;
                }
            }
        }

        private sealed class OwnedCryptoTransform : ICryptoTransform
        {
            private readonly Aes owner;
            private readonly ICryptoTransform transform;

            public int InputBlockSize => transform.InputBlockSize;
            public int OutputBlockSize => transform.OutputBlockSize;
            public bool CanTransformMultipleBlocks => transform.CanTransformMultipleBlocks;
            public bool CanReuseTransform => transform.CanReuseTransform;

            public OwnedCryptoTransform(Aes owner, ICryptoTransform transform)
            {
                this.owner = owner;
                this.transform = transform;
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                return transform.TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset);
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                return transform.TransformFinalBlock(inputBuffer, inputOffset, inputCount);
            }

            public void Dispose()
            {
                transform.Dispose();
                owner.Dispose();
            }
        }
    }

    internal sealed class MegaAuthenticationInfo
    {
        public byte[] PasswordKey { get; set; }
        public string UserHash { get; set; }
    }

    internal class MegaRequestException : Exception
    {
        public MegaRequestException(string message) : base(message)
        {
        }

        public MegaRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    internal sealed class MegaApiException : MegaRequestException
    {
        public long ErrorCode { get; }

        public MegaApiException(long errorCode) : base(GetErrorMessage(errorCode))
        {
            ErrorCode = errorCode;
        }

        private static string GetErrorMessage(long errorCode)
        {
            return errorCode switch
            {
                -3 => "MEGA reported a temporary error. Please try again.",
                -4 => "MEGA is rate limiting requests. Please try again later.",
                -9 => "The requested MEGA item was not found.",
                -11 => "MEGA denied access to the requested item.",
                -14 => "MEGA rejected an encryption key.",
                -15 => "The MEGA session is invalid or expired.",
                -16 => "The MEGA account is blocked.",
                -17 => "The MEGA storage quota has been exceeded.",
                -18 => "MEGA is temporarily unavailable.",
                -26 => "MEGA requires a two-factor authentication code.",
                -27 => "The MEGA two-factor authentication code is invalid.",
                -29 => "MEGA requires additional proof-of-work verification for this request.",
                _ => $"MEGA API error: {errorCode}."
            };
        }
    }
}
