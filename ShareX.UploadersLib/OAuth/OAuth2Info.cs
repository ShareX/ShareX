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

namespace ShareX.UploadersLib
{
    public class OAuth2Info
    {
        public string Client_ID { get; set; }
        public string Client_Secret { get; set; }
        public OAuth2Token Token { get; set; }
        public OAuth2ProofKey Proof { get; set; }

        public OAuth2Info(string client_id, string client_secret)
        {
            Client_ID = client_id;
            Client_Secret = client_secret;
        }

        public static bool CheckOAuth(OAuth2Info oauth)
        {
            return oauth != null && !string.IsNullOrEmpty(oauth.Client_ID) && !string.IsNullOrEmpty(oauth.Client_Secret) &&
                oauth.Token != null && !string.IsNullOrEmpty(oauth.Token.access_token);
        }
    }
}