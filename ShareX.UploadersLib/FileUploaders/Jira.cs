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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace ShareX.UploadersLib.FileUploaders
{
    public class JiraFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.Jira;

        public override Image ServiceImage => Resources.jira;

        public override bool CheckConfig(UploadersConfig config)
        {
            return OAuthInfo.CheckOAuth(config.JiraOAuthInfo);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            return new Jira(config.JiraHost, config.JiraOAuthInfo, config.JiraIssuePrefix);
        }

        public override TabPage GetUploadersConfigTabPage(UploadersConfigForm form) => form.tpJira;
    }

    public class Jira : FileUploader, IOAuth
    {
        private const string PathRequestToken = "/plugins/servlet/oauth/request-token";
        private const string PathAuthorize = "/plugins/servlet/oauth/authorize";
        private const string PathAccessToken = "/plugins/servlet/oauth/access-token";
        private const string PathApi = "/rest/api/2";
        private const string PathSearch = PathApi + "/search";
        private const string PathBrowseIssue = "/browse/{0}";
        private const string PathIssueAttachments = PathApi + "/issue/{0}/attachments";

        private static readonly X509Certificate2 jiraCertificate;

        public OAuthInfo AuthInfo { get; set; }

        private readonly string jiraBaseAddress;
        private readonly string jiraIssuePrefix;

        private Uri jiraRequestToken;
        private Uri jiraAuthorize;
        private Uri jiraAccessToken;
        private Uri jiraPathSearch;

        #region Keypair

        static Jira()
        {
            // Certificate generated using commands:
            // makecert -pe -n "CN=ShareX" -a sha1 -sky exchange -sp "Microsoft RSA SChannel Cryptographic Provider" -sy 12 -len 1024 -sv jira_sharex.pvk jira_sharex.cer
            // pvk2pfx -pvk jira_sharex.pvk -spc jira_sharex.cer -pfx jira_sharex.pfx
            // (Based on: http://nick-howard.blogspot.fr/2011/05/makecert-x509-certificates-and-rsa.html)
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ShareX.UploadersLib.APIKeys.jira_sharex.pfx"))
            {
                byte[] pfx = new byte[stream.Length];
                stream.Read(pfx, 0, pfx.Length);
                jiraCertificate = new X509Certificate2(pfx, "", X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
            }
        }

        internal static string PrivateKey
        {
            get
            {
                return jiraCertificate.PrivateKey.ToXmlString(true);
            }
        }

        internal static string PublicKey
        {
            get
            {
                const int LineBreakIdx = 50;

                string publicKey = Convert.ToBase64String(ExportPublicKey(jiraCertificate.PublicKey));
                int idx = 0;
                StringBuilder sb = new StringBuilder();
                foreach (char c in publicKey)
                {
                    sb.Append(c);
                    if ((++idx % LineBreakIdx) == 0)
                    {
                        sb.AppendLine();
                    }
                }

                return string.Join(Environment.NewLine, new[]
                {
                    "-----BEGIN PUBLIC KEY-----",
                    sb.ToString(),
                    "-----END PUBLIC KEY-----"
                });
            }
        }

        private static byte[] ExportPublicKey(PublicKey key)
        {
            // From: http://pstaev.blogspot.fr/2010/08/convert-rsa-public-key-from-xml-to-pem.html

            byte[] oid = { 0x30, 0xD, 0x6, 0x9, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0xD, 0x1, 0x1, 0x1, 0x5, 0x0 }; // Object ID for RSA

            //Transform the public key to PEM Base64 Format
            List<byte> binaryPublicKey = key.EncodedKeyValue.RawData.ToList();
            binaryPublicKey.Insert(0, 0x0); // Add NULL value

            CalculateAndAppendLength(ref binaryPublicKey);

            binaryPublicKey.Insert(0, 0x3);
            binaryPublicKey.InsertRange(0, oid);

            CalculateAndAppendLength(ref binaryPublicKey);

            binaryPublicKey.Insert(0, 0x30);
            return binaryPublicKey.ToArray();
        }

        private static void CalculateAndAppendLength(ref List<byte> binaryData)
        {
            int len = binaryData.Count;
            if (len <= byte.MaxValue)
            {
                binaryData.Insert(0, Convert.ToByte(len));
                binaryData.Insert(0, 0x81); //This byte means that the length fits in one byte
            }
            else
            {
                binaryData.Insert(0, Convert.ToByte(len % (byte.MaxValue + 1)));
                binaryData.Insert(0, Convert.ToByte(len / (byte.MaxValue + 1)));
                binaryData.Insert(0, 0x82); //This byte means that the length fits in two byte
            }
        }

        #endregion Keypair

        public Jira(string jiraBaseAddress, OAuthInfo oauth, string jiraIssuePrefix = null)
        {
            this.jiraBaseAddress = jiraBaseAddress;
            AuthInfo = oauth;
            this.jiraIssuePrefix = jiraIssuePrefix;

            InitUris();
        }

        public string GetAuthorizationURL()
        {
            using (new SSLBypassHelper())
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args[OAuthManager.ParameterCallback] = "oob"; // Request activation code to validate authentication

                string url = OAuthManager.GenerateQuery(jiraRequestToken.ToString(), args, HttpMethod.POST, AuthInfo);

                string response = SendRequest(HttpMethod.POST, url);

                if (!string.IsNullOrEmpty(response))
                {
                    return OAuthManager.GetAuthorizationURL(response, AuthInfo, jiraAuthorize.ToString());
                }

                return null;
            }
        }

        public bool GetAccessToken(string verificationCode)
        {
            using (new SSLBypassHelper())
            {
                AuthInfo.AuthVerifier = verificationCode;

                NameValueCollection nv = GetAccessTokenEx(jiraAccessToken.ToString(), AuthInfo, HttpMethod.POST);

                return nv != null;
            }
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            using (new SSLBypassHelper())
            {
                using (JiraUpload up = new JiraUpload(jiraIssuePrefix, GetSummary))
                {
                    if (up.ShowDialog() == DialogResult.Cancel)
                    {
                        return new UploadResult
                        {
                            IsSuccess = true,
                            IsURLExpected = false
                        };
                    }

                    Uri uri = Combine(jiraBaseAddress, string.Format(PathIssueAttachments, up.IssueId));
                    string query = OAuthManager.GenerateQuery(uri.ToString(), null, HttpMethod.POST, AuthInfo);

                    NameValueCollection headers = new NameValueCollection();
                    headers.Set("X-Atlassian-Token", "nocheck");

                    UploadResult res = SendRequestFile(query, stream, fileName, "file", headers: headers);
                    if (res.Response.Contains("errorMessages"))
                    {
                        Errors.Add(res.Response);
                    }
                    else
                    {
                        res.IsURLExpected = true;
                        var anonType = new[] { new { thumbnail = "" } };
                        var anonObject = JsonConvert.DeserializeAnonymousType(res.Response, anonType);
                        res.ThumbnailURL = anonObject[0].thumbnail;
                        res.URL = Combine(jiraBaseAddress, string.Format(PathBrowseIssue, up.IssueId)).ToString();
                    }

                    return res;
                }
            }
        }

        private string GetSummary(string issueId)
        {
            using (new SSLBypassHelper())
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args["jql"] = string.Format("issueKey='{0}'", issueId);
                args["maxResults"] = "10";
                args["fields"] = "summary";
                string query = OAuthManager.GenerateQuery(jiraPathSearch.ToString(), args, HttpMethod.GET, AuthInfo);

                string response = SendRequest(HttpMethod.GET, query);
                if (!string.IsNullOrEmpty(response))
                {
                    var anonType = new { issues = new[] { new { key = "", fields = new { summary = "" } } } };
                    var res = JsonConvert.DeserializeAnonymousType(response, anonType);
                    return res.issues[0].fields.summary;
                }

                // This query can returns error so we have to remove last error from errors list
                Errors.Errors.RemoveAt(Errors.Count - 1);

                return null;
            }
        }

        private void InitUris()
        {
            jiraRequestToken = Combine(jiraBaseAddress, PathRequestToken);
            jiraAuthorize = Combine(jiraBaseAddress, PathAuthorize);
            jiraAccessToken = Combine(jiraBaseAddress, PathAccessToken);
            jiraPathSearch = Combine(jiraBaseAddress, PathSearch);
        }

        private Uri Combine(string path1, string path2)
        {
            return new Uri(path1.TrimEnd('/') + path2);
        }
    }
}