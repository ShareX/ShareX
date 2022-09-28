// Credits: https://github.com/Kos9078

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
        public override TextDestination EnumValue { get; } = TextDestination.Privatebin;

        public override Image ServiceImage => Resources.Privatebin;

        public override bool CheckConfig(UploadersConfig config) => true;

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            var settings = config.PrivatebinSettings;
            return new Privatebin(settings);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpPrivatebin;
    }

    public sealed class Privatebin : TextUploader
    {
        private PrivatebinSettings Settings { get; }

        public Privatebin(PrivatebinSettings settings)
        {
            Settings = settings;
        }

        public override UploadResult UploadText(string text, string fileName)
        {
            var ur = new UploadResult();
            if (string.IsNullOrEmpty(text))
            {
                return ur;
            }

            var privateBinHelper = new PrivatebinApiHelper();
            var ct = privateBinHelper.EncryptMessage(text, Settings.PastePassword);

            var meta = new Dictionary<string, object>
            {
                { "expire", Settings.Expiration.GetDescription() }
            };

            var payload = new Dictionary<string, object>
            {
                { "adata", privateBinHelper.PrivatebinPasteMetaStruct.Adata },
                { "meta", meta },
                { "v", 2 },
                { "ct", ct }
            };

            var headers = new NameValueCollection
            {
                { "X-Requested-With", "JSONHttpRequest" }
            };

            if (!string.IsNullOrEmpty(Settings.Username) && !string.IsNullOrEmpty(Settings.Password)) // https://github.com/PrivateBin/PrivateBin/wiki/Restrict-upload-using-NGINX
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
                    ur.URL = URLHelpers.CombineURL(Settings.CustomUrl, response.Url) + '#' + privateBinHelper.PublicDecryptionKey;
                    ur.DeletionURL = URLHelpers.BuildUri(Settings.CustomUrl, null, URLHelpers.CreateQueryString(new Dictionary<string, string>()
                    {
                        { "pasteid", response.Id },
                        { "deletetoken", response.DeleteToken }
                    }));
                }
                else
                {
                    Errors.Add(response.Message);
                }
            }
            else if (LastResponseInfo.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Errors.Add("Server require authorization");
            }

            return ur;
        }

        private class Response
        {
            public int Status { get; set; }
            public string Id { get; set; }
            public string Url { get; set; }
            public string DeleteToken { get; set; }
            public string Message { get; set; }
        }

        private static class PrivatebinConstants
        {
            public static int CIPHER_ITERATION_COUNT = 100000;
            public static int CIPHER_BLOCK_BITS = 256;
            public static int CIPHER_TAG_BITS = 128;
        }

        private class PrivatebinPasteMetaStruct
        {
            public List<object> Adata { get; } = new List<object>();

            public PrivatebinPasteMetaStruct(PrivatebinApiHelper privatebinApiHelper) : this(Convert.ToBase64String(privatebinApiHelper.Iv),
                Convert.ToBase64String(privatebinApiHelper.Salt))
            {
            }

            private PrivatebinPasteMetaStruct(string cipherIv, string kdfSalt)
            {
                List<object> paste_meta = new List<object>
                {
                    cipherIv,
                    kdfSalt,
                    PrivatebinConstants.CIPHER_ITERATION_COUNT,
                    PrivatebinConstants.CIPHER_BLOCK_BITS,
                    PrivatebinConstants.CIPHER_TAG_BITS,
                    "aes",
                    "cbc",
                    "none"
                };

                Adata.Add(paste_meta);
                Adata.Add("plaintext");
                Adata.Add(0);
                Adata.Add(0);
            }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(Adata);
            }
        }

        private class PrivatebinApiHelper
        {
            public PrivatebinPasteMetaStruct PrivatebinPasteMetaStruct { get; }
            public byte[] Iv { get; }
            public byte[] Salt { get; }
            public string PublicDecryptionKey { get; }
            private byte[] pastePasswordBytes;

            public PrivatebinApiHelper()
            {
                pastePasswordBytes = GetRandomBytes(PrivatebinConstants.CIPHER_BLOCK_BITS / 8);
                PublicDecryptionKey = Base58Converter.Encode(pastePasswordBytes);

                Iv = GetRandomBytes(PrivatebinConstants.CIPHER_TAG_BITS / 8);
                Salt = GetRandomBytes(8);
                PrivatebinPasteMetaStruct = new PrivatebinPasteMetaStruct(this);
            }

            public string EncryptMessage(string message, string pastePassword = null)
            {
                if (!string.IsNullOrEmpty(pastePassword))
                {
                    pastePasswordBytes = Combine(pastePasswordBytes, Encoding.UTF8.GetBytes(pastePassword));
                }

                var paste_data_json = new Dictionary<string, string>
                {
                    { "paste", message }
                };
                return CreateCipherText(JsonConvert.SerializeObject(paste_data_json));
            }

            private static byte[] Combine(params byte[][] arrays)
            {
                var rv = new byte[arrays.Sum(a => a.Length)];
                var offset = 0;
                foreach (byte[] array in arrays)
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
                using (var deriveBytes = new Rfc2898DeriveBytes(pastePasswordBytes, Salt, PrivatebinConstants.CIPHER_ITERATION_COUNT, HashAlgorithmName.SHA256))
                {
                    return deriveBytes.GetBytes(PrivatebinConstants.CIPHER_BLOCK_BITS / 8);
                }
            }


            private string CreateCipherText(string plainText)
            {
                return CreateCipherTextCBC(plainText);
            }


            private string CreateCipherTextCBC(string plainText)
            {
                byte[] encrypted;

                // Create an Aes object
                // with the specified key and IV.
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = DeriveKey();
                    aesAlg.IV = Iv;

                    aesAlg.Mode = CipherMode.CBC;

                    // Create an encryptor to perform the stream transform.
                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption.
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(plainText);
                            }
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }

                // Return the encrypted bytes from the memory stream.
                return Convert.ToBase64String(encrypted);
            }
        }
    }

    [DefaultValue(Week1)]
    public enum PrivatebinExpiration
    {
        [Description("5min")]
        Min5,
        [Description("10min")]
        Min10,
        [Description("1hour")]
        Hour1,
        [Description("1day")]
        Day1,
        [Description("1week")]
        Week1,
        [Description("1month")]
        Month1,
        [Description("1year")]
        Year1,
        [Description("never")]
        Never
    }

    public class PrivatebinSettings
    {
        public string Username { get; set; }
        [JsonEncrypt]
        public string Password { get; set; }
        public PrivatebinExpiration Expiration { get; set; } = PrivatebinExpiration.Week1;
        public string CustomUrl { get; set; } = "https://privatebin.net/";
        [JsonEncrypt]
        public string PastePassword { get; set; } = string.Empty;
    }
}