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

using ShareX.HelpersLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace ShareX.IRCLib
{
    public class IRCInfo : SettingsBase<IRCInfo>
    {
        [Description("IRC server address."), DefaultValue("chat.freenode.net")]
        public string Server { get; set; } = "chat.freenode.net";

        [Description("IRC server port."), DefaultValue(6667)]
        public int Port { get; set; } = 6667;

        [Description("IRC server password. In some servers can be used to identify."), PasswordPropertyText(true)]
        public string Password { get; set; }

        [Description("Nickname.")]
        public string Nickname { get; set; }

        [Description("Username. This show up in WHOIS result."), DefaultValue("username")]
        public string Username { get; set; } = "username";

        [Description("Realname. This show up in WHOUS result."), DefaultValue("realname")]
        public string Realname { get; set; } = "realname";

        [Description("IRC invisible mode."), DefaultValue(true)]
        public bool Invisible { get; set; } = true;

        [Description("When disconnected from server auto reconnect."), DefaultValue(true)]
        public bool AutoReconnect { get; set; } = true;

        [Description("Wait specific milliseconds before reconnecting."), DefaultValue(5000)]
        public int AutoReconnectDelay { get; set; } = 5000;

        [Description("When got kicked from channel auto rejoin."), DefaultValue(false)]
        public bool AutoRejoinOnKick { get; set; }

        [Description("Don't show 'Message of the day' texts in output."), DefaultValue(true)]
        public bool SuppressMOTD { get; set; } = true;

        [Description("When you disconnect what message gonna show to other people."), DefaultValue("Leaving")]
        public string QuitReason { get; set; } = "Leaving";

        [Description("When connected these commands will automatically execute."),
        Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> ConnectCommands { get; set; } = new List<string>();

        [Description("When connected automatically join these channels."),
        TypeConverter(typeof(StringCollectionToStringTypeConverter)),
        Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> AutoJoinChannels { get; set; } = new List<string>() { "#ShareX" };

        [Description("Wait identify confirmation before auto join channels."), DefaultValue(false)]
        public bool AutoJoinWaitIdentify { get; set; }

        [Description("Enable/Disable auto response system which using AutoResponseList."), DefaultValue(false)]
        public bool AutoResponse { get; set; }

        [Description("After successful auto response match how much milliseconds wait for next auto response. Delay independant per response."), DefaultValue(10000)]
        public int AutoResponseDelay { get; set; } = 10000;

        [Description("When specific message written in channel automatically response with your message.")]
        public List<AutoResponseInfo> AutoResponseList { get; set; } = new List<AutoResponseInfo>();

        public string GetAutoResponses()
        {
            List<string> messages = new List<string>();

            foreach (AutoResponseInfo autoResponseInfo in AutoResponseList)
            {
                if (autoResponseInfo.Messages.Count > 1)
                {
                    messages.Add($"[{string.Join(", ", autoResponseInfo.Messages)}]");
                }
                else if (autoResponseInfo.Messages.Count == 1)
                {
                    messages.Add(autoResponseInfo.Messages[0]);
                }
            }

            return string.Join(", ", messages);
        }
    }
}