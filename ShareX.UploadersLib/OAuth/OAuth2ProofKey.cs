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

            byte[] buffer = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            CodeVerifier = CleanBase64(buffer);
            CodeChallenge = CodeVerifier;

            if (Method == OAuth2ChallengeMethod.SHA256)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    sha.ComputeHash(Encoding.UTF8.GetBytes(CodeVerifier));
                    CodeChallenge = CleanBase64(sha.Hash);
                }
            }
        }

        private string CleanBase64(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder(Convert.ToBase64String(buffer));
            sb.Replace('+', '-');
            sb.Replace('/', '_');
            sb.Replace("=", "");
            return sb.ToString();
        }
    }
}