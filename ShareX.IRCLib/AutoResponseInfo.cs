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

using ShareX.HelpersLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text.RegularExpressions;

namespace ShareX.IRCLib
{
    public class AutoResponseInfo
    {
        [Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> Messages { get; set; }

        [Editor("System.Windows.Forms.Design.StringCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> Responses { get; set; }

        [DefaultValue(IRCAutoResponseType.Contains)]
        public IRCAutoResponseType Type { get; set; }

        private Stopwatch lastMatchTimer = new Stopwatch();

        public AutoResponseInfo()
        {
            Messages = new List<string>();
            Responses = new List<string>();
            Type = IRCAutoResponseType.Contains;
        }

        public AutoResponseInfo(List<string> message, List<string> response, IRCAutoResponseType type)
        {
            Messages = message;
            Responses = response;
            Type = type;
        }

        public bool IsMatch(string message, string nick, string mynick)
        {
            bool isMatch = Messages.Select(m => m.Replace("$nick", nick).Replace("$mynick", mynick)).Any(x =>
            {
                switch (Type)
                {
                    default:
                    case IRCAutoResponseType.Contains:
                        return Regex.IsMatch(message, $"\\b{Regex.Escape(x)}\\b", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
                    case IRCAutoResponseType.StartsWith:
                        return message.StartsWith(x, StringComparison.InvariantCultureIgnoreCase);
                    case IRCAutoResponseType.ExactMatch:
                        return message.Equals(x, StringComparison.InvariantCultureIgnoreCase);
                }
            });

            if (isMatch)
            {
                lastMatchTimer.Restart();
            }

            return isMatch;
        }

        public bool CheckLastMatchTimer(int milliseconds)
        {
            return milliseconds <= 0 || !lastMatchTimer.IsRunning || lastMatchTimer.Elapsed.TotalMilliseconds > milliseconds;
        }

        public string RandomResponse(string nick, string mynick)
        {
            int index = MathHelpers.Random(Responses.Count - 1);
            return Responses.Select(r => r.Replace("$nick", nick).Replace("$mynick", mynick)).ElementAt(index);
        }

        public override string ToString()
        {
            if (Messages != null && Messages.Count > 0)
            {
                return string.Join(", ", Messages);
            }

            return "...";
        }
    }
}