#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2026 ShareX Team

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
using System.Collections.Specialized;

namespace ShareX.UploadersLib
{
    internal static class RequestHelpers
    {
        public const string ContentTypeMultipartFormData = "multipart/form-data";
        public const string ContentTypeJSON = "application/json";
        public const string ContentTypeXML = "application/xml";
        public const string ContentTypeURLEncoded = "application/x-www-form-urlencoded";
        public const string ContentTypeOctetStream = "application/octet-stream";

        public static string CreateBoundary()
        {
            return new string('-', 20) + Guid.NewGuid().ToString("N");
        }

        public static NameValueCollection CreateAuthenticationHeader(string username, string password)
        {
            string authorization = TranslatorHelper.TextToBase64(username + ":" + password);
            NameValueCollection headers = new NameValueCollection();
            headers["Authorization"] = "Basic " + authorization;
            return headers;
        }
    }
}
