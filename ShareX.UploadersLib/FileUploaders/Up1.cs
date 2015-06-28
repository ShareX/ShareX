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
            
            // Encode the metadata with UTF-16 and a double-null-byte terminator, and append data
            // Unfortunately, the CCM cipher mode can't stream the encryption, and so we have to GetBytes() on the source.
            // We do limit the source to 50MB however
            var data = Encoding.BigEndianUnicode.GetBytes(JsonConvert.SerializeObject(metadataMap)).Concat(new byte[] { 0, 0 }).Concat(source.GetBytes()).ToArray();

            // Calculate the length of the CCM IV and copy it over
            var ccmIVLen = FindIVLen(data.Length);
            var ccmIV = new byte[ccmIVLen];
            Array.Copy(iv, ccmIV, ccmIVLen);

            // Set up the encryption parameters
            var keyParam = new KeyParameter(key);
            var ccmParams = new CcmParameters(keyParam, MacSize, ccmIV, new byte[0]);
            var ccmMode = new CcmBlockCipher(new AesFastEngine());
            ccmMode.Init(true, ccmParams);

            // Perform the encryption
            var encBytes = new byte[ccmMode.GetOutputSize(data.Length)];
            var res = ccmMode.ProcessBytes(data, 0, data.Length, encBytes, 0);
            ccmMode.DoFinal(encBytes, res);

            return new MemoryStream(encBytes);
        }

        public override UploadResult Upload(Stream input, string fileName)
        {
            // Make sure the file (or memory stream) is less than 50MB
            if (input.Length > 50000000)
                throw new ArgumentException("Input files for Up1 cannot be more than 50MB in size");

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

    
}
