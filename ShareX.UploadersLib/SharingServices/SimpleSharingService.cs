using System.ComponentModel;
using ShareX.HelpersLib;

namespace ShareX.UploadersLib.SharingServices
{
    /// <summary>
    /// Base class for services that just open a share dialog in a browser
    /// </summary>
    public abstract class SimpleSharingService : SharingService
    {
        /// <summary>
        /// A string formatted URL that opens a share dialog
        /// </summary>
        /// <value>a string with a one placeholder for the URL to share</value>
        [Localizable(false)]
        protected abstract string UrlFormatString { get; }

        public override void ShareURL(string url, UploadersConfig uploadersConfig)
        {
            string encodedUrl = URLHelpers.URLEncode(url);
            URLHelpers.OpenURL(string.Format(UrlFormatString, encodedUrl));
        }

        public override bool CheckConfig(UploadersConfig uploadersConfig) => true;
    }
}