﻿#region License Information (GPL v3)

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
using System.Linq;

namespace ShareX.UploadersLib.URLShorteners
{
    public class CustomURLShortenerService : URLShortenerService
    {
        public override UrlShortenerType EnumValue { get; } = UrlShortenerType.CustomURLShortener;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomURLShortenerSelected);
        }

        public override URLShortener CreateShortener(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            int index;

            if (taskInfo.OverrideCustomUploader)
            {
                index = taskInfo.CustomUploaderIndex.BetweenOrDefault(0, config.CustomUploadersList.Count - 1);
            }
            else
            {
                index = config.CustomURLShortenerSelected;
            }

            CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

            if (customUploader != null)
            {
                return new CustomURLShortener(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomURLShortener : URLShortener
    {
        private CustomUploaderItem uploader;

        public CustomURLShortener(CustomUploaderItem customUploaderItem)
        {
            uploader = customUploaderItem;
        }

        public override UploadResult ShortenURL(string url)
        {
            UploadResult result = new UploadResult { URL = url };
            CustomUploaderInput input = new CustomUploaderInput("", url);

            CustomUploaderRequestFormat requestFormat = uploader.GetRequestFormat(CustomUploaderDestinationType.URLShortener);

            if (requestFormat == CustomUploaderRequestFormat.MultipartFormData)
            {
                result.Response = SendRequestMultiPart(uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input), null,
                    uploader.ResponseType, uploader.RequestType);
            }
            else if (requestFormat == CustomUploaderRequestFormat.URLQueryString)
            {
                result.Response = SendRequest(uploader.RequestType, uploader.GetRequestURL(input), uploader.GetArguments(input),
                    uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else if (requestFormat == CustomUploaderRequestFormat.JSON)
            {
                result.Response = SendRequest(uploader.RequestType, uploader.GetRequestURL(input), uploader.GetData(input), UploadHelpers.ContentTypeJSON,
                    uploader.GetArguments(input), uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else if (requestFormat == CustomUploaderRequestFormat.FormURLEncoded)
            {
                result.Response = SendRequestURLEncoded(uploader.RequestType, uploader.GetRequestURL(input), uploader.GetArguments(input),
                    uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else
            {
                throw new Exception("Unsupported request format.");
            }

            try
            {
                uploader.ParseResponse(result, input, true);
            }
            catch (Exception e)
            {
                Errors.Add(Resources.CustomFileUploader_Upload_Response_parse_failed_ + Environment.NewLine + e);
            }

            return result;
        }
    }
}