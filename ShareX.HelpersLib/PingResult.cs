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

using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace ShareX.HelpersLib
{
    public class PingResult
    {
        public List<PingReply> PingReplyList { get; private set; }

        public int Min
        {
            get
            {
                return (int)PingReplyList.Where(x => x.Status == IPStatus.Success).Min(x => x.RoundtripTime);
            }
        }

        public int Max
        {
            get
            {
                return (int)PingReplyList.Where(x => x.Status == IPStatus.Success).Max(x => x.RoundtripTime);
            }
        }

        public int Average
        {
            get
            {
                return (int)PingReplyList.Where(x => x.Status == IPStatus.Success).Average(x => x.RoundtripTime);
            }
        }

        public PingResult()
        {
            PingReplyList = new List<PingReply>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (PingReply pingReply in PingReplyList)
            {
                if (pingReply != null)
                {
                    switch (pingReply.Status)
                    {
                        case IPStatus.Success:
                            sb.AppendLine(string.Format("Reply from {0}: bytes={1} time={2}ms TTL={3}", pingReply.Address, pingReply.Buffer.Length, pingReply.RoundtripTime, pingReply.Options.Ttl));
                            break;
                        case IPStatus.TimedOut:
                            sb.AppendLine("Request timed out.");
                            break;
                        default:
                            sb.AppendLine(string.Format("Ping failed: {0}", pingReply.Status.ToString()));
                            break;
                    }
                }
            }

            if (PingReplyList.Any(x => x.Status == IPStatus.Success))
            {
                sb.AppendLine(string.Format("Minimum = {0}ms, Maximum = {1}ms, Average = {2}ms", Min, Max, Average));
            }

            return sb.ToString().Trim();
        }
    }
}