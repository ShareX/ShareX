#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

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

namespace ShareX.UploadersLib
{
    public class CustomUploaderItem
    {
        [DefaultValue("")]
        public string Name { get; set; }

        public bool ShouldSerializeName() => !string.IsNullOrEmpty(Name) && Name != URLHelpers.GetHostName(RequestURL);

        [DefaultValue(CustomUploaderDestinationType.None)]
        public CustomUploaderDestinationType DestinationType { get; set; }

        [DefaultValue(CustomUploaderRequestType.POST)]
        public CustomUploaderRequestType RequestType { get; set; }

        [DefaultValue("")]
        public string RequestURL { get; set; }

        [DefaultValue("")]
        public string FileFormName { get; set; }

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

        public HttpMethod GetHttpMethod()
        {
            switch (RequestType)
            {
                default:
                case CustomUploaderRequestType.POST:
                    return HttpMethod.POST;
                case CustomUploaderRequestType.GET:
                    return HttpMethod.GET;
                case CustomUploaderRequestType.PUT:
                    return HttpMethod.PUT;
                case CustomUploaderRequestType.PATCH:
                    return HttpMethod.PATCH;
                case CustomUploaderRequestType.DELETE:
                    return HttpMethod.DELETE;
            }
        }

        public string GetRequestURL()
        {
            if (string.IsNullOrEmpty(RequestURL))
            {
                throw new Exception(Resources.CustomUploaderItem_GetRequestURL_RequestURLMustBeConfigured);
            }

            CustomUploaderParser parser = new CustomUploaderParser();
            string url = parser.Parse(RequestURL);
            return URLHelpers.FixPrefix(url);
        }

        public string GetFileFormName()
        {
            if (string.IsNullOrEmpty(FileFormName))
            {
                throw new Exception(Resources.CustomUploaderItem_GetFileFormName_FileFormNameMustBeConfigured);
            }

            return FileFormName;
        }

        public Dictionary<string, string> GetArguments(CustomUploaderArgumentInput input)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            if (Arguments != null)
            {
                foreach (KeyValuePair<string, string> arg in Arguments)
                {
                    arguments.Add(arg.Key, input.Parse(arg.Value));
                }
            }

            return arguments;
        }

        public NameValueCollection GetHeaders(CustomUploaderArgumentInput input)
        {
            if (Headers != null && Headers.Count > 0)
            {
                NameValueCollection collection = new NameValueCollection();

                foreach (KeyValuePair<string, string> header in Headers)
                {
                    collection.Add(header.Key, input.Parse(header.Value));
                }

                return collection;
            }

            return null;
        }

        public void ParseResponse(UploadResult result, bool isShortenedURL = false)
        {
            if (result != null && !string.IsNullOrEmpty(result.Response))
            {
                CustomUploaderParser parser = new CustomUploaderParser(result.Response, RegexList);

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
    }
}