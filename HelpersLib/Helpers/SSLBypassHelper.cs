using System;

namespace HelpersLib
{
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;

    public class SSLBypassHelper : IDisposable
    {
        public SSLBypassHelper()
        {
            ServicePointManager.ServerCertificateValidationCallback += this.ServerCertificateValidationCallback;
        }

        public void Dispose()
        {
            ServicePointManager.ServerCertificateValidationCallback -= this.ServerCertificateValidationCallback;
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
