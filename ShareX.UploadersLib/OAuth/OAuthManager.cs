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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ShareX.UploadersLib
{
    public static class OAuthManager
    {
        private const string ParameterConsumerKey = "oauth_consumer_key";
        private const string ParameterSignatureMethod = "oauth_signature_method";
        private const string ParameterSignature = "oauth_signature";
        private const string ParameterTimestamp = "oauth_timestamp";
        private const string ParameterNonce = "oauth_nonce";
        private const string ParameterVersion = "oauth_version";
        private const string ParameterToken = "oauth_token";
        private const string ParameterTokenSecret = "oauth_token_secret";
        private const string ParameterVerifier = "oauth_verifier";
        internal const string ParameterCallback = "oauth_callback";

        private const string PlainTextSignatureType = "PLAINTEXT";
        private const string HMACSHA1SignatureType = "HMAC-SHA1";
        private const string RSASHA1SignatureType = "RSA-SHA1";

        public static string GenerateQuery(string url, Dictionary<string, string> args, HttpMethod httpMethod, OAuthInfo oauth)
        {
            return GenerateQuery(url, args, httpMethod, oauth, out _);
        }

        public static string GenerateQuery(string url, Dictionary<string, string> args, HttpMethod httpMethod, OAuthInfo oauth, out Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(oauth.ConsumerKey) ||
                (oauth.SignatureMethod == OAuthInfo.OAuthInfoSignatureMethod.HMAC_SHA1 && string.IsNullOrEmpty(oauth.ConsumerSecret)) ||
                (oauth.SignatureMethod == OAuthInfo.OAuthInfoSignatureMethod.RSA_SHA1 && string.IsNullOrEmpty(oauth.ConsumerPrivateKey)))
            {
                throw new Exception("ConsumerKey or ConsumerSecret or ConsumerPrivateKey empty.");
            }

            parameters = new Dictionary<string, string>();
            parameters.Add(ParameterVersion, oauth.OAuthVersion);
            parameters.Add(ParameterNonce, GenerateNonce());
            parameters.Add(ParameterTimestamp, GenerateTimestamp());
            parameters.Add(ParameterConsumerKey, oauth.ConsumerKey);
            switch (oauth.SignatureMethod)
            {
                case OAuthInfo.OAuthInfoSignatureMethod.HMAC_SHA1:
                    parameters.Add(ParameterSignatureMethod, HMACSHA1SignatureType);
                    break;
                case OAuthInfo.OAuthInfoSignatureMethod.RSA_SHA1:
                    parameters.Add(ParameterSignatureMethod, RSASHA1SignatureType);
                    break;
                default:
                    throw new NotImplementedException("Unsupported signature method");
            }

            string secret = null;

            if (!string.IsNullOrEmpty(oauth.UserToken) && !string.IsNullOrEmpty(oauth.UserSecret))
            {
                secret = oauth.UserSecret;
                parameters.Add(ParameterToken, oauth.UserToken);
            }
            else if (!string.IsNullOrEmpty(oauth.AuthToken) && !string.IsNullOrEmpty(oauth.AuthSecret))
            {
                secret = oauth.AuthSecret;
                parameters.Add(ParameterToken, oauth.AuthToken);

                if (!string.IsNullOrEmpty(oauth.AuthVerifier))
                {
                    parameters.Add(ParameterVerifier, oauth.AuthVerifier);
                }
            }

            if (args != null)
            {
                foreach (KeyValuePair<string, string> arg in args)
                {
                    parameters[arg.Key] = arg.Value;
                }
            }

            string normalizedUrl = NormalizeUrl(url);
            string normalizedParameters = NormalizeParameters(parameters);
            string signatureBase = GenerateSignatureBase(httpMethod, normalizedUrl, normalizedParameters);
            byte[] signatureData;
            switch (oauth.SignatureMethod)
            {
                case OAuthInfo.OAuthInfoSignatureMethod.HMAC_SHA1:
                    signatureData = GenerateSignature(signatureBase, oauth.ConsumerSecret, secret);
                    break;
                case OAuthInfo.OAuthInfoSignatureMethod.RSA_SHA1:
                    signatureData = GenerateSignatureRSASHA1(signatureBase, oauth.ConsumerPrivateKey);
                    break;
                default:
                    throw new NotImplementedException("Unsupported signature method");
            }

            string signature = Convert.ToBase64String(signatureData);
            parameters[ParameterSignature] = signature;

            return string.Format("{0}?{1}&{2}={3}", normalizedUrl, normalizedParameters, ParameterSignature, URLHelpers.URLEncode(signature));
        }

        public static string GetAuthorizationURL(string requestTokenResponse, OAuthInfo oauth, string authorizeURL, string callback = null)
        {
            string url = null;

            NameValueCollection args = HttpUtility.ParseQueryString(requestTokenResponse);

            if (args[ParameterToken] != null)
            {
                oauth.AuthToken = args[ParameterToken];
                url = string.Format("{0}?{1}={2}", authorizeURL, ParameterToken, oauth.AuthToken);

                if (!string.IsNullOrEmpty(callback))
                {
                    url += string.Format("&{0}={1}", ParameterCallback, URLHelpers.URLEncode(callback));
                }

                if (args[ParameterTokenSecret] != null)
                {
                    oauth.AuthSecret = args[ParameterTokenSecret];
                }
            }

            return url;
        }

        public static NameValueCollection ParseAccessTokenResponse(string accessTokenResponse, OAuthInfo oauth)
        {
            NameValueCollection args = HttpUtility.ParseQueryString(accessTokenResponse);

            if (args != null && args[ParameterToken] != null)
            {
                oauth.UserToken = args[ParameterToken];

                if (args[ParameterTokenSecret] != null)
                {
                    oauth.UserSecret = args[ParameterTokenSecret];

                    return args;
                }
            }

            return null;
        }

        private static string GenerateSignatureBase(HttpMethod httpMethod, string normalizedUrl, string normalizedParameters)
        {
            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToString());
            signatureBase.AppendFormat("{0}&", URLHelpers.URLEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", URLHelpers.URLEncode(normalizedParameters));
            return signatureBase.ToString();
        }

        private static byte[] GenerateSignature(string signatureBase, string consumerSecret, string userSecret = null)
        {
            using (HMACSHA1 hmacsha1 = new HMACSHA1())
            {
                string key = string.Format("{0}&{1}", Uri.EscapeDataString(consumerSecret),
                    string.IsNullOrEmpty(userSecret) ? "" : Uri.EscapeDataString(userSecret));

                hmacsha1.Key = Encoding.ASCII.GetBytes(key);

                byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
                return hmacsha1.ComputeHash(dataBuffer);
            }
        }

        private static byte[] GenerateSignatureRSASHA1(string signatureBase, string privateKey)
        {
            byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);

            using (SHA1CryptoServiceProvider sha1 = GenerateSha1Hash(dataBuffer))
            using (AsymmetricAlgorithm algorithm = new RSACryptoServiceProvider())
            {
                algorithm.FromXmlString(privateKey);
                RSAPKCS1SignatureFormatter formatter = new RSAPKCS1SignatureFormatter(algorithm);
                formatter.SetHashAlgorithm("MD5");
                return formatter.CreateSignature(sha1);
            }
        }

        private static SHA1CryptoServiceProvider GenerateSha1Hash(byte[] dataBuffer)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            using (CryptoStream cs = new CryptoStream(Stream.Null, sha1, CryptoStreamMode.Write))
            {
                cs.Write(dataBuffer, 0, dataBuffer.Length);
            }

            return sha1;
        }

        private static string GenerateTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private static string GenerateNonce()
        {
            return Helpers.GetRandomAlphanumeric(12);
        }

        private static string NormalizeUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                string port = "";

                if ((uri.Scheme == "http" && uri.Port != 80) ||
                    (uri.Scheme == "https" && uri.Port != 443) ||
                    (uri.Scheme == "ftp" && uri.Port != 20))
                {
                    port = ":" + uri.Port;
                }

                url = uri.Scheme + "://" + uri.Host + port + uri.AbsolutePath;
            }

            return url;
        }

        private static string NormalizeParameters(Dictionary<string, string> parameters)
        {
            return string.Join("&", parameters.OrderBy(x => x.Key).ThenBy(x => x.Value).Select(x => x.Key + "=" + URLHelpers.URLEncode(x.Value)).ToArray());
        }
    }
}