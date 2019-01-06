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
using System.Linq;

namespace ShareX.UploadersLib.SharingServices
{
    public class CustomURLSharingService : URLSharingService
    {
        public override URLSharingServices EnumValue { get; } = URLSharingServices.CustomURLSharingService;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomURLSharingServiceSelected);
        }

        public override URLSharer CreateSharer(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            int index;

            if (taskInfo.OverrideCustomUploader)
            {
                index = taskInfo.CustomUploaderIndex.BetweenOrDefault(0, config.CustomUploadersList.Count - 1);
            }
            else
            {
                index = config.CustomURLSharingServiceSelected;
            }

            CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

            if (customUploader != null)
            {
                return new CustomURLSharer(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomURLSharer : URLSharer
    {
        private CustomUploaderItem uploader;

        public CustomURLSharer(CustomUploaderItem customUploaderItem)
        {
            uploader = customUploaderItem;
        }

        public override UploadResult ShareURL(string url)
        {
            UploadResult result = new UploadResult { URL = url, IsURLExpected = false };
            CustomUploaderInput input = new CustomUploaderInput("", url);

            if (uploader.RequestFormat == CustomUploaderRequestFormat.None)
            {
                result.Response = SendRequest(uploader.RequestType, uploader.GetRequestURL(input), null,
                    uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else if (uploader.RequestFormat == CustomUploaderRequestFormat.MultipartFormData)
            {
                result.Response = SendRequestMultiPart(uploader.GetRequestURL(input), uploader.GetArguments(input), uploader.GetHeaders(input), null,
                    uploader.ResponseType, uploader.RequestType);
            }
            else if (uploader.RequestFormat == CustomUploaderRequestFormat.JSON)
            {
                result.Response = SendRequest(uploader.RequestType, uploader.GetRequestURL(input), uploader.GetData(input), UploadHelpers.ContentTypeJSON,
                    null, uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else if (uploader.RequestFormat == CustomUploaderRequestFormat.FormURLEncoded)
            {
                result.Response = SendRequestURLEncoded(uploader.RequestType, uploader.GetRequestURL(input), uploader.GetArguments(input),
                    uploader.GetHeaders(input), null, uploader.ResponseType);
            }
            else
            {
                throw new Exception("Unsupported request format: " + uploader.RequestFormat);
            }

            return result;
        }
    }
}