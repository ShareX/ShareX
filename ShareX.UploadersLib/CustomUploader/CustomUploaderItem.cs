#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2019 ShareX Team

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
using ShareX.UploadersLib.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;

namespace ShareX.UploadersLib
{
    public class CustomUploaderItem
    {
        [DefaultValue("")]
        public string Version { get; set; }

        [DefaultValue("")]
        public string Name { get; set; }

        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name) && Name != URLHelpers.GetHostName(RequestURL);

        [DefaultValue(CustomUploaderDestinationType.None)]
        public CustomUploaderDestinationType DestinationType { get; set; }

        [DefaultValue(HttpMethod.POST)]
        public HttpMethod RequestType { get; set; }

        [DefaultValue("")]
        public string RequestURL { get; set; }

        [DefaultValue(CustomUploaderRequestFormat.None)]
        public CustomUploaderRequestFormat RequestFormat { get; set; }

        [DefaultValue("")]
        public string FileFormName { get; set; }

        public bool ShouldSerializeFileFormName() => (RequestFormat == CustomUploaderRequestFormat.None && RequestType == HttpMethod.POST) ||
            RequestFormat == CustomUploaderRequestFormat.MultipartFormData;

        [DefaultValue("")]
        public string Data { get; set; }

        public bool ShouldSerializeData() => RequestFormat == CustomUploaderRequestFormat.JSON;

        [DefaultValue(null)]
        public Dictionary<string, string> Arguments { get; set; }

        public bool ShouldSerializeArguments() => Arguments != null && Arguments.Count > 0;

        [DefaultValue(null)]
        public Dictionary<string, string> Headers { get; set; }

        public bool ShouldSerializeHeaders() => Headers != null && Headers.Count > 0;

        [DefaultValue(ResponseType.Text)]
        public ResponseType ResponseType { get; set; }

        [DefaultValue(null)]
        public List<string> RegexList { get; set; }

        public bool ShouldSerializeRegexList() => RegexList != null && RegexList.Count > 0;

        [DefaultValue("")]
        public string URL { get; set; }

        [DefaultValue("")]
        public string ThumbnailURL { get; set; }

        [DefaultValue("")]
        public string DeletionURL { get; set; }

        public CustomUploaderItem()
        {
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }

            string name = URLHelpers.GetHostName(RequestURL);

            if (!string.IsNullOrEmpty(name))
            {
                return name;
            }

            return "Name";
        }

        public string GetFileName()
        {
            return ToString() + ".sxcu";
        }

        public string GetRequestURL(CustomUploaderInput input)
        {
            if (string.IsNullOrEmpty(RequestURL))
            {
                throw new Exception(Resources.CustomUploaderItem_GetRequestURL_RequestURLMustBeConfigured);
            }

            CustomUploaderParser parser = new CustomUploaderParser(input);
            parser.URLEncode = true;

            string url = parser.Parse(RequestURL);
            return URLHelpers.FixPrefix(url);
        }

        public string GetData(CustomUploaderInput input)
        {
            CustomUploaderParser parser = new CustomUploaderParser(input);
            parser.UseNameParser = true;
            parser.JSONEncode = RequestFormat == CustomUploaderRequestFormat.JSON;

            return parser.Parse(Data);
        }

        public string GetFileFormName()
        {
            if (string.IsNullOrEmpty(FileFormName))
            {
                throw new Exception(Resources.CustomUploaderItem_GetFileFormName_FileFormNameMustBeConfigured);
            }

            return FileFormName;
        }

        public Dictionary<string, string> GetArguments(CustomUploaderInput input)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            if (Arguments != null)
            {
                CustomUploaderParser parser = new CustomUploaderParser(input);
                parser.UseNameParser = true;

                foreach (KeyValuePair<string, string> arg in Arguments)
                {
                    arguments.Add(arg.Key, parser.Parse(arg.Value));
                }
            }

            return arguments;
        }

        public NameValueCollection GetHeaders(CustomUploaderInput input)
        {
            if (Headers != null && Headers.Count > 0)
            {
                NameValueCollection collection = new NameValueCollection();

                CustomUploaderParser parser = new CustomUploaderParser(input);
                parser.UseNameParser = true;

                foreach (KeyValuePair<string, string> header in Headers)
                {
                    collection.Add(header.Key, parser.Parse(header.Value));
                }

                return collection;
            }

            return null;
        }

        public void ParseResponse(UploadResult result, CustomUploaderInput input, bool isShortenedURL = false)
        {
            if (result != null && !string.IsNullOrEmpty(result.Response))
            {
                CustomUploaderParser parser = new CustomUploaderParser(result.Response, RegexList);
                parser.Filename = input.Filename;
                parser.URLEncode = true;

                string url;

                if (!string.IsNullOrEmpty(URL))
                {
                    url = parser.Parse(URL);
                }
                else
                {
                    url = parser.Response;
                }

                if (isShortenedURL)
                {
                    result.ShortenedURL = url;
                }
                else
                {
                    result.URL = url;
                }

                result.ThumbnailURL = parser.Parse(ThumbnailURL);
                result.DeletionURL = parser.Parse(DeletionURL);
            }
        }

        public void CheckBackwardCompatibility()
        {
            if (string.IsNullOrEmpty(Version) || Helpers.CompareVersion(Version, "12.3.1") <= 0)
            {
                if (RequestType == HttpMethod.POST)
                {
                    RequestFormat = CustomUploaderRequestFormat.MultipartFormData;
                }
                else
                {
                    RequestFormat = CustomUploaderRequestFormat.URLQueryString;
                }
            }

            Version = Application.ProductVersion;
        }
    }
}