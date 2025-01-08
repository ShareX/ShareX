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

namespace ShareX.UploadersLib.FileUploaders
{
    public static class SendSpaceManager
    {
        public static string Token;
        public static string SessionKey;
        public static DateTime LastSessionKey;
        public static AccountType AccountType;
        public static string Username;
        public static string Password;
        public static SendSpace.UploadInfo UploadInfo;

        public static UploaderErrorManager PrepareUploadInfo(string apiKey, string username = null, string password = null)
        {
            SendSpace sendSpace = new SendSpace(apiKey);

            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    AccountType = AccountType.Anonymous;

                    UploadInfo = sendSpace.AnonymousUploadGetInfo();
                    if (UploadInfo == null) throw new Exception("UploadInfo is null.");
                }
                else
                {
                    AccountType = AccountType.User;
                    Username = username;
                    Password = password;

                    if (string.IsNullOrEmpty(Token))
                    {
                        Token = sendSpace.AuthCreateToken();
                        if (string.IsNullOrEmpty(Token)) throw new Exception("Token is null or empty.");
                    }
                    if (string.IsNullOrEmpty(SessionKey) || (DateTime.Now - LastSessionKey).TotalMinutes > 30)
                    {
                        SessionKey = sendSpace.AuthLogin(Token, username, password).SessionKey;
                        if (string.IsNullOrEmpty(SessionKey)) throw new Exception("SessionKey is null or empty.");
                        LastSessionKey = DateTime.Now;
                    }
                    UploadInfo = sendSpace.UploadGetInfo(SessionKey);
                    if (UploadInfo == null) throw new Exception("UploadInfo is null.");
                }
            }
            catch (Exception e)
            {
                if (sendSpace.Errors.Count > 0)
                {
                    DebugHelper.WriteException(new Exception(sendSpace.ToErrorString()));
                }
                else
                {
                    DebugHelper.WriteException(e);
                }
            }

            return sendSpace.Errors;
        }
    }
}