using System;
using System.Security.Cryptography;
using System.Text;

namespace ShareX.UploadersLib
{
    public enum OAuth2ChallengeMethod
    {
        Plain, SHA256
    }

    public class OAuth2ProofKey
    {
        public string CodeVerifier { get; private set; }
        public string CodeChallenge { get; private set; }
        private OAuth2ChallengeMethod Method;
        public string ChallengeMethod
        {
            get
            {
                switch (Method)
                {
                    case OAuth2ChallengeMethod.Plain: return "plain";
                    case OAuth2ChallengeMethod.SHA256: return "S256";
                }
                return "";
            }
        }

        public OAuth2ProofKey(OAuth2ChallengeMethod method)
        {
            Method = method;

            var buffer = new byte[32];

            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buffer);

            CodeVerifier = CleanBase64(buffer);
            CodeChallenge = CodeVerifier;

            if (Method == OAuth2ChallengeMethod.SHA256)
            {
                var sha = new SHA256Managed();
                sha.ComputeHash(Encoding.UTF8.GetBytes(CodeVerifier));

                CodeChallenge = CleanBase64(sha.Hash);
            }
        }

        private string CleanBase64(byte[] buffer)
        {
            var sb = new StringBuilder(Convert.ToBase64String(buffer));
            sb.Replace('+', '-');
            sb.Replace('/', '_');
            sb.Replace("=", "");
            return sb.ToString();
        }
    }
}