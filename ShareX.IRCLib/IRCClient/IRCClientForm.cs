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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShareX.IRCLib
{
    public partial class IRCClientForm : Form
    {
        public IRCInfo Info { get; private set; }
        public IRC IRC { get; private set; }

        private TabManager tabManager;
        private string lastCommand, lastMessage;

        public IRCClientForm() : this(new IRCInfo())
        {
        }

        public IRCClientForm(IRCInfo info)
        {
            InitializeComponent();
            Icon = ShareXResources.Icon;
            ((ToolStripDropDownMenu)tsmiColors.DropDown).ShowImageMargin = false;

            tabManager = new TabManager(tcMessages);

            Info = info;
            pgSettings.SelectedObject = Info;

            IRC = new IRC(Info);
            IRC.Disconnected += IRC_Disconnected;
            IRC.Output += IRC_Output;
            IRC.Message += IRC_Message;
            IRC.UserJoined += IRC_UserJoined;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (IRC != null)
                {
                    IRC.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void AppendMessage(string message)
        {
            txtMessage.AppendTextToSelection(message);
            txtMessage.Focus();
        }

        private void SendCommand()
        {
            string command = txtCommand.Text;

            if (!string.IsNullOrEmpty(command))
            {
                lastCommand = command;

                if (IRC.IsConnected)
                {
                    SendCommand(command);
                }
            }

            txtCommand.Clear();
        }

        private void SendCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                if (command.StartsWith("/"))
                {
                    command = command.Substring(1);
                }

                IRC.SendRawMessage(command);
            }
        }

        private void SendMessage()
        {
            string message = txtMessage.Text;

            if (!string.IsNullOrEmpty(message))
            {
                lastMessage = message;

                if (IRC.IsConnected)
                {
                    SendMessage(message);
                }
            }

            txtMessage.Clear();
        }

        private void SendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message.StartsWith("/"))
                {
                    IRC.SendRawMessage(message.Substring(1));
                }
                else
                {
                    IRC.SendMessage(message, txtChannel.Text);
                }
            }
        }

        private void Connect()
        {
            if (CheckInfo())
            {
                btnConnect.Text = "Disconnect";
                btnCommandSend.Enabled = btnMessageSend.Enabled = true;
                tcMain.SelectedTab = tpOutput;
                IRC.Connect();
            }
        }

        private void Disconnect()
        {
            btnConnect.Text = "Connect";
            btnCommandSend.Enabled = btnMessageSend.Enabled = false;
            IRC.Disconnect();
        }

        private bool CheckInfo()
        {
            if (string.IsNullOrEmpty(Info.Server))
            {
                MessageBox.Show("Server field cannot be empty.", "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Info.Port <= 0)
            {
                MessageBox.Show("Invalid server port.", "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(Info.Nickname))
            {
                MessageBox.Show("Nickname field cannot be empty.", "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(Info.Username))
            {
                MessageBox.Show("Username field cannot be empty.", "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrEmpty(Info.Realname))
            {
                MessageBox.Show("Realname field cannot be empty.", "ShareX - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #region Form events

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!IRC.IsConnected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        private void txtCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendCommand();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up && !string.IsNullOrEmpty(lastCommand))
            {
                txtCommand.Text = lastCommand;
                txtCommand.SelectionStart = txtCommand.TextLength;
                e.SuppressKeyPress = true;
            }
        }

        private void btnCommandSend_Click(object sender, EventArgs e)
        {
            SendCommand();
        }

        private void tcMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabInfo tabInfo = tabManager.ActiveTab;

            if (tabInfo != null)
            {
                txtChannel.Text = tabInfo.Name;
            }
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendMessage();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Up && !string.IsNullOrEmpty(lastMessage))
            {
                txtMessage.Text = lastMessage;
                txtMessage.SelectionStart = txtMessage.TextLength;
                e.SuppressKeyPress = true;
            }
        }

        private void btnMessagesMenu_MouseClick(object sender, MouseEventArgs e)
        {
            cmsMessage.Show(btnMessagesMenu, new Point(0, btnMessagesMenu.Height + 1));
        }

        #region Message menu

        private void tsmiMessageBold_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Bold);
        }

        private void tsmiMessageItalic_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Italic);
        }

        private void tsmiMessageUnderline_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Underline);
        }

        private void tsmiMessageNormal_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Reset);
        }

        private void tsmiColorWhite_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.White));
        }

        private void tsmiColorBlack_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Black));
        }

        private void tsmiColorBlue_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Blue));
        }

        private void tsmiColorGreen_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Green));
        }

        private void tsmiColorLightRed_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.LightRed));
        }

        private void tsmiColorBrown_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Brown));
        }

        private void tsmiColorPurple_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Purple));
        }

        private void tsmiColorOrange_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Orange));
        }

        private void tsmiColorYellow_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Yellow));
        }

        private void tsmiColorLightGreen_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.LightGreen));
        }

        private void tsmiColorCyan_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Cyan));
        }

        private void tsmiColorLightCyan_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.LightCyan));
        }

        private void tsmiColorLightBlue_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.LightBlue));
        }

        private void tsmiColorPink_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Pink));
        }

        private void tsmiColorGrey_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.Grey));
        }

        private void tsmiColorLightGrey_Click(object sender, EventArgs e)
        {
            AppendMessage(IRCText.Color(IRCColors.LightGrey));
        }

        #endregion Message menu

        private void btnMessageSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        #endregion Form events

        #region IRC events

        private void IRC_Disconnected()
        {
            if (!Info.AutoReconnect)
            {
                Disconnect();
            }
        }

        private void IRC_Output(MessageInfo messageInfo)
        {
            this.InvokeSafe(() =>
            {
                txtOutput.AppendText($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {messageInfo.Content}\r\n");
            });
        }

        private void IRC_Message(UserInfo user, string channel, string message)
        {
            this.InvokeSafe(() =>
            {
                tabManager.AddMessage(channel, $"{DateTime.Now:HH:mm:ss} - {user.Nickname}: {message}");
            });
        }

        private void IRC_UserJoined(UserInfo user, string channel)
        {
            if (user.Nickname == Info.Nickname && user.Username == Info.Username)
            {
                this.InvokeSafe(() =>
                {
                    tabManager.AddChannel(channel);

                    if (string.IsNullOrEmpty(txtChannel.Text))
                    {
                        txtChannel.Text = channel;
                    }
                });
            }
        }

        #endregion IRC events
    }
}