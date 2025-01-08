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
using System.Net;
using System.Text;

namespace ShareX.UploadersLib
{
    public class ResponseInfo
    {
        public HttpStatusCode StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public bool IsSuccess => WebHelpers.IsSuccessStatusCode(StatusCode);
        public string ResponseURL { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public string ResponseText { get; set; }

        public string ToReadableString(bool includeResponseText)
        {
            StringBuilder sbResponseInfo = new StringBuilder();

            sbResponseInfo.AppendLine("Status code:");
            sbResponseInfo.Append($"({(int)StatusCode}) {StatusDescription}");

            if (!string.IsNullOrEmpty(ResponseURL))
            {
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine("Response URL:");
                sbResponseInfo.Append(ResponseURL);
            }

            if (Headers != null && Headers.Count > 0)
            {
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine("Headers:");
                sbResponseInfo.Append(Headers.ToString().TrimEnd());
            }

            if (includeResponseText && !string.IsNullOrEmpty(ResponseText))
            {
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine();
                sbResponseInfo.AppendLine("Response text:");
                sbResponseInfo.Append(ResponseText);
            }

            return sbResponseInfo.ToString();
        }
    }
}