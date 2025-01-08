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
using System.Text;

namespace ShareX.UploadersLib
{
    public class UploadResult
    {
        public string URL { get; set; }
        public string ThumbnailURL { get; set; }
        public string DeletionURL { get; set; }
        public string ShortenedURL { get; set; }

        private bool isSuccess;

        public bool IsSuccess
        {
            get
            {
                return isSuccess && !string.IsNullOrEmpty(Response);
            }
            set
            {
                isSuccess = value;
            }
        }

        public string Response { get; set; }
        public UploaderErrorManager Errors { get; set; }
        public bool IsURLExpected { get; set; }

        public bool IsError
        {
            get
            {
                return Errors != null && Errors.Count > 0 && (!IsURLExpected || string.IsNullOrEmpty(URL));
            }
        }

        public ResponseInfo ResponseInfo { get; set; }

        public UploadResult()
        {
            Errors = new UploaderErrorManager();
            IsURLExpected = true;
        }

        public UploadResult(string source, string url = null) : this()
        {
            Response = source;
            URL = url;
        }

        public void ForceHTTPS()
        {
            URL = URLHelpers.ForcePrefix(URL);
            ThumbnailURL = URLHelpers.ForcePrefix(ThumbnailURL);
            DeletionURL = URLHelpers.ForcePrefix(DeletionURL);
            ShortenedURL = URLHelpers.ForcePrefix(ShortenedURL);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(ShortenedURL))
            {
                return ShortenedURL;
            }

            if (!string.IsNullOrEmpty(URL))
            {
                return URL;
            }

            return "";
        }

        public string ErrorsToString()
        {
            if (IsError)
            {
                return Errors.ToString();
            }

            return null;
        }

        public string ToSummaryString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("URL: " + URL);
            sb.AppendLine("Thumbnail URL: " + ThumbnailURL);
            sb.AppendLine("Shortened URL: " + ShortenedURL);
            sb.AppendLine("Deletion URL: " + DeletionURL);
            return sb.ToString();
        }
    }
}