using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using ShareX.HelpersLib;

namespace ShareX.UploadersLib.FileUploaders
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

    [Localizable(false)]
    public sealed class Up1 : FileUploader
    {
        private const int MacSize = 64;

        public string ApiKey { get; set; }
        public string SystemUrl { get; set; }

        public Up1(string systemUrl, string apiKey)
        {
            SystemUrl = systemUrl;
            ApiKey = apiKey;
        }

        /* Since we're dealing with URLs, the regular base64 encoding using +, / and = will mess things up
         * Therefore we'll use the URL base64 standard (as defined in the RFC)
         */
        public static string UrlBase64Encode(byte[] input)
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
        public static void DeriveParams(byte[] seed, out byte[] key, out byte[] iv, out string ident)
        {
            // Hash the output using sha512
            var sha512csp = new SHA512CryptoServiceProvider();
            var seed_output = sha512csp.ComputeHash(seed);

            // Take key from first 32 bytes
            key = new byte[32];
            Buffer.BlockCopy(seed_output, 0, key, 0, 32);

            // Take IV from next 16 bytes
            iv = new byte[16];
            Buffer.BlockCopy(seed_output, 32, iv, 0, 16);

            // Take server identifier (the "ident") from last 16 bytes and base64url encode it
            var ident_raw = new byte[16];
            Buffer.BlockCopy(seed_output, 48, ident_raw, 0, 16);
            ident = UrlBase64Encode(ident_raw);
        }

        public static Stream Encrypt(Stream source, string fileName, out string seed_encoded, out string ident)
        {
            // Randomly generate a new seed for upload
            var rngCsp = new RNGCryptoServiceProvider();
            var seed = new byte[16];
            rngCsp.GetBytes(seed);
            seed_encoded = UrlBase64Encode(seed);

            // Derive the parameters (key, IV, ident) from the seed
            byte[] key, iv;
            DeriveParams(seed, out key, out iv, out ident);
            
            // Create a new String->String map for JSON blob, and define filename and metadata
            var metadataMap = new Dictionary<string, string>();
            metadataMap["mime"] = Helpers.IsTextFile(fileName) ? "text/plain" : Helpers.GetMimeType(fileName);
            metadataMap["name"] = fileName;
            
            // Encode the metadata with UTF-16 and a double-null-byte terminator
            var metadata = Encoding.BigEndianUnicode.GetBytes(JsonConvert.SerializeObject(metadataMap)).Concat(new byte[] { 0, 0 }).ToArray();

            // Calculate the length of the CCM IV and copy it over
            var ccmIVLen = FindIVLen(metadata.Length + source.Length);
            var ccmIV = new byte[ccmIVLen];
            Array.Copy(iv, ccmIV, ccmIVLen);

            // Set up the encryption parameters
            var keyParam = new KeyParameter(key);
            var ccmParams = new CcmParameters(keyParam, MacSize, ccmIV, new byte[0]);
            var ccmMode = new CcmBlockCipher(new AesFastEngine());
            ccmMode.Init(true, ccmParams);

            return new Up1Stream(source, metadata, ccmMode, metadata.Length + (int)source.Length);
        }

        public override UploadResult Upload(Stream input, string fileName)
        {
            // Initialize the encrypted stream
            string seed, ident;
            var encryptedStream = Encrypt(input, fileName, out seed, out ident);

            // Set up the form upload
            var uploadArgs = new Dictionary<string, string>();
            uploadArgs["ident"] = ident;
            uploadArgs["api_key"] = ApiKey;

            // Upload and stream encrypt
            var result = UploadData(encryptedStream, SystemUrl + "/up", "blob", "file", uploadArgs);

            if (!result.IsSuccess) return result;

            // Return the output URLs
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.Response);
            result.URL = SystemUrl + "/#" + seed;
            result.DeletionURL = SystemUrl + "/del?ident=" + ident + "&delkey=" + values["delkey"];

            return result;
        }
    }

    /* This custom stream is used to encrypt the data on-the-fly. As the CCM functions in BouncyCastle are designed to 
     * have a known input and unknown output, there are functions to accommodate this (GetOutputSize). Unfortunately as
     * a Stream we are left with the other way around (unknown input and known output). In this case, we need to use an
     * overrun buffer, as we can't perfectly estimate the size of intermediate buffers.
     */
    sealed class Up1Stream : Stream
    {
        private Stream _source;
        private byte[] _overrun;
        private CcmBlockCipher _ccmMode;
        private bool _isCCMFinal;
        private long _length;

        public Up1Stream(Stream source, byte[] metadata, CcmBlockCipher ccmMode, long length)
        {
            _source = source;
            _ccmMode = ccmMode;
            _overrun = EncryptMetadata(metadata); // Nice little hack to ensure the metadata is written first
            _isCCMFinal = false;
            _length = length;
        }

        public byte[] EncryptMetadata(byte[] metadata)
        {
            var inArray = new byte[metadata.Length];
            Array.Copy(metadata, inArray, metadata.Length);
            var metaLen = _ccmMode.GetOutputSize(metadata.Length);
            var outMeta = new byte[metaLen];
            var outRealLen = _ccmMode.ProcessBytes(inArray, 0, inArray.Length, outMeta, 0);

            Array.Resize(ref outMeta, outRealLen);
            return outMeta;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // Check if the overrun is enough to satisfy the buffer
            if (_overrun.Length >= count)
            {
                Array.Copy(_overrun, 0, buffer, offset, count);
                _overrun = _overrun.Skip(count).ToArray();
                return count;
            }

            // Check if we're on the last legs (CCM has been finalized)
            if (_isCCMFinal)
            {
                if (_overrun.Length != 0)
                    Array.Copy(_overrun, 0, buffer, offset, _overrun.Length);
                return _overrun.Length;
            }

            // If overrun contains anything, throw it in the buffer immediately
            var remainCount = count;
            var remainOffset = offset;
            if (_overrun.Length != 0)
            {
                remainOffset += _overrun.Length;
                remainCount -= _overrun.Length;
                Array.Copy(_overrun, buffer, _overrun.Length);
                // At this point, ignore anything in _overrun
            }

            // Read chunk of data from input
            var inBytes = new byte[remainCount];
            var readBytes = _source.Read(inBytes, 0, remainCount);

            // Calculate how much length we need depending on if we're at the end or not
            var encLength = _ccmMode.GetOutputSize(readBytes);

            // Close the file and finalize if we're at the end of the input
            if (readBytes == 0)
            {
                _source.Close();
                _isCCMFinal = true;
            }

            // Create overrun buffer if we have too much output
            if (encLength > remainCount)
            {
                _overrun = new byte[encLength - remainCount];
                var encBytes = new byte[encLength];
                if (!_isCCMFinal)
                    _ccmMode.ProcessBytes(inBytes, 0, readBytes, encBytes, 0);
                else
                    _ccmMode.DoFinal(encBytes, 0);

                Array.Copy(encBytes, 0, buffer, remainOffset, remainCount);
                Array.Copy(encBytes, encLength - remainCount, _overrun, 0, encLength - remainCount);
            }
            // Otherwise, encrypt directly to the given buffer
            else
            {
                if (_overrun.Length != 0)
                    _overrun = new byte[0]; // Clear the overrun buffer if exists
                if (!_isCCMFinal)
                    _ccmMode.ProcessBytes(inBytes, 0, readBytes, buffer, remainOffset);
                else
                    _ccmMode.DoFinal(buffer, remainOffset);
            }

            return remainCount;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        // We can't implement anything else in this stream

        public override long Length
        {
            get
            {
                return _length;
            }
        }
        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
