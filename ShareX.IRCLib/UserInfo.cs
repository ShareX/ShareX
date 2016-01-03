#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2016 ShareX Team

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

namespace ShareX.IRCLib
{
    public class UserInfo
    {
        public IRCUserType UserType { get; set; }

        public string Nickname { get; set; }
        public string Username { get; set; }
        public string Realname { get; set; }
        public string Host { get; set; }
        public string Identity { get; set; }
        public List<string> Channels { get; set; }
        public string Server { get; set; }
        public bool SecureConnection { get; set; }
        public TimeSpan IdleTime { get; set; }
        public DateTime SignOnDate { get; set; }

        public UserInfo()
        {
            Channels = new List<string>();
        }

        public override string ToString()
        {
            string channels = string.Join(" ", Channels);
            return $"Nickname: {Nickname}, Username: {Username}, Realname: {Realname}, Host: {Host}, Identity: {Identity}, Channels: {channels}";
        }
    }
}