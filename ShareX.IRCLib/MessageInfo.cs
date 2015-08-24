#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2015 ShareX Team

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
using System.Collections.Generic;
using System.Text;

namespace ShareX.IRCLib
{
    public class MessageInfo
    {
        public string Content { get; private set; }
        public UserInfo User { get; private set; }
        public string Command { get; private set; }
        public List<string> Parameters { get; private set; }
        public string Message { get; private set; }

        public MessageInfo(string content)
        {
            Content = content;
            User = new UserInfo();
            Parameters = new List<string>();
        }

        public static MessageInfo Parse(string content)
        {
            MessageInfo messageInfo = new MessageInfo(content);

            int index;
            string nickname = ParseSection(content, out index);

            // Is it not my message?
            if (nickname.StartsWith(":"))
            {
                nickname = nickname.Substring(1);
                int usernameIndex = nickname.IndexOf("!", StringComparison.Ordinal);

                // Is it not server?
                if (usernameIndex > -1)
                {
                    //nickname!~username@host
                    messageInfo.User.UserType = IRCUserType.User;
                    messageInfo.User.Nickname = nickname.Remove(usernameIndex);

                    nickname = nickname.Substring(usernameIndex + 1);

                    if (nickname[0] == '~') // Remove Ident character
                    {
                        nickname = nickname.Substring(1);
                    }

                    int hostIndex = nickname.IndexOf("@", StringComparison.Ordinal);
                    messageInfo.User.Username = nickname.Remove(hostIndex);
                    messageInfo.User.Host = nickname.Substring(hostIndex + 1);
                }
                else
                {
                    //irc.freenode.net
                    messageInfo.User.UserType = IRCUserType.Server;
                    messageInfo.User.Host = nickname;
                }

                content = content.Substring(index + 1);
                messageInfo.Command = ParseSection(content, out index);
            }
            else
            {
                // It is my command
                messageInfo.User.UserType = IRCUserType.Me;
                messageInfo.Command = nickname;
            }

            while (index > -1)
            {
                content = content.Substring(index + 1);
                string check = ParseSection(content, out index);

                // Is it parameter?
                if (!check.StartsWith(":"))
                {
                    messageInfo.Parameters.Add(check);
                }
                else
                {
                    // It is message
                    messageInfo.Message = content.Substring(1);
                    break;
                }
            }

            return messageInfo;
        }

        private static string ParseSection(string message, out int index)
        {
            index = message.IndexOf(' ');

            if (index > -1)
            {
                return message.Remove(index);
            }

            return message;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Type: {0}, ", User.UserType);

            switch (User.UserType)
            {
                case IRCUserType.User:
                    sb.AppendFormat("Nickname: {0}, Username: {1}, Host: {2}, ", User.Nickname, User.Username, User.Host);
                    break;
                case IRCUserType.Server:
                    sb.AppendFormat("Host: {0}, ", User.Host);
                    break;
            }

            if (!string.IsNullOrEmpty(Command))
            {
                sb.AppendFormat("Command: {0}, ", Command);
            }

            foreach (string parameter in Parameters)
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    sb.AppendFormat("Parameter: {0}, ", parameter);
                }
            }

            if (!string.IsNullOrEmpty(Message))
            {
                sb.AppendFormat("Message: {0}", Message);
            }

            return sb.ToString();
        }
    }
}