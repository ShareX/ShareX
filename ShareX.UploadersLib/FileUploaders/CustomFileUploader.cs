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
using ShareX.UploadersLib.Properties;
using System;
using System.Drawing;
using System.IO;

namespace ShareX.UploadersLib.FileUploaders
{
    public class CustomFileUploaderService : FileUploaderService
    {
        public override FileDestination EnumValue { get; } = FileDestination.CustomFileUploader;

        public override Image ServiceImage => Resources.globe_network;

        public override bool CheckConfig(UploadersConfig config)
        {
            return config.CustomUploadersList != null && config.CustomUploadersList.IsValidIndex(config.CustomFileUploaderSelected);
        }

        public override GenericUploader CreateUploader(UploadersConfig config, TaskReferenceHelper taskInfo)
        {
            int index;

            if (taskInfo.OverrideCustomUploader)
            {
                index = taskInfo.CustomUploaderIndex.BetweenOrDefault(0, config.CustomUploadersList.Count - 1);
            }
            else
            {
                index = config.CustomFileUploaderSelected;
            }

            CustomUploaderItem customUploader = config.CustomUploadersList.ReturnIfValidIndex(index);

            if (customUploader != null)
            {
                return new CustomFileUploader(customUploader);
            }

            return null;
        }
    }

    public sealed class CustomFileUploader : FileUploader
    {
        private CustomUploaderItem uploader;

        public CustomFileUploader(CustomUploaderItem customUploaderItem)
        {
            uploader = customUploaderItem;
        }

        public override UploadResult Upload(Stream stream, string fileName)
        {
            UploadResult result = new UploadResult();
            CustomUploaderInput input = new CustomUploaderInput(fileName, "");

            if (uploader.Body == CustomUploaderBody.MultipartFormData)
            {
                result = SendRequestFile(uploader.GetRequestURL(input), stream, fileName, uploader.GetFileFormName(), uploader.GetArguments(input),
                    uploader.GetHeaders(input), null, uploader.RequestMethod);
            }
            else if (uploader.Body == CustomUploaderBody.Binary)
            {
                result.Response = SendRequest(uploader.RequestMethod, uploader.GetRequestURL(input), stream, MimeTypes.GetMimeTypeFromFileName(fileName), null,
                    uploader.GetHeaders(input));
            }
            else
            {
                throw new Exception("Unsupported request format: " + uploader.Body);
            }

            uploader.TryParseResponse(result, LastResponseInfo, Errors, input);

            return result;
        }
    }
}