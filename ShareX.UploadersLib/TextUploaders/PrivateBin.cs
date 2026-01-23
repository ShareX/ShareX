using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShareX.HelpersLib;
using ShareX.UploadersLib.Properties;

namespace ShareX.UploadersLib.TextUploaders
{
    public class PrivateBinUploaderService : TextUploaderService
    {
        public override TextDestination EnumValue { get; } = TextDestination.PrivateBin;

        public override Image ServiceImage => Resources.PrivateBin;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            var settings = config.PrivateBinSettings;
            return new PrivateBin(settings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPrivateBin;
    }

    public sealed class PrivateBin(PrivateBinSettings settings) : TextUploader
    {
        private PrivateBinSettings Settings { get; } = settings;

        public override UploadResult UploadText(string text, string fileName)
        {
            var result = new UploadResult();

            if (string.IsNullOrEmpty(text))
                return result;

            var privateBinHelper = new PrivateBinApiHelper(Settings);
            var ct = privateBinHelper.EncryptMessage(text, Settings.PastePassword);

            var meta = new Dictionary<string, object>
            {
                { "expire", Settings.Expiration.GetDescription() }
            };

            var payload = new Dictionary<string, object>
            {
                { "adata", privateBinHelper.PrivateBinPasteMetaStruct.Adata },
                { "meta", meta },
                { "v", 2 },
                { "ct", ct }
            };

            var headers = new NameValueCollection
            {
                { "X-Requested-With", "JSONHttpRequest" }
            };

            if (!string.IsNullOrEmpty(Settings.Username) && !string.IsNullOrEmpty(Settings.Password))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(Settings.Username + ':' + Settings.Password));

                headers.Add("Authorization", "Basic " + token);
            }

            SendRequest(HttpMethod.POST, Settings.CustomUrl, JsonConvert.SerializeObject(payload), RequestHelpers.ContentTypeJSON, null, headers);

            if (LastResponseInfo.IsSuccess)
            {
                var response = JsonConvert.DeserializeObject<Response>(LastResponseInfo.ResponseText);

                if (response.Status == 0)
                {
                    result.URL = URLHelpers.CombineURL(Settings.CustomUrl, response.Url) + '#' + privateBinHelper.PublicDecryptionKey;
                    result.DeletionURL = URLHelpers.BuildUri(Settings.CustomUrl, null, URLHelpers.CreateQueryString(new Dictionary<string, string>()
                    {
                        { "pasteid", response.Id },
                        { "deletetoken", response.DeleteToken }
                    }));
                }
                else
                    Errors.Add(response.Message);
            }
            else if (LastResponseInfo.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                Errors.Add("Server require authorization");

            return result;
        }

        private class Response
        {
            public int Status { get; set; }
            public string Id { get; set; }
            public string Url { get; set; }
            public string DeleteToken { get; set; }
            public string Message { get; set; }
        }

        private static class PrivateBinConstants
        {
            public const int CIPHER_ITERATION_COUNT = 100000;
            public const int CIPHER_BLOCK_BITS = 256;
            public const int CIPHER_TAG_BITS = 128;
        }

        private class PrivateBinPasteMetaStruct
        {
            private PrivateBinSettings Settings { get; }
            public List<object> Adata { get; } = [];

            public PrivateBinPasteMetaStruct(PrivateBinApiHelper privatebinApiHelper, PrivateBinSettings settings) : this(settings, Convert.ToBase64String(privatebinApiHelper.Iv), Convert.ToBase64String(privatebinApiHelper.Salt))
            {
                Settings = settings;
            }

            private PrivateBinPasteMetaStruct(PrivateBinSettings settings, string cipherIv, string kdfSalt)
            {
                Settings = settings;

                var meta = new List<object>
                {
                    cipherIv,
                    kdfSalt,
                    PrivateBinConstants.CIPHER_ITERATION_COUNT,
                    PrivateBinConstants.CIPHER_BLOCK_BITS,
                    PrivateBinConstants.CIPHER_TAG_BITS,
                    "aes",
                    "cbc",
                    "none"
                };

                Adata.Add(meta);
                Adata.Add(Settings.Format.GetDescription());
                Adata.Add(0);
                Adata.Add(Settings.BurnAfterReading ? 1 : 0);
            }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(Adata);
            }
        }

        private class PrivateBinApiHelper
        {
            private PrivateBinSettings Settings { get; }
            public PrivateBinPasteMetaStruct PrivateBinPasteMetaStruct { get; }
            public byte[] Iv { get; }
            public byte[] Salt { get; }
            public string PublicDecryptionKey { get; }
            private byte[] pastePasswordBytes;

            public PrivateBinApiHelper(PrivateBinSettings settings)
            {
                Settings = settings;
                pastePasswordBytes = GetRandomBytes(PrivateBinConstants.CIPHER_BLOCK_BITS / 8);
                PublicDecryptionKey = Base58Converter.Encode(pastePasswordBytes);

                Iv = GetRandomBytes(PrivateBinConstants.CIPHER_TAG_BITS / 8);
                Salt = GetRandomBytes(8);
                PrivateBinPasteMetaStruct = new PrivateBinPasteMetaStruct(this, Settings);
            }

            public string EncryptMessage(string message, string pastePassword = null)
            {
                if (!string.IsNullOrEmpty(pastePassword))
                    pastePasswordBytes = Combine(pastePasswordBytes, Encoding.UTF8.GetBytes(pastePassword));

                var json = new Dictionary<string, string>
                {
                    { "paste", message }
                };

                return CreateCipherText(JsonConvert.SerializeObject(json));
            }

            private static byte[] Combine(params byte[][] arrays)
            {
                var rv = new byte[arrays.Sum(a => a.Length)];
                var offset = 0;

                foreach (var array in arrays)
                {
                    Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                    offset += array.Length;
                }

                return rv;
            }

            private static byte[] GetRandomBytes(int count)
            {
                var res = new byte[count];
                RandomCrypto.NextBytes(res);

                return res;
            }

            private byte[] DeriveKey()
            {
                using var deriveBytes = new Rfc2898DeriveBytes(pastePasswordBytes, Salt, PrivateBinConstants.CIPHER_ITERATION_COUNT, HashAlgorithmName.SHA256);
                return deriveBytes.GetBytes(PrivateBinConstants.CIPHER_BLOCK_BITS / 8);
            }


            private string CreateCipherText(string plainText)
            {
                return CreateCipherTextCBC(plainText);
            }


            private string CreateCipherTextCBC(string plainText)
            {
                byte[] encrypted;

                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = DeriveKey();
                    aesAlg.IV = Iv;

                    aesAlg.Mode = CipherMode.CBC;

                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                            swEncrypt.Write(plainText);

                        encrypted = msEncrypt.ToArray();
                    }
                }

                return Convert.ToBase64String(encrypted);
            }
        }
    }

    [DefaultValue(W1)]
    public enum PrivateBinExpiration
    {
        [Description("5min")] M5,
        [Description("10min")] M10,
        [Description("1hour")] H1,
        [Description("1day")] D1,
        [Description("1week")] W1,
        [Description("1month")] M1,
        [Description("1year")] Y1,
        [Description("never")] N
    }

    [DefaultValue(PlainText)]
    public enum PrivateBinFormat
    {
        [Description("plaintext")] PlainText,
        [Description("syntaxhighlighting")] SyntaxHighlighting,
        [Description("markdown")] Markdown
    }

    public class PrivateBinSettings
    {
        public string Username { get; set; }
        [JsonEncrypt] public string Password { get; set; }
        public bool BurnAfterReading { get; set; }
        public PrivateBinExpiration Expiration { get; set; } = PrivateBinExpiration.W1;
        public PrivateBinFormat Format { get; set; } = PrivateBinFormat.PlainText;
        public string CustomUrl { get; set; } = "https://privatebin.net/";
        [JsonEncrypt] public string PastePassword { get; set; } = string.Empty;
    }
}